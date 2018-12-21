using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrowdSource.Grains.State;
using CrowdSource.GrainInterfaces;

namespace CrowdSource.Grains.StateGraphLogic
{
    public abstract class BaseSGLogic : IStateGraphLogic
    {
        protected StateGraphNodeState _state;
        protected Orleans.IGrainFactory _grainFactory;
        protected readonly Guid _selfKey;

        public BaseSGLogic(Guid selfKey, StateGraphNodeState state, Orleans.IGrainFactory grainFactory)
        {
            this._selfKey = selfKey;
            this._state = state;
            this._grainFactory = grainFactory;
        }

        public abstract Task HandleChildNodeFinished(Guid childKey);
        public Task HandleFinish()
        {
            if (this.HasParent())
            {
                this.GetParentNode().OnChildNodeFinished(_selfKey);
            }
            return Task.CompletedTask;
        }

        protected IStateGraphNode GetSelf()
        {
            return this._grainFactory.GetGrain<IStateGraphNode>(_selfKey);
        }

        protected bool HasParent()
        {
            return this._state.ParentNode != null;
        }

        protected IStateGraphNode GetParentNode()
        {
            return this._grainFactory.GetGrain<IStateGraphNode>(_state.ParentNode);
        }

        public Task HandleTaken()
        {
            // do nothing
            return Task.CompletedTask;
        }

        Task IStateGraphLogic.HandleCancel()
        {
            // cancel all sub tasks
            foreach (var item in _state.ChildNodes)
            {
                var child = _grainFactory.GetGrain<IStateGraphNode>(item);
                child.OnCancel();
            }
            return Task.CompletedTask;
        }

        public Task HandleChildNodeTaken(Guid childKey)
        {
            throw new NotImplementedException();
        }

        public abstract Task HandleStart();
    }
}
