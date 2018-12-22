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
    /// <summary>
    /// Tests that involve EntityGrain, StepGrain and ProjectGrain,
    /// as well as the different StepLogics.
    /// </summary>
    public class PipelineTest : BaseGrainTest
    {
        
        [Fact]
        public async Task Project_Test()
        {
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

        //[Fact]
        public void Project_Test2()
        {
            var configStr = @"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <Sequence Id=""seq1"">
                            <Sequence.Children>
                                <Sequence Id=""seq_inner"">
                                    <Sequence.Children>
                                        <DummyStep Id=""dummy1"" />
                                        <DummyStep Id=""dummy2"" />
                                    </Sequence.Children>
                                </Sequence>
                                <DummyStep Id=""dummy3"" />
                            </Sequence.Children>
                        </Sequence>
                    </Project.Pipeline>
                </Project>
            ";
        }
    }

}
