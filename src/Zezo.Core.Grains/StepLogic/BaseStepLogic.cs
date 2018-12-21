using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic {
    public abstract class BaseStepLogic : IStepLogic {
        protected StepGrainState state;
        protected readonly Orleans.IGrainFactory grainFactory;
        protected readonly StepNode config;
        protected readonly Guid selfKey;

        public BaseStepLogic(Guid selfKey, StepGrainState state, Orleans.IGrainFactory grainFactory, StepNode config)
        {
            this.selfKey = selfKey;
            this.state = state;
            this.grainFactory = grainFactory;
            this.config = config;
        }

        public abstract Task HandleInit();
        public abstract Task HandleReady();
        public abstract Task HandlePausing();
        public abstract Task HandleResuming();
        public abstract Task HandleStopping();
        public abstract Task HandleChildStarted(Guid caller);
        public abstract Task HandleChildStopped(Guid caller);
        public abstract Task HandleChildPaused(Guid caller);
    }
}