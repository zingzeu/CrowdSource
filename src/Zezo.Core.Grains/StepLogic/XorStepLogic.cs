using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic
{
    public class XorStepLogic : BaseStepLogic
    {
        public XorStepLogic(IContainer container) : base(container)
        {
        }

        public override async Task OnInit()
        {
            await base.OnInit();
            var xorConfig = container.State.Config as XorNode;
            if (xorConfig == null)
            {
                Logger.LogWarning("OnInit: Config is null...");
                await container.CompleteSelf(false);
                return;
            }
            // Spawn children nodes
            container.State.ChildNodes.Clear();
            foreach (StepNode childConfig in xorConfig.Children) {
                var childKey = await container.SpawnStep(childConfig);
                container.State.ChildNodes.Add(childKey);
            }
            Logger.LogInformation($"XorStep: {xorConfig.Children.Count} children created.");

        }

        public override async Task OnActivate()
        {
            // Make all children ready.
            if (container.State.ChildCount > 0) {
                foreach (var child in container.State.ChildNodes)
                {
                    await container.GetStepGrain(child).Activate();
                }
            } else {
                Logger.LogWarning("No children, moving to Completed state");
                await container.CompleteSelf(true);
            }
        }

        public override Task OnPausing()
        {
            throw new NotImplementedException();
        }

        public override Task OnResuming()
        {
            throw new NotImplementedException();
        }

        public override Task OnStopping()
        {
            throw new NotImplementedException();
        }

        public override async Task HandleChildStarted(Guid caller)
        {
            if (container.Status == StepStatus.ActiveIdle) {
                foreach (var child in container.State.ChildNodes)
                {
                    // pause other
                    if (child != caller)
                    {
                        try
                        {
                            await container.GetStepGrain(child).Pause();
                        }
                        catch (InvalidOperationException)
                        {
                            // do nothing
                        }
                    }
                }
                await container.MarkSelfBusy();
            }
        }

        public override async Task HandleChildIdle(Guid caller)
        {
            if (container.Status == StepStatus.Working)
            {
                await container.MarkSelfIdle();
                foreach (var child in container.State.ChildNodes)
                {
                    if (child != caller)
                    {
                        try
                        {
                            var childGrain = container.GetStepGrain(child);
                            var status = await childGrain.GetStatus();
                            if (status == StepStatus.Paused)
                            {
                                _ = childGrain.Resume();
                            } else if (status == StepStatus.Inactive)
                            {
                                _ = childGrain.Activate();
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // do nothing
                        }
                    }
                }
            }
        }

        public override async Task HandleChildStopped(Guid caller)
        {
            // check all children
            foreach (var child in container.State.ChildNodes)
            {
                var childGrain = container.GetStepGrain(child);
                var s = await childGrain.GetStatus();
                if (s == StepStatus.Error)
                {
                    await container.CompleteSelf(false);
                    return;
                } 
                else if (s != StepStatus.Completed)
                {
                    await childGrain.Stop();
                }
            }
            await container.CompleteSelf(true);
        }


        public override Task HandleForceStart()
        {
            throw new NotImplementedException();
        }
    }
}