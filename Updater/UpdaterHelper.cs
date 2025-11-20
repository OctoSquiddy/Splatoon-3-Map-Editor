using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading;

namespace Updater
{
    /// <summary>
    /// A helper class for downloading releases content
    /// </summary>
    public class JsonUpdaterHelper
    {
        public record RemoteVersion
        {
            public required string Version { get; set; }
            public required string DownloadUrl { get; set; }
            public required DateTimeOffset UpdatedAt { get; set; }
        }

        private static string _updateUrl = "";
        private static string _process_name = "";

        private static RemoteVersion? remoteVersion;

        /// <summary>
        /// Prepares the updater with the update URL and process to target installing.
        /// </summary>
        public static void Setup(string updateUrl, string process = "")
        {
            _updateUrl = updateUrl;
            _process_name = process;

            GetRemoteVersion().Wait();
        }

        /// <summary>
        /// Gets the remote version info after setup.
        /// </summary>
        public static RemoteVersion? GetRemoteVersionInfo()
        {
            return remoteVersion;
        }

        private static async Task GetRemoteVersion()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            try
            {
                var json = await client.GetStringAsync(_updateUrl);
                remoteVersion = JsonSerializer.Deserialize<RemoteVersion>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch remote version: {ex.Message}");
                remoteVersion = null;
            }
        }

        /// <summary>
        /// Downloads the latest release if the version does not match the current version.
        /// </summary>
        public static void DownloadLatest(string folder, bool force = false)
        {
            Console.WriteLine("Checking for updates...");
            var localVersionStr = GetLocalVersion(folder);
            if (remoteVersion == null)
            {
                Console.WriteLine("Cannot check for updates: remote version unavailable.");
                return;
            }
            var localVersion = Version.TryParse(localVersionStr, out var lv) ? lv : new Version(0,0,0);
            var remoteVer = Version.TryParse(remoteVersion.Version, out var rv) ? rv : new Version(0,0,0);
            if (force || remoteVer > localVersion)
            {
                Console.WriteLine($"An update is available: {localVersion} -> {remoteVersion.Version}");
                DownloadRelease(folder, remoteVersion).Wait();
                WriteLocalVersion(folder, remoteVersion);
            }
            else
            {
                Console.WriteLine("Application is up to date.");
            }
        }

        private static async Task DownloadRelease(string folder, RemoteVersion remote)
        {
            ProgressBar progressBar = new ProgressBar();
            Console.WriteLine();
            Console.WriteLine($"Downloading update from {remote.DownloadUrl}");
            string name = "latest";
            //Download the releases zip
            using (var webClient = new WebClient())
            {
                IWebProxy webProxy = WebRequest.DefaultWebProxy;
                webProxy.Credentials = CredentialCache.DefaultCredentials;
                webClient.Proxy = webProxy;
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    progressBar.Report(e.ProgressPercentage / 100.0f);
                };
                Uri uri = new Uri(remote.DownloadUrl);
                await webClient.DownloadFileTaskAsync(uri, $"{folder}\\{name}.zip").ConfigureAwait(false);

                progressBar.Report(1.0f);
                progressBar.Dispose();
                Console.WriteLine($"");
                Console.WriteLine($"Extracting update!");
                //Extract the zip for installing
                ExtractZip($"{folder}\\{name}");
                Console.WriteLine($"Download finished!");
            }
        }

        /// <summary>
        /// Installs the currently downloaded and extracted update to the given folder directory.
        /// </summary>
        public static void Install(string folderDir)
        {
            string path = $"{folderDir}\\latest\\net8.0";
            if (!Directory.Exists(path))
            {
                Console.WriteLine("No downloaded directory found!");
                return;
            }
            if (Process.GetProcessesByName(_process_name).Any())
            {
                Console.WriteLine("Cannot install update while application is running. Please close it then try again!");
                return;
            }
            //Transfer the downloaded update files onto the current tool. 
            foreach (string dir in Directory.GetDirectories(path))
            {
                string dirName = new DirectoryInfo(dir).Name;
                //Remove existing directories
                if (Directory.Exists(Path.Combine(folderDir, dirName + @"\")))
                    Directory.Delete(Path.Combine(folderDir, dirName + @"\"), true);
                Directory.Move(dir, Path.Combine(folderDir, dirName + @"\"));
            }
            foreach (string file in Directory.GetFiles(path))
            {
                //Little hacky. Just skip the updater files as it currently uses the same directory as the installed tool.
                if (Path.GetFileName(file).StartsWith("Updater"))
                    continue;
                //Remove existing files
                if (File.Exists(Path.Combine(folderDir, Path.GetFileName(file))))
                    File.Delete(Path.Combine(folderDir, Path.GetFileName(file)));
                File.Move(file, Path.Combine(folderDir, Path.GetFileName(file)));
            }
            Directory.Delete($"{folderDir}\\latest", true);
        }

        public static string GetLocalVersion(string folder)
        {
            if (!File.Exists($"{folder}\\Version.txt"))
                return "0.0.0";
            var line = File.ReadLines($"{folder}\\Version.txt").FirstOrDefault();
            return string.IsNullOrEmpty(line) ? "0.0.0" : line;
        }

        static void WriteLocalVersion(string folder, RemoteVersion remote)
        {
            using (StreamWriter writer = new StreamWriter($"{folder}\\Version.txt"))
            {
                writer.WriteLine(remote.Version);
                writer.WriteLine(remote.UpdatedAt.ToString());
            }
        }

        static void ExtractZip(string filePath)
        {
            //Extract the updated zip
            ZipFile.ExtractToDirectory(filePath + ".zip", filePath + "/");
            //Zip not needed anymore
            File.Delete(filePath + ".zip");
        }
    }
}
