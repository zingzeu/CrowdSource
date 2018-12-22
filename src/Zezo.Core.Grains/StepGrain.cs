using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
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

        public override Task OnActivateAsync() {
            logger.LogInformation("activating...");
            
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
                State.Status = StepStatus.Initializing;

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
                    State.Status = StepStatus.Error;
                    throw;
                }

                try
                {
                    await logic.HandleInit();
                }
                catch (Exception e)
                {
                    logger.LogError($"Step failed due to Exception during HandleInit: {e}");
                    State.Status = StepStatus.Error;
                    throw;
                }
                State.Status = StepStatus.Inactive;
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
            State.Status = StepStatus.Active;
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


    }
}
