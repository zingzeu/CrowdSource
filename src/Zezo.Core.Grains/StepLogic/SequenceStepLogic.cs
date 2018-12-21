using System;
using System.Threading.Tasks;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.StepLogic
{
    public sealed class SequenceStepLogic : BaseStepLogic
    {
        private SequenceNode seqConfig;
        public SequenceStepLogic(Guid selfKey, StepGrainState state, Orleans.IGrainFactory grainFactory, StepNode config) 
            : base(selfKey, state, grainFactory, config)
        {
            seqConfig = this.config as SequenceNode;
        }

        public override Task HandleChildPaused(Guid caller)
        {
            throw new NotImplementedException();
        }

        public override Task HandleChildStarted(Guid caller)
        {
            throw new NotImplementedException();
        }

        public override Task HandleChildStopped(Guid caller)
        {
            throw new NotImplementedException();
        }

        public override Task HandleInit()
        {
            throw new NotImplementedException();
        }

        public override Task HandlePausing()
        {
            throw new NotImplementedException();
        }


        public override Task HandleReady()
        {
            throw new NotImplementedException();
        }


        public override Task HandleResuming()
        {
            throw new NotImplementedException();
        }

        public override Task HandleStopping()
        {
            throw new NotImplementedException();
        }
    }
}
