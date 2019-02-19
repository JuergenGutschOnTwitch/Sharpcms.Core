using Microsoft.AspNetCore.Http;

namespace Sharpcms.Base.Library.Http
{
    public class HttpPage
    {
        public HttpPage(HttpContext context)
        {
            Request = new HttpRequestWrapper(context.Request);
            Response = new HttpResponseWrapper(context.Response);
            Session = new HttpSessionWrapper(context.Session);
            Server = new HttpServerWrapper();
            Application = new HttpApplicationState();
        }

        public HttpRequestWrapper Request { get; set; }
        public HttpSessionWrapper Session { get; set; }
        public HttpResponseWrapper Response { get; set; }
        public HttpServerWrapper Server { get; internal set; }
        public HttpApplicationState Application { get; set; }
    }
}
