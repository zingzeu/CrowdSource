using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class WhileNode : StepNode {
        public new static string TagName { get { return "While"; } }

        public StepNode ChildTemplate { get; private set; }

        public ConfigurationNode Condition { get; private set; }
    }

}