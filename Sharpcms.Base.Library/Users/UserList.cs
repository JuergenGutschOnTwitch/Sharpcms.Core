// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace Sharpcms.Base.Library.Users
{
    public class UserList : DataElementList
    {
        public UserList(XmlNode parentNode) : base(parentNode)
        {
        }

        public User this[int index]
        {
            get
            {
                string xPath = string.Format("user[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                User user = null;

                if (node != null)
                {
                    user = new User(node);
                }

                return user;
            }
        }

        public User this[string name]
        {
            get
            {
                //ToDo this is not implementet yet
                string xPath = string.Format("user[login='{0}']", Common.Common.CleanToSafeString(name));
                XmlNode node = ParentNode.SelectSingleNode(xPath);
                User user = null;

                if (node != null)
                {
                    user = new User(node);
                }

                return user;
            }
        }

        public User Create(string login)
        {
            XmlNode node = Document.CreateElement("user");
            ParentNode.AppendChild(node);
            User user = new User(node) {Login = login};

            return user;
        }

        public void Remove(int index)
        {
            User user = this[index];
            ParentNode.RemoveChild(user.Node);
        }

        public void Remove(string name)
        {
            User user = this[name];
            ParentNode.RemoveChild(user.Node);
        }
    }
}