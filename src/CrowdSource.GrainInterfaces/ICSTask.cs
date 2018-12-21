using System;
using System.Collections.Generic;
using System.Text;
using CrowdSource.Core;
using System.Threading.Tasks;

namespace CrowdSource.GrainInterfaces
{
    /// <summary>
    /// An instance of CrowdSource Task.
    /// </summary>
    public interface ICSTask
    {
        Task<TaskDefinition> GetTaskDefinition();
        Task AssignToUser();
        Task Finished();
        /// <summary>
        /// An immutable map of fields that are used as inputs.
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<InputDefinition, object>> GetInputs();

        Task<TaskState> GetTaskState();

        Task<HashSet<Guid>> GetExclusionKeys();

        Task SetExclusionKey(Guid guid);

    }
}
