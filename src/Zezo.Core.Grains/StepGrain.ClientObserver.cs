using System.Threading.Tasks;
using Zezo.Core.GrainInterfaces.Observers;

namespace Zezo.Core.Grains
{
    public partial class StepGrain
    {
        public Task Subscribe(IStepGrainObserver observer)
        {
            _subsManager.Subscribe(observer);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(IStepGrainObserver observer)
        {
            _subsManager.Unsubscribe(observer);
            return Task.CompletedTask;
        }
    }
}