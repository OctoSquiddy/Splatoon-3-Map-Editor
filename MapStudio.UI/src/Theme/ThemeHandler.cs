using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;
using Newtonsoft.Json;

namespace MapStudio.UI
{
    /// <summary>
    /// The theme instance of the application.
    /// </summary>
    public class ThemeHandler
    {
        public virtual string Name { get; set; }

        public static List<ThemeHandler> Themes = new List<ThemeHandler>();

        public virtual Vector4 Text { get; set; }
        public virtual Vector4 WindowBg { get; set; }
        public virtual Vector4 ChildBg { get; set; }
        public virtual Vector4 Border { get; set; }
        public virtual Vector4 PopupBg { get; set; }
        public virtual Vector4 FrameBg { get; set; }
        public virtual Vector4 FrameBgHovered { get; set; }
        public virtual Vector4 FrameBgActive { get; set; }
        public virtual Vector4 TitleBg { get; set; }
        public virtual Vector4 TitleBgActive { get; set; }
        public virtual Vector4 CheckMark { get; set; }
        public virtual Vector4 ButtonActive { get; set; }
        public virtual Vector4 Button{ get; set; }
        public virtual Vector4 Header { get; set; }
        public virtual Vector4 HeaderHovered { get; set; }
        public virtual Vector4 HeaderActive { get; set; }
        public virtual Vector4 SeparatorHovered { get; set; }
        public virtual Vector4 SeparatorActive { get; set; }
        public virtual Vector4 Separator { get; set; }
        public virtual Vector4 Tab { get; set; }
        public virtual Vector4 TabHovered { get; set; }
        public virtual Vector4 TabActive { get; set; }
        public virtual Vector4 TabUnfocused { get; set; }
        public virtual Vector4 TabUnfocusedActive { get; set; }
        public virtual Vector4 DockingPreview { get; set; }
        public virtual Vector4 DockingEmptyBg { get; set; }
        public virtual Vector4 TextSelectedBg { get; set; }
        public virtual Vector4 NavHighlight { get; set; }
        public virtual Vector4 Error { get; set; }
        public virtual Vector4 Warning { get; set; }

        public static Vector4 ActiveTextHighlight { get; set; }
        public static Vector4 HyperLinkText { get; set; }

        public static ThemeHandler Theme;

        public ThemeHandler()
        {
 
        }

        /// <summary>
        /// Updates the current theme of the application.
        /// </summary>
        public static void UpdateTheme(ThemeHandler theme)
        {
            Theme = theme;

            ImGui.GetStyle().WindowPadding = new Vector2(2);
            ImGui.GetStyle().FrameRounding = 5;

            ImGui.GetStyle().Colors[(int)ImGuiCol.Text] = theme.Text;
            ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg] = theme.WindowBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.ChildBg] = theme.ChildBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.Border] = theme.Border;
            ImGui.GetStyle().Colors[(int)ImGuiCol.PopupBg] = theme.PopupBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg] = theme.FrameBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBgHovered] = theme.FrameBgHovered;
            ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBgActive] = theme.FrameBgActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TitleBg] = theme.TitleBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TitleBgActive] = theme.TitleBgActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.CheckMark] = theme.CheckMark;
            ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonActive] = theme.ButtonActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.Header] = theme.Header;
            ImGui.GetStyle().Colors[(int)ImGuiCol.HeaderHovered] = theme.HeaderHovered;
            ImGui.GetStyle().Colors[(int)ImGuiCol.HeaderActive] = theme.HeaderActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.SeparatorHovered] = theme.SeparatorHovered;
            ImGui.GetStyle().Colors[(int)ImGuiCol.SeparatorActive] = theme.SeparatorActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.Separator] = theme.Separator;
            ImGui.GetStyle().Colors[(int)ImGuiCol.Tab] = theme.Tab;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TabHovered] = theme.TabHovered;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TabActive] = theme.TabActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TabUnfocused] = theme.TabUnfocused;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TabUnfocusedActive] = theme.TabUnfocusedActive;
            ImGui.GetStyle().Colors[(int)ImGuiCol.DockingPreview] = theme.DockingPreview;
            ImGui.GetStyle().Colors[(int)ImGuiCol.DockingEmptyBg] = theme.DockingEmptyBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.TextSelectedBg] = theme.TextSelectedBg;
            ImGui.GetStyle().Colors[(int)ImGuiCol.NavHighlight] = theme.NavHighlight;
            ImGui.GetStyle().Colors[(int)ImGuiCol.Button] = theme.Button;
            ImGui.GetStyle().Colors[(int)ImGuiCol.MenuBarBg] = theme.WindowBg;

            ActiveTextHighlight = new Vector4(0.46f, 0.73f, 1.00f, 1.000f);
            HyperLinkText = new Vector4(0.327f, 0.415f, 1.000f, 1.000f);
           // ImGui.GetStyle().Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.46f, 0.73f, 1.00f, 1.000f);
        }

        public static void Load()
        {
            string folder = Path.Combine(Toolbox.Core.Runtime.ExecutableDir,"Lib","Themes");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            Themes.Clear();
            foreach (var theme in Directory.GetFiles(folder)) {
               Themes.Add(JsonConvert.DeserializeObject<ThemeHandler>(File.ReadAllText(theme)));
            }
        }

        public static void Save()
        {
            string folder = Path.Combine(Toolbox.Core.Runtime.ExecutableDir,"Lib","Themes");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var theme in Themes)
                theme.Export(Path.Combine(folder,$"{theme.Name}.json"));
        }

        public void Export(string fileName)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.File.WriteAllText(fileName, json);
        }
    }

    /// <summary>
    /// A standard light theme.
    /// </summary>
    public class LightTheme : ThemeHandler
    {
        public override string Name { get; set; } = "LIGHT_THEME";

        public LightTheme()
        {
            Text = new Vector4(0, 0, 0, 1.00f);
            WindowBg = new Vector4(0.91f, 0.91f, 0.91f, 0.94f);
            ChildBg = new Vector4(0, 0, 0, 0.00f);
            Border = new Vector4(0.80f, 0.80f, 0.80f, 0.50f);
            PopupBg = new Vector4(1, 1, 1, 0.94f);
            FrameBg = new Vector4(1, 1, 1, 1);
            FrameBgActive = new Vector4(0.5f, 0.5f, 0.5f, 0.67f);
            TitleBg = new Vector4(0.85f, 0.85f, 0.85f, 1.000f);
            TitleBgActive = new Vector4(0.84f, 0.84f, 0.84f, 1.00f);
            CheckMark = new Vector4(0.37f, 0.53f, 0.71f, 1.00f);
            ButtonActive = new Vector4(0.34f, 0.54f, 1, 1.00f);
            Button = new Vector4(0.75f, 0.75f, 0.75f, 1.00f);
            Header = new Vector4(0.7f, 0.7f, 0.7f, 0.31f);
            HeaderHovered = new Vector4(0.7f, 0.7f, 0.7f, 0.80f);
            HeaderActive = new Vector4(0.7f, 0.7f, 0.7f, 1.00f);
            SeparatorHovered = new Vector4(0.82f, 0.82f, 0.82f, 0.78f);
            SeparatorActive = new Vector4(0.53f, 0.53f, 0.53f, 1.00f);
            Separator = new Vector4(0.85f, 0.85f, 0.85f, 1.00f);
            Tab = new Vector4(1, 1, 1, 0.86f);
            TabHovered = new Vector4(0.9f, 0.9f, 0.9f, 0.80f);
            TabActive = new Vector4(0.9f, 0.9f, 0.9f, 1.00f);
            TabUnfocused = new Vector4(0.9f, 0.9f, 0.9f, 0.98f);
            TabUnfocusedActive = new Vector4(0.9f, 0.9f, 0.9f, 1.00f);
            DockingPreview = new Vector4(0.6f, 0.6f, 0.6f, 0.70f);
            DockingEmptyBg = new Vector4(0.65f, 0.65f, 0.65f, 0.70f);
            TextSelectedBg = new Vector4(0.24f, 0.45f, 0.68f, 0.35f);
            NavHighlight = new Vector4(0.4f, 0.4f, 0.4f, 0);
            Error = new Vector4(1f, 0.3f, 0.3f, 1.0f);
            Warning = new Vector4(1, 1, 0.3f, 1.0f);
        }
    }

    /// <summary>
    /// A standard dark theme.
    /// </summary>
    public class DarkTheme : ThemeHandler
    {
        public override string Name { get; set; } = "DARK_THEME";

        public DarkTheme()
        {
            Text = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            WindowBg = new Vector4(0.17f, 0.17f, 0.17f, 0.94f);
            ChildBg = new Vector4(0.30f, 0.30f, 0.30f, 0.00f);
            Border = new Vector4(0.00f, 0.00f, 0.00f, 0.50f);
            PopupBg = new Vector4(0.2f, 0.2f, 0.2f, 0.94f);
            FrameBg = new Vector4(0.09f, 0.09f, 0.09f, 0.40f);
            FrameBgActive = new Vector4(0.42f, 0.42f, 0.42f, 0.67f);
            TitleBg = new Vector4(0.147f, 0.147f, 0.147f, 1.000f);
            TitleBgActive = new Vector4(0.13f, 0.13f, 0.13f, 1.00f);
            CheckMark = new Vector4(0.37f, 0.53f, 0.71f, 1.00f);
            ButtonActive = new Vector4(0.53f, 0.54f, 0.54f, 1.00f);
            Button = new Vector4(0.24f, 0.24f, 0.24f, 1.00f);
            Header = new Vector4(0.37f, 0.37f, 0.37f, 0.31f);
            HeaderHovered = new Vector4(0.46f, 0.46f, 0.46f, 0.80f);
            HeaderActive = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            SeparatorHovered = new Vector4(0.82f, 0.82f, 0.82f, 0.78f);
            SeparatorActive = new Vector4(0.53f, 0.53f, 0.53f, 1.00f);
            Separator = new Vector4(0.21f, 0.21f, 0.21f, 1.00f);
            Tab = new Vector4(0.16f, 0.16f, 0.16f, 0.86f);
            TabHovered = new Vector4(0.22f, 0.22f, 0.22f, 0.80f);
            TabActive = new Vector4(0.27f, 0.27f, 0.27f, 1.00f);
            TabUnfocused = new Vector4(0.12f, 0.12f, 0.12f, 0.98f);
            TabUnfocusedActive = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            DockingPreview = new Vector4(0.34f, 0.34f, 0.34f, 0.70f);
            TextSelectedBg = new Vector4(0.24f, 0.45f, 0.68f, 0.35f);
            NavHighlight = new Vector4(0.4f, 0.4f, 0.4f, 0);
            Error = new Vector4(1f, 0.3f, 0.3f, 1.0f);
            Warning = new Vector4(1, 1, 0.3f, 1.0f);
        }
    }

    /// <summary>
    /// A nicely dark blue theme.
    /// </summary>
    public class DarkBlueTheme : ThemeHandler
    {
        public override string Name { get; set; } = "DARK_BLUE_THEME";

        public DarkBlueTheme()
        {
            Text = new Vector4(0.80f, 0.83f, 0.96f, 1.00f);
            WindowBg = new Vector4(0.16f, 0.18f, 0.21f, 0.94f);
            ChildBg = new Vector4(0.16f, 0.18f, 0.21f, 0.00f);
            Border = new Vector4(0.13f, 0.14f, 0.16f, 0.50f);
            PopupBg = new Vector4(0.20f, 0.21f, 0.25f, 0.94f);
            FrameBg = new Vector4(0.08f, 0.08f, 0.08f, 0.40f);
            FrameBgActive = new Vector4(0.16f, 0.18f, 0.21f, 0.67f);
            TitleBg = new Vector4(0.14f, 0.15f, 0.18f, 1.000f);
            TitleBgActive = new Vector4(0.13f, 0.14f, 0.17f, 1.00f);
            CheckMark = new Vector4(0.37f, 0.53f, 0.71f, 1.00f);
            ButtonActive = new Vector4(0.53f, 0.54f, 0.54f, 1.00f);
            Button = new Vector4(0.24f, 0.25f, 0.29f, 1.00f);
            Header = new Vector4(0.32f, 0.34f, 0.38f, 0.31f);
            HeaderHovered = new Vector4(0.46f, 0.46f, 0.46f, 0.80f);
            HeaderActive = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            SeparatorHovered = new Vector4(0.82f, 0.82f, 0.82f, 0.78f);
            SeparatorActive = new Vector4(0.53f, 0.53f, 0.53f, 1.00f);
            Separator = new Vector4(0.21f, 0.21f, 0.21f, 1.00f);
            Tab = new Vector4(0.11f, 0.12f, 0.15f, 0.86f);
            TabHovered = new Vector4(0.19f, 0.19f, 0.22f, 0.80f);
            TabActive = new Vector4(0.16f, 0.18f, 0.21f, 1.00f);
            TabUnfocused = new Vector4(0.13f, 0.13f, 0.14f, 0.98f);
            TabUnfocusedActive = new Vector4(0.16f, 0.18f, 0.21f, 1.00f);
            DockingPreview = new Vector4(0.12f, 0.12f, 0.14f, 0.70f);
            DockingEmptyBg = new Vector4(0.10f, 0.10f, 0.11f, 1.00f);
            TextSelectedBg = new Vector4(0.24f, 0.45f, 0.68f, 0.35f);
            NavHighlight = new Vector4(0.26f, 0.66f, 1.00f, 0);
            Error = new Vector4(1f, 0.3f, 0.3f, 1.0f);
            Warning = new Vector4(1, 1, 0.3f, 1.0f);
        }
    }

    /// <summary>
    /// A theme somewhat based on UE4.
    /// </summary>
    public class UE4Theme : ThemeHandler
    {
        public override string Name { get; set; } = "UE4_THEME";

        public UE4Theme()
        {
            Text = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            WindowBg = new Vector4(0.06f, 0.06f, 0.06f, 0.94f);
            ChildBg = new Vector4(1.00f, 1.00f, 1.00f, 0.00f);
            Border = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            PopupBg = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
            FrameBg = new Vector4(0.20f, 0.21f, 0.22f, 0.54f);
            FrameBgActive = new Vector4(0.18f, 0.18f, 0.18f, 0.67f);
            TitleBg = new Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            TitleBgActive = new Vector4(0.29f, 0.29f, 0.29f, 1.00f);
            CheckMark = new Vector4(0.37f, 0.53f, 0.71f, 1.00f);
            ButtonActive = new Vector4(0.53f, 0.54f, 0.54f, 1.00f);
            Button = new Vector4(0.24f, 0.24f, 0.24f, 1.00f);
            Header = new Vector4(0.37f, 0.37f, 0.37f, 0.31f);
            HeaderHovered = new Vector4(0.46f, 0.46f, 0.46f, 0.80f);
            HeaderActive = new Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            SeparatorHovered = new Vector4(0.82f, 0.82f, 0.82f, 0.78f);
            SeparatorActive = new Vector4(0.53f, 0.53f, 0.53f, 1.00f);
            Separator = new Vector4(0.21f, 0.21f, 0.21f, 1.00f);
            Tab = new Vector4(0.16f, 0.16f, 0.16f, 0.86f);
            TabHovered = new Vector4(0.22f, 0.22f, 0.22f, 0.80f);
            TabActive = new Vector4(0.27f, 0.27f, 0.27f, 1.00f);
            TabUnfocused = new Vector4(0.12f, 0.12f, 0.12f, 0.98f);
            TabUnfocusedActive = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            DockingPreview = new Vector4(0.34f, 0.34f, 0.34f, 0.70f);
            TextSelectedBg = new Vector4(0.24f, 0.45f, 0.68f, 0.35f);
            NavHighlight = new Vector4(0.26f, 0.66f, 1.00f, 0);
            Error = new Vector4(1f, 0.3f, 0.3f, 1.0f);
            Warning = new Vector4(1, 1, 0.3f, 1.0f);
        }
    }
}
