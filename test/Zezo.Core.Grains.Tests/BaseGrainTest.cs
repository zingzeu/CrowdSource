using System;
using Xunit;
using Orleans;
using Orleans.TestingHost;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using Zezo.Core.Configuration;
using static Zezo.Core.GrainInterfaces.EntityGrainData;
using Orleans.Hosting;

namespace Zezo.Core.Grains.Tests
{
    public class BaseGrainTest : IDisposable
    {
        private readonly TestCluster cluster;
    
        protected TestCluster Cluster => cluster;
        protected IGrainFactory GrainFactory => cluster.GrainFactory;
        public BaseGrainTest()
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
