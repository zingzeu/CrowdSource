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
        }

        public override Task HandleChildStarted(Guid caller)
        {
            if (container.Status == StepStatus.Inactive) {
                return container.MarkSelfBusy();
            }
            return Task.CompletedTask;
        }

        public override async Task HandleChildIdle(Guid caller)
        {
            if (container.Status == StepStatus.Working)
            {
                await container.MarkSelfIdle();
            }
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
                        if (i == container.State.ChildCount-1) 
                        {
                            // final one
                            await container.CompleteSelf(true);
                        }
                        else
                        {
                            // activate next child
                            _ = container.GetStepGrain(container.State.ChildNodes[i+1]).Activate();
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
                
                // Stop the rest of children.
                await container.CompleteSelf(false);
            }
        }

        public override async Task HandleForceStart()
        {
            // finds first child that is ready, and force it
            foreach (var childKey in container.State.ChildNodes) {
                var child = container.GetStepGrain(childKey);
                if (await child.GetStatus() != StepStatus.Inactive) continue;
                _ = child.ForceStart();
                return;
            }
        }

        public override async Task OnInit()
        {
            await base.OnInit();
            seqConfig = container.State.Config as SequenceNode;
            if (seqConfig == null)
            {
                Logger.LogWarning("HandleInit: Config is null...");
                return;
            }
            // Spawn children nodes
            container.State.ChildNodes.Clear();
            foreach (StepNode childConfig in seqConfig.Children) {
                var childKey = await container.SpawnStep(childConfig);
                container.State.ChildNodes.Add(childKey);
            }
            Logger.LogInformation($"SequenceStep: {seqConfig.Children.Count} children created.");
        }

        public override Task OnActivate()
        {
            // Make first child ready.
            if (container.State.ChildCount > 0) {
                var firstChild = container.State.ChildNodes[0];
                container.GetStepGrain(firstChild).Activate();
            } else {
                Logger.LogWarning("No children, moving to Completed state");
                container.CompleteSelf(true);
            }
            return Task.CompletedTask;
        }
        public override Task OnPausing()
        {
            throw new NotImplementedException();
        }

        public override Task OnResuming()
        {
            throw new NotImplementedException();
        }

        public override async Task OnStopping()
        {
            // finds first child that is ready, and force it
            foreach (var childKey in container.State.ChildNodes) {
                var child = container.GetStepGrain(childKey);
                var childStatus = await child.GetStatus();
                if (childStatus != StepStatus.Completed && childStatus != StepStatus.Error) {
                    await child.Stop();
                }
            }
        }
    }
}
