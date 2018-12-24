using System;
using Orleans;

namespace Zezo.Core.GrainInterfaces.Observers
{
    public interface IStepGrainObserver : IGrainObserver
    {
        void OnStatusChanged(Guid caller, StepStatus newStatus);
    }
}