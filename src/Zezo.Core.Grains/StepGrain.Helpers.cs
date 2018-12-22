using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains
{
    public partial class StepGrain
    {

        private bool IsInitialized => State.Status != StepStatus.Uninitialized &&
                                      State.Status != StepStatus.Initializing;
        private bool IsStopped => State.Status == StepStatus.StoppedWithSuccess ||
                                  State.Status == StepStatus.Error;
    }
}