using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class SequenceNode : StepNode {
        public new static string TagName { get { return "Sequence"; } }

        private readonly List<StepNode> _children = new List<StepNode>();
        public IReadOnlyList<StepNode> Children { get { return _children; } }
        public SequenceNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser)
        {
            var children = parser.GetComplexAttribute(xmlElem, this.GetTagName()+"."+"Children");
            foreach (XmlNode xmlChild in children) {
                var xmlChildNode = xmlChild as XmlElement;
                if (xmlChildNode != null) {
                    var childStep = parser.ParseXmlElement(xmlChildNode) as StepNode;
                    if (childStep != null) {
                        this._children.Add(childStep);
                    }
                }
            }
        }
    }

}