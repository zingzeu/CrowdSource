using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps.Condition;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public class FalseConditionLogic : IConditionLogic
    {
        public FalseConditionLogic(ConditionNode node)
        {
            
        }

        public Task<bool> Evaluate()
        {
            return Task.FromResult(false);
        }
    }
}