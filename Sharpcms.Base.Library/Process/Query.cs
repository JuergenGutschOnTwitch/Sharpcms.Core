using System;

namespace Sharpcms.Base.Library.Process
{
    public class Query
    {
        public readonly String Name;
        public readonly String Value;

        public Query(String name, String value)
        {
            Name = name;
            Value = value;
        }
    }
}