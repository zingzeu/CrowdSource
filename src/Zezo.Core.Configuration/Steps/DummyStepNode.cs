using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class DummyStepNode : StepNode {
        public new static string TagName { get { return "DummyStep"; } }
        public override string StepType { get => "DummyStep"; }

        public DummyStepNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser)
        {
        }
    }

}