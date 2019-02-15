using System;
using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library.Process
{
    public class ControlList : DataElementList
    {
        public ControlList(XmlNode parentNode) : base(parentNode) { }

        public XmlNode this[int index]
        {
            get
            {
                String xPath = String.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.CreateNew);

                return xmlNode;
            }
        }

        public XmlNode this[String name]
        {
            get
            {
                XmlNode xmlNode = GetControlNode(name);

                return xmlNode;
            }
            set
            {
                GetControlNode(name).InnerXml = value.InnerXml;
            }
        }

        public ControlList GetSubControl(String name)
        {
            ControlList subControl = name != String.Empty ? new ControlList(GetControlNode(name)) : null;

            return subControl;
        }

        private XmlNode GetControlNode(String name)
        {
            String xPath = String.Format("{0}", name);
            XmlNode node = CommonXml.GetNode(ParentNode, xPath);

            return node;
        }
    }
}