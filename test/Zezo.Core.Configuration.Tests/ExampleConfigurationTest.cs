using System;
using Xunit;
using Zezo.Core.Configuration.Steps;
using static Zezo.Core.Configuration.Steps.IfNode;

namespace Zezo.Core.Configuration.Tests
{
    public class ExampleConfigurationTest : ConfigurationNodeTest
    {

        [Fact]
        public void ADFDConfigurationTest()
        {
            var doc = ReadDataFile("adfd.xml");
            ConfigurationNode configRoot = parser.ParseXmlString(doc);
            Assert.IsType<ProjectNode>(configRoot);

            ProjectNode node = configRoot as ProjectNode;
            Assert.Equal("adfd", node.Id);
            Assert.Equal("Digitalisation of ADFD", node.Name);

            Assert.IsType<SequenceNode>(node.Pipeline);
            var normal_flow = node.Pipeline as SequenceNode;
            Parse_normal_flow(normal_flow);

            Assert.Equal(3, node.Datastores.Count);
            Assert.Equal(5, node.Queues.Count);
        }

        private void Parse_normal_flow(SequenceNode normal_flow) {
            Assert.Equal("normal_flow", normal_flow.Id);
            Assert.Equal(2, normal_flow.Children.Count);
            Assert.IsType<XorNode>(normal_flow.Children[0]);
            Assert.IsType<DoWhileNode>(normal_flow.Children[1]);

            var transcribe = normal_flow.Children[0] as XorNode;
            var review = normal_flow.Children[1] as DoWhileNode;

            Parse_transcribe(transcribe);
        }

        private void Parse_transcribe(XorNode transcribe) {
            Assert.Equal("transcribe", transcribe.Id);
            Assert.Equal(2, transcribe.Children.Count);
            Assert.IsType<IfNode>(transcribe.Children[0]);
            Assert.IsType<ParallelNode>(transcribe.Children[1]);

            var transcribe_all_wrapper = transcribe.Children[0] as IfNode;
            var transcribe_separate = transcribe.Children[1] as ParallelNode;

            Parse_transcribe_all_wrapper(transcribe_all_wrapper);
            Parse_transcribe_seperate(transcribe_separate);
        }

        private void Parse_transcribe_all_wrapper(IfNode transcribe_all_wrapper) {
            Assert.Equal("transcribe_all_wrapper", transcribe_all_wrapper.Id);
            Assert.IsType<ScriptConditionNode>(transcribe_all_wrapper.Condition);

            var condition = transcribe_all_wrapper.Condition as ScriptConditionNode;
            Assert.Equal("not (all fields are valid)", condition.InlineSource.Trim());

            Assert.IsType<InteractiveNode>(transcribe_all_wrapper.Child);

            var transcribe_all = transcribe_all_wrapper.Child as InteractiveNode;
            Assert.Equal("transcribe_all", transcribe_all.Id);
            Assert.Equal("all", transcribe_all.Queue);
            Assert.Equal(TimeSpan.FromMinutes(5), transcribe_all.TimeLimit);
            Assert.Equal(1, transcribe_all.BeforePublish.Count);
            Assert.Equal(2, transcribe_all.BeforeSubmit.Count);
            Assert.Equal(1, transcribe_all.AfterSubmit.Count);
        }
    
        private void Parse_transcribe_seperate(ParallelNode transcribe_separate) {
            Assert.Equal("transcribe_separate", transcribe_separate.Id);
        }
    }
}
