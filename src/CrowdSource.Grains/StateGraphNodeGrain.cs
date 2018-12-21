using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrowdSource.GrainInterfaces;
using CrowdSource.Grains.State;
using CrowdSource.Grains.StateGraphLogic;
using Orleans;

namespace CrowdSource.Grains
{
    public class StateGraphNodeGrain : Orleans.Grain<StateGraphNodeState>, IStateGraphNode
    {
        private IStateGraphLogic logic;

        Task IStateGraphNode.Initialise(string type)
        {
            this.State = new StateGraphNodeState();
            this.State.Type = type;
            InitialiseLogic();
            return Task.CompletedTask;
        }

        private void InitialiseLogic()
        {
            if (this.State != null)
            {
                switch (this.State.Type)
                {
                    case "WaitAll":
                        this.logic = new WaitAllSGLogic(this.GetPrimaryKey(), this.State, this.GrainFactory);
                        break;
                    default:
                        throw new Exception("Unrecognised Node Type");
                }
            }
        }

        Task IStateGraphNode.OnCancel()
        {
            if (logic != null)
            {
                logic.HandleCancel();
            }
            return Task.CompletedTask;
        }

        Task IStateGraphNode.OnChildNodeFinished(Guid childKey)
        {
            if (logic != null)
            {
                logic.HandleChildNodeFinished(childKey);
            }
            return Task.CompletedTask;
        }

        Task IStateGraphNode.OnFinish()
        {
            if (logic != null)
            {
                logic.HandleFinish();
            }
            return Task.CompletedTask;
        }

        Task IStateGraphNode.OnStart()
        {
            if (logic != null)
            {
                logic.HandleStart();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the task is assigned.
        /// Used for mutually-exclusive tasks scenarioes where 
        /// all other tasks gets cancelled once one task is assigned.
        /// </summary>
        /// <returns></returns>
        Task IStateGraphNode.OnTaken()
        {
            if (logic != null)
            {
                logic.HandleTaken();
            }
            return Task.CompletedTask;
        }
    }
}
