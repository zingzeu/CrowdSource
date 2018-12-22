using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using System;
using Zezo.Core.Configuration;
using static Zezo.Core.GrainInterfaces.EntityGrainData;

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

        [Fact]
        public async Task Project_Test()
        {
            Console.WriteLine("=================\n\n\n Test1 \n================\n\n\n\n\n");
            var configStr = @"<Project Id=""test"">
                <Project.Pipeline>
                    <Sequence Id=""seq"">
                        <Sequence.Children>
                            <DummyStep Id=""dummy1"" />
                            <DummyStep Id=""dummy2"" />
                        </Sequence.Children>
                    </Sequence>
                </Project.Pipeline>
            </Project>";
            var parser = new Parser();
            var config = parser.ParseXmlString(configStr) as ProjectNode;
            Console.WriteLine("=================\n\n\n parsed \n================\n\n\n\n\n");
            
            Console.WriteLine("Creating project...");
            var project = GrainFactory.GetGrain<IProjectGrain>(Guid.NewGuid());
            Console.WriteLine("Loading config...");
            await project.LoadConfig(config);
            Console.WriteLine("Creating entity");
            var e1K = await project.CreateEntity(1);
            Console.WriteLine($"Entity {e1K} created.");
            var e1 = GrainFactory.GetGrain<IEntityGrain>(e1K);
            Assert.Equal(EntityStatus.Initialized, await e1.GetStatus());
            Console.WriteLine("Starting entity");
            await e1.Start();
            Console.WriteLine("Started entity");
        }

        [Fact]
        public async Task HelloGrain_Test2()
        {
            Console.WriteLine("=================\n\n\n HelloTest2 \n================\n\n\n\n\n");

            var hello  = GrainFactory.GetGrain<IHello>(2);
            var greeting = await hello.SayHello("foo");
            Assert.Equal("Hello, foo! from grain #2.", greeting);
        }

    }

}
