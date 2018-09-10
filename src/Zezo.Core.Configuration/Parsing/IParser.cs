using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration {

    public interface IParser {
        ConfigurationNode ParseXmlString(string xmlDoc);
        ConfigurationNode ParseXmlElement(XmlElement elem);
    }

    public interface IXmlParsingHelper {
      
  
    }
}