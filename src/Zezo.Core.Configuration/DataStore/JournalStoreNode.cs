using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Datastore {
    
    /// <summary>
    /// Configuration for Journal (revisioned) Datastore
    /// </summary>
    public sealed class JournalStoreNode : DatastoreNode {
        public new static string TagName { get { return "JournalStore"; } }
        public string Id { get; private set; }
        public JournalStoreNode(XmlElement xmlElem, IParser parser) {
            this.Id = xmlElem.GetStringAttribute("Id");
            this._fields.AddRange(xmlElem.GetCollectionAttribute<FieldDefNode>("Fields", parser));
        }

        private readonly List<FieldDefNode> _fields = new List<FieldDefNode>();

        public IReadOnlyList<FieldDefNode> Fields { get { return this._fields;}}

        public sealed class FieldDefNode : ConfigurationNode {
            public new static string TagName { get { return "FieldDef"; } }
            public string Id { get; private set; }
            public string Type { get; private set; }
            public bool Nullable { get; private set; }

            public FieldDefNode(XmlElement xmlElem, IParser parser) {
                this.Id = xmlElem.GetStringAttribute("Id");
                this.Type = xmlElem.GetStringAttribute("Type");
                this.Nullable = xmlElem.GetBooleanAttribute("Nullable");
            }
        }
    }

    

}