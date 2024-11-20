using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using MapStudio.UI;
using System.IO;

namespace MapStudio.UI
{
    public class ProjectList
    {
        public List<string> Projects = new List<string>();
        public List<string> FilteredProjects = new List<string>();

        public string SelectedProject { get; set; }

        private bool isSearch = false;
        private string _searchText = "";

        public ProjectList() { Init(); }

        public void Init()
        {
            Projects = Directory.GetDirectories(GlobalSettings.Current.Program.ProjectDirectory).ToList();
        }

        public void Render()
        {
            ImGui.BeginTabBar("projectTab");
            if (ImguiCustomWidgets.BeginTab("projectTab", "Projects"))
            {
                var width = ImGui.GetWindowWidth();
                if (ImGui.Button($"  {IconManager.DELETE_ICON}   "))
                {
                    string projectName = new DirectoryInfo(SelectedProject).Name;
                    int result = TinyFileDialog.MessageBoxInfoYesNo(string.Format("Are you sure you want to delete {0}?", projectName));
                    if (result == 1)
                        RemoveProject(SelectedProject);
                }
                ImGui.SameLine();

                //Search bar
                {
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text(TranslationSource.GetText("SEARCH"));
                    ImGui.SameLine();

                    var posX = ImGui.GetCursorPosX();

                    //Span across entire outliner width
                    ImGui.PushItemWidth(width - posX);
                    if (ImGui.InputText("##search_box", ref _searchText, 200))
                    {
                        isSearch = !string.IsNullOrWhiteSpace(_searchText);

                        FilteredProjects.Clear();
                        foreach (var project in Projects)
                        {
                            string name = new DirectoryInfo(project).Name;
                            if (name.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                                FilteredProjects.Add(project);
                        }
                    }
                    ImGui.PopItemWidth();
                }

                ImGui.BeginChild("projectChild");

                var projects = isSearch ? FilteredProjects : Projects;
                foreach (var dir in projects) {
                    DisplayProject(dir);
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }

        private void RemoveProject(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return;

            //Remove project directory
            FileUtility.DeleteFolder(folder);
            //Reload project list
            Projects = Directory.GetDirectories(GlobalSettings.Current.Program.ProjectDirectory).ToList();
        }

        private void DisplayProject(string folder)
        {
            string thumbFile = Path.Combine(folder,"Thumbnail.png");
            string projectFile = Path.Combine(folder,"Project.json");
            string projectName = new DirectoryInfo(folder).Name;

            if (!File.Exists(projectFile))
                return;

            var icon = IconManager.GetTextureIcon("BLANK");
            if (File.Exists(thumbFile))
            {
                IconManager.LoadTextureFile(thumbFile, 64, 64);
                icon = IconManager.GetTextureIcon(thumbFile);
            }

            //Make the whole menu item region selectable
            var width = ImGui.GetWindowWidth();
            var size = new System.Numerics.Vector2(width, 64);

            bool isSelected = SelectedProject == folder;

            var pos = ImGui.GetCursorPos();
            if (ImGui.Selectable($"##{folder}", isSelected, ImGuiSelectableFlags.None, size))
                SelectedProject = folder;

            var doubleClicked = ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0);

            ImGui.SetCursorPos(pos);

            //Load an icon preview of the project
            ImGui.AlignTextToFramePadding();
            ImGui.Image((IntPtr)icon, new System.Numerics.Vector2(64, 64));
            //Project name
            ImGui.SameLine();

            var textpos = ImGui.GetCursorPos();
            ImGui.Text(projectName);

            var endPos = ImGui.GetCursorPos();

            ImGui.SameLine();

            ImGui.SetCursorPos(textpos);
            ImGuiHelper.IncrementCursorPosY(30);

            ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), folder);


            ImGui.SetCursorPos(endPos);

            //Load file when clicked on
            if (doubleClicked) {
                DialogHandler.ClosePopup(true);
            }
        }
    }
}
