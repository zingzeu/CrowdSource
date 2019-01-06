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
using Polly.Timeout;
using Xunit;
using Xunit.Abstractions;
using Zezo.Core.Configuration;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.Tests
{
    public class BaseGrainTest : IDisposable
    {
        /**
         * Whether or not there is a grain test in action.
         * Used to prevent accidental parallel running.
         */
        private static int testsInAction = 0;
        private static object locker = new object();
        
        private TestCluster cluster;
        protected readonly ITestOutputHelper _testOutputHelper;
        private readonly IParser _parser = new Parser();
    
        protected TestCluster Cluster => cluster;
        protected IGrainFactory GrainFactory => cluster.GrainFactory;

        protected BaseGrainTest(ITestOutputHelper testOutputHelper)
        {
            // detect and prevent parallel Grain Tests.
            lock (locker)
            {
                if (testsInAction > 0)
                {
                    throw new InvalidOperationException("Unit Tests involving Grains and Silos should not be" +
                                                        " run in parallel. Did you forget to a [Collection] attribute?");
                }

                ++testsInAction;
            }
            
            
            _testOutputHelper = testOutputHelper;

            _testOutputHelper.WriteLine("Starting Test Silo...");
            Policy
                .Timeout(60, TimeoutStrategy.Pessimistic, (_,__,___) 
                    => { _testOutputHelper.WriteLine("***********\n\n Timeout starting TestSilo \n\n***********");})
                .Wrap(
                    Policy.Handle<Exception>()
                    .Retry(3, async (_, __) =>
                    {
                        await Task.Delay(3000);
                        _testOutputHelper.WriteLine("Retrying...");
                    })
                )
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
                .Timeout(60, TimeoutStrategy.Pessimistic, (_,__,___) 
                    => { _testOutputHelper.WriteLine("***********\n\n Timeout stopping TestSilo \n\n***********");})
                .Wrap(
                    Policy.Handle<Exception>()
                    .Retry(3, async (_, __) =>
                    {
                        await Task.Delay(3000);
                        _testOutputHelper.WriteLine("Retrying...");
                    })
                )
                .Execute(() => { cluster.StopAllSilos(); });

            lock (locker)
            {
                testsInAction--;
            }
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
        
        /// <summary>
        /// Create a Project with the given config, and instantiates an Entity
        /// under that Project.
        /// </summary>
        /// <param name="projConfig"></param>
        /// <returns>The EntityGrain</returns>
        protected async Task<IEntityGrain> CreateSingleEntityProject(ProjectNode projConfig)
        {
            var project = GrainFactory.GetGrain<IProjectGrain>(Guid.NewGuid());
            await project.LoadConfig(projConfig);
            var e1K = await project.CreateEntity(1);
            var e1 = GrainFactory.GetGrain<IEntityGrain>(e1K);
            return e1;
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
