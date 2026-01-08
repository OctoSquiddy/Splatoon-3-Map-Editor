using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMapEditor
{
    public class GlobalSettings
    {
        //public static Dictionary<int, ActorDefinition> ActorDatabase = new Dictionary<int, ActorDefinition>();    // use name instead
        public static Dictionary<string, ActorDefinition> ActorDatabase = new Dictionary<string, ActorDefinition>();

        // Auto-detected game version from ActorInfo.Product.XXX.rstbl.byml.zs
        public static string DetectedGameVersion { get; private set; } = "";

        public static string GamePath { get; set; }

        public static string IPPath { get; set; }

        public static string SDORPath { get; set; }

        public static string ModOutputPath { get; set; }

        public static bool IsLinkingComboBoxActive { get; set; } = true;

        //public static bool IsMK8D => File.Exists($"{GamePath}\\RaceCommon\\TS_PolicePackun\\TS_PolicePackun.bfres"); // no.

        public static PathSettings PathDrawer = new PathSettings();

        /// <summary>
        /// Loads the actor database
        /// </summary>
        public static void LoadDataBase()
        {
            Console.WriteLine("~ Called GlobalSettings.LoadDataBase() ~");
            if (ActorDatabase.Count > 0)
                return;

            LoadActorDb();

            Console.WriteLine("~ Called GlobalSettings.LoadDataBase() ~");
        }

        /// <summary>
        /// Gets content path from either the update, game, or aoc directories based on what is present.
        /// </summary>
        public static string GetContentPath(string relativePath)
        {
            //Update first then base package.
            if (File.Exists($"{ModOutputPath}\\{relativePath}")) return $"{ModOutputPath}\\{relativePath}";
            if (File.Exists($"{IPPath}\\{relativePath}")) return $"{IPPath}\\{relativePath}";
            if (File.Exists($"{SDORPath}\\{relativePath}")) return $"{SDORPath}\\{relativePath}";
            if (File.Exists($"{GamePath}\\{relativePath}")) return $"{GamePath}\\{relativePath}";

            return relativePath;
        }

        /// <summary>
        /// Finds a versioned file (e.g., SomeFile.Product.*.rstbl.byml.zs) in all game paths.
        /// </summary>
        public static string FindVersionedFile(string folder, string filePattern)
        {
            string[] searchPaths = { ModOutputPath, IPPath, SDORPath, GamePath };

            foreach (var basePath in searchPaths)
            {
                if (string.IsNullOrEmpty(basePath))
                    continue;

                string searchPath = Path.Combine(basePath, folder);
                if (!Directory.Exists(searchPath))
                    continue;

                var files = Directory.GetFiles(searchPath, filePattern);
                if (files.Length > 0)
                    return files[0];
            }

            return null;
        }

        static void LoadActorDb()
        {
            Console.WriteLine("~ Called GlobalSettings.LoadActorDb() ~");

            // Auto-detect ActorInfo.Product.XXX.rstbl.byml.zs file
            string path = FindActorDbFile();

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Console.WriteLine($"ActorInfo.Product.*.rstbl.byml.zs could not be found in any game path!");
                return;
            }

            Console.WriteLine($"[ActorDB] Auto-detected: {path}");

            var actorDb = new ActorDefinitionDb(path);
            foreach (var actor in actorDb.Definitions)
            {
                //Console.WriteLine($"~ Adding Actor: {actor.Name}");
                ActorDatabase.Add(actor.Name, actor);
            }
        }

        /// <summary>
        /// Searches for ActorInfo.Product.*.rstbl.byml.zs in all game paths and returns the first found.
        /// </summary>
        static string FindActorDbFile()
        {
            string[] searchPaths = { ModOutputPath, IPPath, SDORPath, GamePath };
            string pattern = "ActorInfo.Product.*.rstbl.byml.zs";

            foreach (var basePath in searchPaths)
            {
                if (string.IsNullOrEmpty(basePath))
                    continue;

                string rsdbPath = Path.Combine(basePath, "RSDB");
                if (!Directory.Exists(rsdbPath))
                    continue;

                var files = Directory.GetFiles(rsdbPath, pattern);
                if (files.Length > 0)
                {
                    // Extract version from filename for other uses
                    string fileName = Path.GetFileName(files[0]);
                    // ActorInfo.Product.a10.rstbl.byml.zs -> a10 (supports alphanumeric versions)
                    var match = System.Text.RegularExpressions.Regex.Match(fileName, @"ActorInfo\.Product\.([a-zA-Z0-9]+)\.rstbl\.byml\.zs");
                    if (match.Success)
                    {
                        DetectedGameVersion = match.Groups[1].Value;
                        Console.WriteLine($"[ActorDB] Detected game version: {DetectedGameVersion}");
                    }
                    return files[0];
                }
            }

            return null;
        }



        public class PathSettings
        {
            public PathColor RailColor0 = new PathColor(new Vector3(170, 0, 160), new Vector3(255, 64, 255), new Vector3(255, 64, 255));
            public PathColor RailColor1 = new PathColor(new Vector3(170, 160, 0), new Vector3(255, 0, 0), new Vector3(255, 0, 0));
        }

        public class PathColor
        {
            public Vector3 PointColor = new Vector3(0, 0, 0);
            public Vector3 LineColor = new Vector3(0, 0, 0);
            public Vector3 ArrowColor = new Vector3(0, 0, 0);

            [JsonIgnore]
            public EventHandler OnColorChanged;

            public PathColor(Vector3 point, Vector3 line, Vector3 arrow)
            {
                PointColor = new Vector3(point.X / 255f, point.Y / 255f, point.Z / 255f);
                LineColor = new Vector3(line.X / 255f, line.Y / 255f, line.Z / 255f);
                ArrowColor = new Vector3(arrow.X / 255f, arrow.Y / 255f, arrow.Z / 255f);
            }
        }
    }
}
