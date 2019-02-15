// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.IO;
using System.Xml;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Providers.ErrorLog
{
    public class ProviderErrorLog : BasePlugin2, IPlugin2
    {
        public ProviderErrorLog()
        {
            Process = null;
        }

        public ProviderErrorLog(Process process) : this()
        {
            Process = process;
        }

        public new String Name
        {
            get
            {
                return "ErrorLog";
            }
        }

        public new void Handle(String mainEvent)
        {
            switch (mainEvent)
            {
                case "log":
                    HandleLog();
                    break;
            }
        }

        private void HandleLog()
        {
            var messagesNode = CommonXml.GetNode(Process.XmlData, "messages", EmptyNodeHandling.Ignore);
            if (messagesNode == null)
            {
                return;
            }

            var logFileName = Process.Settings["errorlog/logpath"];

            var items = messagesNode.SelectNodes("item");
            if (items != null)
            {
                foreach (XmlNode item in items)
                {
                    bool writtenToLogFile = false;

                    try
                    {
                        if (CommonXml.GetAttributeValue(item, "writtenToLogFile") == "true")
                        {
                            writtenToLogFile = true;
                        }
                    }
                    catch
                    {
                        writtenToLogFile = false;
                    }

                    if (writtenToLogFile)
                    {
                        continue;
                    }

                    if (CommonXml.GetAttributeValue(item, "messagetype") != "Error")
                    {
                        continue;
                    }

                    var errorType = CommonXml.GetAttributeValue(item, "type");
                    var message = item.InnerText;
                    File.AppendAllText(logFileName, String.Format("{0};{1};{2}\r\n", DateTime.Now.ToUniversalTime(), errorType, message));
                    CommonXml.SetAttributeValue(item, "writtenToLogFile", "true");
                }
            }
        }
    }
}