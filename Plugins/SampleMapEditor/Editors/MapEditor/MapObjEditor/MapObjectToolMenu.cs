using CafeLibrary.Rendering;
using GLFrameworkEngine;
using ImGuiNET;
using IONET.Collada.FX.Rendering;
using MapStudio.UI;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;

namespace SampleMapEditor.LayoutEditor
{
    public class MapObjectToolMenu : IToolWindowDrawer
    {
        ObjectEditor Editor;
        public List<ObjectEditor> Editors = new List<ObjectEditor>();

        int LayerSelectorIndex = 0;

        List<string> LayerInternalNames = new List<string>() { "None", "Cmn", "Pnt", "Var", "Vlf", "Vgl", "Vcl", "Tcl", "Low", "Mid", "High", "Day",
                "DayOrSuntet", "Sunset", "Night", "FestFirstHalf", "FestSecondHalf", "FestAnnounce", "FestWaitingForResult",
                "FestNone", "BigRun", "Sound", "Area01", "Area02", "Area03", "Area04", "Area05", "Area06" };

        List<string> LayerExternalNames = new List<string> { "None", "Common", "Turf War", "Splat Zones", "Tower Control", "Rainmaker", "Clam Blitz",
                "Tricolor Turf War", "Salmon Run Low Tide", "Salmon Run Mid Tide", "Salmon Run High Tide", "Day",
                "Either Day or Sunset", "Sunset", "Night", "First Half of the Splatfest", "Second Half of the Splatfest",
                "Splatfest Announcement", "Splatfest Waiting for Results", "Outside of Splatfest", "Big Run", "Sound",
                "Future Utopia Island", "Cozy & Safe Factory", "Cryogenic Hopetown", "Landfill Dreamland",
                "Eco-Forest Treehills", "Happiness Research Lab" };

        public MapObjectToolMenu(ObjectEditor editor, StageLayoutPlugin MapEditor)
        {
            // Get this singled out for editor specifics
            Editor = editor;

            // Get every object editors in a list
            Editors = new List<ObjectEditor>();

            foreach (ILayoutEditor mEditor in MapEditor.Editors)
            {
                if (mEditor is ObjectEditor)
                {
                    Editors.Add((ObjectEditor)mEditor);
                }
            }
        }

        public void Render()
        {
            var toolMenus = Editor.GetToolMenuItems();
            var editMenus = Editor.GetEditMenuItems();

            var size = new System.Numerics.Vector2(150, 22);

            if (ImGui.CollapsingHeader("Tools", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Text("Filter to layer");

                ImGui.SameLine();

                ImGui.PushItemWidth(ImGui.GetWindowSize().X - ImGui.CalcTextSize("Filter to layer").X - 10);

                if (ImGui.BeginCombo($"###LinkSelectorCBox.Name", LayerExternalNames[Editor.LayerSelectorIndex], ImGuiComboFlags.HeightLarge))
                {
                    for (int i = 0; i < LayerExternalNames.Count; i++)
                    {
                        bool isSelected = Editor.LayerSelectorIndex == i;

                        if (ImGui.Selectable($"{LayerExternalNames[i]}###LayerSelector{i}", isSelected))
                        {
                            // Assign new layer to display to editors
                            for (int e = 0; e < Editors.Count; e++)
                            {
                                Editors[e].LayerSelectorIndex = i;

                                var selected = Editors[e].Root.Children;
                                if (selected.Count != 0)
                                {
                                    foreach (EditableObjectNode obj in selected)
                                    {
                                        string Layer = ((MuObj)obj.Object.UINode.Tag).Layer;

                                        switch (Layer)
                                        {
                                            case "None":
                                            case "Cmn":
                                            case "Sound":
                                                if (obj.Object is BfresRender)
                                                    obj.IsChecked = true;
                                                break;

                                            default:
                                                if (obj.Object is BfresRender)
                                                {
                                                    obj.IsChecked = Layer == LayerInternalNames[Editor.LayerSelectorIndex];
                                                }
                                                else
                                                {
                                                    if (obj.IsChecked)
                                                    {
                                                        obj.IsChecked = Layer == LayerInternalNames[Editor.LayerSelectorIndex];
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopItemWidth();

                if (ImGui.Checkbox("Hide objects with no model", ref Editor.ObjectNoModelHide))
                {
                    for (int e = 0; e < Editors.Count; e++)
                    {
                        Editors[e].ObjectNoModelHide = Editor.ObjectNoModelHide;

                        var selected = Editors[e].Root.Children;

                        if (selected.Count != 0)
                        {
                            foreach (EditableObjectNode obj in selected)
                            {
                                if (!(obj.Object is BfresRender))
                                {
                                    obj.IsChecked = !Editors[e].ObjectNoModelHide;
                                }
                            }
                        }
                    }
                }

                

                if (ImGui.Checkbox("Display Sub Models", ref Editor.ObjectSubModelDisplay))
                {
                    for (int e = 0; e < Editors.Count; e++)
                    {
                        Editors[e].ObjectSubModelDisplay = Editor.ObjectSubModelDisplay;

                        var selected = Editors[e].Root.Children;

                        if (selected.Count != 0)
                        {
                            foreach (EditableObjectNode obj in selected)
                            {
                                if (obj.Object is BfresRender)
                                {
                                    List<string> subModelNames = GlobalSettings.ActorDatabase[obj.Header].SubModels;

                                    foreach (var model in ((BfresRender)obj.Object).Models)
                                    {
                                        if (subModelNames.Contains(model.Name))
                                            model.IsVisible = Editor.ObjectSubModelDisplay;
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var menu in toolMenus)
                {
                    if (ImGui.Button(menu.Header, size))
                    {
                        menu.Command.Execute(menu);
                    }
                }
            }
            if (editMenus.Count > 0 && ImGui.CollapsingHeader("Actions", ImGuiTreeNodeFlags.DefaultOpen))
            {
                foreach (var menu in editMenus)
                {
                    if (ImGui.Button(menu.Header, size))
                    {
                        menu.Command.Execute(menu);
                    }
                }
            }

            if (ImGui.CollapsingHeader($"Actions on Selected Layer ({LayerExternalNames[Editor.LayerSelectorIndex]})###ActionSelectedLayer", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###DeleteLayerObjects", size))
                {
                    Editor.MapEditor.Scene.BeginUndoCollection();
                    Editor.RemoveByLayer(LayerInternalNames[Editor.LayerSelectorIndex]);
                    Editor.MapEditor.Scene.EndUndoCollection();
                }
            }
        }
    }
}
