using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic
{
    public class ParallelStepLogic : BaseStepLogic
    {
        public ParallelStepLogic(IContainer container) : base(container)
        {
        }

        public override async Task OnInit()
        {
            await base.OnInit();
            var parConfig = container.State.Config as ParallelNode;
            if (parConfig == null)
            {
                Logger.LogWarning("HandleInit: Config is null...");
                return;
            }
            // Spawn children nodes
            container.State.ChildNodes.Clear();
            foreach (StepNode childConfig in parConfig.Children) {
                var childKey = await container.SpawnStep(childConfig);
                container.State.ChildNodes.Add(childKey);
            }
            Logger.LogInformation($"SequenceStep: {parConfig.Children.Count} children created.");

        }

        public override Task OnActivate()
        {
            // Make all children ready.
            if (container.State.ChildCount > 0) {
                foreach (var child in container.State.ChildNodes)
                {
                    _ = container.GetStepGrain(child).Activate();
                }
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

        public override Task HandleStopping()
        {
            throw new NotImplementedException();
        }

        public override Task HandleChildStarted(Guid caller)
        {
            if (container.Status == StepStatus.Inactive) {
                return container.MarkSelfStarted();
            }
            return Task.CompletedTask;
        }

        public override async Task HandleChildStopped(Guid caller)
        {
            // check all children
            foreach (var child in container.State.ChildNodes)
            {
                var s = await container.GetStepGrain(child).GetStatus();
                if (s == StepStatus.Error)
                {
                    await container.CompleteSelf(false);
                    return;
                } 
                else if (s != StepStatus.Completed)
                {
                    return;
                }
            }

            await container.CompleteSelf(true);
        }

        public override Task HandleChildPaused(Guid caller)
        {
            throw new NotImplementedException();
        }

        public override Task HandleForceStart()
        {
            throw new NotImplementedException();
        }
    }
}