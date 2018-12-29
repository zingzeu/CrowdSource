using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps.Condition;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public class FalseConditionLogic : IConditionLogic
    {
        public FalseConditionLogic(FalseNode node)
        {
            
        }

        public Task<bool> Evaluate()
        {
            return Task.FromResult(false);
        }
    }
}