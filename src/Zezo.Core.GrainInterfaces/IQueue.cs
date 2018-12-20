using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zezo.Core.GrainInterfaces
{
    public interface IQueue : Orleans.IGrainWithGuidKey
    {
        Task<IProject> GetProject();
        Task<IAssignment> RequestNextItem();
        Task<bool> PublishItem(IAssignment a);
        Task<bool> UnpublishItem(IAssignment a);

        Task<int> UnpublishAll();

        Task<IReadOnlyList<IAssignment>> Enumerate();

        Task<int> ItemsCount();
    }
}
