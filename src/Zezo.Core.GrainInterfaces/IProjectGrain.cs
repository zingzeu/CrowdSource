using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zezo.Core.Configuration;

namespace Zezo.Core.GrainInterfaces
{
    public interface IProjectGrain : Orleans.IGrainWithGuidKey
    {
        Task LoadConfig(ProjectNode config);
        Task<IReadOnlyList<Guid>> GetEntities();
        Task<IQueue> GetQueue(string id);

        Task<Guid> CreateEntity(object initialData);
    }
}
