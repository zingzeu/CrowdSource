using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;
using Config = Zezo.Core.Configuration;

namespace Zezo.Core.GrainInterfaces
{
    public interface IStepGrain : Orleans.IGrainWithGuidKey
    {
        Task<StepStatus> GetStatus();
        Task<bool> IsRoot();

        // Called by parent/executor

        /// <summary>
        /// Called when the Step is first created.
        /// Should only be called once, when the Step is "Uninitialized".
        /// </summary>
        Task OnInit(Guid parentNode, Guid entity, StepNode config);

        /// <summary>
        /// Called when the Step is ready (allowed to start).
        /// For root Step, it will be triggered when the user starts the Entry.
        /// For other Steps, it will be called by their parents when appropriate.
        /// </summary>
        Task OnReady();

        /// <summary>
        /// Called when the Step is requested to pause.
        /// Should return immediately (without changing the state).
        /// If "Pause" is not applicable, ignore.
        /// </summary>
        Task OnPausing();

        /// <summary>
        /// Called when the Step is requested to Resume.
        /// </summary>
        /// <returns>The resuming.</returns>
        Task OnResuming();

        /// <summary>
        /// Ons the stopping.
        /// </summary>
        /// <returns>The stopping.</returns>
        Task OnStopping();

        // Called by children
        Task OnChildStarted(Guid caller);
        Task OnChildStopped(Guid caller);
        Task OnChildPaused(Guid caller);
    }

    public enum StepStatus {
        Uninitialized = 0,
        Initialized = 1,
        Ready = 2,
        Working = 3,
        Pausing = 4,
        Paused = 5,
        Stopped = 6
    }
}
