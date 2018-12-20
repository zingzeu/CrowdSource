using System;
using System.Threading.Tasks;

namespace Zezo.Core.GrainInterfaces
{
    public interface IAssignment : Orleans.IGrainWithGuidKey
    {
        Task<bool> IsAssigned();
        Task<IUser> GetAssignee();
        Task<int> GetTaskId();
        Task<string> GetData();

        // called by executor
        Task OnCancel();

        // user actions
        Task<String> GetDefinition();
        Task<AssignmentFeedback> Submit(AssignmentResult data);  
    }
}
