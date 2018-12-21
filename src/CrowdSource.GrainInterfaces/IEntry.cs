using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSource.GrainInterfaces
{
    public interface IEntry : Orleans.IGrainWithGuidKey
    {
        Task GetMetadata();

        /// <summary>
        /// The ID of the project which this Entry belongs to.
        /// </summary>
        /// <returns>Task\<int></returns>
        Task<int> GetProjectId();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task KickOffStateGraph();

        Task<Dictionary<StateDefinition, object>> GetEntryState();

        Task GetCurrentEntryFields();

        Task<List<FieldCommit>> GetFieldCommits();
    }
}
