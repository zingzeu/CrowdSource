using System;
using System.Reflection;
using Orleans.CodeGeneration;
using Orleans.Concurrency;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains
{
    public partial class StepGrain
    {
        private bool IsInitialized => (State.Status & (StepStatus) 0b1000_0000) != 0;

        private bool IsStopped => (State.Status & (StepStatus) 0b0100_0000) != 0;

        private static bool CanChangeState(StepStatus oldStatus, StepStatus newStatus)
        {
            if ((oldStatus & (StepStatus) 0b0100_0000) != 0) // once stopped, can never be restarted
            {
                return false;
            }

            return true;
        }
    }

}