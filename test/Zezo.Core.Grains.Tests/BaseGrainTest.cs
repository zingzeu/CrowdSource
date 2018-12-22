using System;
using Orleans;
using Orleans.TestingHost;
using Orleans.Hosting;

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
        }
        
        public void Dispose()
        {
            cluster.StopAllSilos();
        }
    }

    internal class TestSiloConfigurator : ISiloBuilderConfigurator
    {
        public void Configure(ISiloHostBuilder hostBuilder)
        {
            hostBuilder.AddMemoryGrainStorage("DevStore");
        }
    }
}
