using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Zezo.Core.Configuration;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.Tests
{
    [Collection("Default")] // All tests in the same collection, prevents parallel runs
    public class DatastoreTest : BaseGrainTest
    {
        public DatastoreTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        [Fact]
        public async Task Can_Create_Entity_with_Datastore()
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
                                        return Datastores[""default""][""ImageFile""] == null;
                                    ]]>
                                </ScriptCondition>
                            </If.Condition>
                        </If>
                    </Project.Pipeline>
                    <Project.Datastores>
                        <SimpleStore Id=""default"">
                            <SimpleStore.Fields>
                                <FieldDef Id=""ImageFile"" Type=""String"" Nullable=""True"" />
                            </SimpleStore.Fields>
                        </SimpleStore>
                    </Project.Datastores>
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
        
        /// <summary>
        /// Test that the SimpleStore datastore can store and modify integer correctly. 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleStore_Integer_Increment_Test()
        {
            var config = ParseConfig(@"
                <Project Id=""test"">
                    <Project.Pipeline>
                        <Sequence Id=""seq1"">
                            <Sequence.Children>
                                <If Id=""if1"">
                                    <If.Child>
                                        <DummyStep Id=""dummy1"" BeforeStart=""10ms"" Working=""1000ms"" />
                                    </If.Child>
                                    <If.Condition>
                                        <ScriptCondition Language=""csharp"">
                                            <![CDATA[
                                                var counter = Datastores[""default""][""counter""];
                                                Datastores[""default""][""counter""]=1;
                                                return counter == null;
                                            ]]>
                                        </ScriptCondition>
                                    </If.Condition>
                                </If>
                                <If Id=""if2"">
                                    <If.Child>
                                        <DummyStep Id=""dummy2"" BeforeStart=""10ms"" Working=""1000ms"" />
                                    </If.Child>
                                    <If.Condition>
                                        <ScriptCondition Language=""csharp"">
                                            <![CDATA[
                                                var counter = Datastores[""default""][""counter""];
                                                Datastores[""default""][""counter""] = counter + 1;
                                                return counter == 1;
                                            ]]>
                                        </ScriptCondition>
                                    </If.Condition>
                                </If>
                            </Sequence.Children>
                        </Sequence>
                    </Project.Pipeline>
                    <Project.Datastores>
                        <SimpleStore Id=""default"">
                            <SimpleStore.Fields>
                                <FieldDef Id=""counter"" Type=""Integer"" Nullable=""True"" />
                            </SimpleStore.Fields>
                        </SimpleStore>
                    </Project.Datastores>
                </Project>
            ") as ProjectNode;
            
            var e1 = await CreateSingleEntityProject(config);

            var dummy1 = await GetStepGrainById(e1, "dummy1");
            var dummy2 = await GetStepGrainById(e1, "dummy2");
            var if1 = await GetStepGrainById(e1, "if1");
            var if2 = await GetStepGrainById(e1, "if2");
            var seqNode = await GetStepGrainById(e1, "seq1");

            using (var observer = new TestObserver(_testOutputHelper, GrainFactory))
            {
                await observer.ObserverStep(seqNode, "seq1");
                await observer.ObserverStep(if1, "if1");
                await observer.ObserverStep(if2, "if2");
                await observer.ObserverStep(dummy1, "dummy1");
                await observer.ObserverStep(dummy2, "dummy2");
                
                Assert.Equal(StepStatus.Inactive, await seqNode.GetStatus());
                Assert.Equal(StepStatus.Inactive, await if1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await if2.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Inactive, await dummy2.GetStatus());
                
                // kick off
                await e1.Start();

                await observer.WaitUntilStatus("seq1", s => s == StepStatus.Completed);
                Assert.Equal(StepStatus.Completed, await dummy1.GetStatus());
                Assert.Equal(StepStatus.Completed, await dummy2.GetStatus());
            }
        }
    }
}