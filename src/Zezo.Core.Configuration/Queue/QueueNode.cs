using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration {
    public sealed class QueueNode : ConfigurationNode {
        public new static string TagName { get { return "Queue"; } }
        public string Id { get; private set; }
        public string Name { get; private set; }
        
        private readonly List<PermissionNode> _permissions = new List<PermissionNode>();

        public IReadOnlyList<PermissionNode> Permissions { get {return _permissions;} }

        public QueueNode(XmlElement xmlElem, IParser parser)
        {
            this.Id = xmlElem.GetAttribute("Id");
            this.Name = xmlElem.GetStringAttribute("Name").Trim();
            this._permissions.AddRange(xmlElem.GetCollectionAttribute<PermissionNode>("Permissions", parser));
        }

    }
}