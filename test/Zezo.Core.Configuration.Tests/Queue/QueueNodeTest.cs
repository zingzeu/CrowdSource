using System;
using Xunit;

using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.Configuration.Tests
{
    public class QueueNodeTest : ConfigurationNodeTest
    {
        [Fact]
        public void Can_Parse_QueueNode_Basic()
        {
            var doc = @"<Queue Id=""review"" Name=""Reviewing queue"">
                            <Queue.Permissions>
                                <HasRole Role=""Administrator"" />
                            </Queue.Permissions>
                        </Queue>";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<QueueNode>(configNode);

            QueueNode node = configNode as QueueNode;
            Assert.Equal("review", node.Id);
            Assert.Equal("Reviewing queue", node.Name);
            Assert.Equal(1, node.Permissions.Count);
            Assert.IsType<HasRoleNode>(node.Permissions[0]);
        }

    }
}
