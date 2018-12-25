using System.Threading.Tasks;

namespace Zezo.Core.Grains.StepLogic.Condition
{
    public interface IConditionLogic
    {
        Task<bool> Evaluate();
    }
}