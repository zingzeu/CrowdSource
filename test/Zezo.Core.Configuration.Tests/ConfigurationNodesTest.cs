using System;
using Xunit;

using Zezo.Core.Configuration;

namespace Zezo.Core.Configuration.Tests
{
    public class ConfigurationNodesTest
    {

        IParser parser = new Parser();

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
        public void CanParseProjectNodePipeline() {

        }
    }
}
