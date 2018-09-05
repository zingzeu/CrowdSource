using System;
using System.IO;
using System.Reflection;
using Xunit;

using Zezo.Core.Configuration;

namespace Zezo.Core.Configuration.Tests
{
    public abstract class ConfigurationNodeTest
    {
        protected IParser parser = new Parser();

        protected string ReadDataFile(string filename) {
            var basePath = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            var filePath = Path.Combine(basePath, ".." , "data", filename);
            return File.ReadAllText(filePath);
        }
    }
}
