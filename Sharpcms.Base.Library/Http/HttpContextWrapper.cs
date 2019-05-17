using Microsoft.AspNetCore.Http;
using System;

namespace Sharpcms.Base.Library.Http
{
    public class HttpContextWrapper
    {
        public HttpContextWrapper(HttpContext context)
        {
            Server = new HttpServerWrapper(context);
        }
        public static HttpContextWrapper Current { get; } = new HttpContextWrapper(null);
        public HttpRequestWrapper Request { get; set; }

        public HttpServerWrapper Server { get; internal set; }

        public void RewritePath(string rewritePath)
        {
            throw new NotImplementedException();
        }
    }
}
