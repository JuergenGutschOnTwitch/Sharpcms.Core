using Sharpcms.Base.Library.Process;

namespace Sharpcms.Base.Library.Plugin
{
    public interface IPlugin2 : IPlugin
    {
        string[] Implements { get; }
        object Invoke(string api, string action, params object[] args);
        void Load(ControlList control, string action, string value, string pathTrail);
    }
}