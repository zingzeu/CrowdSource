using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration {
    
    public sealed class HasRoleNode : PermissionNode {
        public new static string TagName { get { return "HasRole"; } }
        public string Role { get; private set; }

        public HasRoleNode(XmlElement xmlElem, IParser parser)
        {
            this.Role = xmlElem.GetStringAttribute("Role").Trim();
        }
    }

}