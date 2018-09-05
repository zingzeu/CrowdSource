using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Zezo.Core.Configuration {

    public class Parser : IParser, IXmlParsingHelper {
        private readonly Dictionary<string, Type> concreteNodeTypes = new Dictionary<string, Type>();
        public Parser()
        {
            FindAllNodeTypes();
        }

        private void FindAllNodeTypes() {
            Assembly configurationAssm = this.GetType().Assembly;
            foreach (Type type in configurationAssm.GetTypes())
            {
                if (type.IsSubclassOf(typeof(ConfigurationNode)) && !type.IsAbstract) {
                    var tagNameProp = type.GetProperty("TagName");
                    var tagName = tagNameProp.GetValue(null) as string;
                    concreteNodeTypes.Add(tagName, type);
                }
            }
        }
        public ConfigurationNode ParseXmlString(string xmlDoc)
        {
            XmlDocument doc = new XmlDocument();  
            doc.LoadXml(xmlDoc);
            return ParseXmlElement(doc.FirstChild as XmlElement);  
        }

        protected ConfigurationNode ParseXmlElement(XmlElement elem) {
            var nodeTypeName = elem.LocalName;
            Type nodeType;
            if (!concreteNodeTypes.TryGetValue(nodeTypeName, out nodeType)) {
                throw new Exception($"Unknown XML element {nodeTypeName}");
            }

            // Get the public instance constructor that takes an XmlElement and an IParser.
            Type[] constructorParamTypes = new Type[2] { typeof(XmlElement), typeof(IParser) };
            ConstructorInfo constructorInfoObj = nodeType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public, null,
                CallingConventions.HasThis, constructorParamTypes, null);
            if (constructorInfoObj == null)
            {
                throw new Exception($"Config node type {nodeTypeName} does not have proper constructor.");
            }

            // Instantiate the ConfigurationNode
            try {
                ConfigurationNode node = constructorInfoObj.Invoke(new object[] {elem, this}) as ConfigurationNode;
                return node;
            } catch (Exception e) {
                throw new Exception($"Error when calling constructor of {nodeTypeName}: {e.ToString()}");
            }
        }

        public string GetStringAttribute(XmlElement elem, string key, string qualifiedKey)
        {
            if (elem.GetAttributeNode(key) != null) {
                return elem.GetAttribute(key);
            }
            // find a subnode
            var subNodes = elem.SelectNodes($"./{qualifiedKey}");
            if (subNodes.Count > 1) {
                throw new Exception($"Attribute {qualifiedKey} occurs more than once.");
            } else if (subNodes.Count == 0) {
                return "";
            } else {
                var node = subNodes.Item(0) as XmlElement;
                return node?.InnerText ?? "";
            }
        }
    }
}