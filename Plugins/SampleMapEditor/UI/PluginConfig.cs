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
    public class PluginConfig : IPluginConfig
    {
        internal static bool init = false;

        public PluginConfig() { init = true; }

        [JsonProperty]
        public static string S3GameVersion1 = "a";

        [JsonProperty]
        public static string S3GameVersion2 = "1";

        [JsonProperty]
        public static string S3GameVersion3 = "0";

        [JsonProperty]
        public static string S3GamePath = "";

        [JsonProperty]
        public static string S3IPPath = "";

        [JsonProperty]
        public static string S3SDORPath = "";

        [JsonProperty]
        public static string S3ModPath = "";

        [JsonProperty]
        public static string ModPath = "";

        public void DrawUI()
        {
            if (ImguiCustomWidgets.PathSelector("Splatoon 3", ref S3GamePath)) Save();
            if (ImguiCustomWidgets.PathSelector("Splatoon 3: Inkopolis Plaza", ref S3IPPath)) Save();
            if (ImguiCustomWidgets.PathSelector("Splatoon 3: Side Order", ref S3SDORPath)) Save();
            if (ImguiCustomWidgets.PathSelector("Mod Saving Folder", ref ModPath)) Save();
        }

        public void DrawInSettings()
        {

            byte[] buffer1 = new byte[8];
            byte[] buffer2 = new byte[8];
            byte[] buffer3 = new byte[8];

            Array.Copy(System.Text.Encoding.ASCII.GetBytes(S3GameVersion1 ?? ""), buffer1, Math.Min(S3GameVersion1?.Length ?? 0, buffer1.Length));
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(S3GameVersion2 ?? ""), buffer2, Math.Min(S3GameVersion2?.Length ?? 0, buffer2.Length));
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(S3GameVersion3 ?? ""), buffer3, Math.Min(S3GameVersion3?.Length ?? 0, buffer3.Length));

            bool changed = false;

            ImGui.PushItemWidth(50); // Optional: control width of each input box

            changed |= ImGui.InputText("##ver1", buffer1, (uint)buffer1.Length);
            ImGui.SameLine();
            changed |= ImGui.InputText("##ver2", buffer2, (uint)buffer2.Length);
            ImGui.SameLine();
            changed |= ImGui.InputText("##ver3", buffer3, (uint)buffer3.Length);

            ImGui.PopItemWidth();

            ImGui.SameLine();
            ImGui.Text("Game Version");

            if (changed)
            {
                S3GameVersion1 = CleanInput(buffer1);
                S3GameVersion2 = CleanInput(buffer2);
                S3GameVersion3 = CleanInput(buffer3);

                Save();
            }
        }

        private static string CleanInput(byte[] buffer)
        {
            int len = Array.IndexOf(buffer, (byte)0);
            if (len < 0) len = buffer.Length;
            return System.Text.Encoding.ASCII.GetString(buffer, 0, len).Trim();
        }

        public static PluginConfig Load()
        {
            Console.WriteLine("Loading config...");
            string path = $"{Runtime.ExecutableDir}\\SampleMapEditorConfig.json";

            if (!File.Exists(path))
                new PluginConfig().Save();

            var config = JsonConvert.DeserializeObject<PluginConfig>(File.ReadAllText(path));
            config.Reload();
            return config;
        }

        public void Save()
        {
            Console.WriteLine("Saving config...");
            File.WriteAllText($"{Runtime.ExecutableDir}\\SampleMapEditorConfig.json", JsonConvert.SerializeObject(this));
            Reload();
        }

        public void Reload()
        {
            GlobalSettings.GamePath = S3GamePath;
            GlobalSettings.IPPath = S3IPPath;
            GlobalSettings.SDORPath = S3SDORPath;
            GlobalSettings.ModOutputPath = S3ModPath;
            MapStudio.UI.GlobalSettings.Current.Program.ModPath = ModPath;

            GlobalSettings.S3GameVersion1 = S3GameVersion1;
            GlobalSettings.S3GameVersion2 = S3GameVersion2;
            GlobalSettings.S3GameVersion3 = S3GameVersion3;
        }
    }
}
