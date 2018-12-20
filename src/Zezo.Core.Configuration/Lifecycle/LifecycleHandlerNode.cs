using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Lifecycle {
    
    /// <summary>
    /// Represents a LifecycleHandler's configuration.
    /// </summary>
    public abstract class LifecycleHandlerNode : ConfigurationNode {
        /// <summary>
        /// The trigger of the LifecycleHandler.
        /// </summary>
        /// <value></value>
        public string On { get; protected set; }

        public LifecycleHandlerNode(XmlElement xmlElem, IParser parser)
        {
            this.On = xmlElem.GetAttribute("On");
        }
    }

}