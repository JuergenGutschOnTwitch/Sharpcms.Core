// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Xml;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;
using Sharpcms.Base.Library.Users;

namespace Sharpcms.Providers.Base
{
    public class ProviderUser : BasePlugin2, IPlugin2
    {
        private Users _users;

        public Users Users
        {
            get
            {
                Users users = _users ?? (_users = new Users(Process));

                return users;
            }
        }

        public new String Name
        {
            get
            {
                return "User";
            }
        }

        public new String[] Implements
        {
            get
            {
                return new[] {"users"};
            }
        }

        public ProviderUser() { }

        public ProviderUser(Process process)
        {
            Process = process;
        }

        public new void Handle(String mainEvent)
        {
            switch (mainEvent)
            {
                case "saveuser":
                    HandleSaveUser();
                    break;
                case "deleteuser":
                    HandleDeleteUser();
                    break;
                case "adduser":
                    HandleAddUser();
                    break;
                case "addgroup":
                    HandleAddGroup();
                    break;
                case "deletegroup":
                    HandleDeleteGroup();
                    break;
            }
        }

        public new void Load(ControlList control, String action, String value, String pathTrail)
        {
            switch (action)
            {
                case "users":
                    LoadUsers(control);
                    break;
                case "groups":
                    LoadGroups(control);
                    break;
                case "user":
                    LoadUser(control, pathTrail);
                    break;
                case "frontpage":
                    FrontPage();
                    break;
            }
        }

        public new Object Invoke(String api, String action, params Object[] args)
        {
            Object objectInvoke = null;

            switch (api)
            {
                case "users":
                    objectInvoke = InvokeUsers(action, args);
                    break;
            }

            return objectInvoke;
        }

        private void HandleDeleteGroup()
        {
            Users users = new Users(Process);
            users.GroupList.Remove(Process.QueryEvents["mainvalue"]);
            users.Save();
        }

        private void HandleAddGroup()
        {
            Users users = new Users(Process);
            if (users.GroupList[Process.QueryEvents["mainvalue"]] == null)
            {
                Group group = users.GroupList.Create(Process.QueryEvents["mainvalue"]);
                if (group.Name.Length > 0)
                {
                    users.Save();
                }
            }
        }

        private void HandleAddUser()
        {
            Users users = new Users(Process);
            if (users.UserList[Process.QueryEvents["mainvalue"]] == null)
            {
                User user = users.UserList.Create(Process.QueryEvents["mainvalue"]);
                if (user.Login.Length > 0)
                {
                    users.Save();
                }
            }
        }

        private void HandleSaveUser()
        {
            Users users = new Users(Process);

            User user = users.UserList[Process.QueryEvents["mainvalue"]];
            user.Login = Process.QueryData["user_login"];
            
            if ("emptystring" != Process.QueryData["user_password"])
            {
                user.Password = Process.QueryData["user_password"];
            }

            user.GroupList.Clear();

            String groups = Process.QueryData["user_groups"];
            foreach (String groupname in groups.Split(','))
            {
                user.GroupList.Create(groupname);
            }

            users.Save();
        }

        private void HandleDeleteUser()
        {
            Users users = new Users(Process);
            users.UserList.Remove(Process.QueryEvents["mainvalue"]);
            users.Save();
        }

        private void FrontPage()
        {
            Object[] results = Process.Plugins.InvokeAll("users", "list_groups", Process.CurrentUser);
            List<String> userGroups = new List<String>(Common.FlattenToStrings(results));

            foreach (String group in userGroups)
            {
                String xPath = String.Format("groups/item[@name='{0}']", group);
                XmlNode node;

                try
                {
                    node = Process.Settings.GetAsNode(xPath);
                }
                catch
                {
                    node = null;
                }

                if (node != null)
                {
                    var frontPage = CommonXml.GetAttributeValue(node, "frontpage");
                    if (!String.IsNullOrEmpty(frontPage))
                    {
                        Process.HttpPage.Response.Redirect(frontPage + "/");
                    }
                }
            }
        }

        private void LoadGroups(ControlList control)
        {
            control["groups"].InnerXml = Users.GroupList.ParentNode.InnerXml;
        }

        private void LoadUser(ControlList control, String value)
        {
            Users users = new Users(Process);
            if (value != null && users.UserList[value] != null)
            {
                control["user"].InnerXml = users.UserList[value].Node.InnerXml;
            }
        }

        private void LoadUsers(ControlList control)
        {
            Users users = new Users(Process);
            control["users"].InnerXml = users.UserList.ParentNode.InnerXml;
        }

        private Object InvokeUsers(String action, Object[] args)
        {
            Object invokeUsers = null;

            switch (action)
            {
                case "verify":
                    invokeUsers = UsersVerify(args);
                    break;
                case "list_groups":
                    invokeUsers = UsersGroups(args);
                    break;
            }

            return invokeUsers;
        }

        private Object UsersVerify(Object[] args)
        {
            Object usersVerify = null;

            if (args != null && args.Length > 1)
            {
                String username = args[0].ToString();
                String password = args[1].ToString();
                User user = Users.UserList[username];
                
                usersVerify = user != null && user.CheckPassword(password);
            }

            return usersVerify;
        }

        private Object UsersGroups(Object[] args)
        {
            Object usersGroups = null;

            if (args != null && args.Length > 0)
            {
                var username = args[0].ToString();
                var groups = new List<String>();

                var user = Users.UserList[username];
                if (user != null)
                {
                    int groupCount = user.GroupList.Count;
                    for (int i = 0; i < groupCount; i++)
                    {
                        groups.Add(user.GroupList[i].Name);
                    }
                }

                usersGroups = groups.Count > 0 ? groups.ToArray() : null;
            }

            return usersGroups;
        }
    }
}