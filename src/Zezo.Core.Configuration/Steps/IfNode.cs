using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class IfNode : StepNode {
        public new static string TagName { get { return "If"; } }

        public StepNode Child { get; private set; }

        public ConfigurationNode Condition { get; private set; }
    }

}