using System.Collections.Generic;

namespace Zezo.Core.Configuration {
    
    public sealed class VariableNode : ConfigurationNode {
        public new static string TagName { get { return "Variable"; } }

        public string Name { get; private set; }
        public string Value { get; private set; }
    }

}