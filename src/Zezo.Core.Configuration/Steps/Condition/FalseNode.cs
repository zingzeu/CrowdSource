using System.Xml;

namespace Zezo.Core.Configuration.Steps.Condition
{
    public sealed class FalseNode : ConditionNode
    {
        public new static string TagName => "False";
        public FalseNode(XmlElement xmlElem, IParser parser) 
        {
        }  
    }
}