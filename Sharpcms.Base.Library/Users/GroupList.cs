// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace Sharpcms.Base.Library.Users
{
    public class GroupList : DataElementList
    {
        public GroupList(XmlNode parentNode) : base(parentNode) { }

        public Group this[int index]
        {
            get
            {
                string xPath = string.Format("group[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                Group group = null;

                if (node != null)
                {
                    group = new Group(node);
                }

                return group;
            }
        }

        public Group this[string name]
        {
            // ToDo: group is not consistent yet
            get
            {
                string xPath = string.Format("group[@name='{0}']", Common.Common.CleanToSafeString(name));
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                Group group = null;

                if (node != null)
                {
                    group = new Group(node);
                }

                return group;
            }
        }

        public Group Create(string name)
        {
            XmlNode node = Document.CreateElement("group");
            ParentNode.AppendChild(node);
            Group group = new Group(node) {Name = name};

            return group;
        }

        public void Remove(int index)
        {
            Group group = this[index];
            ParentNode.RemoveChild(group.Node);
        }

        public void Remove(string name)
        {
            Group group = this[name];
            ParentNode.RemoveChild(group.Node);
        }

        public void Clear()
        {
            ParentNode.RemoveAll();
        }
    }
}