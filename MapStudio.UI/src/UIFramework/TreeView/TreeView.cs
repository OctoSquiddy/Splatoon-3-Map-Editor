using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using MapStudio.UI;

namespace UIFramework
{
    public class TreeView
    {
        /// <summary>
        /// The root nodes of the tree view.
        /// </summary>
        public List<TreeNode> Nodes = new List<TreeNode>();

        /// <summary>
        /// The selected nodes of the tree view.
        /// </summary>
        protected readonly List<TreeNode> SelectedNodes = new List<TreeNode>();

        public List<TreeNode> GetSelectedNodes() => SelectedNodes;

        /// <summary>
        /// Determines if the tree has focus or not.
        /// </summary>
        public bool IsFocused = false;

        /// <summary>
        /// Determines to display the search box or not.
        /// </summary>
        public bool DisplaySearchBox = true;

        /// <summary>
        /// Determines to use the selection box or not.
        /// </summary>
        public bool UseSelectionBox = true;

        public EventHandler OnSelectionChanged;
        public EventHandler OnNodeChecked;
        public EventHandler OnNodeLeftClicked;

        public TreeNode GetDroppedTreeNode() => dragDroppedNode;

        public int SpacingAmount = 0;

        public int ColumnCount = 1;

        //Scroll handling
        protected float ScrollX;
        protected float ScrollY;

        public float GetScrollY() => ScrollY;

        public Func<TreeNode, bool> CanDisplayNode;

        //Filter operations
        private bool isSearch = false;
        private string _searchText = "";

        //Rename operations
        private TreeNode renameNode;
        private bool isNameEditing = false;
        private string _renameText = "";
        private double renameClickTime;

        //Constants
        const float RENAME_DELAY_TIME = 0.5f;
        const bool RENAME_ENABLE = false;

        //Drag/drop
        private TreeNode dragDroppedNode;

        //Selection tools
        private TreeNode previousSelectedNode = null;

        private SelectionBox SelectionBox = new SelectionBox();
        private bool previousFocus = false;

        private TreeNode focusedNode = null;

        private UPDATE_FLAGS UpdateFlags;

        public TreeView()
        {
            SelectionBox.OnSelectionStart += delegate {
                if (!ImGui.GetIO().KeyCtrl)
                    DeselectAll();
            };
        }

        public void AddSelection(TreeNode node) {
            if (SelectedNodes.Contains(node))
                return;

            node.IsSelected = true;
            SelectedNodes.Add(node);
            OnSelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        public void RemoveSelection(TreeNode node) {
            if (!SelectedNodes.Contains(node))
                return;

            node.IsSelected = false;
            SelectedNodes.Remove(node);
            OnSelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        public void DeselectAll()
        {
            var selected = SelectedNodes.ToList();
            foreach (var node in selected) {
                node.IsSelected = false;
                SelectedNodes.Remove(node);
            }
            OnSelectionChanged?.Invoke(null, EventArgs.Empty);
        }

        public void ScrollToSelected(TreeNode target)
        {
            if (target == null)
                return;

            //Do not scroll to displayed selected node
            if (SelectedNodes.Contains(target))
                return;

            //Expand parents if necessary
            target.ExpandParent();

            var itemHeight = ImGui.GetTextLineHeightWithSpacing() + 3;

            //Calculate position node is at.
            float pos = 0;
            foreach (var node in Nodes) {
                if (GetNodePosition(target, node, ref pos, itemHeight))
                    break;
            }

            ScrollY = pos;
            UpdateFlags |= UPDATE_FLAGS.SCROLL;
        }

        public void UpdateScroll(float scrollX, float scrollY)
        {
            ScrollX = scrollX;
            ScrollY = scrollY;
            UpdateFlags |= UPDATE_FLAGS.SCROLL;
        }

        public void Render()
        {
            var itemHeight = ImGui.GetTextLineHeightWithSpacing() + 3;

            if (DisplaySearchBox)
                DrawSearchBox();

            //Set the same header colors as hovered and active. This makes nav scrolling more seamless looking
            var active = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, active);
            ImGui.PushStyleColor(ImGuiCol.NavHighlight, new Vector4(0));

            var width = ImGui.GetWindowWidth();
            var height = ImGui.GetWindowHeight();
            var posY = ImGui.GetCursorPosY();

            ImGui.BeginChild("##window_space", new Vector2(width, height - posY - 5));
            bool isWindowFocused = ImGui.IsWindowFocused();
            bool isWindowHovered = ImGui.IsWindowHovered();

            //Put the entire control within a child
            if (ImGui.BeginChild("##tree_view1", new Vector2(width - SpacingAmount, height - posY - 5)))
            {
                IsFocused = ImGui.IsWindowFocused();
                SelectionBox.Enabled = true;
                if (ImGui.IsWindowHovered())
                    isWindowHovered = true;

                //Scroll to specified position
                if (UpdateFlags.HasFlag(UPDATE_FLAGS.SCROLL)) {
                    ImGui.SetScrollX(ScrollX);
                    ImGui.SetScrollY(ScrollY);
                }
                else {
                    //Store current scroll placements in tree
                    ScrollX = ImGui.GetScrollX();
                    ScrollY = ImGui.GetScrollY();
                }

                if (ColumnCount > 1)
                    ImGui.Columns(ColumnCount);

                foreach (var child in Nodes)
                    DrawNode(child, itemHeight);

                if (ColumnCount > 1)
                    ImGui.Columns(1);

                //Don't apply selection box if any items are hovered
                if (ImGui.IsAnyItemHovered())
                    SelectionBox.Enabled = false;
            }
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

            previousFocus = hasSelectionFocus;

            ImGui.EndChild();

            ImGui.PopStyleColor(2);

            UpdateFlags = UPDATE_FLAGS.NONE;
        }

        private void DrawSearchBox()
        {
            ImGuiHelper.IncrementCursorPosX(13);

            ImGui.Text(IconManager.SEARCH_ICON.ToString());
            ImGui.SameLine();

            ImGuiHelper.IncrementCursorPosX(11);

            var posX = ImGui.GetCursorPosX();
            var width = ImGui.GetWindowWidth();

            //Span across entire outliner width
            ImGui.PushItemWidth(width - posX - 5);
            if (ImGui.InputText("##search_box", ref _searchText, 200)) {
                isSearch = !string.IsNullOrWhiteSpace(_searchText);
            }
            ImGui.PopItemWidth();
        }

        private bool DisplayNode(TreeNode node)
        {
            if (CanDisplayNode != null) {
               return CanDisplayNode.Invoke(node);
            }
            return true;
        }

        private void DrawNode(TreeNode node, float itemHeight)
        {
            //Check if the node is within the necessary search filter requirements if search is used
            bool HasText = node.Header != null &&
                 node.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            //Node was selected manually outside the outliner so update the list
            if (node.IsSelected && !SelectedNodes.Contains(node))
                AddSelection(node);

            //Node was deselected manually outside the outliner so update the list
            if (!node.IsSelected && SelectedNodes.Contains(node))
                RemoveSelection(node);

            bool displayNode = DisplayNode(node);
            bool isVisible = (isSearch && HasText || !isSearch) && displayNode;
            if (isVisible)
                RenderNode(node, itemHeight);

            if (isSearch || node.IsExpanded)
            {
                foreach (var child in node.Children)
                    DrawNode(child, itemHeight);

                if (!isSearch && displayNode)
                    ImGui.TreePop();
            }
        }

        private void RenderNode(TreeNode node, float itemHeight)
        {
            var flags = PrepareNodeFlags(node);
            bool isRenaming = renameNode == node && isNameEditing;

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

            //Shift position after drawing expander element
            ImGui.SameLine(); ImGuiHelper.IncrementCursorPosX(3);

            //Node events
            bool leftDoubleClicked = ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);
            bool leftClicked = ImGui.IsItemClicked(ImGuiMouseButton.Left);
            bool rightClicked = ImGui.IsItemClicked(ImGuiMouseButton.Right);
            bool isToggleOpened = ImGui.IsItemToggledOpen();
            bool beginDragDropSource = !isRenaming && node.CanDragDrop && ImGui.BeginDragDropSource();
            bool isItemFocused = ImGui.IsItemFocused();

            //Force left/right click during a context menu popup
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup))
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    leftClicked = true;
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    rightClicked = true;
            }

            bool initiateRename = false;

            //Do not activate selection box during a node hover
            if (!SelectionBox.IsActive && ImGui.IsItemHovered())
                SelectionBox.Enabled = false;

            SelectionBox.CheckFrameSelection(node);

            if (beginDragDropSource)
                HandleDragDrop(node);

            bool hasContextMenu = node.ContextMenus.Count > 0;
            //Apply a pop up menu for context items. Only do this if the menu has possible items used
            if (hasContextMenu && SelectedNodes.Contains(node))
                HandleContextMenus(node);

            if (node.HasCheckBox)
                HandleCheckbox(node);

            TryDrawNodeIcon(node);

            ImGui.AlignTextToFramePadding();


            var textSize = ImGui.CalcTextSize(node.Header);
            var pos = ImGui.GetCursorScreenPos();

            var textRenameMin = new Vector2(pos.X - 5, pos.Y + 2);
            var textRenameMax = new Vector2(pos.X + textSize.X + 25, pos.Y + textSize.Y + 5);

            if (ImGui.IsMouseHoveringRect(textRenameMin, textRenameMax) && leftClicked)
                initiateRename = true;

            bool debug = false;
            if (debug)
            {
                ImGui.GetWindowDrawList().AddRect(
                   textRenameMin, textRenameMax,
                    ImGui.ColorConvertFloat4ToU32(new Vector4(1)));
            }

            bool nodeFocused = false;
            //Only check when the node focus is changed which gets activated during arrow keys
            //Imgui keeps IsItemFocused() on the node and only removes it during a left click
            if (isItemFocused && focusedNode != node)
            {
                focusedNode = node;
                nodeFocused = true;
            }

            if (isRenaming)
                DrawRenamingNode(node);
            else {
                if (node.RenderOverride != null)
                    node.RenderOverride.Invoke(this, EventArgs.Empty);
                else
                {
                    ImGui.Text(node.Header);
                    if (ColumnCount > 0) {
                        //Shift to each column. Requires a render override to make use of columns.
                        for (int i = 0; i < ColumnCount; i++)
                            ImGui.NextColumn();
                    }
                }
            }

            ImGui.PopStyleVar();

            if (!isRenaming)
            {
                //Check for rename selection on selected renamable node
                if (node.IsSelected && node.CanRename && !SelectionBox.IsActive)
                    HandleRenaming(node, initiateRename && !leftDoubleClicked);

                //Deselect node during ctrl held when already selected
                if (leftClicked && ImGui.GetIO().KeyCtrl && node.IsSelected)
                {
                    RemoveSelection(node);
                    node.IsSelected = false;
                }
                //Click event executed on item
                else if ((leftClicked || rightClicked) && !isToggleOpened) //Prevent selection change on toggle
                {
                    //Reset all selection unless shift/control held down
                    if (!ImGui.GetIO().KeyCtrl && !ImGui.GetIO().KeyShift)
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
                else if (nodeFocused && !isToggleOpened && !flags.HasFlag(ImGuiTreeNodeFlags.Selected))
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

                if (leftClicked && node.IsSelected)
                    OnNodeLeftClicked?.Invoke(node, EventArgs.Empty);

                //Double click event
                if (leftDoubleClicked && !isToggleOpened && node.IsSelected) {
                    node.OnDoubleClicked();
                }
            }
        }

        private void SelectNodeRange(TreeNode node)
        {
            if (previousSelectedNode == null || previousSelectedNode == node)
                return;

            bool isInRange = false;

            //Loop through all the tree nodes to select a range
            foreach (var n in Nodes)
                if (SelectNodeRange(n, previousSelectedNode, node, ref isInRange))
                    break;
        }

        private bool SelectNodeRange(TreeNode node, TreeNode selectedNode1, TreeNode selectedNode2, ref bool isInRange)
        {
            //Node has found proper range
            bool isHit = node == selectedNode1 || node == selectedNode2;
            //Node range has been fully reached
            if (isHit && isInRange) {
                OnSelectionChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }

            //Select nodes within range
            if (isInRange)
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

        private void HandleRenaming(TreeNode node, bool leftClicked)
        {
            //Node forcefully renamed
            if (node.ActivateRename)
            {
                //Name edit executed. Setup data for renaming.
                isNameEditing = true;
                _renameText = node.Header;
                renameNode = node;
                //Reset the time
                renameClickTime = 0;
                return;
            }
            if (!RENAME_ENABLE)
                return;

            bool renameStarting = renameClickTime != 0;
            bool wasCancelled = false;

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
                    _renameText = node.Header;
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

        private void DrawRenamingNode(TreeNode node)
        {
            var bg = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];

            //Make the textbox frame background blend with the tree background
            //This is so we don't see the highlight color and can see text clearly
            ImGui.PushStyleColor(ImGuiCol.FrameBg, bg);
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1, 1, 1, 0.2F));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1.5f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2);

            var length = ImGui.CalcTextSize(_renameText).X + 20;
            ImGui.PushItemWidth(length);

            ImGuiHelper.IncrementCursorPosX(-4);

            if (!ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
                ImGui.SetKeyboardFocusHere(0);

            void applyRename()
            {
                node.Header = _renameText;
                node.OnHeaderRenamed?.Invoke(this, EventArgs.Empty);

                node.ActivateRename = false;
                isNameEditing = false;
            }

            if (ImGui.InputText("##RENAME_NODE", ref _renameText, 512,
                ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion |
                ImGuiInputTextFlags.CallbackHistory | ImGuiInputTextFlags.NoHorizontalScroll |
                ImGuiInputTextFlags.AutoSelectAll))
            {
                applyRename();
            }

            if (!ImGui.IsItemHovered() && (ImGui.IsMouseClicked(ImGuiMouseButton.Left) ||
                                          ImGui.IsMouseClicked(ImGuiMouseButton.Right)))
            {
                applyRename();
            }

            ImGui.PopItemWidth();
            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor(2);

            if (ColumnCount > 1)
            {
                for (int i = 0; i < ColumnCount; i++)
                    ImGui.NextColumn();
            }
        }

        private void HandleCheckbox(TreeNode node)
        {
            ImGui.SetItemAllowOverlap();

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2, 2));

            bool check = node.IsChecked;

            if (ImGui.Checkbox($"##check{node.ID}", ref check))
            {
                foreach (var n in SelectedNodes)
                    n.IsChecked = check;
                this.OnNodeChecked?.Invoke(this, EventArgs.Empty);
            }
            ImGui.PopStyleVar();

            ImGui.SameLine();
        }

        private void HandleContextMenus(TreeNode node)
        {
            ImGui.PushID(node.Header);

            //Better menu visuals
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(8, 1));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(8, 2));
            ImGui.PushStyleColor(ImGuiCol.Separator, new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1.0f));

            if (ImGui.BeginPopupContextItem("##OUTLINER_POPUP", ImGuiPopupFlags.MouseButtonRight))
            {
                DrawRightClickMenu(node);
                ImGui.EndPopup();
            }
            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor(1);

            ImGui.PopID();
        }

        private void HandleDragDrop(TreeNode node)
        {
            SelectionBox.Enabled = false;

            //Placeholder pointer data. Instead use drag/drop nodes from GetDragDropNode()
            GCHandle handle1 = GCHandle.Alloc(node.ID);
            ImGui.SetDragDropPayload("OUTLINER_ITEM", (IntPtr)handle1, sizeof(int), ImGuiCond.Once);
            handle1.Free();

            dragDroppedNode = node;

            ImGuiHelper.IncrementCursorPosX(3);

            //Display icon
            ImGuiHelper.IncrementCursorPosX(4);

            ImGui.AlignTextToFramePadding();
            TryDrawNodeIcon(node, true);

            //Display text for item being dragged
            ImGui.AlignTextToFramePadding();
            ImGui.Text($"{node.Header}");
            ImGui.EndDragDropSource();
        }

        private void TryDrawNodeIcon(TreeNode node, bool isDragDrop = false)
        {
            //Ajudst position if using checkbox
            if (node.HasCheckBox && !isDragDrop)
                ImGuiHelper.IncrementCursorPosX(5);

            node.IconDrawer?.Invoke(this, EventArgs.Empty);

            if (IconManager.HasIcon(node.Icon))
            {
                int iconID = IconManager.GetTextureIcon(node.Icon);

                ImGui.AlignTextToFramePadding();
                ImGui.Image((IntPtr)iconID, new System.Numerics.Vector2(22, 22));
                ImGui.SameLine();
            }
            else
            {
                //Assume the icon is just a char
                if (node.Icon?.Length == 1)
                {
                    IconManager.DrawIcon(node.Icon[0], node.IconColor);
                    ImGui.SameLine();
                }
            }
            //Shift
            ImGuiHelper.IncrementCursorPosX(3);
        }

        private ImGuiTreeNodeFlags PrepareNodeFlags(TreeNode node)
        {
            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
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
            if (node.IsSelected)
                flags |= ImGuiTreeNodeFlags.Selected;

            return flags;
        }

        private void DrawRightClickMenu(TreeNode node)
        {
            foreach (var item in node.ContextMenus)
                ImGuiHelper.DrawMenuItem(item);
        }

        private bool GetNodePosition(TreeNode target, TreeNode parent, ref float pos, float itemHeight)
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

        enum UPDATE_FLAGS
        {
            NONE,
            SCROLL,
        }
    }
}
