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
    public class StepGrain : Orleans.Grain<StepGrainData>, IStepGrain, IContainer
    {
        public override Task OnDeactivateAsync() {
            logger.LogInformation("deactivating...");
            return Task.CompletedTask;
        }
        private readonly ILogger logger;

        private IStepLogic logic;

        public ILogger Logger => logger;

        public StepStatus Status => State?.Status ?? StepStatus.Uninitialized;

        StepGrainData IContainer.State => this.State;

        public Guid SelfKey => this.GetPrimaryKey();

        public override Task OnActivateAsync() {
            logger.LogInformation("activating...");
            if (this.State.Status != StepStatus.Uninitialized && this.State.Status != StepStatus.Stopped) {
                logger.LogInformation("restore previous logic...");
                this.logic = GetStepLogic(State.Config);
            }
            return Task.CompletedTask;
        }
        public StepGrain(ILogger<StepGrain> logger)
        {
            this.logger = logger;
        }

        public Task<StepStatus> GetStatus()
        {
            return Task.FromResult(this.State.Status);
        }

        public Task<bool> IsRoot()
        {
            return Task.FromResult(this.State.ParentNode == null);
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

        public async Task OnInit(Guid? parentNode, Guid entity, StepNode config)
        {
            if (this.State.Status != StepStatus.Uninitialized) {
                throw new Exception("Cannot initialize a StepGrain twice!");
            } else {
                logger.LogInformation($"StepGrain initialising with {config.StepType}...");
                State.Config = config;
                State.ParentNode = parentNode;
                State.Entity = entity;
                logic = GetStepLogic(config);
                await logic.HandleInit();
                this.State.Status = StepStatus.Initialized;
                logger.LogInformation($"initialized...");
                return;
            }
        }

        public Task OnPausing()
        {
            throw new NotImplementedException();
        }

        public Task OnReady()
        {
            DelayDeactivation(TimeSpan.FromMinutes(10));
            State.Status = StepStatus.Ready;
            return logic.HandleReady();
        }

        public Task OnResuming()
        {
            throw new NotImplementedException();
        }

        public Task OnStopping()
        {
            throw new NotImplementedException();
        }

        public Task ForceStart()
        {
            if (this.Status == StepStatus.Ready) {
                return logic.HandleForceStart();
            } else {
                Logger.LogWarning("Did not start because Step is not ready.");
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

        public void CompleteSelf(bool success)
        {
            if (State.Status == StepStatus.Ready ||
                State.Status  == StepStatus.Working) {
                State.Status = StepStatus.Stopped;
                if (this.State.ParentNode == null) {
                    // root
                    // TODO: inform Entity
                } else {
                    // fire and forget
                    GetParentGrain().OnChildStopped(this.GetPrimaryKey());
                }
            }
            else 
            {
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Stopped.");
            }
        }

        public IStepGrain GetParentGrain()
        {
            if (State.ParentNode != null) {
                return GetStepGrain(State.ParentNode.GetValueOrDefault());
            } else {
                return null;
            }
        }

        public void MarkSelfStarted()
        {
            if (State.Status == StepStatus.Ready) {
                State.Status = StepStatus.Working;
                if (this.State.ParentNode == null) {
                    // inform Entity
                } else {
                    GetParentGrain().OnChildStarted(this.GetPrimaryKey());
                }
            }
            else 
            {
                logger.LogError($"Cannot change from status {State.Status} to Started.");
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Started.");
            }
        }

        public void SpawnChild(StepNode childConfig)
        {
            throw new NotImplementedException();
        }

        public Task<StepStopReason> GetStopReason()
        {
            return Task.FromResult(StepStopReason.Completed);
        }

        public IEntityGrain GetEntityGrain()
        {
            return GrainFactory.GetGrain<IEntityGrain>(State.Entity);
        }
    }
}
