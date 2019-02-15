// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library
{
    public class DataElementList
    {
        private readonly XmlNode _parentNode;

        protected DataElementList(XmlNode parentNode)
        {
            _parentNode = parentNode;
        }

        public XmlNode ParentNode
        {
            get
            {
                return _parentNode;
            }
        }

        protected XmlDocument Document
        {
            get
            {
                XmlDocument xmlDocument = _parentNode != null 
                    ? _parentNode.OwnerDocument 
                    : null;

                return xmlDocument;
            }
        }

        public int Count
        {
            get
            {
                int count = 0;

                if (_parentNode != null)
                {
                    XmlNodeList xmlNodeList = _parentNode.SelectNodes("*");
                    if (xmlNodeList != null)
                    {
                        count = xmlNodeList.Count;
                    }
                }

                return count;
            }
        }

        protected XmlNode GetNode(string cleanPath, EmptyNodeHandling emptyNode)
        {
            XmlNode xmlNode = CommonXml.GetNode(_parentNode, cleanPath, emptyNode);

            return xmlNode;
        }
    }
}