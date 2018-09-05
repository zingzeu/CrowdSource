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

        [Fact]
        public void CanParseSequenceNodeChildren() {
            var doc = @"<Sequence Id=""testid"">
                            <Sequence.Children>
                                <Sequence Id=""subsequence0"" />
                                <Sequence Id=""subsequence1"" />
                                <Sequence Id=""subsequence2"" />
                                <Sequence Id=""subsequence3"" />
                                <Sequence Id=""subsequence4"" />
                            </Sequence.Children>
                        </Sequence>";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<SequenceNode>(configNode);

            SequenceNode node = configNode as SequenceNode;
            Assert.Equal("testid", node.Id);
            Assert.Equal(5, node.Children.Count);
            for (var i = 0; i < 5; ++i) {
                var childstep = node.Children[i];
                Assert.IsType<SequenceNode>(childstep);
                Assert.Equal($"subsequence{i}", childstep.Id);
            }

        }
   
    }
}
