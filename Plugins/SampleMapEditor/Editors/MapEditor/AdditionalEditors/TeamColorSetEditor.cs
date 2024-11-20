using GLFrameworkEngine;
using MapStudio.UI;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.ViewModels;
using ImGuiNET;
using Toolbox.Core;

namespace SampleMapEditor.LayoutEditor
{
    public class TeamColorSetEditor : ILayoutEditor
    {
        public MuTeamColorDataSet TeamColorDataSet { get; set; }

        public string Name => "TeamColor Editor";

        public StageLayoutPlugin MapEditor { get; set; }

        public IToolWindowDrawer ToolWindowDrawer { get; }

        public List<IDrawable> Renderers { get; set; }

        public NodeBase Root { get; set; }

        public List<MenuItemModel> MenuItems { get; set; }

        public bool IsActive { get; set; }

        public TeamColorSetEditor(StageLayoutPlugin editor, MuTeamColorDataSet mTeamColorDataSet)
        {
            MapEditor = editor;
            TeamColorDataSet = mTeamColorDataSet;

            Root = new NodeBase(Name);
            Root.Icon = $"{IconManager.SETTINGS_ICON}";
            Root.IconColor = new System.Numerics.Vector4(0.0f, 1.0f, 0.753f, 1.0f);

            Root.TagUI.UIDrawer += delegate
            {
                // Loader
                string path = TeamColorDataSet.FileName;
                if (ImguiCustomWidgets.FileSelector("Team Color File", ref path, new string[1] { ".zs" }, TeamColorDataSet.IsFileFound))
                {
                    if (TeamColorDataSet.IsFileFound && path != TeamColorDataSet.FileName)
                    {
                        MapEditor.MapLoader.stageDefinition.TryLoadColorDataSetFile(path);
                        TeamColorDataSet = MapEditor.MapLoader.stageDefinition.TeamColorDataSet;
                        ReloadEntries(TeamColorDataSet.ColorDataArray);
                    }
                }

                float AvailableWidth = ImGui.GetContentRegionAvail().X;

                if (ImGui.Button("Save###SaveTeamColorSet", new System.Numerics.Vector2(AvailableWidth / 2.0f - 5.0f, 20.0f)))
                {
                    MapEditor.MapLoader.stageDefinition.SaveColorDataSetFile();
                    TeamColorDataSet = MapEditor.MapLoader.stageDefinition.TeamColorDataSet;
                    ReloadEntries(TeamColorDataSet.ColorDataArray);
                }

                ImGui.SameLine();

                if (ImGui.Button("Save As###SaveAsTeamColorSet", new System.Numerics.Vector2(AvailableWidth / 2.0f - 5.0f, 20.0f)))
                {
                    var dialog = new ImguiFileDialog();
                    dialog.SaveDialog = true;

                    dialog.AddFilter(".zs", "Team Color File");
                    if (dialog.ShowDialog())
                    {
                        TeamColorDataSet.FileName = dialog.FilePath;
                        MapEditor.MapLoader.stageDefinition.SaveColorDataSetFile();
                        TeamColorDataSet = MapEditor.MapLoader.stageDefinition.TeamColorDataSet;
                        ReloadEntries(TeamColorDataSet.ColorDataArray);
                    }
                }

                bool isChecked = TeamColorDataSet.IsZSTDCompressed;
                if (ImGui.Checkbox("The file is zstd compressed###TeamColorZSTDSet", ref isChecked))
                {
                    TeamColorDataSet.IsZSTDCompressed = isChecked;
                }

                string Name = "";
                if (ImGui.CollapsingHeader("Color Entries", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    for (int i = 0; i < TeamColorDataSet.ColorDataArray.Count; i++)
                    {
                        if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###ColorEntries.{i}"))
                        {
                            if (TeamColorDataSet.ColorDataArray.Count > 0)
                                TeamColorDataSet.ColorDataArray.RemoveAt(i);

                            ReloadEntries(TeamColorDataSet.ColorDataArray);
                        }

                        ImGui.SameLine();

                        ImGui.PushItemWidth(ImGui.GetWindowSize().X - ImGui.GetCursorPosX());

                        Name = TeamColorDataSet.ColorDataArray[i].__RowId;
                        if (ImGui.InputText($"###ColorEntriesName.{i}", ref Name, 0x100000))
                        {
                            TeamColorDataSet.ColorDataArray[i].__RowId = Name;
                            ReloadEntries(TeamColorDataSet.ColorDataArray);
                        }

                        ImGui.PopItemWidth();
                    }
                }
            };

            ReloadEntries(mTeamColorDataSet.ColorDataArray);
            Renderers = new List<IDrawable>();
        }

        public void ReloadEntries(List<MuTeamColorDataSet.MuTeamColorData> colorDataArray)
        {
            if (colorDataArray != null)
            {
                Root.Children.Clear();
                foreach (MuTeamColorDataSet.MuTeamColorData ColorData in colorDataArray)
                    CreateEntry(ColorData);
            }
        }

        public void CreateEntry(MuTeamColorDataSet.MuTeamColorData ColorData)
        {
            NodeBase ColorNode = new NodeBase(ColorData.__RowId);
            ColorNode.Icon = $"{IconManager.SETTINGS_ICON}";
            ColorNode.IconColor = new System.Numerics.Vector4(0.0f, 1.0f, 0.753f, 1.0f);

            ColorNode.Tag = ColorData;

            ColorNode.TagUI.UIDrawer += delegate
            {
                ImGui.Text($"Tag: {ColorData.Tag}");
            };

            Root.AddChild(ColorNode);
        }
















        public void DrawEditMenuBar()
        {

        }

        public void DrawHelpWindow()
        {

        }

        public List<NodeBase> GetSelected()
        {
            return null;
        }

        public void OnKeyDown(KeyEventInfo keyInfo)
        {

        }

        public void OnMouseDown(MouseEventInfo mouseInfo)
        {

        }

        public void OnMouseUp(MouseEventInfo mouseInfo)
        {

        }

        public void OnSave(StageDefinition stage)
        {

        }

        public void ReloadEditor()
        {

        }

        public void RemoveSelected()
        {

        }
    }
}
