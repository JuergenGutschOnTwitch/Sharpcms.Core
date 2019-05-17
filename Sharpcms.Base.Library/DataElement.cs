// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library
{
    public class DataElement
    {
        private readonly AttributeList _attributes;
        private readonly XmlNode _xmlNode;

        protected DataElement(XmlNode node)
        {
            _xmlNode = node;
            _attributes = new AttributeList(node);
        }

        public XmlNode Node
        {
            get
            {
                return _xmlNode;
            }
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument document = _xmlNode != null ? _xmlNode.OwnerDocument : null;

                return document;
            }
        }

        protected AttributeList Attributes
        {
            get
            {
                return _attributes;
            }
        }

        protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
        {
            XmlNode xmlNode = CommonXml.GetNode(_xmlNode, cleanPath, emptyNode);

            return xmlNode;
        }

        private XmlNode SelectNode(string xPath)
        {
            XmlNode xmlNode = _xmlNode.SelectSingleNode(xPath);

            return xmlNode;
        }

        protected string GetNodeValue(string name)
        {
            string nodeValue = string.Empty;

            if (_xmlNode != null)
            {
                XmlNode node = SelectNode(name);
                if (node != null)
                {
                    nodeValue = node.InnerText;
                }
            }

            return nodeValue;
        }
    }
}