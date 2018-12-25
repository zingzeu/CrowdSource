using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.Configuration.Steps.Condition;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.Grains.StepLogic.Condition;

namespace Zezo.Core.Grains.StepLogic
{
    public class IfStepLogic : BaseStepLogic
    {
        public IfStepLogic(IContainer container) : base(container)
        {
        }

        public override async Task OnInit()
        {
            await base.OnInit();
            var ifConfig = container.State.Config as IfNode;
            if (ifConfig == null)
            {
                Logger.LogWarning("OnInit: Config is null...");
                return;
            }
            // Spawn child node
            container.State.ChildNodes.Clear();
            StepNode childConfig = ifConfig.Child;
            if (childConfig != null)
            {
                var childKey = await container.SpawnStep(childConfig);
                container.State.ChildNodes.Add(childKey);
                Logger.LogInformation($"IfStep: one child created.");                
            }
            else
            {
                Logger.LogWarning("IfStep: no child.");
            }
        }

        public override async Task OnActivate()
        {
            // Evaluate the condition now
            var config = container.State.Config as IfNode;
            var conditionConfig = config.Condition;
            var conditionLogic = GetConditionLogic(conditionConfig);
            if (await conditionLogic.Evaluate())
            {
                // activate child
                if (container.State.ChildCount > 0)
                {
                    var firstAndOnlyChild = container.GetStepGrain(container.State.ChildNodes[0]);
                    _ = firstAndOnlyChild.Activate();
                }
                else
                {
                    await container.CompleteSelf(true);
                }
        }
            else
            {
                // skip child
                await container.CompleteSelf(true);
            }
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
            return container.MarkSelfStarted();
        }

        public override async Task HandleChildStopped(Guid caller)
        {
            var child = container.GetStepGrain(caller);
            var childStatus = await child.GetStatus();
            if (childStatus == StepStatus.Error)
            {
                await container.CompleteSelf(false);
            }
            else if (childStatus == StepStatus.Completed)
            {
                await container.CompleteSelf(true);
            }
            else
            {
                Logger.LogWarning($"Unexpected child status [{childStatus}].");
            }
        }

        public override Task HandleChildPaused(Guid caller)
        {
            throw new NotImplementedException();
        }

        public override Task HandleForceStart()
        {
            throw new NotImplementedException();
        }
        
        private IConditionLogic GetConditionLogic(ConditionNode conditionConfig)
        {
            switch (conditionConfig)
            {
                case TrueNode trueNode:
                    return new TrueConditionLogic(trueNode);
                default:
                    throw new Exception($"Unknown Condition logic type.");
            }
        }
    }
}