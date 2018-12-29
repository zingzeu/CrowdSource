using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Zezo.Core.Configuration;

namespace Zezo.Core.Grains.Tests
{
    public class DatastoreTest : BaseGrainTest
    {
        public DatastoreTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

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
                                        return a == b;
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
        }
    }
}