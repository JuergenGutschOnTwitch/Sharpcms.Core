// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderLoadXml : BasePlugin2, IPlugin2
    {
        public new string Name
        {
            get
            {
                return "LoadXml";
            }
        }

        public ProviderLoadXml() { }

        public ProviderLoadXml(Process process)
        {
            Process = process;
        }

        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "submitform":

                    break;
            }
        }

        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Process.Settings["loadxml/" + action]);

            if (xmlDocument.DocumentElement != null)
            {
                control[action].InnerXml = xmlDocument.DocumentElement.InnerXml;
            }
        }

        public void HandleSubmitForm()
        {

        }
    }
}