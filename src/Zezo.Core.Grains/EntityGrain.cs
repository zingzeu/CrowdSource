using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Datastore;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.GrainInterfaces.Datastores;
using static Zezo.Core.GrainInterfaces.EntityGrainData;

namespace Zezo.Core.Grains
{
    [StorageProvider(ProviderName = "DevStore")]
    public class EntityGrain : Grain<EntityGrainData>, IEntityGrain
    {
        private readonly ILogger<HelloGrain> logger;

        public EntityGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        public Task<EntityStatus> GetStatus()
        {
            return Task.FromResult(this.State.Status);
        }

        public async Task Init(Guid project, ProjectNode projectConfig)
        {
            State.ProjectKey = project;
            logger.LogInformation($"Entity {this.GetPrimaryKey()} created from project {project}");
            logger.LogInformation("Spawning first child");
            State.PipelineRoot = await SpawnRoot(projectConfig.Pipeline);
            // init data stores
            await InitDatastores(projectConfig.Datastores);
            State.Status = EntityStatus.Initialized;
        }

        private async Task InitDatastores(IReadOnlyList<DatastoreNode> datastoreConfigs)
        {
            foreach (var datastoreConfig in datastoreConfigs)
            {
                // TODO: ensure unique Datastore Id.
                switch (datastoreConfig)
                {
                    case SimpleStoreNode simpleStoreConfig:
                        var simpleStore = GrainFactory.GetGrain<ISimpleStoreGrain>(this.GetPrimaryKey(), simpleStoreConfig.Id, null);
                        await simpleStore.Init(simpleStoreConfig.Fields.Select(x => x.ToFieldDef()).ToList());
                        break;
                    default:
                        logger.LogWarning($"Unknown DataStore Type {datastoreConfig.GetTagName()}, ignoring...");
                        continue;
                }
            }
        }

        public async Task<Guid> SpawnChild(StepNode config, Guid parentStep)
        {
            var newKey = Guid.NewGuid();
            var newStep = GrainFactory.GetGrain<IStepGrain>(newKey);
            await newStep.Init(parentStep, this.GetPrimaryKey(), config);
            State.Steps[config.Id] = newKey;
            logger.LogInformation($"spawned child step id={config.Id}");
            return newKey;
        }

        public async Task<Guid> SpawnRoot(StepNode config)
        {
            var newKey = Guid.NewGuid();
            var newStep = GrainFactory.GetGrain<IStepGrain>(newKey);
            await newStep.Init(null, this.GetPrimaryKey(), config);
            State.Steps[config.Id] = newKey;
            logger.LogInformation($"spawned root step id={config.Id}");
            return newKey;
        }

        public Task<Guid?> GetStepById(string id)
        {
            return State.Steps.TryGetValue(id, out var guid) 
                ? Task.FromResult((Guid?)guid) : Task.FromResult((Guid?)null);
        }

        public Task Start()
        {
            State.Status = EntityStatus.Active;
            return GrainFactory.GetGrain<IStepGrain>(State.PipelineRoot).Activate();
        }
    }

    public static class Extension
    {
        public static FieldDef ToFieldDef(this FieldDefNode config)
        {
            Type type = null;
            switch (config.Type)
            {
                case "String":
                    type = typeof(string);
                    break;
                case "Boolean":
                    type = typeof(bool);
                    break;
                case "Integer":
                    type = typeof(int);
                    break;
                default:
                    throw new Exception($"Unknown field type '{config.Type}'.");
            }
            return new FieldDef()
            {
                Name = config.Id,
                Nullable = config.Nullable,
                Type = type
            };
        }
    }
}