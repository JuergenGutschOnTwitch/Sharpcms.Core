using System;

namespace Sharpcms.Base.Library.Http
{
    public class HttpContextWrapper
    {
        public HttpContextWrapper()
        {
            Server = new HttpServerWrapper();
        }
        public static HttpContextWrapper Current { get; } = new HttpContextWrapper();
        public HttpRequestWrapper Request { get; set; }

        public HttpServerWrapper Server { get; internal set; }

        public void RewritePath(string rewritePath)
        {
            throw new NotImplementedException();
        }
    }
}
