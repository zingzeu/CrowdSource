using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Datastore {
    
    /// <summary>
    /// Configuration for Flags Datastore
    /// </summary>
    public sealed class FlagsStore : DatastoreNode {
        public new static string TagName { get { return "FlagsStore"; } }

        public FlagsStore(XmlElement xmlElem, IParser parser): base(xmlElem, parser) {
            // todo
        }
    }

}