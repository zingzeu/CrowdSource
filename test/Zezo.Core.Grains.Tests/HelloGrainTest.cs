using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;

namespace Zezo.Core.Grains.Tests
{
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
