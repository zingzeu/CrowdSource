using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.GrainInterfaces
{
    public interface IStep : Orleans.IGrainWithGuidKey
    {
        Task<StepStatus> GetStatus();
        Task<bool> IsRoot();

        // Called by parent/executor
        Task OnInit(StepNode config);
        Task OnReady();
        Task OnPausing();
        Task OnResuming();
        Task OnStopping();
        
        // Called by child
        Task OnChildStarted(IStep caller);
        Task OnChildCompleted(IStep caller);
        Task OnChildPaused(IStep caller);
    }

    public enum StepStatus {
        Uninitialized,
        Initialized,
        Ready,
        Working,
        Pausing,
        Paused,
        Stopped
    }
}
