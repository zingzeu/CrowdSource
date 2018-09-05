using System.Collections.Generic;
using Zezo.Core.Configuration.Middleware;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class InteractiveNode : StepNode {
        public new static string TagName { get { return "Interactive"; } }

        // id of the queue to publish to
        public string Queue { get; private set; }
        
        public IReadOnlyList<MiddlewareNode> BeforePublish { get; private set; }
        public IReadOnlyList<MiddlewareNode> BeforeSubmit { get; private set; }
        public IReadOnlyList<MiddlewareNode> AfterSubmit { get; private set; }

    }

}