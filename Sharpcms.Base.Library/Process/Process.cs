// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Microsoft.AspNetCore.Http;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Http;
using Sharpcms.Base.Library.Plugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Sharpcms.Base.Library.Process
{
    public class Process
    {
        private const String CookieSeparator = "cookieseparator";
        private readonly String _basePath;
        private Cache _cache;
        private String _currentProcess;
        private Settings _settings;
        private Dictionary<String, String> _variables;
        public String MainTemplate; //ToDo: this should be more logical
        public bool OutputHandledByModule;
        public readonly HttpPage HttpPage;
        public readonly PluginServices Plugins;
        public readonly ControlList Content;
        public readonly XmlItemList Attributes;
        public readonly XmlItemList QueryData;
        public readonly XmlItemList QueryEvents;
        public readonly XmlItemList QueryOther;

        public Process(HttpPage httpPage, PluginServices pluginServices)
        {
            _currentProcess = String.Empty;

            Plugins = pluginServices;
            HttpPage = httpPage;
            XmlData = new XmlDocument();

            Plugins.FindPlugins(this, Common.Common.CombinePaths(Root, "Bin"));

            var xmlNode = XmlData.CreateElement("data");
            XmlData.AppendChild(xmlNode);

            Content = new ControlList(xmlNode);

            _basePath = GetBasePath(httpPage);
            Content["basepath"].InnerText = _basePath;

            var referrer = httpPage.Server.UrlEncode(httpPage.Request.Referrer?.ToString());
            Content["referrer"].InnerText = referrer ?? String.Empty;

            var domain = httpPage.Server.UrlEncode(httpPage.Request.ServerName);
            Content["domain"].InnerText = domain ?? String.Empty;

            var useragent = httpPage.Server.UrlEncode(httpPage.Request.UserAgent);
            Content["useragent"].InnerText = useragent ?? String.Empty;

            var sessionid = httpPage.Server.UrlEncode(httpPage.Session.Id.ToString(CultureInfo.InvariantCulture));
            Content["sessionid"].InnerText = sessionid ?? String.Empty;

            var ip = httpPage.Server.UrlEncode(httpPage.Request.RemoteAddress.ToString());
            Content["ip"].InnerText = ip ?? String.Empty;

            Attributes = new XmlItemList(CommonXml.GetNode(xmlNode, "attributes", EmptyNodeHandling.CreateNew));
            QueryData = new XmlItemList(CommonXml.GetNode(xmlNode, "query/data", EmptyNodeHandling.CreateNew));
            QueryEvents = new XmlItemList(CommonXml.GetNode(xmlNode, "query/events", EmptyNodeHandling.CreateNew));
            QueryOther = new XmlItemList(CommonXml.GetNode(xmlNode, "query/other", EmptyNodeHandling.CreateNew));

            ProcessQueries();
            ConfigureDebugging();
            LoginByCookie();

            var mainEvent = QueryEvents["main"];
            var mainEventValue = QueryEvents["mainValue"];

            if (mainEvent == "login")
            {
                if (!Login(QueryData["login"], QueryData["password"]))
                {
                    if (_settings != null && _settings["messages/loginerror"] != String.Empty)
                    {
                        httpPage.Response.Redirect(GetErrorUrl(httpPage.Server.UrlEncode(_settings["messages/loginerror"])));
                    }
                    else
                    {
                        httpPage.Response.Redirect(GetRedirectUrl());
                    }
                }
            }
            else if (mainEvent == "logout")
            {
                Logout();
                if (mainEventValue != String.Empty)
                {
                    HttpPage.Response.Redirect(mainEventValue);
                }
            }
            else if (mainEvent == String.Empty)
            {
                if (mainEventValue != String.Empty)
                {
                    HttpPage.Response.Redirect("/");
                }
            }

            UpdateCookieTimeout();

            LoadBaseData();
            // loads new user...
        }

        public Dictionary<String, String> Variables
        {
            get
            {
                return _variables ?? (_variables = new Dictionary<String, String>());
            }
        }

        public String RedirectUrl { get; set; }

        private String GetRedirectUrl()
        {
            var process = QueryOther["process"];

            if (process.Trim().EndsWith("/"))
            {
                process = process.Remove(process.Length - 1, 1);
            }

            var redirectUrl = String.Format("{0}login/?redirect={1}", BasePath, process);

            return redirectUrl;
        }

        private String GetErrorUrl(String loginError)
        {
            var process = QueryOther["process"];

            if (process.Trim().EndsWith("/"))
            {
                process = process.Remove(process.Length - 1, 1);
            }

            var errorUrl = String.Format("{0}login/?error={1}&redirect={2}", BasePath, loginError, process);

            return errorUrl;
        }

        private String GetBasePath(HttpPage httpPage)
        {
            var basePath = String.Empty;

            if (httpPage.Request.ApplicationPath != null)
            {
                var serverProtocol = httpPage.Request.ServerProtocol.Split('/')[0].ToLower();
                var serverName = httpPage.Request.ServerName;
                var serverPort = httpPage.Request.ServerPort;
                var applicationPath = httpPage.Request.ApplicationPath.TrimEnd('/');

                basePath = $"{serverProtocol}://{serverName}";

                if (serverPort.HasValue && serverPort != 80)
                {
                    basePath += $":{serverPort.Value}";
                }

                var baseUri = new Uri(basePath);
                var applicationBaseUri = new Uri(baseUri, applicationPath);
                
                basePath = applicationBaseUri.AbsoluteUri;
            }

            return basePath;
        }

        private bool DebugEnabled
        {
            get
            {
                var debugEnabled = (HttpPage.Session.ContainsKey("enabledebug") && HttpPage.Session["enabledebug"].ToString() == "true");

                return debugEnabled;
            }
            set
            {
                HttpPage.Session["enabledebug"] = value
                    ? "true"
                    : "false";
            }
        }

        private String BasePath
        {
            get
            {
                return _basePath;
            }
        }

        public Cache Cache
        {
            get
            {
                return _cache ?? (_cache = new Cache(HttpPage.Application));
            }
        }

        public XmlDocument XmlData { get; private set; }

        public String CurrentProcess
        {
            get
            {
                if (_currentProcess == String.Empty)
                {
                    var process = QueryOther["process"];

                    _currentProcess = process != String.Empty
                        ? process
                        : Settings["general/stdprocess"];
                }

                return _currentProcess;
            }
        }

        public String Root
        {
            get
            {
                return HttpPage.Server.MapPath("");
            }
        }

        public Settings Settings
        {
            get
            {
                return _settings ?? (_settings = new Settings(this, Root));
            }
        }

        public Object SearchContext
        {
            set
            {
                HttpPage.Session["SearchContext"] = value;
            }
        }

        public String CurrentUser
        {
            get
            {
                if (!HttpPage.Session.ContainsKey("current_username"))
                {
                    Logout();
                }

                return HttpPage.Session.ContainsKey("current_username")
                    ? HttpPage.Session["current_username"].ToString()
                    : String.Empty;
            }
        }

        private void AddMessage(String message, MessageType messageType, String type)
        {
            var xmlNode = CommonXml.GetNode(XmlData.DocumentElement, "messages", EmptyNodeHandling.CreateNew);
            xmlNode = CommonXml.GetNode(xmlNode, "item", EmptyNodeHandling.ForceCreateNew);
            xmlNode.InnerText = message;

            CommonXml.SetAttributeValue(xmlNode, "messagetype", messageType.ToString());
            CommonXml.SetAttributeValue(xmlNode, "type", type);

            var plugin = Plugins["ErrorLog"];
            if (plugin != null)
            {
                plugin.Handle("log");
            }
        }

        public void AddMessage(String message, MessageType messageType = MessageType.Message)
        {
            AddMessage(message, messageType, String.Empty);
        }

        public void AddMessage(Exception exception)
        {
            AddMessage(exception.Message, MessageType.Error, exception.GetType().ToString());
        }

        public void DebugMessage(Object message)
        {
            DebugMessage(message.ToString());
        }

        private void DebugMessage(String message)
        {
            if (DebugEnabled)
            {
                AddMessage(message, MessageType.Debug);
            }
        }

        private void LoadBaseData()
        {
            var userNode = Content.GetSubControl("basedata")["currentuser"];
            CommonXml.GetNode(userNode, "username").InnerText = CurrentUser;
            var groupNode = CommonXml.GetNode(userNode, "groups");
            var resultsGroups = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            var userGroups = new List<String>(Common.Common.FlattenToStrings(resultsGroups));

            foreach (var group in userGroups)
            {
                CommonXml.GetNode(groupNode, "group", EmptyNodeHandling.ForceCreateNew).InnerText = group;
            }

            var baseData = Content.GetSubControl("basedata");

            baseData["pageviewcount"].InnerText = PageViewCount().ToString(CultureInfo.InvariantCulture);
            baseData["defaultpage"].InnerText = Settings["sitetree/stdpage"];

            foreach (var pageInHistory in History())
            {
                var ownerDocument = baseData["history"].OwnerDocument;

                if (ownerDocument != null)
                {
                    var historyNode = ownerDocument.CreateElement("item");

                    historyNode.InnerText = pageInHistory;
                    baseData["history"].AppendChild(historyNode);
                }
            }
        }

        private void ConfigureDebugging()
        {
            if (HttpPage.Request.RemoteAddress.ToString() != "127.0.0.1")
            {
                return;
            }

            if (HttpPage.Session["enabledebug"] == null)
            {
                DebugEnabled = true;
            }

            if (QueryOther["enabledebug"] == "true" || QueryOther["enabledebug"] == "false")
            {
                DebugEnabled = (QueryOther["enabledebug"] == "true");
            }
        }

        private void ProcessQueries()
        {
            List<String> keys = HttpPage.Request.Form.Cast<String>().ToList();

            keys.AddRange(HttpPage.Request.Query.Cast<String>());

            foreach (var key in keys)
            {
                if (key != null)
                {
                    var keyParts = key.Split('_');
                    var value = HttpPage.Request[key];

                    switch (keyParts[0])
                    {
                        case "data":
                            QueryData[String.Join("_", Common.Common.RemoveOne(keyParts))] = value;
                            break;
                        case "event":
                            QueryEvents[String.Join("_", Common.Common.RemoveOne(keyParts))] = value;
                            break;
                        case "process":
                            QueryOther[key] = CleanUpProcess(value);
                            break;
                        default:
                            QueryOther[key] = value;
                            break;
                    }
                }
            }
        }

        private String CleanUpProcess(String value)
        {
            if (value.Length > 0 && value.Substring(0, 1) == ",")
            {
                return value.Substring(1, value.Length - 1);
            }
            else
            {
                return value;
            }
        }

        public String GetUrl(String process, String querystring = null)
        {
            var baseUri = new Uri(BasePath);
            var uri = new Uri(baseUri, String.Format("{0}{1}", process, querystring));

            return uri.AbsoluteUri;
        }

        private IEnumerable<String> History()
        {
            var history = HttpPage.Session.ContainsKey("history")
                ? (List<String>)HttpPage.Session["history"]
                : new List<String>();

            history.Add(CurrentProcess);
            HttpPage.Session["history"] = history;

            return history;
        }

        private int PageViewCount()
        {
            Dictionary<String, int> pageViewCounts;

            if (HttpPage.Session.ContainsKey("pageviews"))
            {
                pageViewCounts = (Dictionary<String, int>)HttpPage.Session["pageviews"];

                if (pageViewCounts.ContainsKey(CurrentProcess))
                {
                    pageViewCounts[CurrentProcess] += 1;
                }
                else
                {
                    pageViewCounts[CurrentProcess] = 1;
                }
            }
            else
            {
                pageViewCounts = new Dictionary<String, int>();
                pageViewCounts[CurrentProcess] = 1;
            }

            HttpPage.Session["pageviews"] = pageViewCounts;

            return pageViewCounts[CurrentProcess];
        }

        private bool Login(String username, String password)
        {
            var success = false;

            Logout();
            var results = Plugins.InvokeAll("users", "verify", username, password);

            if (results.Length > 0)
            {
                var verified = false;

                foreach (Object result in results)
                {
                    if ((bool)result)
                    {
                        verified = true;
                    }
                }

                if (verified)
                {
                    var value = String.Format("{0}{1}{2}", username, CookieSeparator, password);
                    HttpPage.Response.Cookies.Append("login_cookie", value, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(1)
                    });

                    HttpPage.Session["current_username"] = username;
                    success = true;
                }
            }

            return success;
        }

        private void LoginByCookie()
        {
            if (CurrentUser == "anonymous")
            {
                var httpCookie = HttpPage.Request.Cookies["login_cookie"];

                if (httpCookie != null)
                {
                    var value = httpCookie;

                    if (value != null && value.Contains(CookieSeparator))
                    {
                        var valueParts = Common.Common.SplitByString(value, CookieSeparator);

                        Login(valueParts[0], valueParts[1]);
                    }
                }
            }
        }

        private void UpdateCookieTimeout()
        {
            //if (HttpPage.Response.Cookies["login_cookie"] != null)
            //{
            //    HttpPage.Request.Cookies["login_cookie"].Value = HttpPage.Request.Cookies["login_cookie"].Value;
            //    HttpPage.Response.Cookies["login_cookie"].Expires = DateTime.Now.AddDays(1);
            //}
        }

        public bool CheckGroups(String groups)
        {
            var valid = true;

            var results = Plugins.InvokeAll("users", "list_groups", CurrentUser);
            var userGroups = new List<String>(Common.Common.FlattenToStrings(results));

            if (groups != String.Empty)
            {
                var groupList = groups.Split(',');

                valid = groupList.Any(userGroups.Contains);
            }

            return valid;
        }

        private void Logout()
        {
            HttpPage.Session.Clear();
            HttpPage.Session["current_username"] = "anonymous";
            HttpPage.Response.Cookies.Delete("login_cookie", new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(-1)
            });

        }
    }
}