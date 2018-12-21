using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrowdSource.Grains.State;
namespace CrowdSource.Grains.StateGraphLogic
{
    public class WaitAllSGLogic : BaseSGLogic
    {
        
        public WaitAllSGLogic(Guid selfKey, StateGraphNodeState state, Orleans.IGrainFactory gfc) : base(selfKey, state, gfc)
        {

        }

        public override Task HandleChildNodeFinished(Guid childKey)
        {
            _state.Annotations["Finished_"+childKey] = true;
            int finishedCount = 0;
            foreach (var item in _state.ChildNodes)
            {
                if (_state.Annotations.ContainsKey("Finished_" + item.ToString()))
                {
                    ++finishedCount;
                }
            }

            if (finishedCount >= _state.ChildCount)
            {
                this.GetSelf().OnFinish();
            }
            return Task.CompletedTask;
        }

    }
}
