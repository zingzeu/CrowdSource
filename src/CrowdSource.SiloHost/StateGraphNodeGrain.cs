using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrowdSource.GrainInterfaces;

namespace CrowdSource.Grains
{
    public class StateGraphNodeGrain : Orleans.Grain, IStateGraphNode
    {
        Task IStateGraphNode.OnCancel()
        {
            throw new NotImplementedException();
        }

        Task IStateGraphNode.OnChildNodeFinished(Guid childKey)
        {
            throw new NotImplementedException();
        }

        Task IStateGraphNode.OnFinish()
        {
            throw new NotImplementedException();
        }

        Task IStateGraphNode.OnStart()
        {
            throw new NotImplementedException();
        }

        Task IStateGraphNode.OnTaken()
        {
            throw new NotImplementedException();
        }
    }
}
