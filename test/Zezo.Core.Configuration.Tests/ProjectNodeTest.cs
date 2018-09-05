using System;
using Xunit;

using Zezo.Core.Configuration;
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
    }
}
