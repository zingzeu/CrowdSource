using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using System;

namespace Zezo.Core.Grains.Tests
{
    public class HelloGrainTest : BaseGrainTest
    {
        
        [Fact]
        public async Task HelloGrain_Test()
        {
            Console.WriteLine("=================\n\n\n HelloTest \n================\n\n\n\n\n");

            var hello  = GrainFactory.GetGrain<IHello>(1);
            var greeting = await hello.SayHello("foo");
            Assert.Equal("Hello, foo! from grain #1.", greeting);
        }

    }

}
