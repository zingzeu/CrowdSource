using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using System;
using Zezo.Core.Configuration;
using static Zezo.Core.GrainInterfaces.EntityGrainData;

namespace Zezo.Core.Grains.Tests
{
    [Collection("Default")]
    public class HelloGrainTest : BaseGrainTest
    {
        
        [Fact]
        public async Task HelloGrain_Test()
        {
            var hello  = GrainFactory.GetGrain<IHello>(1);
            var greeting = await hello.SayHello("foo");
            Assert.Equal("Hello, foo! from grain #1.", greeting);
        }

    }

}
