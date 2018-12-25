using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.GrainInterfaces
{
    public interface IStepLogic
    {
        Task OnInit();
        
        /// <summary>
        /// Called when the Step is first allowed to work.
        /// Schedule work here.
        /// Don't perform long running jobs on the Grain's thread.
        /// The grain should only be used for monitoring of task executions.
        /// </summary>
        /// <returns></returns>
        Task OnActivate();
        
        /// <summary>
        /// Called when the Step is being resumed from a Paused status.
        /// Schedule work here.
        /// If no more work, change status to Completed.
        /// </summary>
        /// <returns></returns>
        Task OnResuming();
        Task OnPausing();
        Task OnStopping();

        Task HandleForceStart();

        // Called by child
        Task HandleChildStarted(Guid caller);
        Task HandleChildStopped(Guid caller);
    }
}
