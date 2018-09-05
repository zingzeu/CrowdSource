using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration {

    public interface IParser {
        ConfigurationNode ParseXmlString(string xmlDoc);
        ConfigurationNode ParseXmlElement(XmlElement elem);
        ///<summary>
        ///Gets the XML attribute smartly. Looking for an attribute first,
        ///then fallback to children.
        ///</summary>
        string GetStringAttribute(XmlElement elem, string key, string qualifiedKey);
        IReadOnlyList<XmlElement> GetComplexAttribute(XmlElement elem, string qualifiedKey);
    }

    public interface IXmlParsingHelper {
      
  
    }
}