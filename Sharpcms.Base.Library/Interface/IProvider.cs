using System;
using System.Collections.Generic;
using System.Text;

namespace InventIt.SiteSystem.Library.Interface
{
    public interface IProvider
    {
        void Handle(string mainEvent);
        void Load(string value,string[] args);
        string Name();
    }
}