using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class DoWhileNode : StepNode {
        public new static string TagName { get { return "DoWhile"; } }

        public StepNode ChildTemplate { get; private set; }

        public ConfigurationNode Condition { get; private set; }

        public DoWhileNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {

        }
    }

}