using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zezo.Core.GrainInterfaces;
using System.Threading.Tasks;
using Zezo.Core.Configuration;
using static Zezo.Core.GrainInterfaces.EntityGrainData;
using Xunit.Abstractions;
using Moq;
using Orleans;
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
        public async Task Basic_Sequence_Test()
        {
            var config = ParseConfig(@"<Project Id=""test"">
                <Project.Pipeline>
                    <Sequence Id=""seq"">
                        <Sequence.Children>
                            <DummyStep Id=""dummy1"" BeforeStart=""1000ms"" Working=""100ms"" />
                            <DummyStep Id=""dummy2"" BeforeStart=""100ms"" Working=""100ms"" />
                        </Sequence.Children>
                    </Sequence>
                </Project.Pipeline>
            </Project>") as ProjectNode;

            var e1 = await CreateSingleEntityProject(config);
                
            Assert.Equal(EntityStatus.Initialized, await e1.GetStatus());

            var seq = await GetStepGrainById(e1, "seq");
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(seq, "seq");
                await observer.ObserverStep(dummy1, "dummy1");
                await observer.ObserverStep(dummy2, "dummy2");
            
                // Steps should be initialized but not active yet.
                Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Inactive, await seq.GetStatus());

                await e1.Start();
                var now1 = DateTime.Now;
                _testOutputHelper.WriteLine($"{now1.Minute}:{now1.Second}.{now1.Millisecond} - Started");

                Assert.Equal(EntityStatus.Active, await e1.GetStatus());

                // Now dummy1 will be active but dummy 2 will stay inactive
                _testOutputHelper.WriteLine("Now test for Inactive dummy2 and Active dummy1");
                Assert.Equal(StepStatus.Active, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Active, await seq.GetStatus());

                await observer.WaitUntilStatus("seq", s => s == StepStatus.StoppedWithSuccess);

                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await seq.GetStatus());

                _testOutputHelper.WriteLine("End of test");
            }
        }

        [Fact]
        public async Task Nested_Sequence_Test()
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

            var e1 = await CreateSingleEntityProject(config);
            var seq1 = await GetStepGrainById(e1, "seq1");
            var seqInner = await GetStepGrainById(e1, "seq_inner");
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            var dummy3 = await GetStepGrainById(e1, "dummy3");
            
            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(seq1, "seq1");
                await observer.ObserverStep(seqInner, "seq_inner");
                await observer.ObserverStep(dummy1, "dummy1");
                await observer.ObserverStep(dummy2, "dummy2");
                await observer.ObserverStep(dummy3, "dummy3");
                
                await e1.Start();

                await observer.WaitUntilStatus("seq1", s => s == StepStatus.StoppedWithSuccess);
                
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy3.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await seq1.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await seqInner.GetStatus());
            }
            
        }

        [Fact]
        public async Task Test_Parallel_Basic()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <Parallel Id=""par"">
                            <Parallel.Children>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""100ms"" />
                                <DummyStep Id=""dummy2"" BeforeStart=""10ms"" Working=""100ms"" />
                            </Parallel.Children>
                        </Parallel>
                    </Project.Pipeline>
                </Project>
            ") as ProjectNode;

            var entity = await CreateSingleEntityProject(config);
            var par = await GetStepGrainById(entity, "par");
            var dummy1 = await GetStepGrainById(entity, "dummy1");
            var dummy2 = await GetStepGrainById(entity, "dummy2");

            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(par, "par");
                await observer.ObserverStep(dummy1, "dummy1");
                await observer.ObserverStep(dummy2, "dummy2");
                
                // Initially, all Steps are Inactive
                Assert.Equal(StepStatus.Inactive, await par.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());

                await entity.Start();
                
                Assert.Equal(StepStatus.Active, await par.GetStatus());
                Assert.Equal(StepStatus.Active, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Active, await dummy2.GetStatus());

                await observer.WaitUntilStatus("par", s => s == StepStatus.StoppedWithSuccess);
                    
                Assert.Equal(StepStatus.StoppedWithSuccess, await par.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy2.GetStatus());
            }
            
        }

        [Fact]
        public async Task Basic_If_AlwaysTrue_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <If Id=""if1"">
                            <If.Child>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""1000ms"" />
                            </If.Child>
                            <If.Condition>
                                <True />
                            </If.Condition>
                        </If>
                    </Project.Pipeline>
                </Project>
            ") as ProjectNode;

            var e1 = await CreateSingleEntityProject(config);
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var ifNode = await GetStepGrainById(e1, "if1");
            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(ifNode, "if1");
                await observer.ObserverStep(dummy1, "dummy1");
                
                Assert.Equal(StepStatus.Inactive, await ifNode.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
            
                // kick off
                await e1.Start();

                await observer.WaitUntilStatus("if1", s => s == StepStatus.Working);
            
                Assert.Equal(StepStatus.Working, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Working, await ifNode.GetStatus());

                await observer.WaitUntilStatus("if1", s => s == StepStatus.StoppedWithSuccess);
            
                Assert.Equal(StepStatus.StoppedWithSuccess, await dummy1.GetStatus());
                Assert.Equal(StepStatus.StoppedWithSuccess, await ifNode.GetStatus());
            }
        }
        
        /// <summary>
        /// Create a Project with the given config, and instantiates an Entity
        /// under that Project.
        /// </summary>
        /// <param name="projConfig"></param>
        /// <returns>The EntityGrain</returns>
        protected async Task<IEntityGrain> CreateSingleEntityProject(ProjectNode projConfig)
        {
            var project = GrainFactory.GetGrain<IProjectGrain>(Guid.NewGuid());
            await project.LoadConfig(projConfig);
            var e1K = await project.CreateEntity(1);
            var e1 = GrainFactory.GetGrain<IEntityGrain>(e1K);
            return e1;
        }
        
        
    }

    internal class TestObserver : IStepGrainObserver, IDisposable
    {
        
        private readonly IGrainFactory _grainFactory;
        private readonly ITestOutputHelper _testOutputHelper;
        private IStepGrainObserver selfReference = null;

        private Task<IStepGrainObserver> SelfReference
        {
            get
            {
                if (selfReference != null)
                {
                    return Task.FromResult(selfReference);
                }
                else
                {
                    return _grainFactory.CreateObjectReference<IStepGrainObserver>(this)
                        .ContinueWith((x) =>
                        {
                            selfReference = x.Result;
                            return selfReference;
                        });
                }
            }
        }

        private int counter = -1;

        private Dictionary<string, IStepGrain> idToGrainMapping = new Dictionary<string, IStepGrain>();
        private Dictionary<Guid, string> guidToIdMapping = new Dictionary<Guid, string>();
        private Dictionary<string, List<(StepStatus, int)>> statusHistory 
            = new Dictionary<string, List<(StepStatus, int)>>();
        private ConcurrentDictionary<string, ConcurrentBag<(Func<StepStatus, bool>, TaskCompletionSource<object>)>>
            awaitingTasks 
                = new ConcurrentDictionary<string, ConcurrentBag<(Func<StepStatus, bool>, TaskCompletionSource<object>)>>();

        public TestObserver(ITestOutputHelper testOutputHelper, IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
            _testOutputHelper = testOutputHelper;
        }
        
        private IList<(StepStatus, int)> GetStatusHistory(string stepId)
        {
            if (statusHistory.ContainsKey(stepId))
            {
                return statusHistory[stepId];
            }
            else
            {
                return new List<(StepStatus, int)>();
            }
        }

        public void OnStatusChanged(Guid caller, StepStatus newStatus)
        {
            var now = DateTime.Now;
            ++counter;
            var stepId = guidToIdMapping[caller];
            Assert.NotNull(stepId);
            if (!statusHistory.ContainsKey(stepId))
            {
                statusHistory[stepId] = new List<(StepStatus, int)>(); 
            }
            statusHistory[stepId].Add((newStatus, counter));
            _testOutputHelper
                .WriteLine($"({counter}) {now.Minute}:{now.Second}.{now.Millisecond} - Step {stepId} changed to [{newStatus}]");
            // check awaiting tasks
            foreach (var (predicate, tcs) in awaitingTasks[stepId])
            {
                if (predicate(newStatus))
                {
                    tcs.SetResult(null);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task WaitUntilStatus(string stepId, Func<StepStatus, bool> predicate)
        {
            var sh = GetStatusHistory(stepId);

            if (sh.Count > 0)
            {
                foreach (var (s, _) in sh)
                {
                    if (predicate(s))
                        return Task.CompletedTask;
                }
            }

            // if already waiting
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            awaitingTasks[stepId].Add((predicate, tcs));            
            
            return tcs.Task;
        }
        
        public async Task ObserverStep(IStepGrain step, string stepId)
        {
            await step.Subscribe(await SelfReference);
            idToGrainMapping[stepId] = step;
            guidToIdMapping[step.GetPrimaryKey()] = stepId;
            awaitingTasks[stepId] = new ConcurrentBag<(Func<StepStatus, bool>, TaskCompletionSource<object>)>();
        }

        public void Dispose()
        {
            foreach (var stepGrain in idToGrainMapping.Values)
            {
                stepGrain.Unsubscribe(SelfReference.Result).GetAwaiter().GetResult();
            }
        }
    }
}
