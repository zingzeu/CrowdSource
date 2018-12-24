using System;
using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using Zezo.Core.Configuration;
using static Zezo.Core.GrainInterfaces.EntityGrainData;
using Xunit.Abstractions;
using Moq;
using Zezo.Core.GrainInterfaces.Observers;

namespace Zezo.Core.Grains.Tests
{
    /// <summary>
    /// Tests that involve EntityGrain, StepGrain and ProjectGrain,
    /// as well as the different StepLogic's.
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
                            <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""100ms"" />
                            <DummyStep Id=""dummy2"" BeforeStart=""10ms"" Working=""100ms"" />
                        </Sequence.Children>
                    </Sequence>
                </Project.Pipeline>
            </Project>") as ProjectNode;

            var project = GrainFactory.GetGrain<IProjectGrain>(Guid.NewGuid());
            await project.LoadConfig(config);
            var e1K = await project.CreateEntity(1);
            _testOutputHelper.WriteLine($"Entity {e1K} created.");
            var e1 = GrainFactory.GetGrain<IEntityGrain>(e1K);
            Assert.Equal(EntityStatus.Initialized, await e1.GetStatus());

            var mock = new Mock<IStepGrainObserver>();
            mock.Setup(x => x.OnStatusChanged(It.IsAny<Guid>(), It.IsAny<StepStatus>()))
                .Callback((Guid g, StepStatus s) => { _testOutputHelper.WriteLine($"Step {g} changed to {s}"); });
            
            var observer = await GrainFactory.CreateObjectReference<IStepGrainObserver>(mock.Object);

            var seq = await GetStepGrainById(e1, "seq");
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            await seq.Subscribe(observer);
            await dummy1.Subscribe(observer);
            await dummy2.Subscribe(observer);

            // Steps should be initialized but not active yet.
            Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
            Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
            Assert.Equal(StepStatus.Inactive, await seq.GetStatus());

            await e1.Start();
            Assert.Equal(EntityStatus.Active, await e1.GetStatus());

            _testOutputHelper.WriteLine("Started entity");

            // Now dummy1 will be active but dummy 2 will stay inactive
            Assert.Equal(StepStatus.Active, await dummy1.GetStatus());
            Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
            Assert.Equal(StepStatus.Active, await seq.GetStatus());

            await Task.Delay(1000);

            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
            Assert.Equal(StepStatus.StoppedWithSuccess, await seq.GetStatus());

            _testOutputHelper.WriteLine("End of test");
        }

        [Fact]
        public async Task Nested_SequenceStep_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <Sequence Id=""seq1"">
                            <Sequence.Children>
                                <Sequence Id=""seq_inner"">
                                    <Sequence.Children>
                                        <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""100ms"" />
                                        <DummyStep Id=""dummy2"" BeforeStart=""10ms"" Working=""100ms"" />
                                    </Sequence.Children>
                                </Sequence>
                                <DummyStep Id=""dummy3"" BeforeStart=""10ms"" Working=""100ms"" />
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
            await Task.Delay(1000);

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
