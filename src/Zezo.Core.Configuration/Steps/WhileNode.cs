using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class WhileNode : StepNode {
        public new static string TagName { get { return "While"; } }
        public override string StepType { get => "While"; }
        public StepNode ChildTemplate { get; private set; }
        public ConfigurationNode Condition { get; private set; }

        public WhileNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {

        }

    }

}