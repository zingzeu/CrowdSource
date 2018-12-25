using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Orleans;
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

        public async Task OnChildStarted(Guid caller)
        {
            try
            {
                await logic.HandleChildStarted(caller);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error during HandleChildStarted: {e}");
            }
        }
        
        public async Task OnChildIdle(Guid caller)
        {
            try
            {
                await logic.HandleChildIdle(caller);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error during HandleChildIdle: {e}");
            }
        }

        public Task OnChildStopped(Guid caller, ChildStoppedEventArgs eventArgs)
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
                    await logic.OnInit();
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

        public async Task Pause()
        {
            if (State.Status == StepStatus.ActiveIdle)
            {
                await ChangeStatus(StepStatus.Paused);
            }
            else if (State.Status == StepStatus.Working)
            {
                try
                {
                    await logic.OnPausing();
                    await ChangeStatus(StepStatus.Paused);
                }
                catch (Exception e)
                {
                    logger.LogError($"Error during OnPausing: {e}");
                    await ChangeStatus(StepStatus.Error);
                    throw;
                }
            }
            else
            {
                throw new InvalidOperationException("Not pause-able.");
            }
        }

        public async Task Activate()
        {
            DelayDeactivation(TimeSpan.FromMinutes(10));
            try
            {
                await ChangeStatus(StepStatus.ActiveIdle);
                await logic.OnActivate();
            }
            catch (Exception e)
            {
                logger.LogError("Error during OnActivate: {e}");
                // don't set state to Error (for now) to allow retry
                await ChangeStatus(StepStatus.Inactive);
                throw;
            }
        }

        public async Task Resume()
        {
            if (State.Status == StepStatus.Paused)
            {
                await ChangeStatus(StepStatus.Resuming);
                logger.LogInformation("Resuming...");
                try
                {
                    await logic.OnResuming();
                    await ChangeStatus(s => s == StepStatus.Working
                        ? StepStatus.Working : StepStatus.ActiveIdle);
                }
                catch (Exception e)
                {
                    logger.LogError(("Error during OnResume: {e}"));
                    await ChangeStatus(StepStatus.Error);
                    throw;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot resume. It is not paused.");
            }
        }

        public async Task Stop()
        {
            if (IsStopped)
            {
                throw new InvalidOperationException("Already stopped.");
            }

            if (!IsInitialized)
            {
                throw new InvalidOperationException("Not initialized.");
            }
            if (State.Status == StepStatus.ActiveIdle || State.Status == StepStatus.Paused
                || State.Status == StepStatus.Inactive)
            {
                await ChangeStatus(StepStatus.Skipped);
            }
            else if (State.Status == StepStatus.Working || State.Status == StepStatus.Pausing)
            {
                try
                {
                    await logic.OnStopping();
                    await ChangeStatus( s =>
                        (s & (StepStatus) 0b0100_0000) == 0 ? StepStatus.Skipped : s);
                }
                catch (Exception e)
                {
                    logger.LogError($"Error during OnPausing: {e}");
                    await ChangeStatus(StepStatus.Error);
                    throw;
                }
            }
            else
            {
                logger.LogWarning($"Unexpected state {State.Status}");
                await ChangeStatus(StepStatus.Skipped);
            }
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
                
                case "Parallel":
                    return new ParallelStepLogic(this);

                case "If":
                    return new IfStepLogic(this);
                
                case "Xor":
                    return new XorStepLogic(this);
                
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

        private Task ChangeStatus(StepStatus newStatus)
        {
            return ChangeStatus((_) => newStatus);
        }
        
        private async Task ChangeStatus(Func<StepStatus, StepStatus> predicate)
        {
            var newStatus = predicate(State.Status);
            var oldStatus = State.Status;
            if (newStatus != oldStatus)
            {
                if (!CanChangeState(oldStatus, newStatus))
                {
                    logger.LogWarning($"Illegal status transition: [{State.Status}]->[{newStatus}]");
                    return;
                }
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
                      
                    case StepStatus.Completed:
                        if (State.ParentNode == null) {
                            // root
                            // TODO: inform Entity
                        } else {
                            // fire and forget
                            _ = GetParentGrain().OnChildStopped(SelfKey, 
                                new ChildStoppedEventArgs(StepId, Status));
                        }

                        break;
                    case StepStatus.Inactive:
                        if (oldStatus == StepStatus.Working)
                        {
                            if (State.ParentNode == null) {
                                // TODO: inform Entity
                            } 
                            else 
                            {
                                // fire and forget
                                _ = GetParentGrain().OnChildIdle(SelfKey);
                            }
                        }

                        break;
                    
                    case StepStatus.Error:
                        if (State.ParentNode == null)
                        {
                            
                        }
                        else
                        {
                            _ = GetParentGrain().OnChildStopped(SelfKey,
                                new ChildStoppedEventArgs(StepId, Status)
                                );
                        }

                        break;
                }
            }
        }
        
    }
}
