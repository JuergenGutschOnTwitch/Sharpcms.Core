// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Base.Library;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Data.SiteTree
{
    public class Element : DataElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public Element(XmlNode node) : base(node) { }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified name.
        /// </summary>
        /// <value></value>
        public string this[string name]
        {
            get
            {
                string xPath = string.Format("{0}", name);
                XmlNode node = GetNode(xPath, EmptyNodeHandling.CreateNew);

                return node.InnerText;
            }
            set
            {
                string xPath = string.Format("{0}", name);
                XmlNode node = GetNode(xPath, EmptyNodeHandling.CreateNew);
                node.InnerText = value;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get
            {
                return Attributes["type"];
            }
            set
            {
                Attributes["type"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return Attributes["name"];
            }
            set
            {
                Attributes["name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the publish.
        /// </summary>
        /// <value>The publish.</value>
        public string Publish
        {
            get
            {
                return Attributes["publish"];
            }
            set
            {
                Attributes["publish"] = value;
            }
        }
    }
}