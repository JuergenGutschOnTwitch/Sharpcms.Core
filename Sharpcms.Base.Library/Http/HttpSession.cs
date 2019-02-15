using System.Collections.Generic;

namespace Sharpcms.Base.Library.Http
{
    public class HttpSession : Dictionary<string, object>
    {
        public string LCID { get; set; }
    }
}