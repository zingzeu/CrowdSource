using System.Collections.Generic;

namespace Zezo.Core.Configuration.Steps {
    public sealed class QueueNode : ConfigurationNode {
        public new static string TagName { get { return "Queue"; } }
        public string Id { get; private set; }
        public string Name { get; private set; }
        
        public IEnumerable<PermissionNode> Permissions { get; private set; }

    }
}