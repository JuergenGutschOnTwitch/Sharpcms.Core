// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library
{
    public static class Configuration
    {
        private static string[] GetConfigFileNames(string fileName, IEnumerable<string> paths)
        {
            List<string> processFiles = new List<string>();

            foreach (string path in paths)
            {
                if (path.EndsWith(".xml"))
                {
                    processFiles.Add(path);
                }
                else
                {
                    string[] directories = Directory.GetDirectories(path);

                    processFiles.AddRange(directories.Select(directory => Common.Common.CombinePaths(path, directory, fileName)).Where(File.Exists));
                }
            }

            return processFiles.ToArray();
        }

        private static bool FilesChanged(IEnumerable<string> fileNames, Cache cache)
        {
            bool success = false;

            foreach (string fileName in fileNames)
            {
                if (cache["changed_" + fileName] == null)
                {
                    success = true;
                }
                else
                {
                    DateTime cacheChanged = (DateTime)cache["changed_" + fileName];
                    if (cacheChanged != File.GetLastWriteTime(fileName))
                    {
                        success = true;
                    }
                }
            }

            return success;
        }

        public static void CombineProcessTree(IEnumerable<string> paths, Cache cache)
        {
            string[] fileNames = GetConfigFileNames("Process.xml", paths);
            if (FilesChanged(fileNames, cache))
            {
                XmlDocument combinedProcess = new XmlDocument();
                combinedProcess.AppendChild(combinedProcess.CreateElement("process"));

                foreach (string fileName in fileNames)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);

                    CommonXml.MergeXml(combinedProcess, xmlDocument, "load", "handle");
                    cache["changed_" + fileName] = File.GetLastWriteTime(fileName);
                }

                cache["process"] = combinedProcess;
            }
        }

        public static void CombineSettings(IEnumerable<string> paths, Cache cache)
        {
            string[] fileNames = GetConfigFileNames("Settings.xml", paths);
            if (FilesChanged(fileNames, cache))
            {
                XmlDocument combinedSettings = new XmlDocument();
                combinedSettings.AppendChild(combinedSettings.CreateElement("settings"));

                foreach (string fileName in fileNames)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);

                    CommonXml.MergeXml(combinedSettings, xmlDocument, "item");
                    cache["changed_" + fileName] = File.GetLastWriteTime(fileName);
                }

                cache["settings"] = combinedSettings;
            }
        }
    }
}