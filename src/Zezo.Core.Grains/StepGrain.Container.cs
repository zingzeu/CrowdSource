using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains
{
    public partial class StepGrain : IContainer
    {        
        public ILogger Logger => logger;

        public StepStatus Status => State?.Status ?? StepStatus.Uninitialized;

        StepGrainData IContainer.State => this.State;

        IStepGrain IContainer.SelfReference => GrainFactory.GetGrain<IStepGrain>(SelfKey);

        public Guid SelfKey => this.GetPrimaryKey();

        public async Task CompleteSelf(bool success)
        {
            if (State.Status == StepStatus.Inactive ||
                State.Status == StepStatus.ActiveIdle || 
                State.Status  == StepStatus.Working)
            {
                await ChangeStatus(success ? StepStatus.Completed : StepStatus.Error);
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

        public async Task MarkSelfBusy()
        {
            if (State.Status == StepStatus.ActiveIdle) {
                await ChangeStatus(StepStatus.Working);
            }
            else 
            {
                logger.LogError($"Cannot change from status {State.Status} to Started.");
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Started.");
            }
        }

        public async Task MarkSelfIdle()
        {
            if (State.Status == StepStatus.ActiveIdle)
            {
                // do nothing
            }
            else if (State.Status == StepStatus.Working)
            {
                await ChangeStatus(StepStatus.ActiveIdle);
            }
            else
            {
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Active (Idle)");
            }
        }

        public Task<Guid> SpawnStep(StepNode childConfig)
        {
            var entityGrain = GetEntityGrain();
            return entityGrain.SpawnChild(childConfig, SelfKey);
            
        }

        public IEntityGrain GetEntityGrain()
        {
            return GrainFactory.GetGrain<IEntityGrain>(State.Entity);
        }
        
    }
}