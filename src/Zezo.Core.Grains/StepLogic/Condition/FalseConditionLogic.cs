using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps.Condition;
using Zezo.Core.Grains.Datastores.Scripting;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public class FalseConditionLogic : IConditionLogic
    {
        public FalseConditionLogic(FalseNode node) {}

        public Task<bool> Evaluate(DatastoreRegistry registry)
        {
            return Task.FromResult(false);
        }
    }
}