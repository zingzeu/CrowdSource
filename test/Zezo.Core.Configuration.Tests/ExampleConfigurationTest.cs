using System;
using Xunit;

namespace Zezo.Core.Configuration.Tests
{
    public class ExampleConfigurationTest : ConfigurationNodeTest
    {
        [Fact]
        public void ADFDConfigurationTest()
        {
            var doc = ReadDataFile("adfd.xml");
            ConfigurationNode configRoot = parser.ParseXmlString(doc);
        }
    }
}
