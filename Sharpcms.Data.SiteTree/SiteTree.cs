// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.IO;
using System.Xml;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Data.SiteTree
{
    public class SiteTree
    {
        private readonly String _contentFilenameFormat;
        private readonly String _contentRoot;
        private readonly Process _process;
        private readonly XmlDocument _treeDocument;
        private readonly String _treeFilename;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteTree"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public SiteTree(Process process)
        {
            _process = process;
            _contentRoot = process.Settings["sitetree/contentRoot"];
            _contentFilenameFormat = process.Settings["sitetree/contentFilenameFormat"];
            _treeFilename = process.Settings["sitetree/treeFilename"];
            _treeDocument = new XmlDocument();
            _treeDocument.Load(_treeFilename);
        }

        /// <summary>
        /// Gets the tree document.
        /// </summary>
        /// <value>The tree document.</value>
        public XmlDocument TreeDocument
        {
            get
            {
                return _treeDocument;
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Page GetPage(String path)
        {
            XmlNode pageNode = GetPageNode(path);
            Page page = GetPage(pageNode);

            return page;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="pageNode">The page node.</param>
        /// <returns></returns>
        private Page GetPage(XmlNode pageNode)
        {
            Page page = null;

            if (pageNode != null)
            {
                String path = CommonXml.GetXPath(pageNode);

                String fileName = Path.Combine(_contentRoot, string.Format(_contentFilenameFormat, path));
                if (!File.Exists(fileName))
                {
                    PagePath pagePath = new PagePath(path);
                    CreateFile(pagePath.Path, pagePath.Name, CommonXml.GetAttributeValue(pageNode, "menuname"));
                }

                XmlDocument document = new XmlDocument();
                document.Load(fileName);

                page = new Page(document.SelectSingleNode("//page"), GetPageNode(path), this);
            }

            return page;
        }

        /// <summary>
        /// Gets the page node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private XmlNode GetPageNode(String path)
        {
            XmlNode pageNode = CommonXml.GetNode(_treeDocument, path, EmptyNodeHandling.Ignore);

            return pageNode;
        }

        /// <summary>
        /// Existses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool Exists(String path)
        {
            XmlNode pageNode = GetPageNode(path);

            return pageNode != null;
        }

        /// <summary>
        /// Saves a page.
        /// </summary>
        /// <param name="page">The page to save.</param>
        public void SavePage(Page page)
        {
            SavePage(page, true);
        }

        /// <summary>
        /// Saves the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="saveTree">if set to <c>true</c> [save tree].</param>
        private void SavePage(Page page, bool saveTree)
        {
            if (page != null)
            {
                String filename = GetDocumentFilename(page.TreeNode);
                CommonXml.SaveXmlDocument(filename, page.Node.OwnerDocument);

                if (saveTree)
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Saves the tree.
        /// </summary>
        public void Save()
        {
            Rebuild();
            CommonXml.SaveXmlDocument(_treeFilename, _treeDocument);
        }

        /// <summary>
        /// Rebuilds this instance.
        /// </summary>
        private void Rebuild()
        {
            XmlNodeList xmlNodeList = TreeDocument.SelectNodes("//*[@pageidentifier and not(@pageidentifier = '')]");
            if (xmlNodeList != null)
            {
                foreach (XmlNode pageNode in xmlNodeList)
                {
                    String path = CommonXml.GetXPath(pageNode);
                    Page page = GetPage(path);
                    page["pageidentifier"] = path;

                    SavePage(page, false);
                    CommonXml.SetAttributeValue(pageNode, "pageidentifier", path);
                }
            }
        }

        #region Copy Page

        public void CopyTo(String mainValue)
        {
            string[] parts = mainValue.Split('¤');
            String copyFromPath = parts[0];
            String copyToPath = parts[1];
            String name = parts[2];

            if (copyFromPath.Trim() == string.Empty || copyToPath.Trim() == string.Empty)
            {
                _process.AddMessage("An error occured while copying the page.");
            }
            else
            {
                Page copyFrom = GetPage(copyFromPath);
                Page copyTo = GetPage(copyToPath);

                if (name.Trim() == string.Empty)
                {
                    name = string.Format("Copy of {0}", copyFrom.Name);
                }

                Page newPage = Create(copyTo.PageIdentifier, name, name);
                newPage.Containers.ParentNode.InnerXml = copyFrom.Containers.ParentNode.InnerXml;
                newPage.Save();

                Save();
            }
        }

        #endregion

        #region Create Page

        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        public Page Create(String path, String name, String menuName)
        {
            Page page;

            // Only lowercase URLs are accepted
            path = path.ToLower();
            name = Common.CleanToSafeString(name).ToLower();

            if (GetPageNode(path + "/" + name) != null)
            {
                page = GetPage(path + "/" + name); // The requested page already exists.
            }
            else
            {
                // Update filesystem
                String pathfile = path;
                if (pathfile == ".")
                {
                    pathfile = string.Empty;
                }

                CreateFile(pathfile, name, menuName);

                // Update Xml
                XmlNode pageNode = CreateInTree(path, name, menuName);

                Save();

                page = GetPage(pageNode);
                InitializeCreatedPage(page);
            }

            return page;
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="menuName">Name of the menu.</param>
        private void CreateFile(String path, String name, String menuName)
        {
            String fileDirectory = GetDocumentDirectory(path);
            String filename = GetDocumentFilename(path, name);
            Directory.CreateDirectory(fileDirectory);
            XmlDocument document = new XmlDocument();
            CommonXml.GetNode(document, "page/attributes/pagename", EmptyNodeHandling.CreateNew).InnerText = menuName;
            CommonXml.SaveXmlDocument(filename, document);
        }

        /// <summary>
        /// Creates the in tree.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        private XmlNode CreateInTree(String path, String name, String menuName)
        {
            XmlNode parentNode = GetPageNode(path);
            XmlNode pageNode = _treeDocument.CreateElement(name);
            parentNode.AppendChild(pageNode);

            CommonXml.AppendAttribute(pageNode, "menuname", menuName);

            return pageNode;
        }

        /// <summary>
        /// Initializes the created page.
        /// </summary>
        /// <param name="page">The page.</param>
        private void InitializeCreatedPage(Page page)
        {
            XmlNode xmlNodeContainer = _process.Settings.GetAsNode("sitetree/containers");
            if (xmlNodeContainer.ChildNodes.Count > 0)
            {
                foreach (XmlNode xmlNode in xmlNodeContainer.ChildNodes)
                {
                    Container container = page.Containers[xmlNode.InnerText];
                    // ToDo: hack to secure content container
                }                
            }
            else
            {
                Container container = page.Containers["content"];
                // ToDo: secures older websites - goes obsoletet
            }

            page["status"] = "hide";
            page.Save();
        }

        #endregion

        #region Delete Page

        public void Delete(String path)
        {
            Delete(GetPageNode(path));
        }

        private void Delete(XmlNode pageNode)
        {
            if (pageNode != null)
            {
                // Update filesystem
                DeleteFile(pageNode);

                // Update Xml
                DeleteInTree(pageNode);

                Save();
            }
        }

        private void DeleteFile(XmlNode pageNode)
        {
            String path = CommonXml.GetXPath(pageNode);
            PagePath pagePath = new PagePath(path);
            Common.DeleteFile(GetDocumentFilename(pagePath.Path, pagePath.Name));
            Common.DeleteDirectory(GetDocumentContainerDirectory(pagePath.Path, pagePath.Name));
        }

        private static void DeleteInTree(XmlNode pageNode)
        {
            if (pageNode == null) return;
            if (pageNode.ParentNode == null)  return;
            
            pageNode.ParentNode.RemoveChild(pageNode);
        }

        #endregion

        #region Rename page

        public void Rename(Page page, String renameTo)
        {
            String pageName = renameTo;
            String newName = Common.CleanToSafeString(renameTo);
            String oldName = page.TreeNode.Name;

            if (oldName != newName)
            {
                // Update filesystem
                RenameFile(page, newName);

                // Update Xml
                RenameInTree(page, newName, pageName);

                Save();
            }
        }

        private void RenameFile(Page page, String renameTo)
        {
            String path = CommonXml.GetXPath(page.TreeNode);
            PagePath pagePath = new PagePath(path);

            // Rename file
            String oldFilename = GetDocumentFilename(pagePath.Path, pagePath.Name);
            String newFilename = GetDocumentFilename(pagePath.Path, renameTo);
            File.Move(oldFilename, newFilename);

            // Rename directory
            String oldDirectory = GetDocumentContainerDirectory(pagePath.Path, pagePath.Name);
            if (Directory.Exists(oldDirectory))
            {
                String newDirectory = GetDocumentContainerDirectory(pagePath.Path, renameTo);
                Directory.Move(oldDirectory, newDirectory);
            }
        }

        private void RenameInTree(Page page, String name, String pageName)
        {
            name = Common.CleanToSafeString(name);
            
            XmlNode newTreeNode = _treeDocument.CreateElement(name);
            XmlNode parentNode = page.TreeNode.ParentNode;

            String newPageidentifier = string.Empty;
            if (parentNode != null)
            {
                newPageidentifier = parentNode.Name;
            }

            // Copy children and attributes
            newTreeNode.InnerXml = page.TreeNode.InnerXml;
            CommonXml.CopyAttributes(page.TreeNode, newTreeNode);
            CommonXml.SetAttributeValue(newTreeNode, "name", name);
            CommonXml.SetAttributeValue(newTreeNode, "pagename", pageName);
            CommonXml.SetAttributeValue(newTreeNode, "pageidentifier", newPageidentifier + "/" + name);

            // Replace old node with new
            if (parentNode != null)
            {
                parentNode.ReplaceChild(newTreeNode, page.TreeNode);
            }
        }

        #endregion

        #region Move up / down

        public void MoveUp(String path)
        {
            Page page = GetPage(path);
            MoveUp(page);
        }

        private void MoveUp(Page page)
        {
            CommonXml.MoveUp(page.TreeNode);
            Save();
        }

        public void MoveDown(String path)
        {
            Page page = GetPage(path);
            MoveDown(page);
        }

        private void MoveDown(Page page)
        {
            CommonXml.MoveDown(page.TreeNode);
            Save();
        }

        private void MoveTop(Page page)
        {
            CommonXml.MoveTop(page.TreeNode);
            Save();
        }

        public void MoveTop(String path)
        {
            Page page = GetPage(path);
            MoveTop(page);
        }

        private void MoveBottom(Page page)
        {
            CommonXml.MoveBottom(page.TreeNode);
            Save();
        }

        public void MoveBottom(String path)
        {
            Page page = GetPage(path);
            MoveBottom(page);
        }

        #endregion

        #region Move

        public void Move(String path, String newParentPath)
        {
            Page page = GetPage(path);
            Page newParent = GetPage(newParentPath == "." 
                ? string.Empty 
                : newParentPath);

            Move(page, newParent);
        }

        private void Move(Page page, Page newParent)
        {
            if (page.Node != newParent.Node && !newParent.PageIdentifier.StartsWith(page.PageIdentifier))
            {
                String sourcePath = page.PageIdentifier;
                String newParentPath = newParent.PageIdentifier;

                CommonXml.Move(page.TreeNode, newParent.TreeNode);
                Save();

                String newContainerDirectory = GetDocumentDirectory(newParentPath.Replace("/", @"\"));
                String sourceDirectory = GetDocumentDirectory(sourcePath.Replace("/", @"\"));
                String sourceFile = GetDocumentFilename(sourcePath.Replace("/", @"\"));

                // Ensure that the container directory exists
                Directory.CreateDirectory(newContainerDirectory);

                // Move file
                Common.MoveFile(sourceFile, newContainerDirectory);

                // Copy directory
                if (Directory.Exists(sourceDirectory))
                {
                    Common.CopyDirectory(sourceDirectory, Path.Combine(newContainerDirectory, page.Name), true);
                }

                // (Try to) Remove original directory
                try
                {
                    Common.DeleteDirectory(sourceDirectory);
                }
                catch
                {
                    throw new Exception("Cannot delete original Directory");
                }

                Save();
            }
            else
            {
                throw new Exception("Cannot move page to child");
            }
        }

        #endregion

        #region Directory and filename handling

        private static String GetDocumentContainerDirectory(String path, String name)
        {
            return Path.Combine(path, name);
        }

        private String GetDocumentDirectory(String path)
        {
            String directory = Path.Combine(_contentRoot, path);

            return directory;
        }

        private String GetDocumentFilename(XmlNode treeNode)
        {
            String path = CommonXml.GetXPath(treeNode);
            String documentFilename = GetDocumentFilename(path);

            return documentFilename;
        }

        private String GetDocumentFilename(String fullPath)
        {
            PagePath pagePath = new PagePath(fullPath);
            String documentFilename = GetDocumentFilename(pagePath.Path, pagePath.Name);

            return documentFilename;
        }

        private String GetDocumentFilename(String path, String name)
        {
            String directory = GetDocumentDirectory(path);
            String documentFilename = Path.Combine(directory, string.Format(_contentFilenameFormat, name));

            return documentFilename;
        }

        #endregion
    }
}