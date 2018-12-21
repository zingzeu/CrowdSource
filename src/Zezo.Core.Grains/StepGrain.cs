using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.Grains.StepLogic;

namespace Zezo.Core.Grains
{
    public class StepGrain : Orleans.Grain<StepGrainState>, IStepGrain
    {
        private readonly ILogger logger;

        private IStepLogic logic;

        public override Task OnActivateAsync() {
            if (this.State.Status != StepStatus.Uninitialized && this.State.Status != StepStatus.Stopped) {
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

        public async Task OnInit(Guid parentNode, Guid entity, StepNode config)
        {
            if (this.State.Status != StepStatus.Uninitialized) {
                throw new Exception("Cannot initialize a StepGrain twice!");
            } else {
                logger.LogInformation($"StepGrain initialising with {config.StepType}...");
                logic = GetStepLogic(config);
                State.Config = config;
                State.ParentNode = parentNode;
                State.Entity = entity;
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
            throw new NotImplementedException();
        }

        public Task OnResuming()
        {
            throw new NotImplementedException();
        }

        public Task OnStopping()
        {
            throw new NotImplementedException();
        }

        private IStepLogic GetStepLogic(StepNode config) {
            switch (config.StepType) {
                case "Sequence":
                return new SequenceStepLogic(this.GetPrimaryKey(), State, GrainFactory, State.Config);
                
                default:
                return null;

            }
        }

 
    }
}
