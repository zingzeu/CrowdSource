using System;
using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using Zezo.Core.Configuration;
using static Zezo.Core.GrainInterfaces.EntityGrainData;
using Xunit.Abstractions;

namespace Zezo.Core.Grains.Tests
{
    /// <summary>
    /// Tests that involve EntityGrain, StepGrain and ProjectGrain,
    /// as well as the different StepLogics.
    /// </summary>
    [Collection("Default")] // All tests in the same collection, prevents parallel runs
    public class PipelineTest : BaseGrainTest
    {

        public PipelineTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task Basic_Test()
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
            
            var project = GrainFactory.GetGrain<IProjectGrain>(Guid.NewGuid());
            _testOutputHelper.WriteLine("Loading config...");
            await project.LoadConfig(config);
            _testOutputHelper.WriteLine("Creating entity");
            var e1K = await project.CreateEntity(1);
            _testOutputHelper.WriteLine($"Entity {e1K} created.");
            var e1 = GrainFactory.GetGrain<IEntityGrain>(e1K);
            Assert.Equal(EntityStatus.Initialized, await e1.GetStatus());
            _testOutputHelper.WriteLine("Starting entity");
            await e1.Start();
            _testOutputHelper.WriteLine("Started entity");

            await Task.Delay(20*1000);

            var dummy1Key = await e1.GetStepById("dummy1");
            Assert.NotNull(dummy1Key);
            var dummy1 = GrainFactory.GetGrain<IStepGrain>(dummy1Key.Value);
            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
            
            var dummy2Key = await e1.GetStepById("dummy2");
            Assert.NotNull(dummy2Key);
            var dummy2 = GrainFactory.GetGrain<IStepGrain>(dummy2Key.Value);
            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
            
            var seqKey = await e1.GetStepById("seq");
            Assert.NotNull(dummy1Key);
            var seq = GrainFactory.GetGrain<IStepGrain>(seqKey.Value);
            Assert.Equal(StepStatus.StoppedWithSuccess, await seq.GetStatus());
            
            _testOutputHelper.WriteLine("End of test");

        }

        [Fact]
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
