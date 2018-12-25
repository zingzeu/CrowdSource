using System;
using System.Threading.Tasks;
using Orleans.Concurrency;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces.Observers;
using Config = Zezo.Core.Configuration;

namespace Zezo.Core.GrainInterfaces
{
    public interface IStepGrain : Orleans.IGrainWithGuidKey
    {
        [AlwaysInterleave]
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
        
        // For client & unit testing
        Task Subscribe(IStepGrainObserver observer);
        Task Unsubscribe(IStepGrainObserver observer);
        
    }

    [Flags]
    public enum StepStatus {
        
        /**
         * Bits
         *  7     6     5     4     3     2     1     0
         * 7 - Has initialized or not
         * 6 - Stopped or not
         * 5 - Allowed to work
         * 4 - Has Ongoing Work (Busy)
         * 3-0 - Additional types
         *    3 Has Error or not
         *    2
         *    1
         *    0 
         */
        
        /// <summary>
        /// When the Grain is first created by Orleans, and state has not been set.
        /// </summary>
        Uninitialized = 0b0000_0000,
        
        /// <summary>
        /// Initializing, potentially waiting for children tasks to initialize.
        /// </summary>
        Initializing = 0b0001_0000,

        /// <summary>
        /// Initialized (with all children initialized).
        /// Or after being paused.
        /// </summary>
        Inactive = 0b1000_0000,
        
        Paused = 0b1000_0001,
        
        /// <summary>
        /// Active means allowed to do work, but no actual computation / human task is ongoing.
        /// This means the Step is safe to pause and no clean up is needed.
        /// </summary>
        Active = 0b1010_0000,
        
        /// <summary>
        /// Ongoing work.
        /// This means when the Step is to be paused, clean up (Pausing phase) is needed.
        /// </summary>
        Working = 0b1011_0000,
        
        /// <summary>
        /// Pausing
        /// </summary>
        Pausing = 0b1001_0001,
        
        Stopping = 0b1001_0011,
        
        Resuming = 0b1011_0001,
        
        /// <summary>
        /// Internal occurred or stopped externally.
        /// </summary>
        Error = 0b1100_1000,
        
        /// <summary>
        /// Stopped without error.
        /// (e.g. completed task or skipped)
        /// </summary>
        Completed = 0b1100_0000,
        
        Skipped = 0b1100_0001
        
    }
}
