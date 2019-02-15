// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Providers.Base
{
    public class ProviderAdmin : BasePlugin2, IPlugin2
    {
        public new String Name
        {
            get
            {
                return "Admin";
            }
        }

        public ProviderAdmin()
        {
            Process = null;
        }

        public ProviderAdmin(Process process) : this()
        {
            Process = process;
        }

        public new void Handle(String mainEvent)
        {
            switch (mainEvent)
            {
                case "update":
                    HandleUpdate();
                    break;
                case "clear":
                    Process.Cache.Clean();
                    break;
            }
        }

        public new void Load(ControlList control, String action, String value, String pathTrail)
        {
            switch (action)
            {
                case "adminmenu":
                    LoadMenu(control);
                    break;
            }
        }

        private void HandleUpdate()
        {
            String[] paths = new String[2];
            paths[0] = Process.Settings["general/customrootcomponents"];
            paths[1] = Process.Settings["general/systemrootcomponents"];

            UpdateSnippets(paths);
            UpdateDlls(paths);
        }

        private void UpdateDlls(IEnumerable<String> paths)
        {
            foreach (String dir in paths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    if (Directory.Exists(Common.CombinePaths(subDirectoryInfo.FullName, "Plugins")))
                    {
                        DirectoryInfo pluginDirectoryInfo = new DirectoryInfo(Common.CombinePaths(subDirectoryInfo.FullName, "Plugins"));
                        foreach (FileInfo fileInfo in pluginDirectoryInfo.GetFiles())
                        {
                            if (fileInfo.Extension == ".dll" || fileInfo.Extension == ".pdb")
                            {
                                String destination = Common.CombinePaths(Process.Root, "Bin", fileInfo.Name);
                                if (!File.Exists(destination) || fileInfo.LastWriteTime != File.GetLastWriteTime(destination))
                                {
                                    File.Copy(fileInfo.FullName, Common.CombinePaths(Process.Root, "Bin", fileInfo.Name), true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSnippets(IEnumerable<String> paths)
        {
            String pathsnippets = Process.Settings["general/customrootcomponents"] + "\\snippets.xslt";
            // ToDo: should be more generic

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("<xsl:stylesheet");
            stringBuilder.AppendLine("    version=\"1.0\"");
            stringBuilder.AppendLine("    xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"");
            stringBuilder.AppendLine("    xmlns=\"http://www.w3.org/1999/xhtml\">");

            foreach (String dir in paths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    if (subDirectoryInfo.Name != ".svn")
                    {
                        if (Directory.Exists(Common.CombinePaths(subDirectoryInfo.FullName, "Xsl")))
                        {
                            DirectoryInfo xslDirectoryInfo = new DirectoryInfo(Common.CombinePaths(subDirectoryInfo.FullName, "Xsl"));
                            foreach (FileInfo fileInfo in xslDirectoryInfo.GetFiles())
                            {
                                if ((fileInfo.Extension == ".xslt" || fileInfo.Extension == ".xsl") && fileInfo.Name[0] == '_')
                                {
                                    if (directoryInfo.Parent != null)
                                    {
                                        stringBuilder.AppendLine("<xsl:include href=\"..\\..\\" + directoryInfo.Parent.Name + "\\" + directoryInfo.Name + "\\" + subDirectoryInfo.Name + "\\" + xslDirectoryInfo.Name + "\\" + fileInfo.Name + "\"/>");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            stringBuilder.AppendLine("</xsl:stylesheet>");

            File.WriteAllText(pathsnippets, stringBuilder.ToString(), Encoding.UTF8);
        }

        private void LoadMenu(ControlList control)
        {
            control["adminmenu"] = Process.Settings.GetAsNode("admin/menu");
        }
    }
}