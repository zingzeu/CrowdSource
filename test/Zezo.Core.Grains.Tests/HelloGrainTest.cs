using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Zezo.Core.Grains.Tests
{
    [Collection("Default")]
    public class HelloGrainTest : BaseGrainTest
    {
        
        public HelloGrainTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
        
        [Fact]
        public async Task HelloGrain_Test()
        {
            var hello  = GrainFactory.GetGrain<IHello>(1);
            var greeting = await hello.SayHello("foo");
            Assert.Equal("Hello, foo! from grain #1.", greeting);
        }

        
    }

}
