using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic
{
    public sealed class DummyStepLogic : BaseStepLogic
    {
        private readonly string id;
        public DummyStepLogic(IContainer container) 
            : base(container)
        {
            id = container.State.Config.Id;
        }

        public override Task HandleChildPaused(Guid caller)
        {
            return Task.CompletedTask;
        }

        public override Task HandleChildStarted(Guid caller)
        {
            return Task.CompletedTask;
        }

        public override Task HandleChildStopped(Guid caller)
        {
            return Task.CompletedTask;
        }

        public override async Task HandleForceStart()
        {
            Say("Being forced to start");
            await Task.Delay(2000);
            Say("Starting...");
            await Task.Delay(1000);
            container.MarkSelfStarted();
            Say("Started working...");
        }

        public override Task HandleInit()
        {
            Say($"I am born, my parent is {container.State.ParentNode}");
            return Task.CompletedTask;
        }

        public override Task HandleReady()
        {
            Say($"I am allowed to start working. I will start working later.");
            Task.Factory.StartNew(
                async () => {
                    await Task.Delay(4000);
                    Say("Been ready for a while, now starting...");
                    container.MarkSelfStarted();
                    await Task.Delay(1000);
                    Say("Working...");
                    await Task.Delay(3000);
                    Say("Stopping...");
                    container.CompleteSelf(true);
                }
            );
            return Task.CompletedTask;
        }
        public override Task HandlePausing()
        {
            return Task.CompletedTask;
        }

        public override Task HandleResuming()
        {
            return Task.CompletedTask;
        }

        public override Task HandleStopping()
        {
            return Task.CompletedTask;
        }

        private void Say(string thing) {
            Logger.LogInformation($"{id}: {thing}");
        }
    }
}
