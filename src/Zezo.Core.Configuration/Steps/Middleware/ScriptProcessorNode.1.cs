using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Middleware {
    
    public sealed class SaveRevisionProcessorNode : ProcessorNode {
        public new static string TagName { get { return "SaveRevisionProcessor"; } }
    
        public SaveRevisionProcessorNode(XmlElement xmlElem, IParser parser)
        {
        }
    }

}