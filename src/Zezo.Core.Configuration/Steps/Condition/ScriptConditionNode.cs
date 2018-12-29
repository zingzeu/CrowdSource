using System;
using System.Xml;
using Zezo.Core.Configuration.Extensions;

namespace Zezo.Core.Configuration.Steps.Condition
{
    public sealed class ScriptConditionNode : ConditionNode
    {
        public new static string TagName => "ScriptCondition";
        public string Source { get; private set; }
        public string Class { get; private set; }
        public string InlineSource { get; private set; }
        public string Language { get; private set; }
        
        public ScriptConditionNode(XmlElement xmlElem, IParser parser) 
        {
            Source = xmlElem.GetStringAttribute("Source");
            Class = xmlElem.GetStringAttribute("Class");
            Language = xmlElem.GetStringAttribute("Language");
             InlineSource = xmlElem.InnerText;
            if (!Language.Equals("csharp", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Unknown scripting language '{Language}'.");
            }
        }  
    }
}