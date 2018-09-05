using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class XorNode : StepNode {
        public new static string TagName { get { return "Xor"; } }

        public IEnumerable<StepNode> Children { get; private set; }

        public XorNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {

        }
    }

}