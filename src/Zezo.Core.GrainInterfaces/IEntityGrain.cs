using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.GrainInterfaces
{
    public interface IEntityGrain : Orleans.IGrainWithGuidKey
    {

        // Pause
        // Resume
        // Archive
        // Delete
        Task Init(Guid project, ProjectNode projectConfig);

        Task<Guid> SpawnChild(StepNode config, Guid parent);
        Task<Guid> SpawnRoot(StepNode config);

        Task Start();
    }
}
