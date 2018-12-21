using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic
{
    public sealed class SequenceStepLogic : BaseStepLogic
    {
        private SequenceNode seqConfig;
        public SequenceStepLogic(IContainer container) 
            : base(container)
        {
            seqConfig = container.State.Config as SequenceNode;
        }

        public override Task HandleChildPaused(Guid caller)
        {
            throw new NotImplementedException();
        }

        public override Task HandleChildStarted(Guid caller)
        {
            if (container.Status == StepStatus.Ready) {
                container.MarkSelfStarted();
            }
            return Task.CompletedTask;
        }

        public override async Task HandleChildStopped(Guid caller)
        {
            // find out success or not
            var stopReason = await container.GetStepGrain(caller).GetStopReason();
            if (stopReason == StepStopReason.Completed) {
                // find out which one completes
                for (var i = 0; i < container.State.ChildCount; ++i) 
                {
                    if (container.State.ChildNodes[i] == caller) 
                    {
                        if (i == container.State.ChildCount) 
                        {
                            // final one
                            container.CompleteSelf(true);
                        }
                        else
                        {
                            // activate next child
                            _ = container.GetStepGrain(container.State.ChildNodes[i+1]).OnReady();
                        }
                        return;
                    }
                }
                // not found
                Logger.LogWarning($"{caller} is not my child!");
            }
            else 
            {
                // failure of a single child Step fails the entire SequenceStep
                container.CompleteSelf(false);
            }
        }

        public override async Task HandleForceStart()
        {
            // finds first child that is ready, and force it
            foreach (var childKey in container.State.ChildNodes) {
                var child = container.GetStepGrain(childKey);
                if (await child.GetStatus() == StepStatus.Ready) {
                    _ = child.ForceStart();
                    return;
                }
            }
        }

        public override async Task HandleInit()
        {
            // Spawn childrens
            container.State.ChildNodes.Clear();
            var entityGrain = container.GetEntityGrain();
            foreach (StepNode childConfig in seqConfig.Children) {
                var childKey = await entityGrain.SpawnChild(childConfig, container.SelfKey);
                container.State.ChildNodes.Add(childKey);
            }
            Logger.LogInformation($"SequenceStep: {seqConfig.Children.Count} chilren created.");
        }

        public override Task HandleReady()
        {
            // Make first child ready.
            if (container.State.ChildCount > 0) {
                var firstChild = container.State.ChildNodes[0];
                container.GetStepGrain(firstChild).OnReady();
            } else {
                Logger.LogWarning("No children, moving to Completed state");
                container.CompleteSelf(true);
            }
            return Task.CompletedTask;
        }
        public override Task HandlePausing()
        {
            throw new NotImplementedException();
        }

        public override Task HandleResuming()
        {
            throw new NotImplementedException();
        }

        public override async Task HandleStopping()
        {
            // finds first child that is ready, and force it
            foreach (var childKey in container.State.ChildNodes) {
                var child = container.GetStepGrain(childKey);
                if (await child.GetStatus() != StepStatus.Stopped) {
                    await child.OnStopping();
                }
            }
        }
    }
}
