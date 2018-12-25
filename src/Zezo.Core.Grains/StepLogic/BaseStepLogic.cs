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

        public virtual Task OnInit()
        {
            container.State.Id = container.State.Config.Id;
            return Task.CompletedTask;
        }
        
        public abstract Task OnActivate();
        public abstract Task OnPausing();
        public abstract Task OnResuming();
        public abstract Task HandleForceStart();
        public abstract Task OnStopping();

        
        public abstract Task HandleChildStarted(Guid caller);
        public abstract Task HandleChildIdle(Guid caller);

        public abstract Task HandleChildStopped(Guid caller);

    }
}