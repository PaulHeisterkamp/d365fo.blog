using System.Collections.Generic;
using System.Xml;

namespace ODataClientXmlFiltering
{
    class Program
    {
        static void removeNodes(XmlDocument _xmlDoc, HashSet<string> _namesToKeep)
        {
            bool elementDeleted = false;

            do
            {
                elementDeleted = false;
                XmlNodeList nodes = _xmlDoc.SelectNodes("//*");

                foreach (XmlNode node in nodes)
                {
                    if (node.Name == "EntityType")
                    {
                        if (!_namesToKeep.Contains(node.Attributes.GetNamedItem("Name").Value))
                        {
                            node.ParentNode.RemoveChild(node);
                            elementDeleted = true;
                        }
                    }
                    else if (node.Name == "EnumType")
                    {
                        if (!_namesToKeep.Contains(node.Attributes.GetNamedItem("Name").Value))
                        {
                            node.ParentNode.RemoveChild(node);
                            elementDeleted = true;
                        }
                    }
                    else if (node.Name == "Action")
                    {
                        node.ParentNode.RemoveChild(node);
                        elementDeleted = true;
                    }
                    else if (node.Name == "EntityContainer")
                    {
                        if (node.HasChildNodes)
                        {
                            foreach (XmlNode childNode in node.ChildNodes)
                            {
                                if (childNode.Name == "EntitySet")
                                {
                                    if (!_namesToKeep.Contains(childNode.Attributes.GetNamedItem("Name").Value))
                                    {
                                        childNode.ParentNode.RemoveChild(childNode);
                                        elementDeleted = true;
                                    }
                                }
                            }
                        }
                    }
                }
            } while (elementDeleted);
        }

        static void calcNamesToKeepChildNodes(XmlNode _node, HashSet<string> _namesToKeep)
        {
            if (_node.Attributes != null && _node.Attributes.GetNamedItem("Type") != null)
            {
                string typeString = _node.Attributes.GetNamedItem("Type").Value;
                if (typeString.Contains("Collection("))
                {
                    typeString = typeString.Substring(typeString.IndexOf("Collection(") + 11);
                    typeString = typeString.Substring(0, typeString.Length - 1);
                }

                if (typeString.Contains("Microsoft.Dynamics.DataEntities"))
                {
                    typeString = typeString.Substring(typeString.IndexOf("Microsoft.Dynamics.DataEntities") + 32);
                    if (!_namesToKeep.Contains(typeString))
                    {
                        _namesToKeep.Add(typeString);
                    }
                }
            }

            if (_node.HasChildNodes)
            {
                foreach (XmlNode childNode in _node.ChildNodes)
                {
                    calcNamesToKeepChildNodes(childNode, _namesToKeep);
                }
            }
        }

        static void calcNamesToKeep(XmlDocument _xmlDoc, HashSet<string> _namesToKeep)
        {
            int namesBefore = _namesToKeep.Count;

            XmlNodeList nodes = _xmlDoc.SelectNodes("//*");

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes != null && node.Attributes.GetNamedItem("Name") != null &&
                    _namesToKeep.Contains(node.Attributes.GetNamedItem("Name").Value))
                {
                    if (node.HasChildNodes)
                    {
                        calcNamesToKeepChildNodes(node, _namesToKeep);
                    }
                }
            }

            if (_namesToKeep.Count > namesBefore)
            {
                calcNamesToKeep(_xmlDoc, _namesToKeep);
            }
        }

        static void removeNavigationProperty(XmlDocument _xmlDoc)
        {
            bool elementDeleted = false;

            do
            {
                elementDeleted = false;
                XmlNodeList nodes = _xmlDoc.SelectNodes("//*");

                foreach (XmlNode node in nodes)
                {
                    if (node.Name == "EntityType" && node.HasChildNodes)
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.Name == "NavigationProperty")
                            {
                                childNode.ParentNode.RemoveChild(childNode);
                                elementDeleted = true;
                            }
                        }
                    }
                }
            } while (elementDeleted);
        }

        static void Main(string[] args)
        {
            HashSet<string> namesToKeep = new HashSet<string>();
            namesToKeep.Add("Resources");
            namesToKeep.Add("SalesOrderHeadersV2");
            namesToKeep.Add("SalesOrderHeaderV2");
            namesToKeep.Add("SalesOrderLines");
            namesToKeep.Add("SalesOrderLine");
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"C:\Temp\ODataMetaXml\metaProd.xml");

            removeNavigationProperty(xmlDoc);
            calcNamesToKeep(xmlDoc, namesToKeep);
            removeNodes(xmlDoc, namesToKeep);

            xmlDoc.Save(@"C:\Temp\ODataMetaXml\reducedMetaProd.xml");
        }
    }

}
