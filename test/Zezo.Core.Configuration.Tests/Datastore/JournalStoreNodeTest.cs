using System;
using Xunit;

using Zezo.Core.Configuration;
using Zezo.Core.Configuration.Datastore;
using Zezo.Core.Configuration.Steps;

namespace Zezo.Core.Configuration.Tests.Datastore
{
    public class JournalStoreNodeTest : ConfigurationNodeTest
    {

        [Fact]
        public void Can_Parse_JournalStoreNode_Basic()
        {
            var doc = @"        
            <JournalStore Id=""fields"">
                <JournalStore.Fields>
                    <FieldDef Id=""TextBUC"" Type=""String"" Nullable=""True""/>
                    <FieldDef Id=""TextEng"" Type=""String"" Nullable=""True""/>
                    <FieldDef Id=""TextChi"" Type=""String"" Nullable=""True""/>
                    <FieldDef Id=""IsOral"" Type=""Boolean"" Nullable=""False""/>
                </JournalStore.Fields>
            </JournalStore>";
            ConfigurationNode configNode = parser.ParseXmlString(doc);
            Assert.IsType<JournalStoreNode>(configNode);

            JournalStoreNode node = configNode as JournalStoreNode;
            Assert.Equal("fields", node.Id);
            Assert.Equal(4, node.Fields.Count);

            Assert.Equal("TextBUC", node.Fields[0].Id);
            Assert.Equal("String", node.Fields[0].Type);
            Assert.True(node.Fields[0].Nullable);

            Assert.Equal("TextEng", node.Fields[1].Id);
            Assert.Equal("String", node.Fields[1].Type);
            Assert.True(node.Fields[1].Nullable);

            Assert.Equal("TextChi", node.Fields[2].Id);
            Assert.Equal("String", node.Fields[2].Type);
            Assert.True(node.Fields[2].Nullable);

            Assert.Equal("IsOral", node.Fields[3].Id);
            Assert.Equal("Boolean", node.Fields[3].Type);
            Assert.False(node.Fields[3].Nullable);
        }

       
    }
}
