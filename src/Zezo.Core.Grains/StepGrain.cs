using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.GrainInterfaces.Observers;
using Zezo.Core.Grains.StepLogic;

namespace Zezo.Core.Grains
{
    
    [StorageProvider(ProviderName="DevStore")]
    public partial class StepGrain : Orleans.Grain<StepGrainData>, IStepGrain
    {
        public override Task OnDeactivateAsync() {
            logger.LogInformation("deactivating...");
            return Task.CompletedTask;
        }
        
        private readonly ILogger logger;

        private IStepLogic logic;

        private GrainObserverManager<IStepGrainObserver> _subsManager;

        public override Task OnActivateAsync() {
            _subsManager = new GrainObserverManager<IStepGrainObserver>();
            _subsManager.ExpirationDuration = TimeSpan.FromHours(1);
            logger.LogInformation("Activating...");
            
            if (IsInitialized && !IsStopped) {
                logger.LogInformation("Re-activation of existing StepGrain. Restore previous StepLogic " +
                                      "implementation...");
                this.logic = GetStepLogic(State.Config);
            }
            else if (State.Status == StepStatus.Initializing)
            {
                logger.LogWarning("Re-activation and it was Initializing??? " +
                                  "(Probably initialization failed?)");
            }
            return Task.CompletedTask;
        }
        public StepGrain(ILogger<StepGrain> logger)
        {
            this.logger = logger;
        }

        public Task<StepStatus> GetStatus()
        {
            return Task.FromResult(State.Status);
        }

        public Task<bool> IsRoot()
        {
            return Task.FromResult(State.ParentNode == null);
        }

        public Task OnChildPaused(Guid caller)
        {
            return logic.HandleChildPaused(caller);
        }

        public Task OnChildStarted(Guid caller)
        {
            return logic.HandleChildStarted(caller);
        }

        public Task OnChildStopped(Guid caller)
        {
            return logic.HandleChildStopped(caller);
        }

        public async Task Init(Guid? parentNode, Guid entity, StepNode config)
        {
            if (IsInitialized) 
            {
                logger.LogError("Attempting to initialize twice!");
                throw new Exception("Cannot initialize a StepGrain twice!");
            } 
            else if (State.Status == StepStatus.Initializing)
            {
                logger.LogError("Grain is already initializing!");
                throw new Exception("Grain is already initializing!");
            }
            else
            {
                await ChangeStatus(StepStatus.Initializing);

                logger.LogInformation($"StepGrain initialising with {config.StepType}... \n" +
                                      (parentNode == null ? $"As Root of Entity {entity}" 
                                          : $"With Parent Step {parentNode.Value} ") + "\n"
                                      );
                
                State.Config = config;
                State.ParentNode = parentNode;
                State.Entity = entity;

                try
                {
                    logic = GetStepLogic(config);
                }
                catch (Exception e)
                {
                    logger.LogError($"Step failed due to Exception: {e}");
                    await ChangeStatus(StepStatus.Error);
                    throw;
                }

                try
                {
                    await logic.HandleInit();
                }
                catch (Exception e)
                {
                    logger.LogError($"Step failed due to Exception during HandleInit: {e}");
                    await ChangeStatus(StepStatus.Error);
                    throw;
                }
                await ChangeStatus(StepStatus.Inactive);
                logger.LogInformation($"Successfully initialized.");
            }
        }

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public async Task Activate()
        {
            DelayDeactivation(TimeSpan.FromMinutes(10));
            await ChangeStatus(StepStatus.Active);
            logger.LogInformation("State Changed to Active");
            try
            {
                await logic.OnActivate();
            }
            catch (Exception e)
            {
                logger.LogError("Error during OnActivate: {e}");
                // don't set state to Error (for now) to allow retry
                throw;
            }
        }

        public Task Resume()
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }

        public Task ForceStart()
        {
            if (this.Status == StepStatus.Inactive) {
                return logic.HandleForceStart();
            } else {
                logger.LogWarning("Did not start because Step is not ready.");
                return Task.CompletedTask;
            }
        }

        private IStepLogic GetStepLogic(StepNode config) {
            switch (config.StepType) {
                case "Sequence":
                return new SequenceStepLogic(this);

                case "DummyStep":
                return new DummyStepLogic(this);

                default:
                throw new Exception($"Unknown StepType {config.StepType}");

            }
        }

        public IStepGrain GetStepGrain(Guid key)
        {
            return GrainFactory.GetGrain<IStepGrain>(key);
        }

        
        public Task<StepStopReason> GetStopReason()
        {
            return Task.FromResult(StepStopReason.Completed);
        }


        private async Task ChangeStatus(StepStatus newStatus)
        {
            if (newStatus != State.Status)
            {
                logger.LogInformation($"State changed from [{State.Status}] to [{newStatus}]");
                State.Status = newStatus;

                await WriteStateAsync();
                _subsManager.Notify(x => x.OnStatusChanged(SelfKey, newStatus));
                
                // Calls upstream
                switch (newStatus)
                {
                    case StepStatus.Working:
                        if (State.ParentNode == null) {
                            // inform Entity
                        } else {
                            _ = GetParentGrain().OnChildStarted(SelfKey);
                        }
                        break;
                      
                    case StepStatus.StoppedWithSuccess:
                        if (State.ParentNode == null) {
                            // root
                            // TODO: inform Entity
                        } else {
                            // fire and forget
                            _ = GetParentGrain().OnChildStopped(SelfKey);
                        }

                        break;
                    case StepStatus.Error:
                        if (State.ParentNode == null)
                        {
                            
                        }
                        else
                        {
                            _ = GetParentGrain().OnChildStopped(SelfKey);
                        }

                        break;
                }
            }
        }

    }
}
