// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Base.Library.Process;

namespace Sharpcms.Base.Library.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        Process.Process Process { get; set; }
        IPluginHost Host { get; set; }
        void Initialize();
        void Dispose();
        void Handle(string mainEvent);
        void Load(ControlList control, string action, string pathTrail);
    }
}