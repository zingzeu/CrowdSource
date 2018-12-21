using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrowdSource.GrainInterfaces;
using CrowdSource.Grains.State;

namespace CrowdSource.Grains.StateGraphLogic
{
    public class EitherSGLogic : BaseSGLogic
    {
        public EitherSGLogic(Guid selfKey, StateGraphNodeState state, Orleans.IGrainFactory gfc) : base(selfKey, state, gfc)
        {
            
        }

        public new Task HandleTaken()
        {
            //
            if (this.HasParent())
            {
                this.GetParentNode().OnChildNodeFinished(_selfKey);
            }
            return Task.CompletedTask;
        }

        public new Task HandleChildNodeTaken(Guid childKey)
        {
            // For any subtask taken, we will just finish all;

            foreach (var item in _state.ChildNodes)
            {
                if (childKey != item)
                {
                    _grainFactory.GetGrain<IStateGraphNode>(item).OnCancel();
                }
            }

            this.GetSelf().OnTaken();
            return Task.CompletedTask;
        }

        public override Task HandleChildNodeFinished(Guid childKey)
        {

            this.GetSelf().OnFinish();
            return Task.CompletedTask;
        }

        public override Task HandleStart()
        {
            /// Create the subnodes
        }
    }
}
