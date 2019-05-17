// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Base.Library;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Data.SiteTree
{
    public class Container : DataElement
    {
        public readonly ElementList Elements;

        public Container(XmlNode node) : base(node)
        {
            XmlNode elementNode = CommonXml.GetNode(Node, "elements", EmptyNodeHandling.CreateNew);
            Elements = new ElementList(elementNode);
        }
    }
}