using System;
using System.Collections.Generic;

namespace Sharpcms.Base.Library.Http
{
    public class Page
    {
        public HttpServer Server { get; internal set; }
        public HttpRequest Request { get; set; }
        public HttpSession Session { get; set; }
        public HttpResponse Response { get; set; }
        public HttpApplicationState Application { get; set; }
    }

    public class HttpContext
    {
        public static HttpContext Current { get; } = new HttpContext();
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }

        public HttpServer Server { get; internal set; }

        public void RewritePath(string rewritePath)
        {
            throw new NotImplementedException();
        }
    }

    public class HttpApplicationState : Dictionary<string, object>
    {
    }

    public class HttpCookieCollection : Dictionary<string, HttpCookie>, ICollection<HttpCookie>
    {
        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(HttpCookie item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(HttpCookie item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(HttpCookie[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(HttpCookie item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<HttpCookie> IEnumerable<HttpCookie>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class HttpResponse
    {
        public HttpCookieCollection Cookies { get; set; }
        public HttpCache Cache { get; set; }

        public void Redirect(string uri)
        {
            throw new NotImplementedException();
        }

        public void AddHeader(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public void Write(string v)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string currentFile)
        {
            throw new NotImplementedException();
        }
    }

    public class HttpCache
    {
        public void SetExpires(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void SetCacheability(object @public)
        {
            throw new NotImplementedException();
        }
    }

    public enum HttpCacheability
    {
        Public,
        Private
    }

    public class HttpRequest : Dictionary<string, string>
    {
        public HttpCookieCollection Cookies { get; set; }
        public Dictionary<string, string> ServerVariables { get; set; }
        public string ApplicationPath { get; set; }
        public Dictionary<string, string> Form { get; set; }
        public Dictionary<string, string> QueryString { get; set; }
        public string Path { get; set; }
        public Uri Url { get; internal set; }
        public Dictionary<string,string> Params { get;  set; }
        public Dictionary<int, HttpPostedFile> Files { get; set; }
        public Uri UrlReferrer { get; set; }
    }

    public class HttpPostedFile
    {
        public int ContentLength { get; set; }
        public string FileName { get; set; }

        public void SaveAs(string fullName)
        {
            throw new NotImplementedException();
        }
    }

    public class HttpCookie
    {
        private string key;
        private string v;

        public HttpCookie(string key, string v)
        {
            this.key = key;
            this.v = v;
        }

        public string Value { get; set; }
        public DateTime Expires { get; set; }
    }

    public class HttpServer
    {
        public string UrlEncode(string input)
        {
            throw new NotImplementedException();
        }
        public string MapPath(string relative)
        {
            throw new NotImplementedException();
        }

        public string HtmlDecode(string html)
        {
            throw new NotImplementedException();
        }
    }
}
