using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Steps {
    
    public sealed class IfNode : StepNode {
        public new static string TagName { get { return "If"; } }

        public StepNode Child { get; private set; }

        public ConfigurationNode Condition { get; private set; }

        public IfNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {
            Child = xmlElem.GetComplexSingleAttribute<StepNode>("Child", parser);
            Condition = xmlElem.GetComplexSingleAttribute<ConfigurationNode>("Condition", parser);
        }

        public sealed class ScriptConditionNode : ConfigurationNode {
            public new static string TagName { get { return "ScriptCondition"; } }
            public string InlineSource { get; private set; }

            public ScriptConditionNode(XmlElement xmlElem, IParser parser) {
                InlineSource = xmlElem.InnerText;
            }
        }
    }

}