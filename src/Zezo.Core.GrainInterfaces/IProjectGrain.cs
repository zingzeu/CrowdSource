using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zezo.Core.Configuration;

namespace Zezo.Core.GrainInterfaces
{
    public interface IProjectGrain : Orleans.IGrainWithGuidKey
    {
        Task LoadConfig(ProjectNode config);
        Task<Guid> GetGuid();
        Task<IReadOnlyList<IEntityGrain>> GetEntities();
        Task<IQueue> GetQueue(string id);
    }
}
