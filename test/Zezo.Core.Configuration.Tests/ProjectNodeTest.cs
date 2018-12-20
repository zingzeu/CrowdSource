using System;
using Xunit;

using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Lifecycle;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.Configuration.Tests
{
    public class ProjectNodeTest : ConfigurationNodeTest
    {

        [Fact]
        public void CanParseProjectNode()
        {
            var doc = @"<Project Id=""testid"" Name=""testName"" />";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<ProjectNode>(configNode);

            ProjectNode node = configNode as ProjectNode;
            Assert.Equal("testid", node.Id);
            Assert.Equal("testName", node.Name);
        }

        [Fact]
        public void CanParseProjectNameAsChildNode() {
            var doc = @"<Project Id=""testid"">" +
                      @"  <Project.Name>Longer Name</Project.Name>" + 
                      @"</Project>";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<ProjectNode>(configNode);

            ProjectNode node = configNode as ProjectNode;
            Assert.Equal("Longer Name", node.Name);    
        }

        [Fact]
        public void CanParseProjectNameWithCDATA() {
            var doc = @"<Project Id=""testid"">
                        <Project.Name><![CDATA[
                          Longer Name
                        ]]>
                        </Project.Name>
                      </Project>";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<ProjectNode>(configNode);

            ProjectNode node = configNode as ProjectNode;
            Assert.Equal("Longer Name", node.Name);    
        }

        [Fact]
        public void CanParseProjectNodePipeline() {
            var doc = @"<Project Id=""testid"">
                    <Project.Pipeline>
                        <Sequence Id=""firststep"" />
                    </Project.Pipeline>
                </Project>
            ";
            ProjectNode projNode = parser.ParseXmlString(doc) as ProjectNode;
            Assert.Equal("firststep", projNode.Pipeline.Id);
            Assert.IsType<SequenceNode>(projNode.Pipeline);
        }

        [Fact]
        public void Can_Parse_ProjectNode_LifecycleHandlers() {
            var doc = @"<Project Id=""testid"">
                    <Project.Lifecycle>
                        <ScriptLifecycleHandler On=""DataStoreUpdated"">
                        </ScriptLifecycleHandler>
                    </Project.Lifecycle>
                </Project>
            ";
            ProjectNode projNode = parser.ParseXmlString(doc) as ProjectNode;
            Assert.Equal(1, projNode.Lifecycle.Count);
            Assert.IsType<ScriptLifecycleHandlerNode>(projNode.Lifecycle[0]);
            Assert.Equal("DataStoreUpdated", projNode.Lifecycle[0].On);
        }
    
        [Fact]
        public void Can_Parse_ProjectNode_Queues() {
            var doc = @"<Project Id=""testid"">
                    <Project.Queues>
                        <Queue Id=""review"" Name=""Reviewing queue"">
                            <Queue.Permissions>
                                <HasRole Role=""Administrator"" />
                            </Queue.Permissions>
                        </Queue>
                    </Project.Queues>
                </Project>
            ";
            ProjectNode projNode = parser.ParseXmlString(doc) as ProjectNode;
            Assert.Equal(1, projNode.Queues.Count);
            Assert.IsType<QueueNode>(projNode.Queues[0]);
            Assert.Equal("review", projNode.Queues[0].Id);
            Assert.Equal("Reviewing queue", projNode.Queues[0].Name);
        }
    }
}
