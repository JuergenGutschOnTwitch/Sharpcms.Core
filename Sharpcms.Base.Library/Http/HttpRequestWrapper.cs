using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;

namespace Sharpcms.Base.Library.Http
{
    public class HttpRequestWrapper : Dictionary<string, string>
    {
        private readonly HttpRequest _request;
        private readonly RequestHeaders _requestHeaders;

        public HttpRequestWrapper(HttpRequest request)
        {
            _request = request;
            ProcessFormAndQueryParameters();
            _requestHeaders = _request.GetTypedHeaders();
        }

        private void ProcessFormAndQueryParameters()
        {
            if (_request.HasFormContentType)
            {
                foreach (var key in _request.Form.Keys)
                {
                    Add(key, _request.Form[key]);
                }
            }
            foreach (var key in _request.Query.Keys)
            {
                Add(key, _request.Query[key]);
            }

            var process = Path;
            if (!String.IsNullOrWhiteSpace(ApplicationPath))
            {
                process = process.Replace(ApplicationPath, String.Empty);
            }
            Add("process", process.TrimStart('/'));
        }

        public IRequestCookieCollection Cookies
        {
            get
            {
                return _request.Cookies;
            }
        }

        // REMOTE_ADDR
        // SERVER_NAME
        // SERVER_PROTOCOL
        // SERVER_PORT
        // HTTP_USER_AGENT
        //public Dictionary<string, string> ServerVariables
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public string ServerName
        {
            get
            {
                return _request.Host.Host;
            }
        }
        public int? ServerPort
        {
            get
            {
                return _request.Host.Port;
            }
        }
        public string ServerProtocol
        {
            get
            {
                return _request.Scheme;
            }
        }

        public string UserAgent
        {
            get
            {
                return _request.Headers["User-Agent"];
            }
        }

        public IPAddress RemoteAddress
        {
            get
            {
                return _request.HttpContext.Connection.RemoteIpAddress;
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
                IFormCollection result = new FormCollection(new Dictionary<string, StringValues>());
                if (_request.HasFormContentType)
                {
                    result = _request.Form;
                }
                return result;
            }
        }

        public IQueryCollection Query
        {
            get
            {
                return _request.Query;
            }
        }

        public string QueryString
        {
            get
            {
                return _request.QueryString.Value;
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
                return new Uri($"{_requestHeaders.Host.Value}/{_request.Path}");
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
                return _requestHeaders.Referer;
            }
        }
    }

    public class HttpServerVariablesWrapper
    {
        private readonly HttpContext _context;
        public HttpServerVariablesWrapper(HttpContext context)
        {
            _context = context;
        }
        // REMOTE_ADDR
        public string RemoteAddress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // SERVER_NAME
        public string ServerName { get; set; }

        // SERVER_PROTOCOL
        public string ServerProtocol { get; set; }

        // SERVER_PORT
        public int ServerPort { get; set; }

        // HTTP_USER_AGENT
        public string UserAgend { get; set; }
    }
}
