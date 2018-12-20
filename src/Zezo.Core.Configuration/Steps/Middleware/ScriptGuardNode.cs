using System.Collections.Generic;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Middleware {
    
    public sealed class ScriptGuardNode : ProcessorNode {
        public new static string TagName { get { return "ScriptGuard"; } }

        public string Source { get; private set; }
        public string Class { get; private set; }
        public string InlineSource { get; private set; }
        public string Language { get; private set; }

        public ScriptGuardNode(XmlElement xmlElem, IParser parser)
        {
            this.Source = xmlElem.GetStringAttribute("Source");
            this.Class = xmlElem.GetStringAttribute("Class");
            this.Language = xmlElem.GetStringAttribute("Language");
            this.InlineSource = xmlElem.InnerText;
        }
    }

}