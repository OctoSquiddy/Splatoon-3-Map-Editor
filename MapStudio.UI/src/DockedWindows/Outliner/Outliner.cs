using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.ViewModels;
using ImGuiNET;
using Toolbox.Core;
using OpenTK.Input;
using System.Numerics;
using System.Runtime.InteropServices;
using UIFramework;
using System.Collections;
using System.Reflection;
using static GLFrameworkEngine.RenderablePath;

namespace MapStudio.UI
{
    public class Outliner : DockWindow
    {
        public override string Name => "OUTLINER";

        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.MenuBar;

        public static IFileFormat ActiveFileFormat;

        public bool IsFocused = false;

        public List<MenuItemModel> ContextMenu = new List<MenuItemModel>();
        public List<MenuItemModel> FilterMenuItems = new List<MenuItemModel>();

        public List<NodeBase> Nodes = new List<NodeBase>();
        public List<NodeBase> SelectedNodes = new List<NodeBase>();

        public bool IsHashFilterMainToLinked = true;

        public NodeBase SelectedNode => SelectedNodes.LastOrDefault();

        public void AddSelection(NodeBase node) {
            SelectedNodes.Add(node);
            SelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        public void RemoveSelection(NodeBase node) {
            SelectedNodes.Remove(node);
            SelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        static NodeBase dragDroppedNode;

        const float RENAME_DELAY_TIME = 0.5f;
        const bool RENAME_ENABLE = false;

        /// <summary>
        /// Gets the currently dragged/dropped node from the outliner.
        /// If a node is dropped onto a control, this is used to get the data.
        /// </summary>
        /// <returns></returns>
        public static NodeBase GetDragDropNode()
        {
            return dragDroppedNode;
        }

        public EventHandler SelectionChanged;
        public EventHandler BeforeDrawCallback;

        public bool UseSelectionBox = true;

        //Rename handling
        private NodeBase renameNode;
        private bool isNameEditing = false;
        private string renameText;
        private double renameClickTime;

        public bool ShowWorkspaceFileSetting = true;

        public static bool AddToActiveWorkspace = true;

        public bool ClipNodes = false;

        //Search handling
        public bool ShowSearchBar = true;
        private bool isSearch = false;
        private string _searchText = "";

        private float ItemHeight;

        //Scroll handling
        public float ScrollX;
        public float ScrollY;

        bool updateScroll = false;

        private SelectionBox SelectionBox = new SelectionBox();

        private NodeBase focusedNode = null;
        private NodeBase previousSelectedNode;

        List<string> MainHashSelection = new List<string>();
        List<string> LinkedHashSelection = new List<string>();

        public Outliner(DockSpaceWindow parent) : base(parent)
        {
            SelectionChanged += delegate
            {
                if (SelectedNode == null)
                    return;

                if (SelectedNode.Tag is STBone) {
                    Runtime.SelectedBoneIndex = ((STBone)SelectedNode.Tag).Index;
                }
                else
                    Runtime.SelectedBoneIndex = -1;
                GLFrameworkEngine.GLContext.ActiveContext.UpdateViewport = true;
            };
            SelectionBox.OnSelectionStart += delegate {
                if (!ImGui.GetIO().KeyCtrl && !ImGui.GetIO().KeyShift)
                    DeselectAll();
            };
        }

        private bool isObjectHashInTheFilterOrWhatever(NodeBase node)
        {
            string hash = getHashByPropertyName(node.Tag, "Hash");
            return hash != "" && hash.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RecalculateMainHashFilter(NodeBase node)
        {
            string hashNode = getHashByPropertyName(node.Tag, "Hash");
            if (hashNode != "" && hashNode.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0) MainHashSelection.Add(hashNode);

            var children = node.Children.ToList();
            foreach (var child in children)
                RecalculateMainHashFilter(child);
        }

        private void RecalculateLinkedHashFilter(NodeBase node)
        {
            if (node.Tag != null)
            {
                string hashNode = getHashByPropertyName(node.Tag, "Hash");
                bool HasMainHash = MainHashSelection.Contains(hashNode);

                IList Links = new List<object>();
                PropertyInfo[] infos = node.Tag.GetType().GetProperties();
                foreach (PropertyInfo info in infos)
                {
                    if (info.Name == "Links")
                    {
                        Links = (IList)info.GetValue(node.Tag);
                        break;
                    }
                }

                // Get every dest hash for every object
                List<string> linkDestHash = new List<string>();
                foreach (object Link in Links)
                {
                    linkDestHash.Add(getHashByPropertyName(Link, "Dst"));
                }

                if (IsHashFilterMainToLinked && HasMainHash)
                {
                    foreach (string linkDest in linkDestHash)
                    {
                        if (!LinkedHashSelection.Contains(linkDest))
                        {
                            LinkedHashSelection.Add(linkDest);
                        }
                    }
                }
                else if (!LinkedHashSelection.Contains(hashNode) && linkDestHash.Any(linkDst => linkDst.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    LinkedHashSelection.Add(hashNode);
                }
            }

            var children = node.Children.ToList();
            foreach (var child in children)
                RecalculateLinkedHashFilter(child);
        }

        public void UpdateScroll(float scrollX, float scrollY)
        {
            ScrollX = scrollX;
            ScrollY = scrollY;
            updateScroll = true;
        }

        public void ScrollToSelected(NodeBase target)
        {
            if (target == null)
                return;

            //Do not scroll to displayed selected node
            if (SelectedNodes.Contains(target))
                return;

            //Expand parents if necessary
            target.ExpandParent();

            //Calculate position node is at.
            float pos = 0;
            foreach (var node in Nodes)
            {
                if (GetNodePosition(target, node, ref pos, ItemHeight))
                    break;
            }

            ScrollY = pos;
            updateScroll = true;
        }

        private bool GetNodePosition(NodeBase target, NodeBase parent, ref float pos, float itemHeight)
        {
            bool HasText = parent.Header != null &&
              parent.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            //Search is active and node is found but is not in results so skip scrolling
            if (isSearch && parent == target && !HasText)
                return false;
            //Node is found so return
            if (parent == target) {
                return true;
            }
            //Only update results for visible nodes
            if (isSearch && HasText || !isSearch)
                pos += itemHeight;
            if (parent.IsExpanded)
            {
                foreach (var child in parent.Children) {
                    if (GetNodePosition(target, child, ref pos, itemHeight))
                        return true;
                }
            }

            return false;
        }

        public override void Render()
        {
            BeforeDrawCallback?.Invoke(this, EventArgs.Empty);

            var width = ImGui.GetWindowWidth();
            var height = ImGui.GetWindowHeight();

            //For loading files into the existing workspace
            if (ShowWorkspaceFileSetting)
                ImGui.Checkbox($"Load files to active outliner.", ref AddToActiveWorkspace);

            if (ImGui.BeginMenuBar())
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4());

                if (ImGui.Button($"{IconManager.FILTER_ICON}")) {
                    ImGui.OpenPopup("filter1");
                }

                if (ImGui.BeginPopup("filter1"))
                {
                    foreach (var item in FilterMenuItems)
                        ImGuiHelper.LoadMenuItem(item);

                    List<string> HashFilterModes = new List<string>()
                    {
                        "Main   -> Linked",
                        "Linked -> Main"
                    };

                    int idx = IsHashFilterMainToLinked ? 0 : 1;

                    if (ImGui.BeginCombo($"###OutlinerHashFilterMode", HashFilterModes[idx], ImGuiComboFlags.HeightLarge))
                    {
                        for (int i = 0; i < HashFilterModes.Count; i++)
                        {
                            bool isSelected = i == idx;

                            if (ImGui.Selectable(HashFilterModes[i], isSelected))
                            {
                                IsHashFilterMainToLinked = i == 0;
                            }

                            if (isSelected)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }

                        ImGui.EndCombo();
                    }

                    ImGui.EndPopup();
                }
                ImGui.PopStyleColor();

                ImGuiHelper.IncrementCursorPosX(11);

                if (ShowSearchBar)
                {
                    ImGui.Text(IconManager.SEARCH_ICON.ToString());
                    ImGuiHelper.IncrementCursorPosX(11);

                    var posX = ImGui.GetCursorPosX();

                    //Span across entire outliner width
                    ImGui.PushItemWidth(width - posX);
                    if (ImGui.InputText("##search_box", ref _searchText, 200))
                    {
                        isSearch = !string.IsNullOrWhiteSpace(_searchText);
                    }
                    ImGui.PopItemWidth();
                }
                ImGui.EndMenuBar();
            }
            //Set the same header colors as hovered and active. This makes nav scrolling more seamless looking
            var active = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, active);
            ImGui.PushStyleColor(ImGuiCol.NavHighlight, new Vector4(0));

            ItemHeight = ImGui.GetTextLineHeightWithSpacing() + 3;

            //foreach (var node in this.Nodes)
            //  SetupNodes(node);

            var posY = ImGui.GetCursorPosY();

            ImGui.BeginChild("##window_space", new Vector2(width, height - posY - 5));
            bool isWindowFocused = ImGui.IsWindowFocused();
            bool isWindowHovered = ImGui.IsWindowHovered();
            float spacing = 0;

            //Put the entire control within a child
            if (ClipNodes)
            {
                int count = 0;
                foreach (var node in this.Nodes)
                    CalculateCount(node, ref count);

                ImGuiNative.igSetNextWindowContentSize(new System.Numerics.Vector2(0.0f, count * ItemHeight));
            }
            ImGui.BeginChild("##tree_view1", new Vector2(width - spacing, height - posY - 5));

            SelectionBox.Enabled = true;

            isWindowFocused |= ImGui.IsWindowFocused();
            isWindowHovered |= ImGui.IsWindowHovered();
            IsFocused = ImGui.IsWindowFocused();

            if (updateScroll)
            {
                ImGui.SetScrollX(ScrollX);
                ImGui.SetScrollY(ScrollY);
                updateScroll = false;
            }
            else
            {
                ScrollX = ImGui.GetScrollX();
                ScrollY = ImGui.GetScrollY();
            }

            // Recalc Hash filter
            MainHashSelection = new List<string>();
            LinkedHashSelection = new List<string>();
            if (isSearch)
            {
                foreach (var child in Nodes)
                    RecalculateMainHashFilter(child);

                foreach (var child in Nodes)
                    RecalculateLinkedHashFilter(child);
            }

            foreach (var child in Nodes)
                DrawNode(child, ItemHeight);

            //Don't apply selection box if any items are hovered
            if (ImGui.IsAnyItemHovered())
                SelectionBox.Enabled = false;

            //if (isSearch && Nodes.Count > 0)
            //   ImGui.TreePop();

            ImGui.EndChild();
            ImGui.PopStyleColor(2);

            ImGui.EndChild();

            //Make sure the selection tool must either have the window focused or hovered
            bool hasSelectionFocus = isWindowFocused || IsFocused || isWindowHovered;

            if (UseSelectionBox)
            {
                if (!isNameEditing && hasSelectionFocus)
                    SelectionBox.Render();
                else if (SelectionBox.IsActive && !hasSelectionFocus) //disable when window not focused
                    SelectionBox.Reset();
            }
        }

        private void SetupNodes(NodeBase node)
        {
            node.Visible = false;
            foreach (var c in node.Children)
                SetupNodes(c);
        }

        private void CalculateCount(NodeBase node, ref int counter)
        {
            bool HasText = node.Header != null &&
             node.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            if (isSearch && HasText || !isSearch)
            {
                node.DisplayIndex = counter;
                counter++;
            }

            if (!isSearch && !node.IsExpanded)
                return;

            foreach (var c in node.Children)
                CalculateCount(c, ref counter);
        }

        public void DeselectAll()
        {
            foreach (var node in SelectedNodes)
                node.IsSelected = false;
            SelectedNodes.Clear();
        }

        private string getHashByPropertyName(object tag, string propertyName)
        {
            string hash = "";
            if (tag != null)
            {
                PropertyInfo[] infos = tag.GetType().GetProperties();
                foreach (PropertyInfo info in infos)
                {
                    if (info.Name == propertyName)
                    {
                        hash = $"{info.GetValue(tag)}";
                        break;
                    }
                }
            }

            return hash;
        }

        public void DrawNode(NodeBase node, float itemHeight, int level = 0)
        {
            string hashNode = getHashByPropertyName(node.Tag, "Hash");

            bool HasText = node.Header != null &&
                node.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            bool HasHash = hashNode != "" && MainHashSelection.Contains(hashNode);
            bool HasLinkHash = hashNode != "" && LinkedHashSelection.Contains(hashNode);

            char icon = IconManager.FOLDER_ICON;
            if (node.Tag is STGenericMesh)
                icon = IconManager.MESH_ICON;
            if (node.Tag is STGenericModel)
                icon = IconManager.MODEL_ICON;
            if (node.Tag is STBone)
                icon = IconManager.BONE_ICON;
            if (!string.IsNullOrEmpty(node.Icon) && node.Icon.Length == 1)
                icon = (char)node.Icon[0];

            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
            flags |= ImGuiTreeNodeFlags.AllowItemOverlap;
            flags |= ImGuiTreeNodeFlags.SpanFullWidth;

            if (node.Children.Count == 0 || isSearch)
                flags |= ImGuiTreeNodeFlags.Leaf;
            else
            {
                flags |= ImGuiTreeNodeFlags.OpenOnDoubleClick;
                flags |= ImGuiTreeNodeFlags.OpenOnArrow;
            }

            if (node.IsExpanded && !isSearch) {
                //Flags for opening as default settings
                flags |= ImGuiTreeNodeFlags.DefaultOpen;
                //Make sure the "IsExpanded" can force the node to expand
                ImGui.SetNextItemOpen(true);
            }

            float currentPos = node.DisplayIndex;

            //Node was selected manually outside the outliner so update the list
            if (node.IsSelected && !SelectedNodes.Contains(node))
                AddSelection(node);

            //Node was deselected manually outside the outliner so update the list
            if (!node.IsSelected && SelectedNodes.Contains(node))
                RemoveSelection(node);

            if (SelectedNodes.Contains(node))
                flags |= ImGuiTreeNodeFlags.Selected;

            if ((isSearch && (HasHash || HasLinkHash || HasText)) || !isSearch)
            {
                //Add active file format styling. This determines what file to save.
                //For files inside archives, it gets the parent of the file format to save.               
                bool isActiveFile = Workspace.ActiveWorkspace.ActiveEditor == node.Tag;

                bool isRenaming = node == renameNode && isNameEditing;

                //Improve tree node spacing.
                var spacing = ImGui.GetStyle().ItemSpacing;
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(spacing.X, 1));

                //Align the text to improve selection sizing. 
                ImGui.AlignTextToFramePadding();

                //Disable selection view in renaming handler to make text more clear
                if (isRenaming)
                {
                    flags &= ~ImGuiTreeNodeFlags.Selected;
                    flags &= ~ImGuiTreeNodeFlags.SpanFullWidth;
                }

                //Load the expander or leaf tree node
                if (isSearch) {
                    if (ImGui.TreeNodeEx(node.ID, flags, $"")) { ImGui.TreePop(); }
                }
                else
                    node.IsExpanded = ImGui.TreeNodeEx(node.ID, flags, $"");

                node.Visible = true;

                ImGui.SameLine(); ImGuiHelper.IncrementCursorPosX(3);

                bool leftDoubleClicked = ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);
                bool leftClicked = ImGui.IsItemClicked(ImGuiMouseButton.Left);
                bool rightClicked = ImGui.IsItemClicked(ImGuiMouseButton.Right);
                bool isToggleOpened = ImGui.IsItemToggledOpen();
                bool beginDragDropSource = !isRenaming && node.Tag is IDragDropNode && ImGui.BeginDragDropSource();
                bool isChecked = false;

                //Do not activate selection box during a node hover
                if (!SelectionBox.IsActive && ImGui.IsItemHovered())
                    SelectionBox.Enabled = false;

                SelectionBox.CheckFrameSelection(node);

                //Force left/right click during a context menu popup
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup))
                {
                    //if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                   //     leftClicked = true;
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                        rightClicked = true;
                }

                bool nodeFocused = false;
                //Only check when the node focus is changed which gets activated during arrow keys
                //Imgui keeps IsItemFocused() on the node and only removes it during a left click
                if (ImGui.IsItemFocused() && focusedNode != node) {
                    focusedNode = node;
                    nodeFocused = true;
                }

                if (beginDragDropSource)
                {
                    //Placeholder pointer data. Instead use drag/drop nodes from GetDragDropNode()
                    GCHandle handle1 = GCHandle.Alloc(node.ID);
                    ImGui.SetDragDropPayload("OUTLINER_ITEM", (IntPtr)handle1, sizeof(int), ImGuiCond.Once);
                    handle1.Free();

                    dragDroppedNode = node;

                    //Display icon for texture types
                    if (node.Tag is STGenericTexture)
                        LoadTextureIcon(node);

                    //Display text for item being dragged
                    ImGui.Button($"{node.Header}");
                    ImGui.EndDragDropSource();
                }

                bool hasContextMenu = node is IContextMenu || node is IExportReplaceNode || node.Tag is ICheckableNode ||
                    node.Tag is IFileFormat ||
                    node.Tag is IContextMenu || node.Tag is IExportReplaceNode ||
                    node.Tag is STGenericTexture || node.ContextMenus.Count > 0;

                if (ContextMenu.Count > 0)
                    hasContextMenu = true;

                //Apply a pop up menu for context items. Only do this if the menu has possible items used
                if (hasContextMenu && SelectedNodes.Contains(node))
                {
                    ImGui.PushID(node.Header);

                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(8, 1));
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(8, 2));
                    ImGui.PushStyleColor(ImGuiCol.Separator, new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1.0f));

                    if (ImGui.BeginPopupContextItem("##OUTLINER_POPUP", ImGuiPopupFlags.MouseButtonRight))
                    {
                        SetupRightClickMenu(node);

                        ImGui.EndPopup();
                    }
                    ImGui.PopStyleVar(2);
                    ImGui.PopStyleColor(1);

                    ImGui.PopID();
                }

                if (node.HasCheckBox)
                {
                    ImGui.SetItemAllowOverlap();

                    ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2, 2));

                    bool check = node.IsChecked;
                    Vector2 size = new Vector2(ImGui.GetItemRectSize().Y);

                    if (ImguiCustomWidgets.EyeToggle($"{node.ID}check", ref check, size)) {
                        if (node.IsSelected)
                        {
                            foreach (var n in SelectedNodes)
                                n.IsChecked = check;
                        }
                        else
                        {
                            node.IsChecked = check;
                        }

                        GLFrameworkEngine.GLContext.ActiveContext.UpdateViewport = true;
                        isChecked = true;
                        leftClicked = false;
                        rightClicked = false;
                    }
                    if (ImGui.IsItemHovered())
                    {
                        leftClicked = false;
                        rightClicked = false;
                        nodeFocused = false;
                        leftDoubleClicked = false;
                    }

                    ImGui.PopStyleVar();

                    ImGui.SameLine();
                }

                //Load the icon
                node.IconDrawer?.Invoke(this, EventArgs.Empty);

                if (node.Tag is STGenericTexture) {
                    LoadTextureIcon(node);
                }
                else
                {
                    if (IconManager.HasIcon(node.Icon))
                    {
                        int iconID = IconManager.GetTextureIcon(node.Icon);

                        ImGui.AlignTextToFramePadding();
                        ImGui.Image((IntPtr)iconID, new System.Numerics.Vector2(22, 22));
                        ImGui.SameLine();
                    }
                    else
                    {
                        if (node.HasCheckBox)
                            ImGuiHelper.IncrementCursorPosX(5);

                        IconManager.DrawIcon(icon, node.IconColor);
                        ImGui.SameLine();
                        if (icon == IconManager.BONE_ICON)
                            ImGuiHelper.IncrementCursorPosX(5);
                        else
                            ImGuiHelper.IncrementCursorPosX(3);
                    }
                }

                ImGui.AlignTextToFramePadding();

                //if (node.Tag is ICheckableNode)
                //  ImGuiHelper.IncrementCursorPosY(-2);

                if (isSearch && (HasHash || HasLinkHash))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, HasHash && HasLinkHash ? new Vector4(1.0f, 0.0f, 0.835f, 1.0f) : HasHash ? new Vector4(0.0f, 1.0f, 0.106f, 1) : new Vector4(1.0f, 0.651f, 0.0f, 1.0f));
                }

                if (!isRenaming)
                {
                    //Make the active file noticable
                    if (isActiveFile)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, ThemeHandler.ActiveTextHighlight);
                        ImGui.Text(node.Header);
                        ImGui.PopStyleColor();
                    }
                    else
                        ImGui.Text(node.Header);
                }
                else
                {
                    var bg = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];

                    //Make the textbox frame background blend with the tree background
                    //This is so we don't see the highlight color and can see text clearly
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, bg);
                    ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1, 1, 1, 0.2f));
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2);

                    var length = ImGui.CalcTextSize(renameText).X + 20;
                    ImGui.PushItemWidth(length);

                    ImGuiHelper.IncrementCursorPosX(-4);

                    if (!ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
                        ImGui.SetKeyboardFocusHere(0);

                    void applyRename()
                    {
                        //Renamable tags
                        if (node.Tag is IRenamableNode)
                        {
                            var renamable = node.Tag as IRenamableNode;
                            renamable.Renamed(renameText);
                        }
                        node.Header = renameText;
                        node.OnHeaderRenamed?.Invoke(this, EventArgs.Empty);
                        node.ActivateRename = false;

                        isNameEditing = false;
                    }

                    if (ImGui.InputText("##RENAME_NODE", ref renameText, 512,
                          ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion |
                          ImGuiInputTextFlags.CallbackHistory | ImGuiInputTextFlags.NoHorizontalScroll |
                          ImGuiInputTextFlags.AutoSelectAll))
                    {
                        applyRename();
                    }
                    if (!ImGui.IsItemHovered() && (ImGui.IsMouseClicked(ImGuiMouseButton.Left)|| 
                                                  ImGui.IsMouseClicked(ImGuiMouseButton.Right)))
                    {
                        applyRename();
                    }

                    ImGui.PopItemWidth();
                    ImGui.PopStyleVar(2);
                    ImGui.PopStyleColor(2);
                }
                ImGui.PopStyleVar();

                if (isSearch && (HasHash || HasLinkHash))
                {
                    ImGui.PopStyleColor();
                }

                if (!isRenaming)
                {
                    //Check for rename selection on selected renamable node
                    if (node.IsSelected && node.CanRename)
                    {
                        bool renameStarting = renameClickTime != 0;
                        bool wasCancelled = false;

                        //Node forcefully renamed
                        if (node.ActivateRename)
                        {
                            //Name edit executed. Setup data for renaming.
                            isNameEditing = true;
                            renameText = node.Header;
                            renameNode = node;
                            //Reset the time
                            renameClickTime = 0;
                        }
                        else if (RENAME_ENABLE)
                        {

                            //Mouse click before editing started cancels the event
                            if (renameStarting && leftClicked)
                            {
                                renameClickTime = 0;
                                renameStarting = false;
                                wasCancelled = true;
                            }

                            //Check for delay
                            if (renameStarting)
                            {
                                //Create a delay between actions. This can be cancelled out during a mouse click
                                var diff = ImGui.GetTime() - renameClickTime;
                                if (diff > RENAME_DELAY_TIME)
                                {
                                    //Name edit executed. Setup data for renaming.
                                    isNameEditing = true;
                                    renameText = node.Header;
                                    renameNode = node;
                                    //Reset the time
                                    renameClickTime = 0;
                                }
                            }

                            //User has started a rename click. Start a time check
                            if (leftClicked && renameClickTime == 0 && !wasCancelled)
                            {
                                //Do a small delay for the rename event
                                renameClickTime = ImGui.GetTime();
                            }
                        }
                    }

                    //Deselect node during ctrl held when already selected
                    if (leftClicked && ImGui.GetIO().KeyCtrl && node.IsSelected)
                    {
                        RemoveSelection(node);
                        node.IsSelected = false;
                    }
                    //Click event executed on item
                    else if ((leftClicked || rightClicked) && !isToggleOpened && !isChecked) //Prevent selection change on toggle
                    {
                        //If a node has been clicked and is currently selected, we want to keep the current selection
                        bool isContextMenu = rightClicked && node.IsSelected;

                        //Reset all selection unless shift/control held down
                        if (!isContextMenu && !ImGui.GetIO().KeyCtrl && !ImGui.GetIO().KeyShift)
                        {
                            foreach (var n in SelectedNodes)
                                n.IsSelected = false;
                            SelectedNodes.Clear();
                        }

                        //Reset all selection unless shift/control held down
                        if (ImGui.GetIO().KeyShift)
                            SelectNodeRange(node);
                        else
                            previousSelectedNode = node;

                        //Add the clicked node to selection.
                        node.IsSelected = true;
                        AddSelection(node);
                    }  //Focused during a scroll using arrow keys
                    else if (nodeFocused && !isToggleOpened && !node.IsSelected)
                    {
                        if (!ImGui.GetIO().KeyCtrl && !ImGui.GetIO().KeyShift)
                        {
                            foreach (var n in SelectedNodes)
                                n.IsSelected = false;
                            SelectedNodes.Clear();
                        }

                        //Add the clicked node to selection.
                        AddSelection(node);
                        node.IsSelected = true;
                    }
                    //Expandable hiearchy from an archive file.
                    if (leftClicked && node.IsSelected)
                    {
                        if (node is ArchiveHiearchy && node.Tag == null)
                        {
                            var archiveWrapper = (ArchiveHiearchy)node;
                            archiveWrapper.OpenFileFormat();
                            archiveWrapper.IsExpanded = true;
                        }
                    }
                    //Double click event
                    if (leftDoubleClicked && !isToggleOpened && node.IsSelected) {
                        node.OnDoubleClicked();
                    }

                    //Update the active file format when selected. (updates dockspace layout and file menus)
                    if (node.Tag is IFileFormat && node.IsSelected)
                    {
                        if (ActiveFileFormat != node.Tag)
                            ActiveFileFormat = (IFileFormat)node.Tag;
                    }
                    else if (node.IsSelected && node.Parent != null)
                    {
                    }
                }
            }

            level++;
            if (isSearch || node.IsExpanded)
            {
                //Todo find a better alternative to clip parents
                //Clip only the last level. Don't clip more than 3 levels to prevent clipping issues.
                if (ClipNodes && node.Children.Count > 0 && node.Children[0].Children.Count == 0 && level < 3)
                {
                    var children = node.Children.ToList();
                    if (isSearch)
                        children = GetSearchableNodes(children);

                    var clipper = new ImGuiListClipper2(children.Count, itemHeight);
                    clipper.ItemsCount = children.Count;

                    for (int line_i = clipper.DisplayStart; line_i < clipper.DisplayEnd; line_i++) // display only visible items
                    {
                        DrawNode(children[line_i], itemHeight, level);
                    }
                }
                else
                {
                    var children = node.Children.ToList();
                    foreach (var child in children)
                        DrawNode(child, itemHeight, level);
                }

                if (!isSearch)
                    ImGui.TreePop();
            }
        }

        private void SelectNodeRange(NodeBase node)
        {
            if (previousSelectedNode == null || previousSelectedNode == node)
                return;

            bool isInRange = false;

            //Loop through all the tree nodes to select a range
            foreach (var n in Nodes)
                if (SelectNodeRange(n, previousSelectedNode, node, ref isInRange))
                    break;
        }

        private bool SelectNodeRange(NodeBase node, NodeBase selectedNode1, NodeBase selectedNode2, ref bool isInRange)
        {
            string hashNode = getHashByPropertyName(node.Tag, "Hash");

            bool HasText = node.Header != null &&
                node.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            bool HasHash = hashNode != "" && MainHashSelection.Contains(hashNode);
            bool HasLinkHash = hashNode != "" && LinkedHashSelection.Contains(hashNode);

            //Node has found proper range
            bool isHit = node == selectedNode1 || node == selectedNode2;
            //Node range has been fully reached
            if (isHit && isInRange)
            {
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            //Select nodes within range and filter
            if (isInRange && ((isSearch && (HasHash || HasLinkHash || HasText)) || !isSearch))
                node.IsSelected = true;

            //Range started so start selecting the nodes
            if (isHit)
                isInRange = true;

            if (node.IsExpanded)
            {
                foreach (var c in node.Children)
                    if (SelectNodeRange(c, selectedNode1, selectedNode2, ref isInRange))
                        return true;
            }
            return false;
        }

        private List<NodeBase> GetSearchableNodes(List<NodeBase> nodes)
        {
            List<NodeBase> nodeList = new List<NodeBase>();
            foreach (var node in nodes)
            {
                string hash = "";
                List<string> linkDestHash = new List<string>();

                if (node.Tag != null && node.Tag.GetType().GetProperty("Hash") != null)
                {
                    hash = $"{node.Tag.GetType().GetProperty("Hash").GetValue(node.Tag, null)}";
                }

                if (node.Tag != null && node.Tag.GetType().GetProperty("Links") != null)
                {
                    IList Links = (IList)node.Tag.GetType().GetProperty("Links").GetValue(node.Tag, null);

                    foreach (object Link in Links)
                    {
                        linkDestHash.Add($"{Link.GetType().GetProperty("Dst").GetValue(Link, null)}");
                    }
                }

                bool HasText = node.Header != null &&
                     node.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                bool HasHash = node.Tag != null && node.Tag.GetType().GetProperty("Hash") != null
                    && hash.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                bool HasLinkHash = node.Tag != null && node.Tag.GetType().GetProperty("Links") != null
                    && linkDestHash.Any(Link => Link.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0);

                if (HasHash || HasLinkHash || HasText)
                    nodeList.Add(node);
            }
            return nodeList;
        }

        private void LoadTextureIcon(NodeBase node)
        {
            if (((STGenericTexture)node.Tag).RenderableTex == null)
                ((STGenericTexture)node.Tag).LoadRenderableTexture();

            //Render textures loaded in GL as an icon
            if (((STGenericTexture)node.Tag).RenderableTex != null)
            {
                var tex = ((STGenericTexture)node.Tag);
                //Turn the texture to a cached icon
                IconManager.DrawTexture(node.Header, tex);
                ImGui.SameLine();
            }
        }

        private void SetupRightClickMenu(NodeBase node)
        {
            if (node.Tag is IFileFormat && ((IFileFormat)node.Tag).CanSave)
            {
                ImGui.AlignTextToFramePadding();
                if (ImGui.Selectable(TranslationSource.GetText("SAVE")))
                {
                    UIManager.ActionExecBeforeUIDraw += delegate {
                        Workspace.ActiveWorkspace.SaveFileWithDialog((IFileFormat)node.Tag);
                    };
                }
                string filePath = ((IFileFormat)node.Tag).FileInfo.FilePath;
                if (System.IO.File.Exists(filePath))
                {
                    ImGui.AlignTextToFramePadding();
                    if (ImGui.Selectable(TranslationSource.GetText("OPEN_IN_EXPLORER")))
                        FileUtility.SelectFile(filePath);
                }
            }

            foreach (var menuItem in node.ContextMenus)
            {
                ImGuiHelper.LoadMenuItem(menuItem);
            }

            foreach (var menuItem in ContextMenu)
            {
                ImGuiHelper.LoadMenuItem(menuItem);
            }

            if (node.Tag is ICheckableNode)
            {
                var checkable = (ICheckableNode)node.Tag;
                if (ImGui.Selectable("Enable"))
                {
                    checkable.OnChecked(true);
                }
                if (ImGui.Selectable("Disable"))
                {
                    checkable.OnChecked(false);
                }
            }
            if (node.Tag is IExportReplaceNode)
            {
                var exportReplaceable = (IExportReplaceNode)node.Tag;

                if (ImGui.Selectable("Export"))
                {
                    var dialog = new ImguiFileDialog();
                    dialog.FileName = node.Header;

                    dialog.SaveDialog = true;
                    foreach (var filter in exportReplaceable.ExportFilter)
                        dialog.AddFilter(filter);
                    if (dialog.ShowDialog($"{node.GetType()}export"))
                    {
                        exportReplaceable.Export(dialog.FilePath);
                    }
                }
                if (ImGui.Selectable("Replace"))
                {
                    var dialog = new ImguiFileDialog();
                    foreach (var filter in exportReplaceable.ReplaceFilter)
                        dialog.AddFilter(filter);
                    if (dialog.ShowDialog($"{node.GetType()}replace"))
                    {
                        exportReplaceable.Replace(dialog.FilePath);
                    }
                }
                ImGui.Separator();
            }
            if (node.Tag is IContextMenu)
            {
                var contextMenu = (IContextMenu)node.Tag;
                var menuItems = contextMenu.GetContextMenuItems();
                foreach (var item in menuItems)
                    LoadMenuItem(item);

                ImGui.Separator();
            }
        }

        private void LoadMenuItem(MenuItemModel item)
        {
            if (string.IsNullOrEmpty(item.Header)) {
                ImGui.Separator();
                return;
            }

            ImGui.AlignTextToFramePadding();

            if (item.MenuItems.Count > 0)
            {
                var hovered = ImGui.IsItemHovered();

                if (ImGui.BeginMenu($"   {item.Header}"))
                {
                    foreach (var c in item.MenuItems)
                        LoadMenuItem(c);

                    ImGui.EndMenu();
                }
            }
            else
            {
                if (ImGui.Selectable($"   {item.Header}")) {

                    UIManager.ActionExecBeforeUIDraw = () =>
                    {
                        item.Command?.Execute(item);
                    };
                }
            }
        }

        private void DeselectAll(NodeBase node)
        {
            node.IsSelected = false;
            foreach (var c in node.Children)
                DeselectAll(c);
        }
    }
}
