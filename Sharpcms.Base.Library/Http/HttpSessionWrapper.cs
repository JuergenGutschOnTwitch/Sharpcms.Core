using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Sharpcms.Base.Library.Http
{
    public class HttpSessionWrapper : Dictionary<string, object>
    {
        private readonly ISession _session;

        public HttpSessionWrapper(ISession session)
        {
            _session = session;

        }

        public string Id
        {
            get
            {
                return _session.Id;
            }
        }
    }
}