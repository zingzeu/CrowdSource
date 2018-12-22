using System;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains {
    /// <summary>
    /// Services that the executing container (StepGrain) provides to StepLogic.  
    /// </summary>
    public interface IContainer {
        IStepGrain GetStepGrain(Guid key);
        IStepGrain GetParentGrain();

        /// <summary>
        /// Gets the Entity that this Step is part of.
        /// </summary>
        /// <returns></returns>
        IEntityGrain GetEntityGrain();

        Guid SelfKey { get; }
        ILogger Logger { get; }
        StepStatus Status { get; }
        StepGrainData State { get; }

        // Status changes
        void CompleteSelf(bool success);
        void MarkSelfStarted();

        void SpawnChild(StepNode childConfig);
    }
}