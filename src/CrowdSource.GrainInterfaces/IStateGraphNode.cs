using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSource.GrainInterfaces
{
    public interface IStateGraphNode : Orleans.IGrainWithGuidKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task OnStart();

        Task OnTaken();

        /// <summary>
        /// Cancel all sub-task and self
        /// </summary>
        /// <returns></returns>
        Task OnCancel();

        /// <summary>
        /// Inform parent node
        /// </summary>
        /// <returns></returns>
        Task OnFinish();

        /// <summary>
        /// Called when one of the child node is done.
        /// </summary>
        /// <returns></returns>
        Task OnChildNodeFinished(Guid childKey);

        /// <summary>
        /// Initialise theStateGraphNode.
        /// Should only be called once.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task Initialise(string type);


    }
}
