using System;
using System.Linq;
using System.Text;
using System.IO;

namespace Simple.Utilities
{
    public static class StringIOExtenstions
    {
        public const string ApplicationDirectoryPlaceHolder = "{appPath}";

        public static bool HasExtensionOfAny(this string filePath, params string[] extensions)
        {
            return extensions.Any(extension => filePath.HasExtension(extension));
        }

        public static bool HasExtension(this string filePath, string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return filePath.FileExtension().CaseInsensitiveEquals(extension);
        }

        public static string FileExtension(this string filePath)
        {
            Requires.ArgumentNotNullOrEmptyString(filePath, "filePath");
            return Path.GetExtension(filePath);
        }

        public static string InApplicationDirectory(this string path)
        {
            return
                Path.Combine(ApplicationDirectoryPlaceHolder.AddDirectorySeparator(), path).MakeAppRelative();
        }

        public static string MakeAppRelative(this string path)
        {
            return path.Replace(ApplicationDirectoryPlaceHolder.AddDirectorySeparator(), AppDomain.CurrentDomain.BaseDirectory.AddDirectorySeparator());
        }

        public static bool NotExists(this string path)
        {
            return !path.Exists();
        }

        public static void VerifyDirectoryCreated(this string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static bool Exists(this string path)
        {
            Requires.FileArgumentValid(path, "path");
            return File.Exists(path);
        }

        public static string AddExtension(this string path, string extension)
        {
            Requires.FileArgumentValid(path, "path");
            Requires.ArgumentNotNullOrEmptyString(extension, "extension");

            var builder = new StringBuilder(path.Length + extension.Length + 1);
            builder.Append(path);

            if (!extension.StartsWith("."))
            {
                builder.Append(".");
            }

            builder.Append(extension);
            return builder.ToString();
        }
        public static string AddDirectorySeparator(this string path)
        {
            Requires.ArgumentNotNullOrEmptyString(path, "path");

            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        public static void ForEachFileRecuresive(this string path, string pattern, Action<FileInfo> actionOnFile, Action<DirectoryInfo> actionOnDirectory = null)
        {
            var directoryInfo = new DirectoryInfo(path);

            ForEachFileRecuresive(directoryInfo, pattern, actionOnFile, actionOnDirectory);
        }

        public static void ForEachFileRecuresive(this DirectoryInfo directoryInfo, string pattern, Action<FileInfo> actionOnFile, Action<DirectoryInfo> actionOnDirectory = null)
        {
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(directoryInfo.FullName);
            }
            if (actionOnDirectory.IsNotNull())
            {
                actionOnDirectory(directoryInfo);
            }
            var subDirectories = directoryInfo.GetDirectories();
            foreach (var subDirectory in subDirectories)
            {
                ForEachFileRecuresive(subDirectory, pattern, actionOnFile,actionOnDirectory);
            }

            var files = directoryInfo.GetFiles(pattern);
            foreach (var fileInfo in files)
            {
                actionOnFile(fileInfo);
            }
        }

    }
}
