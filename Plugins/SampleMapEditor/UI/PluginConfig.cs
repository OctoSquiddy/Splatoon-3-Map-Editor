using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;
using Newtonsoft.Json;
using ImGuiNET;
using MapStudio.UI;
using Toolbox.Core;

namespace SampleMapEditor
{
    /// <summary>
    /// Represents UI for the plugin which is currently showing in the Paths section of the main menu UI.
    /// This is used to configure game paths.
    /// </summary>
    public class PluginConfig : IPluginConfig
    {
        //Only load the config once when this constructor is activated.
        internal static bool init = false;

        public PluginConfig() { init = true; }

        /*[JsonProperty]
        public static string GamePath = "";*/

        [JsonProperty]
        public static int S3GameVersion1 = 3;

        [JsonProperty]
        public static int S3GameVersion2 = 1;

        [JsonProperty]
        public static int S3GameVersion3 = 0;

        [JsonProperty]
        public static string S3GamePath = "";

        [JsonProperty]
        public static string S3IPPath = "";

        [JsonProperty]
        public static string S3SDORPath = "";

        [JsonProperty]
        public static string S3ModPath = "";

        /// <summary>
        /// Renders the current configuration UI.
        /// </summary>
        public void DrawUI()
        {
            if (ImguiCustomWidgets.PathSelector("Splatoon 3", ref S3GamePath))
            {
                Save();
            }
            if (ImguiCustomWidgets.PathSelector("Splatoon 3: Inkopolis Plaza", ref S3IPPath))
            {
                Save();
            }
            if (ImguiCustomWidgets.PathSelector("Splatoon 3: Side Order", ref S3SDORPath))
            {
                Save();
            }
            if (ImguiCustomWidgets.PathSelector("Splatoon 3 File Saving Path", ref S3ModPath))
            {
                Save();
            }
        }

        unsafe public void DrawInSettings()
        {
            int[] IntArr = { S3GameVersion1, S3GameVersion2, S3GameVersion3 };

            fixed (int* arrayPtr = &IntArr[0])
            {
                if (ImGui.InputScalarN("Game Version", ImGuiDataType.S32, (IntPtr)arrayPtr, 3))
                {
                    S3GameVersion1 = IntArr[0];
                    S3GameVersion2 = IntArr[1];
                    S3GameVersion3 = IntArr[2];

                    Save();
                }
            }
        }

        /// <summary>
        /// Loads the config json file on disc or creates a new one if it does not exist.
        /// </summary>
        /// <returns></returns>
        public static PluginConfig Load() {
            Console.WriteLine("Loading config...");
            if (!File.Exists($"{Runtime.ExecutableDir}\\SampleMapEditorConfig.json")) { new PluginConfig().Save(); }

            var config = JsonConvert.DeserializeObject<PluginConfig>(File.ReadAllText($"{Runtime.ExecutableDir}\\SampleMapEditorConfig.json"));
            config.Reload();
            return config;
        }

        /// <summary>
        /// Saves the current configuration to json on disc.
        /// </summary>
        public void Save() {
            Console.WriteLine("Saving config...");
            File.WriteAllText($"{Runtime.ExecutableDir}\\SampleMapEditorConfig.json", JsonConvert.SerializeObject(this));
            Reload();
        }

        /// <summary>
        /// Called when the config file has been loaded or saved.
        /// </summary>
        public void Reload()
        {
            GlobalSettings.GamePath = S3GamePath;
            GlobalSettings.IPPath = S3IPPath;
            GlobalSettings.SDORPath = S3SDORPath;
            GlobalSettings.ModOutputPath = S3ModPath;

            GlobalSettings.S3GameVersion1 = S3GameVersion1;
            GlobalSettings.S3GameVersion2 = S3GameVersion2;
            GlobalSettings.S3GameVersion3 = S3GameVersion3;
        }
    }
}
