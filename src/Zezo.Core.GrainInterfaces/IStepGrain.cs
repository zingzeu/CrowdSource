using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;
using Config = Zezo.Core.Configuration;

namespace Zezo.Core.GrainInterfaces
{
    public interface IStepGrain : Orleans.IGrainWithGuidKey
    {
        Task<StepStatus> GetStatus();

        Task<StepStopReason> GetStopReason();

        /// <summary>
        /// Whether this Step is the root Step of an Entity's pipeline.
        /// </summary>
        Task<bool> IsRoot();

        // Called by parent/executor

        /// <summary>
        /// Called when the Step is first created.
        /// Should only be called once, when the Step is "Uninitialized".
        /// </summary>
        Task Init(Guid? parentNode, Guid entity, StepNode config);

        /// <summary>
        /// Called when the Step is ready (allowed to start).
        /// For root Step, it will be triggered when the user starts the Entry.
        /// For other Steps, it will be called by their parents when appropriate.
        /// </summary>
        Task Activate();

        /// <summary>
        /// Called when the Step is requested to pause.
        /// Should return immediately (without changing the state).
        /// If "Pause" is not applicable, ignore.
        /// </summary>
        Task Pause();

        /// <summary>
        /// Called when the Step is requested to Resume.
        /// </summary>
        Task Resume();

        /// <summary>
        /// Ons the stopping.
        /// </summary>
        Task Stop();

        /// <summary>
        /// Called by parent to prompt start a task (From Ready -> Working).
        /// </summary>
        Task ForceStart();

        // Called by children
        Task OnChildStarted(Guid caller);
        Task OnChildStopped(Guid caller);
        Task OnChildPaused(Guid caller);
    }

    public enum StepStatus {
        /// <summary>
        /// When the Grain is first created by Orleans, and state has not been set.
        /// </summary>
        Uninitialized = 0,
        
        /// <summary>
        /// Initializing, potentially waiting for children tasks to initialize.
        /// </summary>
        Initializing = 1,

        /// <summary>
        /// Initialized (with all children initialized).
        /// Or after being paused.
        /// </summary>
        Inactive = 2,
        
        /// <summary>
        /// Active means allowed to do work, but no actual computation / human task is ongoing.
        /// This means the Step is safe to pause and no clean up is needed.
        /// </summary>
        Active = 2,
        
        /// <summary>
        /// Ongoing work.
        /// This means when the Step is to be paused, clean up (Pausing phase) is needed.
        /// </summary>
        Working = 3,
        
        /// <summary>
        /// Pausing
        /// </summary>
        Pausing = 4,
        
        Stopping = 5,
        
        /// <summary>
        /// Internal occurred or stopped externally.
        /// </summary>
        Error = 6,
        
        /// <summary>
        /// Stopped without error.
        /// (e.g. completed task or skipped)
        /// </summary>
        StoppedWithSuccess = 7
    }
}
