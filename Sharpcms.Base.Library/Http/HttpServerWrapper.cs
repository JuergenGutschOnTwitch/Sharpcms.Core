using System;

namespace Sharpcms.Base.Library.Http
{
    public class HttpServerWrapper
    {
        public string UrlEncode(string input)
        {
            return System.Web.HttpUtility.UrlEncode(input);
        }
        public string MapPath(string relative)
        {
            throw new NotImplementedException();
        }

        public string HtmlDecode(string html)
        {
            return System.Web.HttpUtility.HtmlDecode(html);
        }
    }
}
