// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library
{
    public class AttributeList
    {
        private readonly XmlNode _xmlNode;

        public AttributeList(XmlNode xmlNode)
        {
            _xmlNode = xmlNode;
        }

        public String this[string name]
        {
            get
            {
                String attribute = CommonXml.GetAttributeValue(_xmlNode, name);

                return attribute;
            }
            set
            {
                CommonXml.SetAttributeValue(_xmlNode, name, value);
            }
        }
    }
}