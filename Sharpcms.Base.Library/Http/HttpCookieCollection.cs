using System;
using System.Collections.Generic;

namespace Sharpcms.Base.Library.Http
{
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
}
