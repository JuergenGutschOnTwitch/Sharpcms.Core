// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Sharpcms.Base.Library;
using Sharpcms.Base.Library.Common;
using Sharpcms.Base.Library.Http;
using Sharpcms.Base.Library.Process;

namespace Sharpcms.Data.FileTree
{
    public class FileTree
    {
        private readonly Process _process;
        private readonly String _rootFilesPath;

        public FileTree(Process process)
        {
            _process = process;
            _rootFilesPath = Path.Combine(_process.Root, _process.Settings["filetree/filesPath"]);
            RootFolder = new FolderElement(_rootFilesPath);
        }

        public FolderElement RootFolder { get; private set; }

        public bool FolderExists(String path)
        {
            String combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            bool isExist = Directory.Exists(combinedPath);

            return isExist;
        }

        public bool FileExists(String path)
        {
            String combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            bool isExist = File.Exists(combinedPath);

            return isExist;
        }

        public void CreateFolder(String path, String name)
        {
            name = Common.CleanToSafeString(name);
            path = Common.FormatFilePath(path);

            String combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path, name);
            Directory.CreateDirectory(combinedPath);
        }

        public void DeleteFolder(String path)
        {
            String combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            Common.DeleteDirectory(combinedPath);
        }

        public void DeleteFile(String path)
        {
            String combinedPath = Common.CheckedCombinePaths(_rootFilesPath, path);
            File.Delete(combinedPath);
        }

        public FolderElement GetFolder(String folder)
        {
            String path = Path.Combine(_rootFilesPath, folder);

            FolderElement folderElement = Directory.Exists(path)
                ? new FolderElement(path)
                : null;

            return folderElement;
        }

        public void SaveUploadedFiles(String path)
        {
            int fileCount = _process.HttpPage.Request.Files.Count;
            String[] files = new String[fileCount];

            for (int fileIndex = 0; fileIndex < fileCount; fileIndex++)
            {
                var file = _process.HttpPage.Request.Files[fileIndex];

                if (path != null && file.Length > 0)
                {
                    var prepend = _process.QueryOther["file_prepend"];
                    prepend = !String.IsNullOrEmpty(prepend) 
                        ? prepend + "_" 
                        : String.Empty;

                    var filename = String.Join("_", Common.CleanToSafeString(Path.GetFileName(file.FileName)).Split(' '));
                    var fullName = Common.CombinePaths(_rootFilesPath, path, prepend + filename);
                    if (Common.PathIsInSite(fullName) && filename != String.Empty)
                    {
                        using(var fs = new FileStream(fullName, FileMode.CreateNew, FileAccess.Write))
                        {
                            file.CopyTo(fs);
                        }
                        files[fileIndex] = fullName;
                    }
                }
                else if (path != null && file.Length == 0 && file.FileName != String.Empty)
                {
                    _process.AddMessage(
                        String.Format("The file \"{0}\" was ignored because it was empty.", file.FileName),
                        MessageType.Error);
                }
            }
        }

        public async Task SendToBrowser(String filename)
        {
            var response = _process.HttpPage.Response;

            var fileInfo = new FileInfo(Path.Combine(_rootFilesPath, filename));
            if (!fileInfo.FullName.StartsWith(_rootFilesPath)) return;

            if (fileInfo.Exists)
            {
                response.Clear();

                var currentFile = fileInfo.FullName;
                if (Common.IsValidImage(currentFile) && HasImageRenderProperties())
                {
                    currentFile = RenderImage(fileInfo);
                }

                // Send file to browser
                FileInfo file = new FileInfo(currentFile);
                switch (_process.QueryOther["download"])
                {
                    case "true":
                        response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                        break;
                    default:
                        response.AddHeader("Content-Disposition", "filename=" + file.Name);
                        break;
                }

                response.AddHeader("Content-Length", file.Length.ToString(CultureInfo.InvariantCulture));
                response.AddHeader("Content-Type", Common.GetMimeType(file.Extension));
                response.Cache.SetExpires(DateTime.Now + new TimeSpan(7, 0, 0, 0));
                response.Cache.SetCacheability(HttpCacheability.Public);
                await response.WriteFile(currentFile);

            }
        }

        private bool HasImageRenderProperties()
        {
            int borderwidth;
            bool hasBoderWidth = int.TryParse(_process.QueryOther["borderwidth"], out borderwidth);

            String bordercolor;
            bool hasBordercolor = Common.TryParseCssColor(_process.QueryOther["bordercolor"], out bordercolor);

            int height;
            bool hasHeight = int.TryParse(_process.QueryOther["height"], out height);

            int radius;
            bool hasRadius = int.TryParse(_process.QueryOther["radius"], out radius);

            int width;
            bool hasWidth = int.TryParse(_process.QueryOther["width"], out width);
            
            bool result = hasBoderWidth || hasBordercolor || hasHeight || hasRadius || hasWidth;

            return result;
        }

        private String RenderImage(FileInfo fileInfo)
        {
            int width;
            bool hasWidth = int.TryParse(_process.QueryOther["width"], out width);

            int height;
            bool hasHeight = int.TryParse(_process.QueryOther["height"], out height);

            int radius;
            bool hasRadius = int.TryParse(_process.QueryOther["radius"], out radius);

            int borderwidth;
            bool hasBoderWidth = int.TryParse(_process.QueryOther["borderwidth"], out borderwidth);

            String bordercolor;
            bool hasBordercolor = Common.TryParseCssColor(_process.QueryOther["bordercolor"], out bordercolor);

            bool isFixed = _process.QueryOther["fixed"] == "true";
            if ((hasWidth && width > 0) 
                || (hasHeight && height > 0) 
                || (hasRadius && radius >= 0))
            {
                String newFilename = String.Format("{0}_{1}x{2}r{3}", fileInfo.Name.TrimEnd(fileInfo.Extension.ToCharArray()), width, height, radius);

                if (hasBoderWidth && hasBordercolor)
                {
                    newFilename += ("bw" + borderwidth + "bc" + bordercolor);
                }

                newFilename += ("_" + isFixed + ".png");

                if (fileInfo.Directory != null)
                {
                    String thumbnailFile = Common.CombinePaths(fileInfo.Directory.FullName, "thumbs", newFilename);

                    DateTime rerender = new DateTime(2006, 9, 25);
                    FileInfo thumbInfo = null;
                    if (File.Exists(thumbnailFile))
                    {
                        thumbInfo = new FileInfo(thumbnailFile);
                    }

                    Bitmap bitmap;
                    if (thumbInfo == null || thumbInfo.LastWriteTime < fileInfo.LastWriteTime || rerender > thumbInfo.LastWriteTime)
                    {
                        bitmap = new Bitmap(fileInfo.FullName);
                        if (fileInfo.Extension.ToLower() == ".gif")
                        {
                            Bitmap realBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                for (int y = 0; y < bitmap.Height; y++)
                                {
                                    realBitmap.SetPixel(x, y, bitmap.GetPixel(x, y));
                                }
                            }

                            bitmap.Dispose();
                            bitmap = realBitmap;
                        }

                        bitmap = ResizeOrCropImage(bitmap, width, height, isFixed);
                        bitmap = RoundImageVertex(bitmap, radius, borderwidth, bordercolor);
                    }
                    else
                    {
                        return thumbnailFile;
                    }

                    if (bitmap != null)
                    {
                        if (!Directory.Exists(fileInfo.Directory + @"\thumbs"))
                        {
                            Directory.CreateDirectory(fileInfo.Directory + @"\thumbs");
                        }

                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 95L);

                        ImageCodecInfo ici = GetEncoderInfo("image/png");

                        bitmap.Save(thumbnailFile, ici, encoderParameters);
                        bitmap.Dispose();

                        return thumbnailFile;
                    }
                }
            }

            return fileInfo.FullName;
        }

        private static Bitmap ResizeOrCropImage(Image bitmap, int width, int height, bool isFixed)
        {
            Bitmap result;

            if (width >= bitmap.Width && height >= bitmap.Height && !isFixed)
            {
                result = null;
            }
            else
            {
                Image tmpImage = bitmap;

                // Crop
                if (width > 0 && height > 0)
                {
                    tmpImage = isFixed
                        ? ImageResize.FixedSize(tmpImage, width, height, Color.White)
                        : ImageResize.Crop(tmpImage, width, height, ImageResize.AnchorPosition.Center);
                }

                // Resize
                if (width > 0 && width < bitmap.Width)
                {
                    tmpImage = ImageResize.ConstrainProportions(tmpImage, width, ImageResize.Dimensions.Width);
                }
                else if (height > 0 && height < bitmap.Height)
                {
                    tmpImage = ImageResize.ConstrainProportions(tmpImage, height, ImageResize.Dimensions.Height);
                }

                result = (Bitmap)tmpImage;
            }

            return result;
        }

        private static Bitmap RoundImageVertex(Bitmap bitmap, int radius, int borderWidth, String borderColor)
        {
            Bitmap result;

            if (radius <= 0 || (radius * 2) > bitmap.Height || (radius * 2) > bitmap.Width)
            {
                result = bitmap;
            }
            else
            {
                Image tmpImage = bitmap;

                if (!Regex.IsMatch(borderColor, @"[#]([0-9]|[a-f]|[A-F]){6}\b") || borderWidth < 1)
                {
                    tmpImage = ImageVertexRounding.RoundedRectangle(tmpImage, radius);
                }
                else
                {
                    tmpImage = ImageVertexRounding.RoundedRectangle(tmpImage, radius, borderWidth, borderColor);
                }

                result = (Bitmap)tmpImage;
            }


            return result;
        }

        public void MoveFolder(String folder, String newContainingDirectory)
        {
            Common.MoveDirectory(Common.CombinePaths(_rootFilesPath, folder), Common.CombinePaths(_rootFilesPath, newContainingDirectory));
        }

        public void MoveFile(String file, String newContainingDiretory)
        {
            Common.MoveFile(Common.CombinePaths(_rootFilesPath, file), Common.CombinePaths(_rootFilesPath, newContainingDiretory));
        }

        public void RenameFile(String file, String newFileName)
        {
            String filePath = Common.CheckedCombinePaths(_rootFilesPath, file);
            FileInfo fileInfo = new FileInfo(filePath);
            String filePathNew = Common.CheckedCombinePaths(fileInfo.DirectoryName, newFileName);

            fileInfo.MoveTo(filePathNew);
        }

        public void RenameFolder(String folder, String newFolderName)
        {
            String folderPath = Common.CheckedCombinePaths(_rootFilesPath, folder);

            DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
            if (folderInfo.Parent != null)
            {
                String folderPathNew = Common.CheckedCombinePaths(folderInfo.Parent.FullName, newFolderName);

                folderInfo.MoveTo(folderPathNew);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo encoder = encoders.FirstOrDefault(t => t.MimeType == mimeType);

            return encoder;
        }
    }
}