using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MapStudio.UI
{
    //TODO need to support for all operating systems
    public class FileUtility
    {
        /// <summary>
        /// Loads given folder in file explorer.
        /// </summary>
        /// <param name="filePath"></param>
        public static void OpenFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start("explorer.exe", folderPath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("mimeopen", folderPath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", $"-R \"{folderPath}\"");
        }

        /// <summary>
        /// Loads in file explorer and selects the given file path.
        /// </summary>
        /// <param name="filePath"></param>
        public static void SelectFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string argument = "/select, \"" + filePath + "\"";
                Process.Start("explorer.exe", argument);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("mimeopen", filePath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", $"-R \"{filePath}\"");
        }

        public static void DeleteFolder(string folder)
        {
            foreach (var dir in Directory.GetDirectories(folder))
                DeleteFolder(dir);
            foreach (var file in Directory.GetFiles(folder))
                File.Delete(file);

            Directory.Delete(folder);
        }

        /// <summary>
        /// Loads a file with the default application in windows explorer.
        /// </summary>
        /// <param name="path"></param>
        public static void OpenWithDefaultProgram(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using Process fileopener = new Process();

                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
                fileopener.Start();
            }
        }
    }
}
