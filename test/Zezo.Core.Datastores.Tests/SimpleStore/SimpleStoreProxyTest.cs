using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zezo.Core.Datastores.SimpleStore;

namespace Zezo.Core.Datastores.Tests.SimpleStore
{
    public class SimpleStoreProxyTest
    {
        private static readonly IList<FieldDef> SampleFieldDefs = new List<FieldDef>()
        {
            new FieldDef()
            {
                Name = "field1",
                Type = typeof(int)
            },
            new FieldDef()
            {
                Name = "field2",
                Type = typeof(string)
            },
            new FieldDef()
            {
                Name = "field3",
                Type = typeof(bool)
            }
        };

        [Fact]
        public void Basic_Read_Test()
        {
            var x = new SimpleStoreProxy(SampleFieldDefs, new Dictionary<string, object>()
            {
                {"field1", 2},
                {"field2", "testString"},
                {"field3", true}
            });

            Assert.Equal(2, x["field1"]);
            Assert.Equal("testString", x["field2"]);
            Assert.Equal(true, x["field3"]);

            var changeRecords = ((IHasChangeRecords) x)._ChangeRecords;
            Assert.Empty(changeRecords);
        }

        [Fact]
        public void Basic_Write_Test()
        {
            var x = new SimpleStoreProxy(SampleFieldDefs, new Dictionary<string, object>()
            {
                {"field1", 2},
                {"field2", "testString"},
                {"field3", true}
            });

            x["field1"] = 3;
            x["field2"] = "changed";
            x["field3"] = false;
            
            Assert.Equal(3, x["field1"]);
            Assert.Equal("changed", x["field2"]);
            Assert.Equal(false, x["field3"]);
            
            x["field1"] = 920;
            x["field2"] = "changedAgain";
            x["field3"] = true;
                        
            Assert.Equal(920, x["field1"]);
            Assert.Equal("changedAgain", x["field2"]);
            Assert.Equal(true, x["field3"]);
            
            var changeRecords = ((IHasChangeRecords) x)._ChangeRecords;

            
            Assert.Equal(2, changeRecords.Count);
            Assert.True(changeRecords.Count(c => c.Name == "field1" && c.NewValue == 920) == 1);
            Assert.True(changeRecords.Count(c => c.Name == "field2" && c.NewValue == "changedAgain") == 1);
        }

    }
}