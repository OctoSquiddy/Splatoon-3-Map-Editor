using System;
using System.Threading;
using System.IO;
using System.Linq;

namespace Updater
{
    internal class Program
    {
        static string execDirectory = "";

        static void Main(string[] args)
        {
            execDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            bool force = args.Contains("-f");
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-c":
                    case "--check":
                        JsonUpdaterHelper.Setup("https://ylwpnk.dev/mapeditor/update/releases.json", "MapStudio.exe");
                        var localVersion = JsonUpdaterHelper.GetLocalVersion(execDirectory);
                        var remote = JsonUpdaterHelper.GetRemoteVersionInfo();
                        if (remote == null)
                        {
                            Console.WriteLine("Unable to check for updates.");
                            break;
                        }
                        if (Version.Parse(remote.Version) > Version.Parse(localVersion))
                            Console.WriteLine($"Update available: {localVersion} -> {remote.Version}");
                        else
                            Console.WriteLine("Up to date.");
                        break;
                    case "-d":
                    case "--download":
                        JsonUpdaterHelper.Setup("https://ylwpnk.dev/mapeditor/update", "MapStudio.exe");
                        JsonUpdaterHelper.DownloadLatest(execDirectory, force);
                        break;
                    case "-i":
                    case "--install":
                        JsonUpdaterHelper.Install(execDirectory);
                        break;
                    case "-b":
                    case "--boot":
                        Boot();
                        Environment.Exit(0);
                        break;
                    case "-bl":
                    case "--boot_launcher":
                        BootLauncher();
                        Environment.Exit(0);
                        break;
                    case "-e":
                    case "--exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        static void BootLauncher()
        {
            Console.WriteLine("Booting...");

            Thread.Sleep(3000);
            System.Diagnostics.Process.Start(Path.Combine(execDirectory, "TrackStudioLauncher.exe"));
        }

        static void Boot()
        {
            Console.WriteLine("Booting...");

            Thread.Sleep(3000);
            System.Diagnostics.Process.Start(Path.Combine(execDirectory, "MapStudio.exe")); //"TrackStudio.exe"));
        }
    }
}
