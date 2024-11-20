using GLFrameworkEngine;
using ImGuiNET;
using MapStudio.UI;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Toolbox.Core.ViewModels;

namespace SampleMapEditor.LayoutEditor
{
    public class ObjectDataSelector
    {
        // Object and actor are used interchangeably here

        //public int GetSelectedID() => selectedObject;
        //public void SetSelectedID(int id) => selectedObject = id;

        public ByamlVector3F GetTranslate() => new ByamlVector3F(
            selectedObjectHash.Object.Transform.Position.X,
            selectedObjectHash.Object.Transform.Position.Y,
            selectedObjectHash.Object.Transform.Position.Z);

        public void SetSelectedObjectToUsed() => _selectedObject.HasBeenEditedWithSelector = true;

        public bool CloseOnSelect = false;

        public EventHandler SelectionChanged;

        //private bool filterSkybox = false;

        /*private int _selectedObject = 0;
        private int selectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }*/

        private EditableObjectNode _selectedObject = null;
        private EditableObjectNode selectedObjectHash
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string _searchText = "";
        private bool isSearch = false;
        private ObservableCollection<NodeBase> actorList; // objectList;
        private ObservableCollection<NodeBase> filteredActors; // filteredObjects;

        private List<ActorDefinition> ActorDB;

        private Dictionary<ulong, int> EntryNumberObject = new Dictionary<ulong, int>();

        public ObjectDataSelector(ObservableCollection<NodeBase> actors)
        {
            this.actorList = actors;
            this.ActorDB = GlobalSettings.ActorDatabase.Values.OrderBy(x => x.Name).ToList();

            Dictionary<string, int> EntryNumberCurrentObject = new Dictionary<string, int>();
            foreach (NodeBase actor in actors)
            {
                if (!EntryNumberCurrentObject.ContainsKey(actor.Header))
                {
                    EntryNumberCurrentObject.Add(actor.Header, 1);
                }

                EntryNumberObject.Add(((MuObj)((EditableObjectNode)actor).Object.UINode.Tag).Hash, EntryNumberCurrentObject[actor.Header]++);
            }
        }

        public void Render(bool isDialog = true)
        {
            //A search box for filtering objects
            RenderSearchBox();
            //Track the current placement
            var posY = ImGui.GetCursorPosY();
            //Filtered or full object list
            //var objects = (isSearch || filterSkybox) ? filteredObjects : objectList;
            var objects = isSearch ? filteredActors : actorList;

            var itemHeight = 40;
            var windowSize = ImGui.GetWindowSize();

            //Setup a child with the clip size calculations for clipping the list.
            ImGuiNative.igSetNextWindowContentSize(new System.Numerics.Vector2(0.0f, objects.Count * (itemHeight + 1)));
            ImGui.BeginChild("##object_list1", new Vector2(windowSize.X, windowSize.Y - posY - (isDialog ? 30 : 0)));
            //Draw object list
            RenderObjectList();

            ImGui.EndChild();

            //Setup cancel/ok buttons for dialog type.
            if (isDialog)
            {
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 202);

                bool cancel = ImGui.Button("Cancel", new Vector2(100, 23)); ImGui.SameLine();
                bool applied = ImGui.Button("Ok", new Vector2(100, 23)) && selectedObjectHash != null; //selectedObject != 0;

                if (cancel)
                {
                    DialogHandler.ClosePopup(false);
                }
                if (applied)
                {
                    DialogHandler.ClosePopup(true);
                }
            }
        }

        public void RenderSearchBox()
        {
            bool filterUpdate = false;
            /*if (ImGui.Checkbox(TranslationSource.GetText("FILTER_SKYBOXES"), ref filterSkybox))
                filterUpdate = true;*/

            //Search bar
            {
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Search");
                ImGui.SameLine();

                var posX = ImGui.GetCursorPosX();
                var width = ImGui.GetWindowWidth();

                //Span across entire outliner width
                ImGui.PushItemWidth(width - posX);
                if (ImGui.InputText("##search_box", ref _searchText, 200))
                {
                    isSearch = !string.IsNullOrWhiteSpace(_searchText);
                    filterUpdate = true;
                }
                ImGui.PopItemWidth();
            }

            if (filterUpdate)
                filteredActors = UpdateSearch(actorList);
        }

        public void RenderObjectList()
        {
            //var objects = (isSearch || filterSkybox) ? filteredObjects : objectList;
            var objects = isSearch ? filteredActors : actorList;
            var itemHeight = 40;

            var clipper = new ImGuiListClipper2(objects.Count, itemHeight);
            clipper.ItemsCount = objects.Count;

            //Setup list spacing
            var spacing = ImGui.GetStyle().ItemSpacing;
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(spacing.X, 0));

            //2 columns, one for name, another for ID
            ImGui.BeginColumns("##objListColumns", 1);  //  2);

            for (int line_i = clipper.DisplayStart; line_i < clipper.DisplayEnd; line_i++) // display only visible items
            {
                var mapObject = objects[line_i];
                //string resName = mapObject.ResNames.FirstOrDefault();

                //Get the icon
                string resName = "";

                try
                {
                    resName = ActorDB.First(x => x.Name == mapObject.Header).ResName;
                }
                catch (Exception e)
                {
                    e.ToString();
                    //Console.WriteLine(e.ToString());
                }

                //Get the icon
                var icon = IconManager.GetTextureIcon("Node");
                if (IconManager.HasIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\{resName}.png"))
                    icon = IconManager.GetTextureIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\{resName}.png");

                //Load the icon onto the list
                ImGui.Image((IntPtr)icon, new Vector2(itemHeight, itemHeight)); ImGui.SameLine();
                ImGuiHelper.IncrementCursorPosX(3);

                Vector2 itemSize = new Vector2(ImGui.GetWindowWidth(), itemHeight);

                //Selection handling
                //bool isSelected = selectedObject == mapObject.ObjId;
                bool isSelected = selectedObjectHash == (EditableObjectNode)mapObject;
                ImGui.AlignTextToFramePadding();

                if (mapObject.HasBeenEditedWithSelector)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.2f, 0.65f, 0.32f, 1.0f));
                }

                bool select = ImGui.Selectable($"{mapObject.Header} {EntryNumberObject[((MuObj)((EditableObjectNode)mapObject).Object.UINode.Tag).Hash]}##", isSelected, ImGuiSelectableFlags.SpanAllColumns, itemSize);

                if (mapObject.HasBeenEditedWithSelector)
                {
                    ImGui.PopStyleColor();
                }

                bool hovered = ImGui.IsItemHovered();
                ImGui.NextColumn();

                //Display object ID
                ImGui.AlignTextToFramePadding();
                //ImGui.Text($"{mapObject.Name}");
                ImGui.NextColumn();

                if (select)
                {
                    //Update selection
                    selectedObjectHash = (EditableObjectNode)mapObject;
                }
                if (CloseOnSelect && hovered && ImGui.IsMouseClicked(0))
                {
                    //Update selection
                    selectedObjectHash = (EditableObjectNode)mapObject;
                    DialogHandler.ClosePopup(true);
                }

                //if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && selectedObject != 0)
                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && selectedObjectHash != null)
                    DialogHandler.ClosePopup(true);
            }
            ImGui.EndColumns();

            ImGui.PopStyleVar();
        }

        private ObservableCollection<NodeBase> UpdateSearch(ObservableCollection<NodeBase> actors)
        {
            ObservableCollection<NodeBase> filtered = new ObservableCollection<NodeBase>();
            for (int i = 0; i < actors.Count; i++)
            {
                /*if (filterSkybox && !actors[i].VR)
                    continue;*/

                /*bool HasText = actors[i].Label != null &&
                     actors[i].Label.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;*/
                //HasText |= actors[i].ObjId.ToString().IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;


                bool HasText = actors[i].Header != null &&
                    actors[i].Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                //HasText |= actors[i].Name.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0; // Not necessary ???

                if (isSearch && HasText || !isSearch)
                    filtered.Add(actors[i]);
            }
            return filtered;
        }
    }
}

