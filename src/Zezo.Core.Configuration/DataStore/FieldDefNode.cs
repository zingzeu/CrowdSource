using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Datastore
{
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