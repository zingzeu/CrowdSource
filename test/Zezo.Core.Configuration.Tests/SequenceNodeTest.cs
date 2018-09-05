using System;
using Xunit;

using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.Configuration.Tests
{
    public class SequenceNodeTest : ConfigurationNodeTest
    {

        [Fact]
        public void CanParseSequenceNodeBasic()
        {
            var doc = @"<Sequence Id=""testid"" />";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<SequenceNode>(configNode);

            SequenceNode node = configNode as SequenceNode;
            Assert.Equal("testid", node.Id);
            Assert.Equal(0, node.Children.Count);
        }

   
    }
}
