using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.TestingHost;
using Orleans.Hosting;
using Xunit;
using Xunit.Abstractions;
using Zezo.Core.Configuration;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.Tests
{
    public class BaseGrainTest : IDisposable
    {
        private readonly TestCluster cluster;
        protected readonly ITestOutputHelper _testOutputHelper;
        private readonly IParser _parser = new Parser();
    
        protected TestCluster Cluster => cluster;
        protected IGrainFactory GrainFactory => cluster.GrainFactory;

        protected BaseGrainTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
            cluster = builder.Build();
            cluster.Deploy();
        }
        
        public void Dispose()
        {
            cluster.StopAllSilos();
        }
        
        protected async Task<IStepGrain> GetStepGrainById(IEntityGrain entityGrain, string id)
        {
            var key = await entityGrain.GetStepById(id);
            Assert.NotNull(key);
            var stepGrain = GrainFactory.GetGrain<IStepGrain>(key.Value);
            return stepGrain;
        }

        protected ConfigurationNode ParseConfig(string configStr)
        {
            return _parser.ParseXmlString(configStr);
        }
    }

    internal class TestSiloConfigurator : ISiloBuilderConfigurator
    {
        public void Configure(ISiloHostBuilder hostBuilder)
        {
            hostBuilder
                .AddMemoryGrainStorage("DevStore")
#if SiloLogging
                .ConfigureLogging(logging => 
                    logging.AddConsole());
#else
                ;
#endif
        }
    }
}
