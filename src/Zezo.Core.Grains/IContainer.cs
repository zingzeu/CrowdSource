using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.Grains.Datastores.Scripting;

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
        
        IStepGrain SelfReference { get;  }

        // Status changes
        Task CompleteSelf(bool success);
        Task MarkSelfBusy();

        Task MarkSelfIdle();

        /// <summary>
        /// Spawns a new StepGrain in the EntityGrain.
        /// </summary>
        /// <param name="childConfig">The configuration for the new Step.</param>
        /// <returns>The Guid of the newly created StepGrain.</returns>
        Task<Guid> SpawnStep(StepNode childConfig);
        
        // Datastore access
        Task<DatastoreRegistry> GetDatastoreRegistry();
    }
}