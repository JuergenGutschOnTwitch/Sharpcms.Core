// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library.Users
{
    public class Users
    {
        private readonly GroupList _groupList;
        private readonly XmlDocument _userDocument;
        private readonly string _userFileName;
        private readonly UserList _userList;

        public Users(Process.Process process)
        {
            _userFileName = process.Settings["users/filename"];
            _userDocument = new XmlDocument();
            _userDocument.Load(_userFileName);
            _userList = new UserList(CommonXml.GetNode(_userDocument.DocumentElement, "users"));
            _groupList = new GroupList(CommonXml.GetNode(_userDocument.DocumentElement, "groups"));
        }

        public UserList UserList
        {
            get
            {
                return _userList;
            }
        }

        public GroupList GroupList
        {
            get
            {
                return _groupList;
            }
        }

        public void Save()
        {
            _userDocument.Save(_userFileName);
        }
    }
}