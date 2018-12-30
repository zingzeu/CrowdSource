using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Datastore {
    
    /// <summary>
    /// Represents a Datastore's configuration.
    /// </summary>
    public abstract class DatastoreNode : ConfigurationNode {
        public string Id { get; private set; }

        public DatastoreNode(XmlElement xmlElem, IParser parser) {
            Id = xmlElem.GetStringAttribute("Id");
        }
    }

}