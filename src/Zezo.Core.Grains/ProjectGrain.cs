using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Steps;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.Grains.StepLogic;

namespace Zezo.Core.Grains
{
    [StorageProvider(ProviderName="DevStore")]
    public class ProjectGrain : Orleans.Grain<ProjectGrainData>, IProjectGrain
    {
        private ILogger<ProjectGrain> logger;

        public ProjectGrain(ILogger<ProjectGrain> logger)
        {
            this.logger = logger;
        }
        public async Task<Guid> CreateEntity(object initialData)
        {
            var newEntityKey = Guid.NewGuid();
            var newEntity = GrainFactory.GetGrain<IEntityGrain>(newEntityKey);
            await newEntity.Init(this.GetPrimaryKey(), State.Config);
            State.Entities.Add(newEntityKey);
            await WriteStateAsync();
            return newEntityKey;
        }

        public Task<ProjectNode> GetConfig()
        {
            return Task.FromResult(State.Config);
        }

        public Task<IReadOnlyList<Guid>> GetEntities()
        {
            return Task.FromResult(State.Entities as IReadOnlyList<Guid>);
        }

        public Task<Guid> GetGuid()
        {
            return Task.FromResult(this.GetPrimaryKey());
        }

        public Task<IQueue> GetQueue(string id)
        {
            throw new NotImplementedException();
        }

        public Task LoadConfig(ProjectNode config)
        {
            State.Config = config;
            logger.LogInformation($"Config loaded {config} {config.Id}");
            return WriteStateAsync();
        }
    }
}