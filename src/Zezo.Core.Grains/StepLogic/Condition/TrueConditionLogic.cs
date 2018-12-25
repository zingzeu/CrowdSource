using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps.Condition;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public class TrueConditionLogic : IConditionLogic
    {
        public TrueConditionLogic(ConditionNode node)
        {
            
        }

        public Task<bool> Evaluate()
        {
            return Task.FromResult(true);
        }
    }
}