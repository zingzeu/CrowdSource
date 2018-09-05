using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class XorNode : StepNode {
        public new static string TagName { get { return "Xor"; } }

        public IEnumerable<StepNode> Children { get; private set; }
    }

}