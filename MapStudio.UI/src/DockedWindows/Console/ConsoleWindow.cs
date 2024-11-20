using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using ImGuiNET;
using MapStudio.UI;
using Toolbox.Core;
using UIFramework;

namespace MapStudio.UI
{
    public class ConsoleWindow : DockWindow
    {
        public override string Name => "CONSOLE";

        bool displayErrors = true;
        bool displayWarnings = true;
        bool displayInfo = true;

        public ConsoleWindow(DockSpaceWindow parent) : base(parent)
        {

        }

        public override void Render()
        {
            ImGui.Checkbox(TranslationSource.GetText("ERRORS"), ref displayErrors); ImGui.SameLine();
            ImGui.Checkbox(TranslationSource.GetText("WARNINGS"), ref displayWarnings); ImGui.SameLine();
            ImGui.Checkbox(TranslationSource.GetText("MESSAGES"), ref displayInfo); ImGui.SameLine();
            if (ImGui.Button(TranslationSource.GetText("COPY")))
            {
                string text = "";
                if (displayErrors) text += StudioLogger.GetErrorLog();
                if (displayWarnings) text += StudioLogger.GetWarningLog();
                if (displayInfo) text += StudioLogger.GetLog();

                ImGui.SetClipboardText(text);
            }
            ImGui.SameLine();
            if (ImGui.Button("Check Errors"))
            {
                StudioLogger.ResetErrors();
                Workspace.ActiveWorkspace.PrintErrors();
            }

            var color = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
            ImGui.PushStyleColor(ImGuiCol.ChildBg, color);

            ImGui.BeginChild("consoleWindow");

            //Add in transform info wne
            //Todo find a better place for this
            var info = GLFrameworkEngine.GLContext.ActiveContext.TransformTools.GetTextInput();
            if (!string.IsNullOrEmpty(info))
                WriteText(info);

            if (displayErrors) WriteText(StudioLogger.GetErrorLog(), ThemeHandler.Theme.Error);
            if (displayWarnings) WriteText(StudioLogger.GetWarningLog(), ThemeHandler.Theme.Warning);
            if (displayInfo) WriteText(StudioLogger.GetLog());

            ImGui.EndChild();

            ImGui.PopStyleColor();
        }

        private void WriteText(string text)
        {
            ImGui.TextUnformatted(text);
        }

        private void WriteText(string text, Vector4 color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.TextWrapped(text);
            ImGui.PopStyleColor();
        }
    }
}
