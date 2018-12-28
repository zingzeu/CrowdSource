//#define SiloLogging

using System;
using System.Threading.Tasks;
#if SiloLogging
using Microsoft.Extensions.Logging;
#endif
using Orleans;
using Orleans.TestingHost;
using Orleans.Hosting;
using Polly;
using Xunit;
using Xunit.Abstractions;
using Zezo.Core.Configuration;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.Tests
{
    public class BaseGrainTest : IDisposable
    {
        private TestCluster cluster;
        protected readonly ITestOutputHelper _testOutputHelper;
        private readonly IParser _parser = new Parser();
    
        protected TestCluster Cluster => cluster;
        protected IGrainFactory GrainFactory => cluster.GrainFactory;

        protected BaseGrainTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _testOutputHelper.WriteLine("Starting Test Silo...");
            Policy
                .Handle<Exception>()
                .Retry(3, async (_, __) =>
                {
                    await Task.Delay(3000);
                    _testOutputHelper.WriteLine("Retrying...");
                })
                .Execute(() =>
                {
                    var builder = new TestClusterBuilder();
                    builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
                    cluster = builder.Build();
                    cluster.Deploy();
                });

        }
        
        public void Dispose()
        {
            _testOutputHelper.WriteLine("Stopping Test Silo...");
            Policy
                .Handle<Exception>()
                .Retry(3, async (_, __) =>
                {
                    await Task.Delay(3000);
                    _testOutputHelper.WriteLine("Retrying...");
                })
                .Execute(() => { cluster.StopAllSilos(); });

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
                .EnableDirectClient()
#if SiloLogging
                .ConfigureLogging(logging => 
                    logging.AddConsole());
#else
                ;
#endif
        }
    }
}
