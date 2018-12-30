using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Concurrency;
using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Steps;
using static Zezo.Core.GrainInterfaces.EntityGrainData;

namespace Zezo.Core.GrainInterfaces
{
    public interface IEntityGrain : Orleans.IGrainWithGuidKey
    {

        [AlwaysInterleave]
        Task<EntityStatus> GetStatus();
        
        // Pause
        // Resume
        // Archive
        // Delete
        Task Init(Guid project, ProjectNode projectConfig);
        
        Task<Guid> SpawnChild(StepNode config, Guid parent);
        Task<Guid> SpawnRoot(StepNode config);

        /// <summary>
        /// Gets a Step in the Entity by its Id (as defined in the Project Pipeline config).
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Guid of the StepGrain</returns>
        Task<Guid?> GetStepById(string id);

        [AlwaysInterleave]
        Task<IDictionary<string, string>> GetDatastores();

        Task Start();
    }
}
