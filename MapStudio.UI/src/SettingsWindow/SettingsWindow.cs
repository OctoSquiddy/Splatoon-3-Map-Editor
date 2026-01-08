using System;
using System.Numerics;
using System.IO;
using ImGuiNET;
using GLFrameworkEngine;
using Toolbox.Core;

namespace MapStudio.UI
{
    public class SettingsWindow : UIFramework.Window
    {
        public IPluginConfig[] PluginSettingsUI;

        public override string Name => "SETTINGS";

        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoDocking;

        private GlobalSettings Settings;
        private int selectedIndex = 0;

        public SettingsWindow(GlobalSettings settings) {
            Settings = settings;
            Opened = false;
            Size = new Vector2(500, 700);
            PlaceAtCenter = true;
        }

        public override void Render()
        {
            ImGui.Columns(2);

            var color = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
            var selColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TextSelectedBg];

            ImGui.PushStyleColor(ImGuiCol.ChildBg, color);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, selColor);
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, selColor);

            ImGui.SetColumnWidth(0, 150);

            if (ImGui.BeginChild("vertical_tab_settings"))
            {
                RenderTab(0, '\uf013', "GENERAL");
                RenderTab(1, '\uf53f', "APPEARANCE");
                RenderTab(2, '\uf11c', "CONTROLS");
            }
            ImGui.EndChild();

            ImGui.PopStyleColor(3);

            ImGui.NextColumn();

            if (ImGui.BeginChild("tab_window"))
            {
                if (selectedIndex == 0)
                    DrawGeneral();
                if (selectedIndex == 1)
                    DrawAppearance();
                if (selectedIndex == 2)
                    KeyInputWindow.Render();
            }
            ImGui.EndChild();
            ImGui.NextColumn();
        }

        private void RenderTab(int index, char icon, string text)
        {
            var size = new Vector2(ImGui.GetColumnWidth(), 30);

            var pos = ImGui.GetCursorPos();
            if (ImGui.Selectable($"##{text}", selectedIndex == index, ImGuiSelectableFlags.None, size))
            {
                selectedIndex = index;
            }
            var pos2 = ImGui.GetCursorPos();
            var textH= ImGui.CalcTextSize(text).Y;

            ImGui.SetCursorPos(pos);
            ImGui.SetCursorPosY(pos.Y + ((size.Y - textH) * 0.5f));
            ImGui.Text($"    {icon}    {TranslationSource.GetText(text)}");

            ImGui.SetCursorPos(pos2);
        }

        private void DrawGeneral()
        {
            DrawLanguageSettings();

            if (ImGuiHelper.InputFromFloat(TranslationSource.GetText("FONT_SCALE"), Settings.Program, "FontScale"))
            {
                //Set the adjustable global font scale
                ImGui.GetIO().FontGlobalScale = Settings.Program.FontScale;
                Settings.Save();
            }

            // Plugin entry point
            foreach (var config in PluginSettingsUI)
                config.DrawInSettings();
        }

        private void DrawLanguageSettings()
        {
            var language = TranslationSource.LanguageKey;
            if (ImGui.BeginCombo($"{TranslationSource.GetText("LANGUAGE")}", language))
            {
                foreach (var lang in TranslationSource.GetLanguages())
                {
                    string name = Path.GetFileNameWithoutExtension(lang);
                    bool isSelected = name == language;
                    if (ImGui.Selectable(name, isSelected))
                    {
                        TranslationSource.Instance.Update(name);
                        Settings.Program.Language = name;
                        Settings.Save();

                        if (Workspace.ActiveWorkspace != null)
                        {
                            Workspace.ActiveWorkspace.UpdateDockLayout = true;
                            Workspace.ActiveWorkspace.OnLanguageChanged();
                        }
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }
            if (ImGui.Button(TranslationSource.GetText("DUMP_UNTRANSLATED")))
                TranslationSource.Instance.DumpUntranslated();
        }

        private void DrawThemeSettings()
        {
            string currentThemeName = Settings.Program.Theme.ToString();
            if (ImGui.BeginCombo(TranslationSource.GetText("THEME"), TranslationSource.GetText(currentThemeName)))
            {
                foreach (var colorTheme in ThemeHandler.Themes)
                {
                    string themeName = colorTheme.Name;
                    string name = TranslationSource.GetText(themeName);
                    bool selected = themeName == currentThemeName;
                    if (ImGui.Selectable(name, selected))
                    {
                        //Set the current theme instance
                        ThemeHandler.UpdateTheme(colorTheme);

                        Settings.Program.Theme = colorTheme.Name;
                        Settings.Save();
                    }
                    if (selected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
        }

        private void DrawAppearance()
        {
            bool updateSettings = false;
            var clrFlags = ImGuiColorEditFlags.NoInputs;

            if (ImGui.CollapsingHeader($"{TranslationSource.GetText("THEME")}##theme_header", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawThemeSettings();
            }
            if (ImGui.CollapsingHeader(TranslationSource.GetText("BACKGROUND"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                updateSettings |= ImGui.Checkbox($"{TranslationSource.GetText("DISPLAY")}##bgdsp", ref DrawableBackground.Display);
                updateSettings |= ImGui.ColorEdit3(TranslationSource.GetText("COLOR_TOP"), ref DrawableBackground.BackgroundTop, clrFlags);
                updateSettings |= ImGui.ColorEdit3(TranslationSource.GetText("COLOR_BOTTOM"), ref DrawableBackground.BackgroundBottom, clrFlags);
            }
            if (ImGui.CollapsingHeader(TranslationSource.GetText("GRID"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                updateSettings |= ImGui.Checkbox($"{TranslationSource.GetText("DISPLAY")}##gdsp", ref DrawableGridFloor.Display);
                updateSettings |= ImGui.Checkbox(TranslationSource.GetText("SOLID"), ref DrawableInfiniteFloor.IsSolid);
                updateSettings |= ImGui.ColorEdit4(TranslationSource.GetText("COLOR"), ref DrawableGridFloor.GridColor, clrFlags);
                updateSettings |= ImGui.InputInt(TranslationSource.GetText("GRID_COUNT"), ref DrawableGridFloor.CellAmount);
                updateSettings |= ImGui.InputFloat(TranslationSource.GetText("GRID_SIZE"), ref DrawableGridFloor.CellSize);
            }
            if (ImGui.CollapsingHeader(TranslationSource.GetText("BONES"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                updateSettings |= ImGui.Checkbox($"{TranslationSource.GetText("DISPLAY")}##bdsp", ref Runtime.DisplayBones);
                updateSettings |= ImGui.InputFloat(TranslationSource.GetText("POINT_SIZE"), ref Runtime.BonePointSize);
            }
            if (ImGui.CollapsingHeader(TranslationSource.GetText("SHADOWS"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                updateSettings |= ImGui.Checkbox($"{TranslationSource.GetText("DISPLAY")}##shdsp", ref ShadowMainRenderer.Display);
                updateSettings |= ImGui.InputFloat(TranslationSource.GetText("SCALE"), ref ShadowBox.UnitScale);
                updateSettings |= ImGui.InputFloat(TranslationSource.GetText("DISTANCE"), ref ShadowBox.Distance);
#if DEBUG
                updateSettings |= ImGui.Checkbox(TranslationSource.GetText("DEBUG"), ref ShadowMainRenderer.DEBUG_QUAD);
#endif
            }
            if (updateSettings)
                UpdateSettings();
        }

        private void UpdateSettings()
        {
            if (GLContext.ActiveContext != null)
            {
                //Reload existing set values then save
                Settings.LoadCurrentSettings();
                Settings.Save();
                GLContext.ActiveContext.UpdateViewport = true;
            }
        }
    }
}
