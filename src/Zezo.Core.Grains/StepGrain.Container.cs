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

        public Guid SelfKey => this.GetPrimaryKey();
        
        public void CompleteSelf(bool success)
        {
            if (State.Status == StepStatus.Inactive ||
                State.Status  == StepStatus.Working) {
                State.Status = success ? StepStatus.StoppedWithSuccess : StepStatus.Error;
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
            if (State.Status == StepStatus.Inactive) {
                State.Status = StepStatus.Working;
                if (this.State.ParentNode == null) {
                    // inform Entity
                } else {
                    _ = GetParentGrain().OnChildStarted(this.GetPrimaryKey());
                }
            }
            else 
            {
                logger.LogError($"Cannot change from status {State.Status} to Started.");
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Started.");
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