using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.GrainInterfaces.Datastores;
using Zezo.Core.Grains.Datastores.Scripting;

namespace Zezo.Core.Grains
{
    public partial class StepGrain : IContainer
    {
        /// <summary>
        /// Private copy of GrainFactory. Avoids calling Orleans Runtime on non-Orleans threads.
        /// </summary>
        private IGrainFactory _grainFactory = null;
        public ILogger Logger => logger;

        public StepStatus Status => State?.Status ?? StepStatus.Uninitialized;

        StepGrainData IContainer.State => this.State;

        private IStepGrain _selfReference = null;
        IStepGrain IContainer.SelfReference
        {
            get
            {
                if (_selfReference == null)
                {
                    _selfReference = GrainFactory.GetGrain<IStepGrain>(SelfKey);
                }
                return _selfReference;
            }
        }

        public Guid SelfKey => this.GetPrimaryKey();

        public async Task CompleteSelf(bool success)
        {
            if (State.Status == StepStatus.Inactive ||
                State.Status == StepStatus.ActiveIdle || 
                State.Status  == StepStatus.Working)
            {
                await ChangeStatus(success ? StepStatus.Completed : StepStatus.Error);
            }
            else 
            {
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Stopped.");
            }
        }

        public IStepGrain GetParentGrain()
        {
            if (State.ParentNode != null) {
                return GetStepGrain(State.ParentNode.GetValueOrDefault());
            } else {
                return null;
            }
        }

        public async Task MarkSelfBusy()
        {
            if (State.Status == StepStatus.ActiveIdle) {
                await ChangeStatus(StepStatus.Working);
            }
            else 
            {
                logger.LogError($"Cannot change from status {State.Status} to Started.");
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Started.");
            }
        }

        public async Task MarkSelfIdle()
        {
            if (State.Status == StepStatus.ActiveIdle)
            {
                // do nothing
            }
            else if (State.Status == StepStatus.Working)
            {
                await ChangeStatus(StepStatus.ActiveIdle);
            }
            else
            {
                throw new InvalidOperationException($"Cannot change from status {State.Status} to Active (Idle)");
            }
        }

        public Task<Guid> SpawnStep(StepNode childConfig)
        {
            var entityGrain = GetEntityGrain();
            return entityGrain.SpawnChild(childConfig, SelfKey);
            
        }

        public async Task<DatastoreRegistry> GetDatastoreRegistry()
        {
            var dataStores = await GetEntityGrain().GetDatastores();
            var registryBuilder = new DatastoreRegistry.Builder();
            foreach (var pair in dataStores)
            {
                var id = pair.Key;
                var type = pair.Value;
                switch (type)
                {
                    case "SimpleStore":
                        var grain = GrainFactory.GetGrain<ISimpleStoreGrain>(State.Entity, id, null);
                        var proxy = await grain.GetProxy();
                        registryBuilder.AddDatastoreProxy(id, proxy);
                        break;
                    default:
                        break;
                }
            }

            return registryBuilder.Build();
        }

        public IEntityGrain GetEntityGrain()
        {
            return GrainFactory.GetGrain<IEntityGrain>(State.Entity);
        }
        
    }
}