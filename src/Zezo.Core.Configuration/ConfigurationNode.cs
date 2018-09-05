using System.Xml;
using System.Collections.Generic;

namespace Zezo.Core.Configuration
{
    public abstract class ConfigurationNode {
        public static string TagName { get { throw null; }}
        public string GetTagName() {
            return this.GetType().GetProperty("TagName").GetValue(null) as string;
        }
    }
}