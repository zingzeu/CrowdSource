using System.Xml;

namespace Zezo.Core.Configuration {

    public interface IParser {
        ConfigurationNode ParseXmlString(string xmlDoc);
        ///<summary>
        ///Gets the XML attribute smartly. Looking for an attribute first,
        ///then fallback to children.
        ///</summary>
        string GetStringAttribute(XmlElement elem, string key, string qualifiedKey);
    }

    public interface IXmlParsingHelper {
      
  
    }
}