using System.Xml;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Base.PluginInterface
{
    public interface IPlugin
    {
        string Name { get; }
        Process Process { set; }
        IPluginHost Host { set; }

        void Initialize();
        void Dispose();
        void Handle(string mainEvent);
        void Load(string value, string[] args, XmlNode node);
    }
}