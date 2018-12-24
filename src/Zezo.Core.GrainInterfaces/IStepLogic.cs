﻿using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.GrainInterfaces
{
    public interface IStepLogic
    {
        Task OnInit();
        Task OnActivate();
        Task HandlePausing();
        Task HandleResuming();
        Task HandleStopping();

        Task HandleForceStart();

        // Called by child
        Task HandleChildStarted(Guid caller);
        Task HandleChildStopped(Guid caller);
        Task HandleChildPaused(Guid caller); 
    }
}
