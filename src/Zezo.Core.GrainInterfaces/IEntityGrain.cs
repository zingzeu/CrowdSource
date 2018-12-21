using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.GrainInterfaces
{
    public interface IEntityGrain : Orleans.IGrainWithGuidKey
    {
        Task<IProjectGrain> GetProject();

        // Pause
        // Resume
        // Archive
        // Delete

        Task<Guid> SpawnChild(StepNode config, Guid parent);
    }
}
