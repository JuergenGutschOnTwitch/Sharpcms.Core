// sharpcms is licensed under the open source license GPL - GNU General Public License.

using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProvicerReferrer : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "Referrer";
            }
        }

        public ProvicerReferrer() { }

        public ProvicerReferrer(Process process)
        {
            Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }

        private void HandleLog()
        {
            if (Process.HttpPage.Request.Referrer != null)
            {
                Process.Content["referrer"].InnerText = Process.HttpPage.Request.Referrer.ToString();
            }
        }
    }
}