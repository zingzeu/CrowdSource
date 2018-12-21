using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace CrowdSource.GrainInterfaces
{
    public interface IExclusionHandle
    {
        Task<Guid> GetKey();
        Task CancelAllTasksButThis(Guid guid);
        Task RegisterTask(Guid guid);

    }
}
