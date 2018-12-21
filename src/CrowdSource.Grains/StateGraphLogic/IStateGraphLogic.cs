using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace CrowdSource.Grains.StateGraphLogic
{
    public interface IStateGraphLogic
    {
        Task HandleCancel();

        Task HandleFinish();

        Task HandleChildNodeFinished(Guid childKey);

        Task HandleTaken();

        Task HandleChildNodeTaken(Guid childKey);

        Task HandleStart();
    }
}
