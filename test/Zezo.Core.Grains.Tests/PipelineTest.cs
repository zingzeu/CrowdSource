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
                Assert.Equal(StepStatus.ActiveIdle, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
                Assert.Equal(StepStatus.ActiveIdle, await seq.GetStatus());

                await observer.WaitUntilStatus("seq", s => s == StepStatus.Completed);

                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Completed, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Completed, await seq.GetStatus());

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

                await observer.WaitUntilStatus("seq1", s => s == StepStatus.Completed);
                
                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Completed, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Completed, await dummy3.GetStatus());
                Assert.Equal(StepStatus.Completed, await seq1.GetStatus());
                Assert.Equal(StepStatus.Completed, await seqInner.GetStatus());
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
                                <DummyStep Id=""dummy1"" BeforeStart=""1000ms"" Working=""1000ms"" />
                                <DummyStep Id=""dummy2"" BeforeStart=""1000ms"" Working=""1000ms"" />
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

                await Task.WhenAll(
                    observer.WaitUntilStatus("dummy1", s => s == StepStatus.Working),
                    observer.WaitUntilStatus("dummy2", s => s == StepStatus.Working)
                );
                
                observer.Observed()
                    .StartsWith("par", s => s == StepStatus.ActiveIdle)
                    .LaterOn("dummy1", s => s == StepStatus.ActiveIdle)
                    .LaterOn("dummy1", s => s == StepStatus.Working)
                    .Validate();
                
                observer.Observed()
                    .StartsWith("par", s => s == StepStatus.ActiveIdle)
                    .LaterOn("dummy2", s => s == StepStatus.ActiveIdle)
                    .LaterOn("dummy2", s => s == StepStatus.Working)
                    .Validate();

                await observer.WaitUntilStatus("par", s => s == StepStatus.Completed);
                    
                Assert.Equal(StepStatus.Completed, await par.GetStatus());
                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Completed, await dummy2.GetStatus());
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

                await observer.WaitUntilStatus("if1", s => s == StepStatus.Completed);
            
                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Completed, await ifNode.GetStatus());
            }
        }
        
        [Fact]
        public async Task Basic_If_AlwaysFalse_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <If Id=""if1"">
                            <If.Child>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""1000ms"" />
                            </If.Child>
                            <If.Condition>
                                <False />
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

                await observer.WaitUntilStatus("if1", s => s == StepStatus.Completed);
                Assert.Equal(StepStatus.Skipped, await dummy1.GetStatus());
            }
        }
        
        [Fact]
        public async Task Basic_If_ScriptCondition_True_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <If Id=""if1"">
                            <If.Child>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""1000ms"" />
                            </If.Child>
                            <If.Condition>
                                <ScriptCondition Language=""csharp"">
                                    <![CDATA[
                                        var a = 1+1;
                                        var b = 2;
                                        return a == b;
                                    ]]>
                                </ScriptCondition>
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

                await observer.WaitUntilStatus("if1", s => s == StepStatus.Completed);
                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
            }
        }

        [Fact]
        public async Task Basic_If_ScriptCondition_False_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <If Id=""if1"">
                            <If.Child>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""1000ms"" />
                            </If.Child>
                            <If.Condition>
                                <ScriptCondition Language=""csharp"">
                                    <![CDATA[
                                        var a = 1+1;
                                        var b = 2;
                                        return a != b;
                                    ]]>
                                </ScriptCondition>
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

                await observer.WaitUntilStatus("if1", s => s == StepStatus.Completed);
                Assert.Equal(StepStatus.Skipped, await dummy1.GetStatus());
            }
        }

        
        [Fact]
        public async Task Basic_Xor_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <Xor Id=""xor1"">
                            <Xor.Children>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""6000ms"" />
                                <DummyStep Id=""dummy2"" BeforeStart=""1000ms"" Working=""1000ms"" />
                                <DummyStep Id=""dummy3"" BeforeStart=""2000ms"" Working=""1000ms"" />
                            </Xor.Children>
                        </Xor>
                    </Project.Pipeline>
                </Project>
            ") as ProjectNode;

            var e1 = await CreateSingleEntityProject(config);
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            var dummy3 = await GetStepGrainById(e1, "dummy3");
            var xorNode = await GetStepGrainById(e1, "xor1");
            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(xorNode, "xor1");
                await observer.ObserverStep(dummy1, "dummy1");
                await observer.ObserverStep(dummy2, "dummy2");
                await observer.ObserverStep(dummy3, "dummy3");
                
                Assert.Equal(StepStatus.Inactive, await xorNode.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy3.GetStatus());
            
                // kick off
                await e1.Start();

                await observer.WaitUntilStatus("xor1", s => s == StepStatus.Completed);
                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Skipped, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Skipped, await dummy3.GetStatus());
            }
        }
        
        
        [Fact]
        public async Task Xor_PauseResume_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <Xor Id=""xor1"">
                            <Xor.Children>
                                <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""0ms"" WorkingSequence=""1000ms, 5000ms, 1000ms, 1000ms, 1000ms"" />
                                <DummyStep Id=""dummy2"" BeforeStart=""1000ms"" Working=""1000ms"" />
                                <DummyStep Id=""dummy3"" BeforeStart=""2000ms"" Working=""1000ms"" />
                            </Xor.Children>
                        </Xor>
                    </Project.Pipeline>
                </Project>
            ") as ProjectNode;

            /*
             time     10       1100
             dummy1   |----------|
             dummy2
             dummy3
             
             */
            
            var e1 = await CreateSingleEntityProject(config);
            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            var dummy3 = await GetStepGrainById(e1, "dummy3");
            var xorNode = await GetStepGrainById(e1, "xor1");
            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(xorNode, "xor1");
                await observer.ObserverStep(dummy1, "dummy1");
                await observer.ObserverStep(dummy2, "dummy2");
                await observer.ObserverStep(dummy3, "dummy3");
                
                Assert.Equal(StepStatus.Inactive, await xorNode.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy3.GetStatus());
            
                // kick off
                await e1.Start();

                await observer.WaitUntilStatus("xor1", s => s == StepStatus.Completed);
            }
        }
        
    }
}
