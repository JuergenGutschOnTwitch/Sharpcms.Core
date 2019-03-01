using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Sharpcms.Base.Library.Http
{
    public class HttpServerWrapper
    {
        private readonly HttpContext _context;

        public HttpServerWrapper(HttpContext context)
        {
            _context = context;
        }

        public string UrlEncode(string input)
        {
            return System.Web.HttpUtility.UrlEncode(input);
        }

        public string HtmlDecode(string html)
        {
            return System.Web.HttpUtility.HtmlDecode(html);
        }

        public string MapPath(string relative)
        {
            var env =  _context.RequestServices.GetService<IHostingEnvironment>();
            var rootpath = env.ContentRootPath;
            relative = relative.Replace("~/", string.Empty).Replace('/', Path.DirectorySeparatorChar);
            var result = Path.Combine(rootpath, relative);
            return result;
        }
    }
}
