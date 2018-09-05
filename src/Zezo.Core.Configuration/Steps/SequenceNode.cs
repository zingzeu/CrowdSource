using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class SequenceNode : StepNode {
        public new static string TagName { get { return "Sequence"; } }

        public IEnumerable<StepNode> Children { get; private set; }
    }

}