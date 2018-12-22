using System;
using System.Threading.Tasks;
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

        /// <summary>
        /// Spawns a new StepGrain in the EntityGrain.
        /// </summary>
        /// <param name="childConfig">The configuration for the new Step.</param>
        /// <returns>The Guid of the newly created StepGrain.</returns>
        Task<Guid> SpawnStep(StepNode childConfig);
    }
}