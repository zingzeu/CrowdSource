using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;
using Zezo.Core.Configuration.Steps.Condition;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class IfNode : StepNode {
        public new static string TagName { get { return "If"; } }
        public override string StepType { get => "If"; }


        public StepNode Child { get; private set; }

        public ConditionNode Condition { get; private set; }

        public IfNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {
            Child = xmlElem.GetComplexSingleAttribute<StepNode>("Child", parser);
            Condition = xmlElem.GetComplexSingleAttribute<ConditionNode>("Condition", parser);
        }

        public sealed class ScriptConditionNode : ConditionNode {
            public new static string TagName { get { return "ScriptCondition"; } }
            public string InlineSource { get; private set; }

            public ScriptConditionNode(XmlElement xmlElem, IParser parser) {
                InlineSource = xmlElem.InnerText;
            }
        }
    }

}