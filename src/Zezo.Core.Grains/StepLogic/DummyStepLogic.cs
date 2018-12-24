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
        private readonly TimeSpan _beforeStart;
        private readonly TimeSpan _workingTime;
        
        public DummyStepLogic(IContainer container) 
            : base(container)
        {
            id = container.State.Config.Id;

            var config = container.State.Config as DummyStepNode;
            _beforeStart = config.BeforeStart;
            _workingTime = config.Working;
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
            await container.MarkSelfStarted();
            Say("Started working...");
        }

        public override Task HandleInit()
        {
            Say($"I am born, my parent is {container.State.ParentNode}");
            return Task.CompletedTask;
        }

        public override Task OnActivate()
        {
            Say($"I am allowed to start working. I will start working later.");
            Task.Factory.StartNew(
                async () => {
                    await Task.Delay(_beforeStart);
                    Say("Been ready for a while, now starting...");
                    await container.MarkSelfStarted();
                    Say("Working...");
                    await Task.Delay(_workingTime);
                    Say("Stopping...");
                    await container.CompleteSelf(true);
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
