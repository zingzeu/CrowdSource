using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Datastore {
    
    /// <summary>
    /// Configuration for Journal (revisioned) Datastore
    /// </summary>
    public sealed class JournalStoreNode : DatastoreNode {
        public new static string TagName { get { return "JournalStore"; } }
        
        private readonly List<FieldDefNode> _fields = new List<FieldDefNode>();

        public IReadOnlyList<FieldDefNode> Fields => _fields;
        
        public JournalStoreNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {
            this._fields.AddRange(xmlElem.GetCollectionAttribute<FieldDefNode>("Fields", parser));
        }

    }
}