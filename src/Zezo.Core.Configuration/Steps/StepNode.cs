using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Steps {
    
    /// <summary>
    /// Represents a Step's configuration.
    /// </summary>
    public abstract class StepNode : ConfigurationNode {
        public string Id { get; protected set; }

        public string StepType { get; protected set; }

        public IReadOnlyList<VariableNode> Variables { get; protected set; }
        
        public StepNode(XmlElement xmlElem, IParser parser)
        {
            this.Id = xmlElem.GetAttribute("Id");
        }
    }

}