using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrowdSource.Core;

namespace CrowdSource.GrainInterfaces
{
    /// <summary>
    /// Represents a human-input assignment
    /// </summary>
    public interface IAssignment
    {
        Task<AssignmentState> GetState();
        Task<AssignmentAbandonReason> GetAbandonReason();

        Task AssignScore(double score);
        Task<AssignmentResult> GetAssignmentResult();
    }
}
