using System;

namespace Sharpcms.Base.Library.Http
{
    public class HttpCookie
    {
        private string _key;
        private string _value;

        public HttpCookie(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public string Value { get; set; }
        public DateTime Expires { get; set; }
    }
}
