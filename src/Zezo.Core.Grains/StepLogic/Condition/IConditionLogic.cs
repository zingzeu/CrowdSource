using System.Threading.Tasks;
using Zezo.Core.Grains.Datastores.Scripting;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public interface IConditionLogic
    {
        Task<bool> Evaluate(DatastoreRegistry registry = null);
    }
}