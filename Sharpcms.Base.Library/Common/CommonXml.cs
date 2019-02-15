// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Sharpcms.Base.Library.Common
{
    public static class CommonXml
    {
        public static string TransformXsl(string xsl, XmlDocument document, Cache cache)
        {
            XslCompiledTransform transform = GetTransform(xsl, cache);
            TextWriter textWriter = new StringWriter();

            transform.Transform(document, null, textWriter);

            return textWriter.ToString();
        }

        private static List<string> GetXslIncludes(string xsl)
        {
            List<string> includes = new List<string>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xsl);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);
            manager.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");

            XmlNodeList nodeList = xmlDocument.SelectNodes("//xsl:include", manager);
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    string filename = GetAttributeValue(node, "href");
                    string fullFilename = Common.CombinePaths(Path.GetDirectoryName(xsl), filename);
                    includes.Add(fullFilename);

                    List<string> additionalIncludes = GetXslIncludes(fullFilename);

                    if (additionalIncludes != null && additionalIncludes.Count > 0)
                    {
                        includes.AddRange(additionalIncludes);
                    }
                }
            }
            
            return includes;
        }

        public static void MergeXml(XmlDocument baseDocument, XmlDocument xmlDocument, params string[] uniqueIdentifiers)
        {
            DoMergeXml(baseDocument.DocumentElement, xmlDocument.DocumentElement, uniqueIdentifiers);
        }

        private static void DoMergeXml(XmlNode baseXmlNode, XmlNode mergeXmlNode, params string[] uniqueIdentifiers)
        {
            foreach (XmlNode xmlNode in mergeXmlNode)
            {
                if (xmlNode.Name != "#text")
                {
                    EmptyNodeHandling emptyNode = Common.StringArrayContains(uniqueIdentifiers, xmlNode.Name) 
                        ? EmptyNodeHandling.ForceCreateNew
                        : EmptyNodeHandling.CreateNew;

                    XmlNode currentBaseXmlnode = GetNode(baseXmlNode, xmlNode.Name, emptyNode);
                    if (xmlNode.Attributes != null)
                    {
                        foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                        {
                            SetAttributeValue(currentBaseXmlnode, xmlAttribute.Name, xmlAttribute.Value);
                        }
                    }

                    if (xmlNode.HasChildNodes)
                    {
                        DoMergeXml(currentBaseXmlnode, xmlNode, uniqueIdentifiers);
                    }
                }
                else
                {
                    XmlNode textNode = baseXmlNode.SelectSingleNode("text()");
                    if (textNode != null)
                    {
                        textNode.InnerText = xmlNode.Value;
                    }
                    else if (baseXmlNode.OwnerDocument != null)
                    {
                        baseXmlNode.AppendChild(baseXmlNode.OwnerDocument.CreateTextNode(xmlNode.Value));
                    }
                }
            }
        }

        private static XslCompiledTransform GetTransform(string xsl, Cache cache)
        {
            string cacheKey = "transform_" + Common.CleanToSafeString(xsl);
            FileInfo fileDependency = new FileInfo(xsl);

            object cacheTransform = cache[cacheKey, fileDependency];
            if (cacheTransform != null)
            {
                bool useCache = true;

                // Check included files
                Dictionary<string, string> includedFiles = cache[cacheKey + "_includeFiles"] as Dictionary<string, string>;
                if (includedFiles != null)
                {
                    foreach (string file in includedFiles.Keys)
                    {
                        if (File.GetLastWriteTime(file).ToString(CultureInfo.InvariantCulture) != includedFiles[file])
                        {
                            useCache = false;
                        }
                    }
                }

                if (useCache)
                {
                    return cacheTransform as XslCompiledTransform;
                }
            }

            // The file has changed or is not in memory
            XslCompiledTransform transform = new XslCompiledTransform(true);

            XsltSettings xsltSettings = new XsltSettings(true, true);
            transform.Load(xsl, xsltSettings, new XmlUrlResolver());

            cache[cacheKey, fileDependency] = transform;

            List<string> includeFiles = GetXslIncludes(xsl);
            Dictionary<string, string> includeDictionary = new Dictionary<string, string>();
            foreach (string includeFile in includeFiles)
            {
                includeDictionary[includeFile] = File.GetLastWriteTime(includeFile).ToString(CultureInfo.InvariantCulture);
            }

            cache[cacheKey + "_includeFiles"] = includeDictionary;

            return transform;
        }

        public static void SaveXmlDocument(string filename, XmlDocument document)
        {
            if (document == null)
            {
                return;
            }

            XmlWriterSettings writerSettings = new XmlWriterSettings {Indent = true};
            XmlWriter writer = XmlWriter.Create(filename, writerSettings);

            document.WriteContentTo(writer);

            writer.Close();
        }

        public static string GetXPath(XmlNode node)
        {
            List<string> xPath = new List<string>();

            XmlNode currentNode = node;
            while (node.OwnerDocument != null && currentNode != node.OwnerDocument.DocumentElement)
            {
                if (currentNode == null) continue;

                xPath.Add(currentNode.Name);
                currentNode = currentNode.ParentNode;
            }

            xPath.Reverse();

            return string.Join("/", xPath.ToArray());
        }

        #region Attribute-handling

        public static string GetAttributeValue(XmlNode node, string name)
        {
            if (node != null && node.Attributes != null)
            {
                XmlAttribute attributeNode = node.Attributes[name];
                if (attributeNode != null)
                {
                    return attributeNode.Value;
                }
            }

            return string.Empty;
        }

        public static void SetAttributeValue(XmlNode node, string name, string value)
        {
            if (node != null && node.Attributes != null)
            {
                XmlAttribute attribute = node.Attributes[name];
                if (attribute != null)
                {
                    attribute.Value = value;
                }
                else if (node.OwnerDocument != null)
                {
                    attribute = node.OwnerDocument.CreateAttribute(name);
                    attribute.Value = value;
                    node.Attributes.Append(attribute);
                }
            }
        }

        public static void AppendAttribute(XmlNode node, string name, string value)
        {
            if (node.OwnerDocument != null)
            {
                XmlAttribute attributeNode = node.OwnerDocument.CreateAttribute(name);
                attributeNode.Value = value;
                if (node.Attributes != null)
                {
                    node.Attributes.Append(attributeNode);
                }
            }
        }

        public static void CopyAttributes(XmlNode copyFrom, XmlNode copyTo)
        {
            if (copyFrom.Attributes != null)
            {
                foreach (XmlAttribute fromAttribute in copyFrom.Attributes)
                {
                    if (copyTo.OwnerDocument != null)
                    {
                        XmlAttribute toAttribute = copyTo.OwnerDocument.CreateAttribute(fromAttribute.Name);
                        toAttribute.Value = fromAttribute.Value;
                        if (copyTo.Attributes != null)
                        {
                            copyTo.Attributes.Append(toAttribute);
                        }
                    }
                }
            }
        }

        #endregion

        #region Various GetNode's

        /// <summary>
        /// Returns the requested node and creates it if it does not exist.
        /// </summary>
        /// <param name="document">XmlDocument to get the node from.</param>
        /// <param name="path">Path for the node.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlDocument document, string path)
        {
            XmlNode xmlNode = GetNode(document, path, EmptyNodeHandling.CreateNew);

            return xmlNode;
        }

        /// <summary>
        /// Returns the requested node and optionally creates it if it does not exist.
        /// </summary>
        /// <param name="document">XmlDocument to get the node from.</param>
        /// <param name="path">Path for the node.</param>
        /// <param name="emptyNode">Specifies whether to create a node if it does not exist.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlDocument document, string path, EmptyNodeHandling emptyNode)
        {
            if (document != null)
            {
                if (document.ChildNodes.Count == 0)
                {
                    if (emptyNode == EmptyNodeHandling.CreateNew)
                    {
                        int indexOfSlash = path.IndexOf("/", global::System.StringComparison.Ordinal);
                        string rootNodeName = path.Substring(0, indexOfSlash);
                        XmlNode rootNode = document.CreateElement(rootNodeName);
                        document.AppendChild(rootNode);
                        path = path.Substring(indexOfSlash + 1);
                    }
                    else
                    {
                        return null;
                    }
                }

                return GetNode(document.DocumentElement, path, emptyNode);
            }

            // If document is empty, create the root node
            return null;
        }

        /// <summary>
        /// Returns the requested node and creates it if it does not exist.
        /// </summary>
        /// <param name="fromXmlNode">From XML node.</param>
        /// <param name="path">Path for the node.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlNode fromXmlNode, string path)
        {
            XmlNode xmlNode = GetNode(fromXmlNode, path, EmptyNodeHandling.CreateNew);

            return xmlNode;
        }

        /// <summary>
        /// Returns the requested node and optionally creates it if it does not exist.
        /// </summary>
        /// <param name="fromXmlNode">From XML node.</param>
        /// <param name="path">Path for the node.</param>
        /// <param name="emptyNode">Specifies whether to create a node if it does not exist.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlNode fromXmlNode, string path, EmptyNodeHandling emptyNode)
        {
            string[] pathParts = path.Split('/');
            pathParts[0] = RenameIntegerPath(pathParts[0]);

            XmlNode xmlNode = null;
            if (emptyNode != EmptyNodeHandling.ForceCreateNew)
            {
                xmlNode = pathParts[0] == string.Empty 
                    ? fromXmlNode 
                    : fromXmlNode.SelectSingleNode(pathParts[0]);
            }
            else
            {
                if (fromXmlNode.OwnerDocument != null)
                {
                    xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathParts[0]); 
                    fromXmlNode.AppendChild(xmlNode);
                }
            }

            if (xmlNode == null)
            {
                if (emptyNode == EmptyNodeHandling.CreateNew)
                {
                    if (fromXmlNode.OwnerDocument != null)
                    {
                        xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathParts[0]); 
                        fromXmlNode.AppendChild(xmlNode);
                    }
                }
                else if (emptyNode == EmptyNodeHandling.Ignore)
                {
                    return null;
                }
            }

            for (int i = 1; i < pathParts.Length; i++)
            {
                string pathPart = pathParts[i];
                pathPart = RenameIntegerPath(pathPart);

                if (pathPart != string.Empty)
                {
                    XmlNode xmlOldNode = xmlNode;
                    if (emptyNode != EmptyNodeHandling.ForceCreateNew)
                    {
                        if (xmlOldNode != null)
                        {
                            xmlNode = xmlOldNode.SelectSingleNode(pathPart);
                        }
                        else
                        {
                            if (fromXmlNode.OwnerDocument != null)
                            {
                                xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathParts[0]);
                                fromXmlNode.AppendChild(xmlNode);
                            }
                        }
                    }
                    
                    if (xmlNode == null)
                    {
                        if (emptyNode == EmptyNodeHandling.CreateNew)
                        {
                            if (fromXmlNode.OwnerDocument != null)
                            {
                                xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathPart); 
                                
                                if (xmlOldNode != null)
                                {
                                    xmlOldNode.AppendChild(xmlNode);
                                }
                            }
                        }
                        else if (emptyNode == EmptyNodeHandling.Ignore)
                        {
                            break;
                        }
                    }
                }
            }
            return xmlNode;
        }

        public static string RenameIntegerPath(string pathPart)
        {
            string renamedPathPart = pathPart == string.Empty || !char.IsDigit(pathPart[0])
                ? pathPart
                : string.Format("int_{0}", pathPart);

            return renamedPathPart;
        }

        #endregion

        #region Move

        public static void MoveUp(XmlNode node)
        {
            XmlNode previousSibling = node.PreviousSibling;

            if (previousSibling == null) return;

            XmlNode nodeCopy = node.CloneNode(true);
            XmlNode previousCopy = previousSibling.CloneNode(true);

            if (node.ParentNode != null)
            {
                node.ParentNode.ReplaceChild(nodeCopy, previousSibling);
                node.ParentNode.ReplaceChild(previousCopy, node);
            }
        }

        public static void MoveTop(XmlNode node)
        {
            if (node.ParentNode != null)
            {
                XmlNode firsChild = node.ParentNode.FirstChild;

                if (firsChild == null || firsChild == node) return;

                //node.ParentNode.RemoveChild(node);
                node.ParentNode.InsertBefore(node, firsChild);
            }
        }

        public static void MoveBottom(XmlNode node)
        {
            if (node.ParentNode != null)
            {
                XmlNode lastChild = node.ParentNode.LastChild;

                if (lastChild == null || lastChild == node) return;

                //node.ParentNode.RemoveChild(node);
                node.ParentNode.InsertAfter(node, lastChild);
            }
        }

        public static void MoveDown(XmlNode node)
        {
            XmlNode nextSibling = node.NextSibling;

            if (nextSibling == null) return;

            XmlNode nodeCopy = node.CloneNode(true);
            XmlNode nextCopy = nextSibling.CloneNode(true);

            if (node.ParentNode != null)
            {
                node.ParentNode.ReplaceChild(nodeCopy, nextSibling);
                node.ParentNode.ReplaceChild(nextCopy, node);
            }
        }

        public static void Move(XmlNode node, XmlNode newParent)
        {
            Copy(node, newParent);

            if (node.ParentNode != null)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        public static void Copy(XmlNode node, XmlNode newParent)
        {
            XmlNode nodeCopy = node.CloneNode(true);

            newParent.AppendChild(nodeCopy);
        }

        #endregion
    }

    public enum EmptyNodeHandling
    {
        /// <summary>
        /// If the requested node does not exist, it is created, inserted and returned. 
        /// </summary>
        CreateNew,

        /// <summary>
        /// If the requested node does not exist, null is returned.
        /// </summary>
        Ignore,

        /// <summary>
        /// Always create a new node.
        /// </summary>
        ForceCreateNew
    }
}