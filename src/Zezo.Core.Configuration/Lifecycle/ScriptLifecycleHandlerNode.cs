using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Lifecycle {
    
    public sealed class ScriptLifecycleHandlerNode : LifecycleHandlerNode {
        public new static string TagName { get { return "ScriptLifecycleHandler"; } }

        public ScriptLifecycleHandlerNode(XmlElement xmlElem, IParser parser) : base(xmlElem, parser) {
            // todo
        }
    }

}