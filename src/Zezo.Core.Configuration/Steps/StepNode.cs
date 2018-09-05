using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    
    public abstract class StepNode : ConfigurationNode {
        public string Id { get; protected set; }

        public string StepType { get; protected set; }

        public IReadOnlyList<VariableNode> Variables { get; protected set; }
    }

}