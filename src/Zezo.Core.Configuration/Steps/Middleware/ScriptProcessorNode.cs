using System.Collections.Generic;

namespace Zezo.Core.Configuration.Middleware {
    
    public sealed class ScriptProcessorNode : ProcessorNode {
        public new static string TagName { get { return "ScriptProcessor"; } }
        public string Source { get; private set; }
        public string Class { get; private set; }
        public string InlineSource { get; private set; }
        public string Language { get; private set; }
        
    }

}