// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Xml;
using Sharpcms.Base.Library;

namespace Sharpcms.Data.SiteTree
{
    public class ContainerList : DataElementList
    {
        public ContainerList(XmlNode parentNode) : base(parentNode)
        {
        }

        public Container this[int index]
        {
            get
            {
                String xPath = String.Format("container[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                Container container = node != null 
                    ? new Container(node) 
                    : null;

                return container;
            }
        }

        public Container this[String name]
        {
            get
            {
                String xPath = String.Format("container[@name='{0}']", name);
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                Container container;
                
                if (node != null)
                {
                    container = new Container(node);
                }
                else
                {
                    node = Document.CreateElement("container");
                    XmlAttribute typeAttribute = Document.CreateAttribute("name");
                    typeAttribute.Value = name;
                    if (node.Attributes != null)
                    {
                        node.Attributes.Append(typeAttribute);
                    }

                    ParentNode.AppendChild(node);

                    container = new Container(node);
                }

                return container;
            }
        }

        public void Remove(int index)
        {
            String xPath = String.Format("container[{0}]", index + 1);
            XmlNode node = ParentNode.SelectSingleNode(xPath);
            if(node != null)
            {
                ParentNode.RemoveChild(node);
            }
        }

        public int Index(String containerName)
        {
            int index = 0;
            String xPath = String.Format("container");
            XmlNodeList nodes = ParentNode.SelectNodes(xPath);
            
            if (nodes != null)
            {
                int i = 0;
                foreach (XmlNode node in nodes)
                {
                    i++;
                    if (node.Attributes != null && node.Attributes["name"].Value == containerName)
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }
    }
}