using System;
using System.Collections.Generic;
using System.Xml;

namespace Zezo.Core.Configuration.Extensions
{
    public static class XmlElementExtension {
        ///<summary>
        ///Gets the XML attribute smartly. Looking for an attribute first,
        ///then fallback to children.
        ///</summary>
        public static string GetStringAttribute(this XmlElement elem, string key)
        {
            // Try to get the XML attribute first.
            if (elem.GetAttributeNode(key) != null) {
                return elem.GetAttribute(key);
            }

            // If not found, find a subnode with the name "TagName.AttributeName".
            var qualifiedKey = $"{elem.LocalName}.{key}";
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

        public static bool GetBooleanAttribute(this XmlElement elem, string key) {
            string s = GetStringAttribute(elem, key);
            if (s.Equals("true", StringComparison.CurrentCultureIgnoreCase)) {
                return true;
            } else if (s.Equals("false", StringComparison.CurrentCultureIgnoreCase)) {
                return false;
            } else {
                throw new Exception($"{elem}.{key} does not have a Boolean value: \"{s}\"");
            }
        }

        public static IReadOnlyList<XmlElement> GetComplexAttribute(this XmlElement elem, string key)
        {
            var qualifiedKey = $"{elem.LocalName}.{key}";
            // find a subnode
            var attrNodes = elem.SelectNodes($"./{qualifiedKey}");
            if (attrNodes.Count > 1) {
                throw new Exception($"Attribute {qualifiedKey} occurs more than once.");
            } else if (attrNodes.Count == 0) {
                return new List<XmlElement>();
            } else {
                var node = attrNodes.Item(0) as XmlElement;
                var subNodes = new List<XmlElement>();
                foreach (XmlNode subNode in node.ChildNodes) {
                    if (subNode as XmlElement != null) {
                        subNodes.Add(subNode as XmlElement);
                    }
                }
                return subNodes;
            }
        }
    
        public static IReadOnlyList<T> GetCollectionAttribute<T>(this XmlElement elem, string key, IParser parser) where T : ConfigurationNode {
            var typeName = typeof(T).Name;
            
            var tNodes = elem.GetComplexAttribute(key);
            var result = new List<T>();
            if (tNodes.Count > 0) {
                foreach (var tNode in tNodes) {
                    var tmp = parser.ParseXmlElement(tNode) as T;
                    if (tmp == null) {
                        throw new Exception($"{tNode.LocalName} is not a valid {typeName}");
                    }
                    result.Add(tmp);
                }
            }
            return result;
        } 
    
        public static T GetComplexSingleAttribute<T>(this XmlElement elem, string key, IParser parser) where T : ConfigurationNode {
            var typeName = typeof(T).Name;
            var tNodes = elem.GetComplexAttribute(key);
            if (tNodes.Count == 0) {
                return null;
            }
            else if (tNodes.Count > 1) {
                throw new Exception($"{elem.LocalName}.{key} can only have one value, {tNodes.Count} found.");   
            } else {
                var tNode = tNodes[0]; 
                var tmp = parser.ParseXmlElement(tNode) as T;
                if (tmp == null) {
                    throw new Exception($"{tNode.LocalName} is not a valid {typeName}");
                }
                return tmp;
            }
        }
    }
}