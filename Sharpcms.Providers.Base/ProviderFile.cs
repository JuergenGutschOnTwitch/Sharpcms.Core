// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.IO;
using System.Xml;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Plugin;
using Sharpcms.Base.Library.Process;
using Sharpcms.Data.FileTree;

namespace Sharpcms.Providers.Base
{
    public class ProviderFile : BasePlugin2, IPlugin2
    {
        private FileTree _fileTree;

        /// <summary>
        /// Gets the tree.
        /// </summary>
        /// <value>The tree.</value>
        public FileTree Tree
        {
            get
            {
                FileTree fileTree = _fileTree ?? (_fileTree = new FileTree(Process));

                return fileTree;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public new string Name
        {
            get
            {
                return "File";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFile"/> class.
        /// </summary>
        public ProviderFile() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderFile"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public ProviderFile(Process process)
        {
            Process = process;
        }

        /// <summary>
        /// Handles the specified main event.
        /// </summary>
        /// <param name="mainEvent">The main event.</param>
        public new void Handle(string mainEvent)
        {
            switch (mainEvent)
            {
                case "addfolder":
                    HandleAddFolder();
                    break;
                case "removefolder":
                    HandleRemoveFolder();
                    break;
                case "removefile":
                    HandleRemoveFile();
                    break;
                case "uploadfile":
                    HandleUpload();
                    break;
                case "renamefile":
                    HandleRenameFile();
                    break;
                case "renamefolder":
                    HandleRenameFolder();
                    break;
                case "movefolder":
                    HandleMoveFolder();
                    break;
                case "movefile":
                    HandleMoveFile();
                    break;
            }
        }

        /// <summary>
        /// Loads the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="value">The value.</param>
        /// <param name="pathTrail">The path trail.</param>
        public new void Load(ControlList control, string action, string value, string pathTrail)
        {
            switch (action)
            {
                case "tree":
                    LoadTree(control);
                    break;
                case "folder":
                    LoadFolder(pathTrail, control);
                    break;
                case "file":
                    LoadFile(pathTrail, control);
                    break;
                case "download":
                    LoadDownload(pathTrail);
                    break;
            }
        }

        /// <summary>
        /// Loads the download.
        /// </summary>
        /// <param name="value">The value.</param>
        private void LoadDownload(String value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                Process.OutputHandledByModule = true;
                Tree.SendToBrowser(value);
            }
        }

        /// <summary>
        /// Handles the move folder.
        /// </summary>
        private void HandleMoveFolder()
        {
            String[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.MoveFolder(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the move file.
        /// </summary>
        private void HandleMoveFile()
        {
            String[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.MoveFile(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the rename folder.
        /// </summary>
        private void HandleRenameFolder()
        {
            String[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.RenameFolder(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the rename file.
        /// </summary>
        private void HandleRenameFile()
        {
            String[] parameters = Process.QueryEvents["mainvalue"].Split('*');
            if (parameters.Length == 2 && parameters[1].Length > 0 && parameters[0].Length > 0)
            {
                FileTree filetree = new FileTree(Process);
                filetree.RenameFile(parameters[0], parameters[1]);
            }
        }

        /// <summary>
        /// Handles the remove folder.
        /// </summary>
        private void HandleRemoveFolder()
        {
            String path = Process.QueryEvents["mainvalue"];
            FileTree filetree = new FileTree(Process);
            
            filetree.DeleteFolder(path);
        }

        /// <summary>
        /// Handles the remove file.
        /// </summary>
        private void HandleRemoveFile()
        {
            String path = Process.QueryEvents["mainvalue"];
            FileTree filetree = new FileTree(Process);
            
            filetree.DeleteFile(path);
        }

        /// <summary>
        /// Handles the add folder.
        /// </summary>
        private void HandleAddFolder()
        {
            String query = Process.QueryEvents["mainvalue"];
            String[] list = query.Split('*');
            FileTree filetree = new FileTree(Process);
            
            filetree.CreateFolder(list[0], list[1]);
        }

        /// <summary>
        /// Handles the upload.
        /// </summary>
        private void HandleUpload()
        {
            String query = Process.QueryEvents["mainvalue"];
            FileTree fileTree = new FileTree(Process);
            
            fileTree.SaveUploadedFiles(query);
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="control">The control.</param>
        private void LoadFile(String value, ControlList control)
        {
            if (!String.IsNullOrEmpty(value))
            {
                String path = value;
                if (Tree.FileExists(path))
                {
                    XmlNode xmlNode = control["file"];

                    String extension = Path.GetExtension(path);
                    CommonXml.SetAttributeValue(xmlNode, "extension", extension);

                    String mainMimeType = Common.GetMainMimeType(extension);
                    CommonXml.SetAttributeValue(xmlNode, "mainmimetype", mainMimeType);

                    CommonXml.SetAttributeValue(xmlNode, "path", path);
                }
            }
        }

        /// <summary>
        /// Loads the folder.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="control">The control.</param>
        private void LoadFolder(String value, ControlList control)
        {
            if (!String.IsNullOrEmpty(value))
            {
                String path = value;
                if (Tree.FolderExists(path))
                {
                    FolderElement folderElement = Tree.GetFolder(path);
                    XmlNode xmlNode = control["folder"];
                    folderElement.GetXml(xmlNode, SubFolder.OnlyThisFolder);
                    CommonXml.SetAttributeValue(xmlNode, "path", path);
                }
            }
        }

        /// <summary>
        /// Loads the tree.
        /// </summary>
        /// <param name="control">The control.</param>
        private void LoadTree(ControlList control)
        {
            Tree.RootFolder.GetXml(control["filetree"], SubFolder.IncludeSubfolders);
        }
    }
}