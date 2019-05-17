// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Microsoft.AspNetCore.Http;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;
using System;
using System.Web;

namespace Sharpcms.Providers.Cookies
{
    public class ProviderCookies : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "Cookies";
            }
        }

        public ProviderCookies()
        {
        }

        public ProviderCookies(Process process)
        {
            Process = process;
        }

        public new void Initialize()
        {
            // Do nothing
        }

        public new void Dispose()
        {
            // Do nothing
        }

        public new void Handle(String mainEvent)
        {
            switch (mainEvent)
            {
                case "cookie":
                    HandleCookies();
                    break;
            }
        }

        public new void Load(ControlList control, String action, String value, String pathTrail)
        {
            switch (action)
            {
                case "cookie":
                    LoadCookies(control);
                    break;
            }
        }

        private void HandleCookies()
        {
            foreach (var key in Process.HttpPage.Request.Query.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ","))
                {
                    Process.HttpPage.Response.Cookies.Append(key, Process.HttpPage.Request.Query[key], new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(1)
                    });
                }
            }
        }

        private void LoadCookies(ControlList control)
        {
            var cookieData = new XmlItemList(CommonXml.GetNode(control.ParentNode, "items", EmptyNodeHandling.CreateNew));

            foreach (String key in Process.HttpPage.Request.Cookies.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ","))
                {
                    var httpCookie = Process.HttpPage.Request.Cookies[key];
                    if (httpCookie != null)
                    {
                        cookieData[key.Replace(".", String.Empty)] = HttpUtility.UrlEncode(httpCookie);
                    }
                }
            }

            foreach (String key in Process.HttpPage.Request.Cookies.Keys)
            {
                if (Process.Settings["general/cookies"].Contains("," + key + ",") && String.IsNullOrEmpty(cookieData[key.Replace(".", String.Empty)]))
                {
                    var httpCookie = Process.HttpPage.Request.Cookies[key];
                    if (httpCookie != null)
                    {
                        cookieData[key.Replace(".", String.Empty)] = HttpUtility.UrlEncode(httpCookie);
                    }
                }
            }
        }
    }
}
