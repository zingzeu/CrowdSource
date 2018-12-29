using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Datastore {
    
    /// <summary>
    /// </summary>
    public sealed class SimpleStoreNode : DatastoreNode {
        public new static string TagName => "SimpleStore";

        private readonly List<FieldDefNode> _fields = new List<FieldDefNode>();

        public IReadOnlyList<FieldDefNode> Fields => _fields;
        public SimpleStoreNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {
            this._fields.AddRange(xmlElem.GetCollectionAttribute<FieldDefNode>("Fields", parser));
        }
    }

}