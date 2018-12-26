using System.Xml;

namespace Zezo.Core.Configuration.Steps.Condition
{
    public class TrueNode : ConditionNode
    {
        public new static string TagName => "True";
        public TrueNode(XmlElement xmlElem, IParser parser) 
        {
            
        }  
    }
}