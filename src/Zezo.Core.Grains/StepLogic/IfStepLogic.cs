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
            var selfReference = container.SelfReference;
            var hasChild = container.State.ChildCount > 0;
            var firstAndOnlyChild = hasChild ? container.GetStepGrain(container.State.ChildNodes[0]) : null;
            var datastoreRegistry = await container.GetDatastoreRegistry();
            bool result;
            // Because the Evaluation of Condition logic can take a while (if it is a script)
            // we let it run on the .NET thread pool instead of Orleans threads.
            _ = Task.Run(async () =>
            {
                try
                {
                    // TODO: pass some kind of Context object to Evaluate()
                    result = await conditionLogic.Evaluate(datastoreRegistry);
                }
                catch (Exception e)
                {
                    await selfReference._Call("CompleteSelf", false);
                    Logger.LogError($"Error during Evaluate: {e}");
                    return;
                }
            
                if (result)
                {
                    // activate child
                    if (hasChild)
                    {
                        _ = firstAndOnlyChild?.Activate();
                    }
                    else
                    {
                        await selfReference._Call("CompleteSelf", true);
                    }
                }
                else
                {
                    if (hasChild)
                    {
                        await (firstAndOnlyChild?.Stop() ?? Task.CompletedTask);
                    }
                    // skip child
                    await selfReference._Call("CompleteSelf", true);
                }
            });
        }

        public override async Task OnPausing()
        {
            if (container.State.ChildCount > 0)
            {
                var firstAndOnlyChild = container.GetStepGrain(container.State.ChildNodes[0]);
                await firstAndOnlyChild.Pause();
            }
        }

        public override async Task OnResuming()
        {
            if (container.State.ChildCount > 0)
            {
                var firstAndOnlyChild = container.GetStepGrain(container.State.ChildNodes[0]);
                await firstAndOnlyChild.Resume();
            }
        }

        public override async Task OnStopping()
        {
            if (container.State.ChildCount > 0)
            {
                var firstAndOnlyChild = container.GetStepGrain(container.State.ChildNodes[0]);
                await firstAndOnlyChild.Stop();
                if (await firstAndOnlyChild.GetStatus() == StepStatus.Error)
                {
                    await container.CompleteSelf(false);
                }
            }
        }

        public override Task HandleChildStarted(Guid caller)
        {
            return container.MarkSelfBusy();
        }

        public override Task HandleChildIdle(Guid caller)
        {
            return container.MarkSelfIdle();
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

        public override Task HandleForceStart()
        {
            throw new NotImplementedException();
        }
        
        public override Task _Call(string action, params object[] parameters)
        {
            switch (action)
            {
                case "MarkSelfBusy":
                    return container.MarkSelfBusy();
                case "MarkSelfIdle":
                    return container.MarkSelfIdle();
                case "CompleteSelf":
                    return container.CompleteSelf((bool)parameters[0]);
                default:
                    return Task.CompletedTask;
            }
        }
        
        private IConditionLogic GetConditionLogic(ConditionNode conditionConfig)
        {
            switch (conditionConfig)
            {
                case TrueNode trueNode:
                    return new TrueConditionLogic(trueNode);
                case FalseNode falseNode:
                    return new FalseConditionLogic(falseNode);
                case ScriptConditionNode scriptConditionNode:
                    return new ScriptConditionLogic(scriptConditionNode);
                default:
                    throw new Exception($"Unknown Condition logic type {conditionConfig.GetTagName()}.");
            }
        }
    }
}