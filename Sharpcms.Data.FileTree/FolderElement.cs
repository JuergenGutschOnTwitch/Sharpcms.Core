using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;
using Sharpcms.Base.Library.Common;

namespace Sharpcms.Data.FileTree
{
    public class FolderElement
    {
        private readonly DirectoryInfo _directoryInfo;

        private String Name
        {
            get
            {
                return _directoryInfo.Name;
            }
        }

        public FolderElement(String path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        public void GetXml(XmlNode xmlNode, SubFolder subFolder)
        {
            XmlNode folderNode = CommonXml.GetNode(xmlNode, "folder", EmptyNodeHandling.ForceCreateNew);
            CommonXml.SetAttributeValue(folderNode, "name", Name);

            foreach (FileInfo fileInfo in _directoryInfo.GetFiles())
            {
                XmlNode fileNode = CommonXml.GetNode(folderNode, "file", EmptyNodeHandling.ForceCreateNew);
                CommonXml.SetAttributeValue(fileNode, "name", fileInfo.Name);
                CommonXml.SetAttributeValue(fileNode, "extension", fileInfo.Extension);

                GetFileAttributes(fileNode, fileInfo.Name);
            }

            if (subFolder == SubFolder.IncludeSubfolders)
            {
                foreach (DirectoryInfo directoryInfo in _directoryInfo.GetDirectories())
                {
                    if (Filter(directoryInfo.Name))
                    {
                        FolderElement folderElement = new FolderElement(directoryInfo.FullName);

                        folderElement.GetXml(folderNode, SubFolder.IncludeSubfolders);
                    }
                }
            }
        }

        private bool Filter(String name)
        {
            const String illegal = "._";

            return !illegal.Contains(name[0].ToString(CultureInfo.InvariantCulture));
        }

        private void GetFileAttributes(XmlNode xmlNode, String fileName) 
        {
            DataSet dataSet = new DataSet();
            try
            {
                dataSet.ReadXml(_directoryInfo.FullName + "\\data\\gallery.xml");
                if (dataSet.Tables["file"] != null)
                {
                    DataRow[] dataRows = dataSet.Tables["file"].Select("name='" + fileName + "'");
                    if (dataRows.Length != 0)
                    {
                        CommonXml.SetAttributeValue(xmlNode, "title", dataRows[0]["title"].ToString());
                        CommonXml.SetAttributeValue(xmlNode, "description", dataRows[0]["description"].ToString());
                    }
                }
            }
            catch
            {
                dataSet = null;
            }
            finally
            {
                if (dataSet != null)
                {
                    dataSet.Dispose();
                }
            }
        }
    }
}