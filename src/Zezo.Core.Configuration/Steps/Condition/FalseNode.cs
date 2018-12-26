using System.Xml;

namespace Zezo.Core.Configuration.Steps.Condition
{
    public class FalseNode : ConditionNode
    {
        public new static string TagName => "False";
        public FalseNode(XmlElement xmlElem, IParser parser) 
        {
        }  
    }
}