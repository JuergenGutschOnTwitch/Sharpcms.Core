// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Base.Library.Process;

namespace Sharpcms.Base.Library.Plugin
{
    /// <summary>
    /// Implements most of the IPlugin interface. By inherting from this class,
    /// you won't have to implement the methods and properties you don't need.
    /// </summary>
    public class BasePlugin : IPlugin
    {
        public string Name
        {
            get { return "BasePlugin"; }
        }

        public Process.Process Process { get; set; }

        public IPluginHost Host { get; set; }

        public void Initialize()
        {
            // Do nothing
        }

        public void Dispose()
        {
            // Do nothing
        }

        public void Handle(string mainEvent)
        {
            // Do nothing
        }

        public void Load(ControlList control, string action, string pathTrail)
        {
            // Do nothing
        }
    }
}