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
            var config = ParseConfig(@"<Project Id=""test"">
                <Project.Pipeline>
                    <Sequence Id=""seq"">
                        <Sequence.Children>
                            <DummyStep Id=""dummy1"" />
                            <DummyStep Id=""dummy2"" />
                        </Sequence.Children>
                    </Sequence>
                </Project.Pipeline>
            </Project>") as ProjectNode;
            
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

            var seq = await GetStepGrainById(e1, "seq");
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");

            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await seq.GetStatus());
            
            _testOutputHelper.WriteLine("End of test");

        }

        [Fact]
        public async Task Nested_SequenceStep_Test()
        {
            var config = ParseConfig( @"
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
            ") as ProjectNode;
            
            var project = GrainFactory.GetGrain<IProjectGrain>(Guid.NewGuid());
            await project.LoadConfig(config);
            var e1K = await project.CreateEntity(1);
            var e1 = GrainFactory.GetGrain<IEntityGrain>(e1K);
            await e1.Start();
            
            // TODO: Use callback to watch for state changes; or build a state change history object; instead of Task.Delay
            await Task.Delay(40*1000);

            var seq1 = await GetStepGrainById(e1, "seq1");
            var seqInner = await GetStepGrainById(e1, "seq_inner");
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            var dummy3 = await GetStepGrainById(e1, "dummy3");

            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy3.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await seq1.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await seqInner.GetStatus());
            
        }
    }

}
