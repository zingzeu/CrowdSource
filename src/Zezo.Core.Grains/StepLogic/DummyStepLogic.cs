using System;
using System.Collections.Generic;
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
        private readonly IList<TimeSpan> _workingSeq;
        
        public DummyStepLogic(IContainer container) 
            : base(container)
        {
            id = container.State.Config.Id;

            var config = container.State.Config as DummyStepNode;
            _beforeStart = config.BeforeStart;
            _workingTime = config.Working;
            _workingSeq = config.WorkingSequence;
        }

        public override Task HandleChildStarted(Guid caller)
        {
            return Task.CompletedTask;
        }

        public override Task HandleChildIdle(Guid caller)
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
            await container.MarkSelfBusy();
            Say("Started working...");
        }

        public override Task OnInit()
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
                    if (_workingSeq == null)
                    {
                        await container.MarkSelfBusy();
                        Say("Working...");
                        await Task.Delay(_workingTime);
                        Say("Stopping...");
                    }
                    else
                    {
                        var toActive = true;
                        foreach (var time in _workingSeq)
                        {
                            if (toActive)
                            {
                                await container.MarkSelfBusy();
                                Say("Working...");
                                await Task.Delay(time);
                            }
                            else
                            {
                                await container.MarkSelfIdle();
                                Say("Pausing...");
                                await Task.Delay(time);                               
                            }
                            toActive = !toActive;
                        }
                    }
                    await container.CompleteSelf(true);
                }
            );
            return Task.CompletedTask;
        }
        
        public override Task OnPausing()
        {
            return Task.CompletedTask;
        }

        public override Task OnResuming()
        {
            return Task.CompletedTask;
        }

        public override Task OnStopping()
        {
            return Task.CompletedTask;
        }

        private void Say(string thing) {
            Logger.LogInformation($"{id}: {thing}");
        }
    }
}
