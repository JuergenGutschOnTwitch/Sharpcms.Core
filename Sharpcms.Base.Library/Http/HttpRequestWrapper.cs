using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using System;
using System.Collections.Generic;

namespace Sharpcms.Base.Library.Http
{
    public class HttpRequestWrapper : Dictionary<string, string>
    {
        private readonly HttpRequest _request;
        private readonly RequestHeaders _requestheaders;

        public HttpRequestWrapper(HttpRequest request)
        {
            _request = request;
            _requestheaders = _request.GetTypedHeaders();
        }

        public IRequestCookieCollection Cookies
        {
            get
            {
                return _request.Cookies;
            }
        }

        public Dictionary<string, string> ServerVariables
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ApplicationPath
        {
            get
            {
                return _request.PathBase;
            }
        }

        public IFormCollection Form
        {
            get
            {
                return _request.Form;
            }
        }

        public IQueryCollection Query
        {
            get
            {
                return _request.Query;
            }
        }

        public string Path
        {
            get
            {
                return _request.Path;
            }
        }

        public Uri Url
        {
            get
            {
                return new Uri($"{_requestheaders.Host.Value}/{_request.Path}");
            }
        }

        public IFormFileCollection Files
        {
            get
            {
                return _request.Form.Files;
            }
        }

        public Uri Referrer
        {
            get
            {
                return _requestheaders.Referer;
            }
        }
    }
}
