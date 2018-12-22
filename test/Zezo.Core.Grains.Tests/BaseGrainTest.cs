using System;
using Orleans;
using Orleans.TestingHost;
using Orleans.Hosting;
using Microsoft.Extensions.Logging;

namespace Zezo.Core.Grains.Tests
{
    public class BaseGrainTest : IDisposable
    {
        private readonly TestCluster cluster;
    
        protected TestCluster Cluster => cluster;
        protected IGrainFactory GrainFactory => cluster.GrainFactory;

        protected BaseGrainTest()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();
            cluster = builder.Build();
            cluster.Deploy();
            Console.WriteLine("\n\n ******* Silo Up \n\n");
        }
        
        public void Dispose()
        {
            cluster.StopAllSilos();
            Console.WriteLine("\n\n ******* Silo Down \n\n");
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
