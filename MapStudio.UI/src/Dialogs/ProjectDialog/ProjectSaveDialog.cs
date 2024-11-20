using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using MapStudio.UI;

namespace MapStudio.UI
{
    public class ProjectSaveDialog
    {
        string ProjectName;

        public string GetProjectDirectory()
        {
            var settings = GlobalSettings.Current;
            return Path.Combine(settings.Program.ProjectDirectory,ProjectName);
        }

        public ProjectSaveDialog(string name) {
            ProjectName = name;
        }

        public void LoadUI()
        {
            var settings = GlobalSettings.Current;

            ImGui.InputText(TranslationSource.GetText("PROJECT_NAME"), ref ProjectName, 100);
            ImguiCustomWidgets.PathSelector(TranslationSource.GetText("PROJECT_FOLDER"), ref settings.Program.ProjectDirectory);

            var cancel = ImGui.Button(TranslationSource.GetText("CANCEL")); ImGui.SameLine();
            var save = ImGui.Button(TranslationSource.GetText("SAVE"));
            if (cancel)
                DialogHandler.ClosePopup(false);

            if (save && !string.IsNullOrEmpty(ProjectName)) {
                DialogHandler.ClosePopup(true);
            }
        }
    }
}
