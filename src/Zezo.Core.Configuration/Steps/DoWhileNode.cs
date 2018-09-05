using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class DoWhileNode : StepNode {
        public new static string TagName { get { return "DoWhile"; } }

        public StepNode ChildTemplate { get; private set; }

        public ConfigurationNode Condition { get; private set; }
    }

}