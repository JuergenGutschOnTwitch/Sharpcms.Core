using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Sharpcms.Base.Library.Http
{
    public class HttpResponseWrapper
    {
        private HttpResponse _response;

        public HttpResponseWrapper(HttpResponse response)
        {
            _response = response;
        }

        public IResponseCookies Cookies
        {
            get
            {
                return _response.Cookies;
            }
        }
        public HttpCache Cache { get; set; } //TODO: 

        public void Redirect(string uri)
        {
            _response.Redirect(uri);
        }

        public void AddHeader(string key, string value)
        {
            _response.Headers.Append(key, value);
        }

        public async Task WriteAsync(string text)
        {
            await _response.WriteAsync(text);
        }

        public void Clear()
        {
            _response.Clear();
        }

        public async Task WriteFile(string currentFile)
        {
            await _response.SendFileAsync(currentFile);
        }
    }
}
