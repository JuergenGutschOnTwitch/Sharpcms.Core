// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sharpcms.Base.Library.Plugin;

namespace Sharpcms.Base.Library.Common
{
    public static class Common
    {
        public static IEnumerable<string> FlattenToStrings(IEnumerable<object> results)
        {
            IEnumerable<object> flattened = PluginServices.Flatten(results);

            return flattened.Select(result => result as string).ToArray();
        }

        public static string[] RemoveOne(string[] args)
        {
            if (args != null && args.Length > 1)
            {
                string[] argsNew = new string[args.Length - 1];
                for (int i = 1; i < args.Length; i++)
                {
                    argsNew[i - 1] = args[i];
                }

                return argsNew;
            }
            return null;
        }

        public static string[] RemoveOneLast(string[] args)
        {
            if (args != null && args.Length > 1)
            {
                string[] argsNew = new string[args.Length - 1];
                for (int i = 0; i < args.Length - 1; i++)
                {
                    argsNew[i] = args[i];
                }

                return argsNew;
            }
            return null;
        }

        public static bool StringArrayContains(IEnumerable<string> args, string value)
        {
            bool contains = args.Any(currentValue => currentValue == value);

            return contains;
        }

        public static string CombinePaths(params string[] paths)
        {
            //ToDo: this is not safe yeat - a stack overflow happened...
            string combinedPath = string.Empty;

            for (int i = 1; i < paths.Length; i++)
            {
                combinedPath = i == 1 
                    ? Path.Combine(paths[i - 1], paths[i]) 
                    : Path.Combine(combinedPath, paths[i]);
            }

            return combinedPath;
        }

        public static string CleanToSafeString(string dirtyString)
        {
            // ToDo: quick hack to handle Danish characters (should be more generic)
            dirtyString = dirtyString.Replace("æ", "ae").Replace("ø", "oe").Replace("å", "aa");
            dirtyString = dirtyString.Replace("Æ", "Ae").Replace("Ø", "Oe").Replace("Å", "Aa");
            dirtyString = dirtyString.Replace("è", "e").Replace("é", "e").Replace("à", "a");
            dirtyString = dirtyString.Replace("È", "E").Replace("É", "E").Replace("À", "A");
            dirtyString = dirtyString.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue");
            dirtyString = dirtyString.Replace("Ä", "Ae").Replace("Ö", "Oe").Replace("Ü", "Ue");

            // Trim .'s
            dirtyString = dirtyString.Trim('.').Trim(' ').Trim('.');
            dirtyString = dirtyString.Replace("..", ".");
            string semiCleanChars = Settings.DefaultInstance["common/cleanChars/notInBeginning"];
            string cleanChars = Settings.DefaultInstance["common/cleanChars/anywhere"];
            char[] loweredDirtyChars = dirtyString.ToLower().ToCharArray();
            char[] originalChars = dirtyString.ToCharArray();

            for (int index = 0; index < dirtyString.Length; index++)
            {
                bool allowed = false;
                char currentChar = loweredDirtyChars[index];

                if (index > 0)
                {
                    if (semiCleanChars.IndexOf(currentChar) > -1)
                    {
                        allowed = true;
                    }
                }

                if (cleanChars.IndexOf(currentChar) > -1)
                {
                    allowed = true;
                }

                if (!allowed)
                {
                    originalChars[index] = '_';
                }
            }

            string cleanString = new string(originalChars);

            // Remove double underscores
            bool doubleUnderscoresRemoved = true;
            while (doubleUnderscoresRemoved)
            {
                string beforeRemoval = cleanString;
                cleanString = cleanString.Replace("__", "_");

                if (cleanString == beforeRemoval)
                {
                    doubleUnderscoresRemoved = false;
                }
            }

            return cleanString;
        }

        public static bool DeleteDirectory(string path)
        {
            string absolutePath = CombinePaths(Settings.DefaultInstance.RootPath, path);
            if (!Directory.Exists(absolutePath))
            {
                return false;
            }

            if (!PathIsInSite(path))
            {
                return false;
            }

            Directory.Delete(absolutePath, true);

            return true;
        }

        public static bool DeleteFile(string path)
        {
            string absolutePath = CombinePaths(Settings.DefaultInstance.RootPath, path);
            if (!File.Exists(absolutePath))
            {
                return false;
            }

            if (!PathIsInSite(path))
            {
                return false;
            }

            File.Delete(absolutePath);

            return true;
        }

        public static bool MoveFile(string path, string newContainingDirectory)
        {
            string sourceAbsolutePath = CheckedCombinePath(path);
            string newContainingDirectoryAbsolutePath = CheckedCombinePath(newContainingDirectory);

            if (!File.Exists(sourceAbsolutePath))
            {
                return false;
            }

            if (!Directory.Exists(newContainingDirectoryAbsolutePath))
            {
                return false;
            }

            string filename = new FileInfo(sourceAbsolutePath).Name;
            string newFilename = CombinePaths(newContainingDirectoryAbsolutePath, filename);
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }

            File.Move(sourceAbsolutePath, newFilename);

            return true;
        }

        public static bool MoveDirectory(string path, string newContainingDirectory)
        {
            string sourceAbsolutePath = CheckedCombinePath(path);
            string newContainingDirectoryAbsolutePath = CheckedCombinePath(newContainingDirectory);

            if (!Directory.Exists(sourceAbsolutePath))
            {
                return false;
            }

            if (!Directory.Exists(newContainingDirectoryAbsolutePath))
            {
                return false;
            }

            string directoryName = new DirectoryInfo(sourceAbsolutePath).Name;
            string newDirectoryName = CombinePaths(newContainingDirectoryAbsolutePath, directoryName);
            if (Directory.Exists(newDirectoryName))
            {
                Directory.Delete(newDirectoryName);
            }

            Directory.Move(path, newDirectoryName);

            return true;
        }

        public static void CopyDirectory(string srcPath, string destPath, bool recursive = false)
        {
            CopyDirectory(new DirectoryInfo(CheckedCombinePath(srcPath)), new DirectoryInfo(CheckedCombinePath(destPath)), recursive);
        }

        private static void CopyDirectory(DirectoryInfo srcPath, DirectoryInfo destPath, bool recursive)
        {
            if (!PathIsUnderRoot(srcPath.FullName) || !PathIsUnderRoot(destPath.FullName)) return;

            if (!destPath.Exists)
            {
                destPath.Create();
            }

            // Copy files
            foreach (FileInfo fileInfo in srcPath.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(destPath.FullName, fileInfo.Name), true);
            }

            // Copy directories
            if (recursive)
                foreach (DirectoryInfo directoryInfo in srcPath.GetDirectories())
                {
                    if (!directoryInfo.Name.StartsWith("."))
                    {
                        CopyDirectory(directoryInfo, new DirectoryInfo(Path.Combine(destPath.FullName, directoryInfo.Name)), true);
                    }
                }
        }

        /// <summary>
        /// Returns a path combined with the site root.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string CheckedCombinePath(string path)
        {
            return CheckedCombinePaths(Settings.DefaultInstance.RootPath, path);
        }

        public static string CheckedCombinePaths(string root, params string[] paths)
        {
            string[] allPaths = new string[paths.Length + 1];
            allPaths[0] = root;
            paths.CopyTo(allPaths, 1);

            string combinedPath = CombinePaths(allPaths);
            if (PathIsUnderRoot(root, combinedPath))
            {
                return combinedPath;
            }

            throw new ArgumentException("The combined path does not begin with the root path.");
        }

        public static string FormatFilePath(string path)
        {
            return path.Replace("/", "\\");
        }

        private static bool PathIsUnderRoot(string path)
        {
            return PathIsUnderRoot(Settings.DefaultInstance.RootPath, path);
        }

        private static bool PathIsUnderRoot(string root, string path)
        {
            return path.StartsWith(root);
        }

        public static bool PathIsInSite(string path)
        {
            string absolutePath = CombinePaths(Settings.DefaultInstance.RootPath, path);

            return PathIsUnderRoot(Settings.DefaultInstance.RootPath, absolutePath);
        }

        public static string GetMainMimeType(string extension)
        {
            string mimeType = GetMimeType(extension);

            return !string.IsNullOrEmpty(mimeType) ? mimeType.Substring(0, mimeType.IndexOf('/')) : string.Empty;
        }

        public static string GetMimeType(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            string mimeType = Settings.DefaultInstance["common/mimetypes/" + extension];

            return mimeType != string.Empty ? mimeType : Settings.DefaultInstance["mimetypes/defaulttype"];
        }

        public static string[] SplitByString(string originalString, string pattern)
        {
            int offset = 0;
            int index = 0;
            int[] offsets = new int[originalString.Length + 1];

            while (index < originalString.Length)
            {
                int indexOf = originalString.IndexOf(pattern, index, StringComparison.Ordinal);
                if (indexOf != -1)
                {
                    offsets[offset++] = indexOf;
                    index = (indexOf + pattern.Length);
                }
                else
                {
                    index = originalString.Length;
                }
            }

            string[] final = new string[offset + 1];
            if (offset == 0)
            {
                final[0] = originalString;
            }
            else
            {
                offset--;
                final[0] = originalString.Substring(0, offsets[0]);
                for (int i = 0; i < offset; i++)
                {
                    final[i + 1] = originalString.Substring(offsets[i] + pattern.Length,
                                                            offsets[i + 1] - offsets[i] - pattern.Length);}
                final[offset + 1] = originalString.Substring(offsets[offset] + pattern.Length);
            }
            return final;
        }

        public static bool TryParseCssColor(string value, out string cssColor)
        {
            value = string.Format("#{0}", value);
            if (Regex.IsMatch(value, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
            {
                cssColor = value;

                return true;
            }

            cssColor = string.Empty;

            return false;
        }

        public static bool IsValidImage(string imagePath)
        {
            try
            {
                Image image = Image.FromFile(imagePath);

                image.Dispose();
            }
            catch (OutOfMemoryException)
            {
                return false;
            }

            return true;
        }
    }
}