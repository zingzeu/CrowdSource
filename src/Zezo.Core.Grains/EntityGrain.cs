using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
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
            State.Status = EntityStatus.Initialized;
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

        public Task Start()
        {
            State.Status = EntityStatus.Active;
            return GrainFactory.GetGrain<IStepGrain>(State.PipelineRoot).Activate();
        }
    }
}