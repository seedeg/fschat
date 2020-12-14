using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace FsChat.Utilities.Common
{
    public static class FileOp
    {
        public static string GetEmbeddedResource(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void SaveFile(Stream stream, string outputFile)
        {
            using (var fileStream = File.Create(outputFile))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        public static bool IsFileExtensionValid(string filePath, params string[] validExtensions)
        {
            //Get the extension (. is included)
            var fileExtension = Path.GetExtension(filePath);

            //Remove the dot
            if (fileExtension.StartsWith("."))
            {
                fileExtension = fileExtension.Substring(1, fileExtension.Length - 1);
            }

            //Remove the dots from any extensions provided as input
            for (var i = 0; i < validExtensions.Length; i++)
            {
                if (validExtensions[i].StartsWith("."))
                {
                    validExtensions[i] = validExtensions[i].Substring(1, validExtensions[i].Length - 1);
                }
            }

            return validExtensions.Any(x => x.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsImageFile(string filePath)
        {
            return IsFileExtensionValid(filePath, "png", "jpg", "jpeg", "gif");
        }

        public static bool SafeDelete(string filePath)
        {
            const int maxAttempts = 10;
            for (var i = 0; i < maxAttempts; i++)
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }

            return false;
        }
    }
}
