using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic {
    public abstract class BaseStepLogic : IStepLogic {
        protected readonly IContainer container;
        protected ILogger Logger => container.Logger;

        protected BaseStepLogic(IContainer container)
        {
            this.container = container;
        }

        public virtual Task HandleInit()
        {
            container.State.Id = container.State.Config.Id;
            return Task.CompletedTask;
        }
        
        public abstract Task HandleReady();
        public abstract Task HandlePausing();
        public abstract Task HandleResuming();
        public abstract Task HandleForceStart();
        public abstract Task HandleStopping();

        
        public abstract Task HandleChildStarted(Guid caller);
        public abstract Task HandleChildStopped(Guid caller);
        public abstract Task HandleChildPaused(Guid caller);

    }
}