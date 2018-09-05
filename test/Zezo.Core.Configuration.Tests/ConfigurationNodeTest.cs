using System;
using Xunit;

using Zezo.Core.Configuration;

namespace Zezo.Core.Configuration.Tests
{
    public abstract class ConfigurationNodeTest
    {
        protected IParser parser = new Parser();
    }
}
