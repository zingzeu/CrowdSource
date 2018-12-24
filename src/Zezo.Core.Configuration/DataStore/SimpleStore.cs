using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Datastore {
    
    /// <summary>
    /// </summary>
    public sealed class SimpleStore : DatastoreNode {
        public new static string TagName => "SimpleStore";

        public SimpleStore(XmlElement xmlElem, IParser parser) {
            // todo
        }
    }

}