using System;
using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library.Process
{
    public class XmlItemList : DataElementList
    {
        public XmlItemList(XmlNode parentNode) : base(parentNode) { }

        public Query this[int index]
        {
            get
            {
                String xPath = String.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.CreateNew);
                Query query = new Query(xmlNode.Name, xmlNode.InnerText);

                return query;
            }
            set
            {
                String xPath = String.Format("*[{0}]", index + 1);
                XmlNode xmlNode = GetNode(xPath, EmptyNodeHandling.Ignore);

                xmlNode.InnerText = value.Value;
            }
        }

        public String this[String name]
        {
            get
            {
                String xPath = String.Format("{0}", name);
                String innerText = GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText;

                return innerText;
            }
            set
            {
                String xPath = String.Format("{0}", name);

                GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText = value;
            }
        }
    }
}