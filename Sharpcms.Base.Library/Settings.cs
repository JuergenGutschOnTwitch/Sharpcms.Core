// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.IO;
using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Base.Library
{
    public class Settings
    {
        private const string CustomPath = @"Custom\App_Data\CustomSettings.xml";
        private static Settings _defaultInstance;
        private readonly XmlDocument _combinedSettings;
        private readonly string _customFullPath;
        private readonly XmlDocument _customSettings = new XmlDocument();
        private readonly string _rootPath;

        public Settings(Process.Process process, string rootPath)
        {
            _rootPath = rootPath;
            _customFullPath = Path.Combine(rootPath, CustomPath);
            _customSettings.Load(_customFullPath);
            _combinedSettings = process.Cache["settings"] as XmlDocument;
            _defaultInstance = this;
        }

        public string CustomFullPath
        {
            get
            {
                return _customFullPath;
            }
        }

        public string RootPath
        {
            get
            {
                return _rootPath;
            }
        }

        public static Settings DefaultInstance
        {
            get
            {
                return _defaultInstance;
            }
        }

        public string this[string path]
        {
            get
            {
                return this[path, RelativePathHandling.ConvertToAbsolute];
            }
            set
            {
                this[path, RelativePathHandling.ConvertToAbsolute] = value;
            }
        }

        private string this[string path, RelativePathHandling relativePathHandling]
        {
            get
            {
                XmlNode settingsNode = CommonXml.GetNode(_combinedSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew);
                string value = settingsNode.InnerText;

                return relativePathHandling == RelativePathHandling.ConvertToAbsolute 
                    ? ConvertPath(value) 
                    : value;
            }
            set
            {
                CommonXml.GetNode(_customSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew).InnerText = value;
                CommonXml.GetNode(_combinedSettings.SelectSingleNode("settings"), path, EmptyNodeHandling.CreateNew).InnerText = value;

                Save();
            }
        }

        public XmlNode GetAsNode(string path)
        {
            XmlNode xmlNode = CommonXml.GetNode(_combinedSettings.SelectSingleNode("settings"), path);

            return xmlNode;
        }

        private string ConvertPath(string relativePath)
        {
            string convertedPath = relativePath;

            if (relativePath.StartsWith("~/"))
            {
                relativePath = relativePath.Substring(2);

                convertedPath = Common.Common.CombinePaths(RootPath, relativePath.Replace('/', '\\'));
            }

            return convertedPath;
        }

        private void Save()
        {
            //ToDo: should work but has not been testet yet
            _customSettings.Save(_customFullPath);
        }
    }
}