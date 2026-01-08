using GLFrameworkEngine;
using ImGuiNET;
using MapStudio.UI;
using SampleMapEditor.Ainb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Toolbox.Core.ViewModels;

namespace SampleMapEditor.LayoutEditor
{
    /// <summary>
    /// AINB (AI Node Binary) editor integrated into the map editor.
    /// Provides a visual node graph editor for editing game logic.
    /// </summary>
    public class AINBLayoutEditor : ILayoutEditor
    {
        public string Name => "AINB Editor";

        public StageLayoutPlugin MapEditor { get; set; }

        public IToolWindowDrawer ToolWindowDrawer { get; }

        public List<IDrawable> Renderers { get; set; }

        public NodeBase Root { get; set; }

        public List<MenuItemModel> MenuItems { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// The AINB data being edited.
        /// </summary>
        public AINB AinbData { get; set; }

        // Node graph state
        private Vector2 _scrollOffset = Vector2.Zero;
        private float _zoom = 1.0f;
        private int _selectedNodeIndex = -1;
        private bool _isDragging = false;
        private Vector2 _dragStart;
        private Dictionary<int, Vector2> _nodePositions = new Dictionary<int, Vector2>();
        private const float NODE_WIDTH = 200f;
        private const float NODE_HEIGHT_BASE = 60f;
        private const float NODE_HEADER_HEIGHT = 25f;

        // Custom node list state (for property panel)
        private AINB.LogicNode _selectedNode = null;
        private HashSet<int> _expandedGroups = new HashSet<int>();

        public AINBLayoutEditor(StageLayoutPlugin editor, AINB ainbData)
        {
            MapEditor = editor;
            AinbData = ainbData;

            Root = new NodeBase(Name);
            Root.Icon = $"{IconManager.MESH_ICON}";
            Root.IconColor = new Vector4(0.2f, 0.8f, 0.4f, 1.0f);
            Root.IsExpanded = false;

            Renderers = new List<IDrawable>();
            MenuItems = new List<MenuItemModel>();

            // Add context menu for "Clear All"
            Root.ContextMenus.Add(new MenuItemModel("Clear All", ClearAllAINBNodes));

            // Initialize node positions
            InitializeNodePositions();

            // Setup the UI drawer for when the AINB editor node is selected
            Root.TagUI.UIDrawer += delegate
            {
                DrawAINBEditor();
            };

            // Add nodes to Outliner (left side) as flat list
            ReloadOutlinerNodes();
        }

        /// <summary>
        /// Adds AINB nodes to the Outliner as FLAT list (no hierarchy = no collapse on click).
        /// Visual grouping is done via label prefixes.
        /// </summary>
        private void ReloadOutlinerNodes()
        {
            Root.Children.Clear();

            if (AinbData?.Nodes == null)
                return;

            // Find primary nodes (with InstanceName)
            var primaryNodes = AinbData.Nodes.Where(n =>
                n.InternalParameters?.String?.Any(s => s.Name == "InstanceName" && !string.IsNullOrEmpty(s.Value)) == true
            ).OrderBy(n => n.NodeIndex).ToList();

            HashSet<int> addedNodes = new HashSet<int>();

            // Add primary nodes with their chains - ALL FLAT (no children)
            foreach (var primaryNode in primaryNodes)
            {
                var instanceNameParam = primaryNode.InternalParameters?.String?.FirstOrDefault(s => s.Name == "InstanceName");
                string primaryLabel = $"[{primaryNode.NodeIndex}] {primaryNode.Name} ({instanceNameParam?.Value})";

                var primaryItem = new NodeBase(primaryLabel);
                primaryItem.Tag = primaryNode;
                primaryItem.Icon = GetNodeIcon(primaryNode.NodeType);
                primaryItem.IconColor = GetNodeColor(primaryNode.NodeType);

                var capturedPrimary = primaryNode;
                primaryItem.TagUI.UIDrawer += delegate { DrawNodeProperties(capturedPrimary); };

                // Add context menu
                var addNodeMenu = new MenuItemModel("Add AINB Node");
                addNodeMenu.MenuItems = new List<MenuItemModel>
                {
                    // Single nodes
                    new MenuItemModel("GameFlowPulseDelay (Delay)", () => AddPulseDelayNode(capturedPrimary)),
                    new MenuItemModel("SplLogicActor (Activate/Sleep)", () => AddActorNode(capturedPrimary)),
                    new MenuItemModel("SplLogicUIStartAnnounce (Sound)", () => AddUIStartAnnounceNode(capturedPrimary)),
                    new MenuItemModel("SplLogicLftBlitzCompatibles (Object)", () => AddLiftControlNode(capturedPrimary)),
                    new MenuItemModel("SplLogicChangeOceanSimulation (Water)", () => AddOceanSimulationNode(capturedPrimary)),
                    new MenuItemModel("SplLogicActor (SnakeBlock)", () => AddSnakeBlockNode(capturedPrimary)),
                    // Presets with automatic delay
                    new MenuItemModel("--- With Delay ---", () => { }),
                    new MenuItemModel("Delay + SplLogicActor", () => AddDelayAndActorNode(capturedPrimary)),
                    new MenuItemModel("Delay + SplLogicLftBlitzCompatibles", () => AddDelayAndLiftNode(capturedPrimary)),
                    new MenuItemModel("Delay + SplLogicUIStartAnnounce", () => AddDelayAndAnnounceNode(capturedPrimary)),
                    new MenuItemModel("Delay + SplLogicChangeOceanSimulation", () => AddDelayAndOceanNode(capturedPrimary)),
                    new MenuItemModel("Delay + SnakeBlock", () => AddDelayAndSnakeBlockNode(capturedPrimary))
                };
                primaryItem.ContextMenus.Add(addNodeMenu);

                Root.AddChild(primaryItem);
                addedNodes.Add(primaryNode.NodeIndex);

                // Find upstream nodes
                var upstreamNodes = FindUpstreamNodesOrdered(primaryNode.NodeIndex);
                var upstreamIndices = new HashSet<int>(upstreamNodes.Select(n => n.NodeIndex));

                // Add upstream nodes FLAT with visual indent in label
                foreach (var upstreamNode in upstreamNodes)
                {
                    if (addedNodes.Contains(upstreamNode.NodeIndex))
                        continue;

                    string childLabel = $"    [{upstreamNode.NodeIndex}] {upstreamNode.Name}";
                    var childItem = new NodeBase(childLabel);
                    childItem.Tag = upstreamNode;
                    childItem.Icon = GetNodeIcon(upstreamNode.NodeType);
                    childItem.IconColor = GetNodeColor(upstreamNode.NodeType);

                    var capturedUpstream = upstreamNode;
                    childItem.TagUI.UIDrawer += delegate { DrawNodeProperties(capturedUpstream); };

                    Root.AddChild(childItem); // FLAT - add to Root, not primaryItem
                    addedNodes.Add(upstreamNode.NodeIndex);
                }

                // Find sibling nodes (like SplLogicUIStartAnnounce)
                var siblingNodes = AinbData.Nodes.Where(n =>
                    !addedNodes.Contains(n.NodeIndex) &&
                    n.NodeIndex != primaryNode.NodeIndex &&
                    (
                        (n.PreconditionNodes != null && n.PreconditionNodes.Any(p => upstreamIndices.Contains(p))) ||
                        (n.InputParameters?.UserDefined != null && n.InputParameters.UserDefined.Any(p => p.NodeIndex >= 0 && upstreamIndices.Contains(p.NodeIndex)))
                    )
                ).ToList();

                foreach (var siblingNode in siblingNodes)
                {
                    string siblingLabel = $"    [{siblingNode.NodeIndex}] {siblingNode.Name} (linked)";
                    var siblingItem = new NodeBase(siblingLabel);
                    siblingItem.Tag = siblingNode;
                    siblingItem.Icon = GetNodeIcon(siblingNode.NodeType);
                    siblingItem.IconColor = new Vector4(0.4f, 0.9f, 0.9f, 1.0f);

                    var capturedSibling = siblingNode;
                    siblingItem.TagUI.UIDrawer += delegate { DrawNodeProperties(capturedSibling); };

                    // Add delete context menu for sibling nodes
                    var deleteMenu = new MenuItemModel($"Delete Node [{siblingNode.NodeIndex}]", () => DeleteNode(capturedSibling));
                    siblingItem.ContextMenus.Add(deleteMenu);

                    Root.AddChild(siblingItem); // FLAT
                    addedNodes.Add(siblingNode.NodeIndex);
                }
            }

            // Add orphan nodes
            var orphanNodes = AinbData.Nodes.Where(n => !addedNodes.Contains(n.NodeIndex)).ToList();
            foreach (var node in orphanNodes)
            {
                string nodeLabel = $"[{node.NodeIndex}] {node.Name}";
                var nodeItem = new NodeBase(nodeLabel);
                nodeItem.Tag = node;
                nodeItem.Icon = GetNodeIcon(node.NodeType);
                nodeItem.IconColor = GetNodeColor(node.NodeType);

                var capturedNode = node;
                nodeItem.TagUI.UIDrawer += delegate { DrawNodeProperties(capturedNode); };

                // Add delete context menu for orphan nodes
                var deleteMenu = new MenuItemModel($"Delete Node [{node.NodeIndex}]", () => DeleteNode(capturedNode));
                nodeItem.ContextMenus.Add(deleteMenu);

                Root.AddChild(nodeItem);
            }
        }

        /// <summary>
        /// Initializes node positions in a grid layout.
        /// </summary>
        private void InitializeNodePositions()
        {
            if (AinbData?.Nodes == null)
                return;

            int col = 0;
            int row = 0;
            int maxCols = 4;

            foreach (var node in AinbData.Nodes)
            {
                float x = 50 + col * (NODE_WIDTH + 50);
                float y = 50 + row * 150;
                _nodePositions[node.NodeIndex] = new Vector2(x, y);

                col++;
                if (col >= maxCols)
                {
                    col = 0;
                    row++;
                }
            }
        }

        /// <summary>
        /// Draws the custom node list in the property panel.
        /// Uses ImGui.Selectable with context menus - works independently from Outliner.
        /// </summary>
        private void DrawNodeList()
        {
            if (AinbData?.Nodes == null || AinbData.Nodes.Count == 0)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1), "No nodes");
                return;
            }

            // Find primary nodes (nodes with InstanceName)
            var primaryNodes = AinbData.Nodes.Where(n =>
                n.InternalParameters?.String?.Any(s => s.Name == "InstanceName" && !string.IsNullOrEmpty(s.Value)) == true
            ).OrderBy(n => n.NodeIndex).ToList();

            // Track which nodes are displayed
            HashSet<int> displayedNodes = new HashSet<int>();

            // Draw each primary node as a collapsible group
            foreach (var primaryNode in primaryNodes)
            {
                var instanceNameParam = primaryNode.InternalParameters?.String?.FirstOrDefault(s => s.Name == "InstanceName");
                string groupLabel = $"{primaryNode.Name} ({instanceNameParam?.Value})";

                bool isExpanded = _expandedGroups.Contains(primaryNode.NodeIndex);

                // Draw group header with arrow
                ImGui.PushID($"Group_{primaryNode.NodeIndex}");

                // Arrow button for expand/collapse
                string arrow = isExpanded ? "v" : ">";
                if (ImGui.SmallButton(arrow))
                {
                    if (isExpanded)
                        _expandedGroups.Remove(primaryNode.NodeIndex);
                    else
                        _expandedGroups.Add(primaryNode.NodeIndex);
                    isExpanded = !isExpanded;
                }
                ImGui.SameLine();

                // Primary node as selectable (with color)
                Vector4 nodeColor = GetNodeColor(primaryNode.NodeType);
                ImGui.PushStyleColor(ImGuiCol.Text, nodeColor);

                bool isSelected = _selectedNode == primaryNode;
                if (ImGui.Selectable($"[{primaryNode.NodeIndex}] {groupLabel}##primary", isSelected))
                {
                    _selectedNode = primaryNode;
                }
                ImGui.PopStyleColor();

                // Right-click context menu for primary node
                if (ImGui.BeginPopupContextItem($"ctx_primary_{primaryNode.NodeIndex}"))
                {
                    if (ImGui.BeginMenu("Add AINB Node"))
                    {
                        // Single nodes
                        if (ImGui.MenuItem("GameFlowPulseDelay (Delay)"))
                            AddPulseDelayNode(primaryNode);
                        if (ImGui.MenuItem("SplLogicActor (Activate/Sleep)"))
                            AddActorNode(primaryNode);
                        if (ImGui.MenuItem("SplLogicUIStartAnnounce (Sound)"))
                            AddUIStartAnnounceNode(primaryNode);
                        if (ImGui.MenuItem("SplLogicLftBlitzCompatibles (Object)"))
                            AddLiftControlNode(primaryNode);
                        if (ImGui.MenuItem("SplLogicChangeOceanSimulation (Water)"))
                            AddOceanSimulationNode(primaryNode);
                        if (ImGui.MenuItem("SplLogicActor (SnakeBlock)"))
                            AddSnakeBlockNode(primaryNode);

                        ImGui.Separator();
                        ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), "With Delay:");

                        // Presets with automatic delay
                        if (ImGui.MenuItem("Delay + SplLogicActor"))
                            AddDelayAndActorNode(primaryNode);
                        if (ImGui.MenuItem("Delay + SplLogicLftBlitzCompatibles"))
                            AddDelayAndLiftNode(primaryNode);
                        if (ImGui.MenuItem("Delay + SplLogicUIStartAnnounce"))
                            AddDelayAndAnnounceNode(primaryNode);
                        if (ImGui.MenuItem("Delay + SplLogicChangeOceanSimulation"))
                            AddDelayAndOceanNode(primaryNode);
                        if (ImGui.MenuItem("Delay + SnakeBlock"))
                            AddDelayAndSnakeBlockNode(primaryNode);

                        ImGui.EndMenu();
                    }
                    ImGui.EndPopup();
                }

                displayedNodes.Add(primaryNode.NodeIndex);

                // Draw upstream nodes if expanded
                if (isExpanded)
                {
                    var upstreamNodes = FindUpstreamNodesOrdered(primaryNode.NodeIndex);
                    foreach (var upstreamNode in upstreamNodes)
                    {
                        if (displayedNodes.Contains(upstreamNode.NodeIndex))
                            continue;

                        ImGui.Indent(20);

                        Vector4 upstreamColor = GetNodeColor(upstreamNode.NodeType);
                        ImGui.PushStyleColor(ImGuiCol.Text, upstreamColor);

                        bool isUpstreamSelected = _selectedNode == upstreamNode;
                        if (ImGui.Selectable($"[{upstreamNode.NodeIndex}] {upstreamNode.Name}##up_{upstreamNode.NodeIndex}", isUpstreamSelected))
                        {
                            _selectedNode = upstreamNode;
                        }
                        ImGui.PopStyleColor();

                        // Right-click delete for upstream nodes
                        if (ImGui.BeginPopupContextItem($"ctx_up_{upstreamNode.NodeIndex}"))
                        {
                            var capturedUpstream = upstreamNode;
                            if (ImGui.MenuItem($"Delete Node [{upstreamNode.NodeIndex}]"))
                            {
                                DeleteNode(capturedUpstream);
                            }
                            ImGui.EndPopup();
                        }

                        displayedNodes.Add(upstreamNode.NodeIndex);
                        ImGui.Unindent(20);
                    }
                }

                ImGui.PopID();
            }

            // Draw orphan nodes (not connected to any primary node)
            var orphanNodes = AinbData.Nodes.Where(n => !displayedNodes.Contains(n.NodeIndex)).ToList();
            if (orphanNodes.Count > 0)
            {
                ImGui.Separator();
                ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1), "Other Nodes:");

                foreach (var node in orphanNodes)
                {
                    Vector4 nodeColor = GetNodeColor(node.NodeType);
                    ImGui.PushStyleColor(ImGuiCol.Text, nodeColor);

                    bool isSelected = _selectedNode == node;
                    if (ImGui.Selectable($"[{node.NodeIndex}] {node.Name}##orphan_{node.NodeIndex}", isSelected))
                    {
                        _selectedNode = node;
                    }
                    ImGui.PopStyleColor();

                    // Right-click delete for orphan nodes
                    if (ImGui.BeginPopupContextItem($"ctx_orphan_{node.NodeIndex}"))
                    {
                        var capturedOrphan = node;
                        if (ImGui.MenuItem($"Delete Node [{node.NodeIndex}]"))
                        {
                            DeleteNode(capturedOrphan);
                        }
                        ImGui.EndPopup();
                    }
                }
            }
        }

        /// <summary>
        /// Reloads internal state and outliner after node changes.
        /// </summary>
        private void ReloadNodeTree()
        {
            // Clear selection if node was removed
            if (_selectedNode != null && AinbData?.Nodes != null)
            {
                if (!AinbData.Nodes.Contains(_selectedNode))
                    _selectedNode = null;
            }

            // Reload outliner nodes
            ReloadOutlinerNodes();
        }

        /// <summary>
        /// Finds all upstream nodes by following PreconditionNodes and InputParameters recursively.
        /// Returns nodes ordered from the primary node backwards through the chain.
        /// </summary>
        private List<AINB.LogicNode> FindUpstreamNodesOrdered(int targetNodeIndex)
        {
            List<AINB.LogicNode> result = new List<AINB.LogicNode>();
            HashSet<int> visited = new HashSet<int>();

            // Start from the target node and follow PreconditionNodes backwards
            Queue<int> toProcess = new Queue<int>();

            var targetNode = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == targetNodeIndex);
            if (targetNode == null)
                return result;

            visited.Add(targetNodeIndex);

            // Add all precondition nodes of the target to the queue
            if (targetNode.PreconditionNodes != null)
            {
                foreach (var precondIdx in targetNode.PreconditionNodes)
                {
                    if (!visited.Contains(precondIdx))
                        toProcess.Enqueue(precondIdx);
                }
            }

            // Also check InputParameters for node references
            if (targetNode.InputParameters?.UserDefined != null)
            {
                foreach (var param in targetNode.InputParameters.UserDefined)
                {
                    if (param.NodeIndex >= 0 && !visited.Contains(param.NodeIndex))
                        toProcess.Enqueue(param.NodeIndex);
                }
            }

            while (toProcess.Count > 0)
            {
                int currentIndex = toProcess.Dequeue();

                if (visited.Contains(currentIndex))
                    continue;

                visited.Add(currentIndex);

                var node = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == currentIndex);
                if (node == null)
                    continue;

                result.Add(node);

                // Follow this node's PreconditionNodes
                if (node.PreconditionNodes != null)
                {
                    foreach (var precondIdx in node.PreconditionNodes)
                    {
                        if (!visited.Contains(precondIdx))
                            toProcess.Enqueue(precondIdx);
                    }
                }

                // Also check InputParameters for node references
                if (node.InputParameters?.UserDefined != null)
                {
                    foreach (var param in node.InputParameters.UserDefined)
                    {
                        if (param.NodeIndex >= 0 && !visited.Contains(param.NodeIndex))
                            toProcess.Enqueue(param.NodeIndex);
                    }
                }

                if (node.InputParameters?.Int != null)
                {
                    foreach (var param in node.InputParameters.Int)
                    {
                        if (param.NodeIndex >= 0 && !visited.Contains(param.NodeIndex))
                            toProcess.Enqueue(param.NodeIndex);
                    }
                }

                if (node.InputParameters?.Float != null)
                {
                    foreach (var param in node.InputParameters.Float)
                    {
                        if (param.NodeIndex >= 0 && !visited.Contains(param.NodeIndex))
                            toProcess.Enqueue(param.NodeIndex);
                    }
                }

                if (node.InputParameters?.Bool != null)
                {
                    foreach (var param in node.InputParameters.Bool)
                    {
                        if (param.NodeIndex >= 0 && !visited.Contains(param.NodeIndex))
                            toProcess.Enqueue(param.NodeIndex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an icon for a node type.
        /// </summary>
        private string GetNodeIcon(string nodeType)
        {
            if (string.IsNullOrEmpty(nodeType))
                return IconManager.MESH_ICON.ToString();

            if (nodeType.Contains("Condition") || nodeType.Contains("Bool") || nodeType.Contains("Selector"))
                return IconManager.FLAG_CHECKERED.ToString();
            if (nodeType.Contains("Action") || nodeType.Contains("Do"))
                return IconManager.PLAY_ICON.ToString();
            if (nodeType.Contains("Sequence"))
                return IconManager.FOLDER_ICON.ToString();

            return IconManager.MESH_ICON.ToString();
        }

        /// <summary>
        /// Gets a color for a node type.
        /// </summary>
        private Vector4 GetNodeColor(string nodeType)
        {
            if (string.IsNullOrEmpty(nodeType))
                return new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

            if (nodeType.Contains("Condition") || nodeType.Contains("Bool"))
                return new Vector4(0.9f, 0.7f, 0.2f, 1.0f); // Yellow
            if (nodeType.Contains("Selector"))
                return new Vector4(0.2f, 0.7f, 0.9f, 1.0f); // Cyan
            if (nodeType.Contains("Sequence"))
                return new Vector4(0.2f, 0.9f, 0.4f, 1.0f); // Green
            if (nodeType.Contains("Action"))
                return new Vector4(0.9f, 0.3f, 0.3f, 1.0f); // Red

            return new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
        }

        /// <summary>
        /// Draws the main AINB editor UI.
        /// </summary>
        private void DrawAINBEditor()
        {
            if (AinbData == null)
            {
                ImGui.TextColored(new Vector4(1, 0.5f, 0, 1), "No AINB data loaded.");
                return;
            }

            // AINB Info header
            if (ImGui.CollapsingHeader("AINB Info", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.BeginColumns("##AINBInfo", 2);

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Filename");
                ImGui.NextColumn();
                ImGui.Text(AinbData.Info?.Filename ?? "Unknown");
                ImGui.NextColumn();

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Category");
                ImGui.NextColumn();
                ImGui.Text(AinbData.Info?.FileCategory ?? "Unknown");
                ImGui.NextColumn();

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Version");
                ImGui.NextColumn();
                ImGui.Text(AinbData.Info?.Version ?? "Unknown");
                ImGui.NextColumn();

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Node Count");
                ImGui.NextColumn();
                ImGui.Text($"{AinbData.Nodes?.Count ?? 0}");
                ImGui.NextColumn();

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Command Count");
                ImGui.NextColumn();
                ImGui.Text($"{AinbData.Commands?.Count ?? 0}");
                ImGui.NextColumn();

                ImGui.EndColumns();
            }

            // Node List section - custom UI with proper right-click support
            if (ImGui.CollapsingHeader($"Node List ({AinbData.Nodes?.Count ?? 0})##NodeList", ImGuiTreeNodeFlags.DefaultOpen))
            {
                // Scrollable area for node list
                ImGui.BeginChild("NodeListScroll", new Vector2(0, 200), true);
                DrawNodeList();
                ImGui.EndChild();
            }

            // Selected Node Properties
            if (_selectedNode != null)
            {
                if (ImGui.CollapsingHeader($"Selected: [{_selectedNode.NodeIndex}] {_selectedNode.Name}##SelectedNode", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawNodeProperties(_selectedNode);
                }
            }

            // Commands section (AI Group references)
            if (ImGui.CollapsingHeader($"Commands ({AinbData.Commands?.Count ?? 0})"))
            {
                if (AinbData.Commands == null || AinbData.Commands.Count == 0)
                {
                    ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1), "No commands found");
                }
                else
                {
                    foreach (var cmd in AinbData.Commands)
                    {
                        ImGui.PushID($"Cmd_{cmd.Name}");
                        if (ImGui.TreeNode($"{cmd.Name}"))
                        {
                            ImGui.Indent();
                            ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.6f, 1), $"GUID: {cmd.GUID}");
                            ImGui.TextColored(new Vector4(0.4f, 0.8f, 0.4f, 1), $"Left Node: {cmd.LeftNodeIndex}");
                            ImGui.TextColored(new Vector4(0.4f, 0.8f, 0.4f, 1), $"Right Node: {cmd.RightNodeIndex}");

                            // Show linked node names
                            if (cmd.LeftNodeIndex >= 0 && AinbData.Nodes != null)
                            {
                                var leftNode = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == cmd.LeftNodeIndex);
                                if (leftNode != null)
                                    ImGui.TextColored(new Vector4(0.6f, 0.8f, 0.6f, 1), $"  -> {leftNode.Name}");
                            }
                            if (cmd.RightNodeIndex >= 0 && AinbData.Nodes != null)
                            {
                                var rightNode = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == cmd.RightNodeIndex);
                                if (rightNode != null)
                                    ImGui.TextColored(new Vector4(0.6f, 0.8f, 0.6f, 1), $"  -> {rightNode.Name}");
                            }

                            ImGui.Unindent();
                            ImGui.TreePop();
                        }
                        ImGui.PopID();
                    }
                }
            }

            // Node Graph visualization
            if (ImGui.CollapsingHeader("Node Graph"))
            {
                DrawNodeGraph();
            }

            // Actions
            if (ImGui.CollapsingHeader("Actions"))
            {
                if (ImGui.Button("Auto Layout Nodes"))
                {
                    AutoLayoutNodes();
                }
                ImGui.SameLine();
                if (ImGui.Button("Reset View"))
                {
                    _scrollOffset = Vector2.Zero;
                    _zoom = 1.0f;
                }
            }
        }

        /// <summary>
        /// Draws the visual node graph.
        /// </summary>
        private void DrawNodeGraph()
        {
            if (AinbData?.Nodes == null)
                return;

            // Get draw list and canvas area
            var drawList = ImGui.GetWindowDrawList();
            Vector2 canvasPos = ImGui.GetCursorScreenPos();
            Vector2 canvasSize = new Vector2(ImGui.GetContentRegionAvail().X, 400);

            // Draw background
            uint bgColor = ImGui.GetColorU32(new Vector4(0.15f, 0.15f, 0.18f, 1.0f));
            drawList.AddRectFilled(canvasPos, canvasPos + canvasSize, bgColor);

            // Draw grid
            uint gridColor = ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.35f, 0.5f));
            float gridSize = 50 * _zoom;
            for (float x = (_scrollOffset.X % gridSize); x < canvasSize.X; x += gridSize)
            {
                drawList.AddLine(
                    canvasPos + new Vector2(x, 0),
                    canvasPos + new Vector2(x, canvasSize.Y),
                    gridColor);
            }
            for (float y = (_scrollOffset.Y % gridSize); y < canvasSize.Y; y += gridSize)
            {
                drawList.AddLine(
                    canvasPos + new Vector2(0, y),
                    canvasPos + new Vector2(canvasSize.X, y),
                    gridColor);
            }

            // Set up clipping
            drawList.PushClipRect(canvasPos, canvasPos + canvasSize, true);

            // Draw connections first (behind nodes)
            DrawConnections(drawList, canvasPos);

            // Draw nodes
            foreach (var node in AinbData.Nodes)
            {
                DrawNode(drawList, canvasPos, node);
            }

            drawList.PopClipRect();

            // Invisible button for input handling
            ImGui.InvisibleButton("node_canvas", canvasSize);

            // Handle input
            if (ImGui.IsItemHovered())
            {
                // Pan with left mouse button
                if (ImGui.IsMouseDragging(ImGuiMouseButton.Left))
                {
                    Vector2 delta = ImGui.GetIO().MouseDelta;
                    _scrollOffset += delta;
                }

                // Zoom with scroll wheel
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    float zoomDelta = wheel * 0.1f;
                    _zoom = Math.Clamp(_zoom + zoomDelta, 0.25f, 2.0f);
                }
            }
        }

        /// <summary>
        /// Draws a single node.
        /// </summary>
        private void DrawNode(ImDrawListPtr drawList, Vector2 canvasPos, AINB.LogicNode node)
        {
            if (!_nodePositions.TryGetValue(node.NodeIndex, out Vector2 pos))
            {
                pos = new Vector2(50, 50);
                _nodePositions[node.NodeIndex] = pos;
            }

            // Apply scroll and zoom
            Vector2 nodePos = canvasPos + (pos + _scrollOffset) * _zoom;
            float nodeWidth = NODE_WIDTH * _zoom;
            float nodeHeight = NODE_HEIGHT_BASE * _zoom;

            // Node colors
            Vector4 nodeColor = GetNodeColor(node.NodeType);
            uint headerColor = ImGui.GetColorU32(nodeColor);
            uint bodyColor = ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.25f, 0.95f));
            uint borderColor = node.NodeIndex == _selectedNodeIndex
                ? ImGui.GetColorU32(new Vector4(1, 1, 1, 1))
                : ImGui.GetColorU32(new Vector4(0.4f, 0.4f, 0.4f, 1));

            // Draw node body
            drawList.AddRectFilled(nodePos, nodePos + new Vector2(nodeWidth, nodeHeight), bodyColor);

            // Draw node header
            float headerHeight = NODE_HEADER_HEIGHT * _zoom;
            drawList.AddRectFilled(nodePos, nodePos + new Vector2(nodeWidth, headerHeight), headerColor);

            // Draw border
            drawList.AddRect(nodePos, nodePos + new Vector2(nodeWidth, nodeHeight), borderColor);

            // Draw node name
            string displayName = node.Name ?? $"Node {node.NodeIndex}";
            if (displayName.Length > 20)
                displayName = displayName.Substring(0, 17) + "...";

            ImGui.SetCursorScreenPos(nodePos + new Vector2(5, 4) * _zoom);
            ImGui.TextColored(new Vector4(1, 1, 1, 1), displayName);

            // Draw node type below header
            if (!string.IsNullOrEmpty(node.NodeType))
            {
                string typeDisplay = node.NodeType;
                if (typeDisplay.Length > 25)
                    typeDisplay = typeDisplay.Substring(0, 22) + "...";
                ImGui.SetCursorScreenPos(nodePos + new Vector2(5, headerHeight + 5) * _zoom);
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), typeDisplay);
            }
        }

        /// <summary>
        /// Draws connections between nodes.
        /// </summary>
        private void DrawConnections(ImDrawListPtr drawList, Vector2 canvasPos)
        {
            if (AinbData?.Nodes == null)
                return;

            foreach (var node in AinbData.Nodes)
            {
                if (node.LinkedNodes?.BoolFloatInputLinkAndOutputLink == null)
                    continue;

                if (!_nodePositions.TryGetValue(node.NodeIndex, out Vector2 fromPos))
                    continue;

                foreach (var link in node.LinkedNodes.BoolFloatInputLinkAndOutputLink)
                {
                    if (link.NodeIndex < 0)
                        continue;

                    if (!_nodePositions.TryGetValue(link.NodeIndex, out Vector2 toPos))
                        continue;

                    // Calculate connection points
                    Vector2 startPos = canvasPos + (fromPos + _scrollOffset + new Vector2(NODE_WIDTH, NODE_HEIGHT_BASE / 2)) * _zoom;
                    Vector2 endPos = canvasPos + (toPos + _scrollOffset + new Vector2(0, NODE_HEIGHT_BASE / 2)) * _zoom;

                    // Draw bezier curve
                    float distance = Math.Abs(endPos.X - startPos.X) * 0.5f;
                    Vector2 cp1 = startPos + new Vector2(distance, 0);
                    Vector2 cp2 = endPos - new Vector2(distance, 0);

                    uint lineColor = ImGui.GetColorU32(new Vector4(0.8f, 0.8f, 0.3f, 0.8f));
                    drawList.AddBezierCubic(startPos, cp1, cp2, endPos, lineColor, 2.0f);
                }
            }
        }

        /// <summary>
        /// Draws properties for a specific node.
        /// </summary>
        private void DrawNodeProperties(AINB.LogicNode node)
        {
            if (node == null)
                return;

            // Basic properties - editable
            if (ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen))
            {
                // Name - editable
                ImGui.Text("Name:");
                string name = node.Name ?? "";
                ImGui.PushItemWidth(-1);
                if (ImGui.InputText("##NodeName", ref name, 256))
                {
                    node.Name = name;
                }
                ImGui.PopItemWidth();

                // Type - editable
                ImGui.Text("Type:");
                string nodeType = node.NodeType ?? "";
                ImGui.PushItemWidth(-1);
                if (ImGui.InputText("##NodeType", ref nodeType, 256))
                {
                    node.NodeType = nodeType;
                }
                ImGui.PopItemWidth();

                // Index - read only
                ImGui.Text($"Index: {node.NodeIndex}");

                // GUID - editable
                ImGui.Text("GUID:");
                string guid = node.GUID ?? "";
                ImGui.PushItemWidth(-1);
                if (ImGui.InputText("##NodeGUID", ref guid, 256))
                {
                    node.GUID = guid;
                }
                ImGui.PopItemWidth();

                ImGui.Separator();

                // Flags
                ImGui.Text("Flags:");
                if (node.Flags != null && node.Flags.Count > 0)
                {
                    for (int i = 0; i < node.Flags.Count; i++)
                    {
                        string flag = node.Flags[i];
                        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X - 30);
                        if (ImGui.InputText($"##Flag{i}", ref flag, 256))
                        {
                            node.Flags[i] = flag;
                        }
                        ImGui.PopItemWidth();
                        ImGui.SameLine();
                        if (ImGui.Button($"X##RemoveFlag{i}"))
                        {
                            node.Flags.RemoveAt(i);
                        }
                    }
                }
                if (ImGui.Button("+ Add Flag"))
                {
                    if (node.Flags == null) node.Flags = new List<string>();
                    node.Flags.Add("");
                }

                // Precondition Nodes
                if (node.PreconditionNodes != null && node.PreconditionNodes.Count > 0)
                {
                    ImGui.Separator();
                    ImGui.Text("Precondition Nodes:");
                    for (int i = 0; i < node.PreconditionNodes.Count; i++)
                    {
                        int precondIdx = node.PreconditionNodes[i];
                        ImGui.PushItemWidth(100);
                        if (ImGui.InputInt($"##Precond{i}", ref precondIdx))
                        {
                            node.PreconditionNodes[i] = precondIdx;
                        }
                        ImGui.PopItemWidth();
                        ImGui.SameLine();
                        if (ImGui.Button($"X##RemovePrecond{i}"))
                        {
                            node.PreconditionNodes.RemoveAt(i);
                        }
                    }
                }
            }

            // Internal Parameters - contains InstanceName which links to objects
            if (ImGui.CollapsingHeader("Internal Parameters", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawInternalParameters(node);
            }

            // Input Parameters
            if (ImGui.CollapsingHeader("Input Parameters", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawInputParameters(node);
            }

            // Output Parameters
            if (ImGui.CollapsingHeader("Output Parameters", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawOutputParameters(node);
            }

            // Linked Nodes
            if (ImGui.CollapsingHeader("Linked Nodes", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawLinkedNodes(node);
            }
        }

        /// <summary>
        /// Draws input parameters for a node - all editable.
        /// </summary>
        private void DrawInputParameters(AINB.LogicNode node)
        {
            if (node.InputParameters == null)
            {
                ImGui.Text("No input parameters");
                return;
            }

            bool hasAnyParams = false;

            // Int parameters
            if (node.InputParameters.Int != null && node.InputParameters.Int.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Int:");
                for (int i = 0; i < node.InputParameters.Int.Count; i++)
                {
                    var param = node.InputParameters.Int[i];
                    ImGui.PushID($"InputInt{i}");
                    ImGui.Indent();

                    string paramName = param.Name ?? "";
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Name", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();

                    int paramValue = param.Value;
                    ImGui.Text("Value:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(100);
                    if (ImGui.InputInt("##Value", ref paramValue))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // Bool parameters
            if (node.InputParameters.Bool != null && node.InputParameters.Bool.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.8f, 0.4f, 1.0f), "Bool:");
                for (int i = 0; i < node.InputParameters.Bool.Count; i++)
                {
                    var param = node.InputParameters.Bool[i];
                    ImGui.PushID($"InputBool{i}");
                    ImGui.Indent();

                    string paramName = param.Name ?? "";
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Name", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();

                    bool paramValue = param.Value;
                    ImGui.Text("Value:");
                    ImGui.SameLine();
                    if (ImGui.Checkbox("##Value", ref paramValue))
                        param.Value = paramValue;

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // Float parameters
            if (node.InputParameters.Float != null && node.InputParameters.Float.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(0.4f, 1.0f, 0.4f, 1.0f), "Float:");
                for (int i = 0; i < node.InputParameters.Float.Count; i++)
                {
                    var param = node.InputParameters.Float[i];
                    ImGui.PushID($"InputFloat{i}");
                    ImGui.Indent();

                    string paramName = param.Name ?? "";
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Name", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();

                    float paramValue = param.Value;
                    ImGui.Text("Value:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(100);
                    if (ImGui.InputFloat("##Value", ref paramValue))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // String parameters
            if (node.InputParameters.String != null && node.InputParameters.String.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.6f, 0.8f, 1.0f), "String:");
                for (int i = 0; i < node.InputParameters.String.Count; i++)
                {
                    var param = node.InputParameters.String[i];
                    ImGui.PushID($"InputString{i}");
                    ImGui.Indent();

                    string paramName = param.Name ?? "";
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Name", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();

                    string paramValue = param.Value ?? "";
                    ImGui.Text("Value:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Value", ref paramValue, 1024))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // UserDefined parameters
            if (node.InputParameters.UserDefined != null && node.InputParameters.UserDefined.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "UserDefined:");
                for (int i = 0; i < node.InputParameters.UserDefined.Count; i++)
                {
                    var param = node.InputParameters.UserDefined[i];
                    ImGui.PushID($"InputUserDef{i}");
                    ImGui.Indent();

                    string paramName = param.Name ?? "";
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Name", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();

                    string paramClass = param.Class ?? "";
                    ImGui.Text("Class:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Class", ref paramClass, 256))
                        param.Class = paramClass;
                    ImGui.PopItemWidth();

                    int paramValue = param.Value;
                    ImGui.Text("Value:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(100);
                    if (ImGui.InputInt("##Value", ref paramValue))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            if (!hasAnyParams)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No input parameters");
            }
        }

        /// <summary>
        /// Draws output parameters for a node - all editable.
        /// </summary>
        private void DrawOutputParameters(AINB.LogicNode node)
        {
            if (node.OutputParameters == null)
            {
                ImGui.Text("No output parameters");
                return;
            }

            bool hasAnyParams = false;

            // Int parameters
            if (node.OutputParameters.Int != null && node.OutputParameters.Int.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Int:");
                for (int i = 0; i < node.OutputParameters.Int.Count; i++)
                {
                    var param = node.OutputParameters.Int[i];
                    string paramName = param.Name ?? "";
                    ImGui.Indent();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText($"##OutputInt{i}", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();
                    ImGui.Unindent();
                }
            }

            // Bool parameters
            if (node.OutputParameters.Bool != null && node.OutputParameters.Bool.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.8f, 0.4f, 1.0f), "Bool:");
                for (int i = 0; i < node.OutputParameters.Bool.Count; i++)
                {
                    var param = node.OutputParameters.Bool[i];
                    string paramName = param.Name ?? "";
                    ImGui.Indent();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText($"##OutputBool{i}", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();
                    ImGui.Unindent();
                }
            }

            // Float parameters
            if (node.OutputParameters.Float != null && node.OutputParameters.Float.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(0.4f, 1.0f, 0.4f, 1.0f), "Float:");
                for (int i = 0; i < node.OutputParameters.Float.Count; i++)
                {
                    var param = node.OutputParameters.Float[i];
                    string paramName = param.Name ?? "";
                    ImGui.Indent();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText($"##OutputFloat{i}", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();
                    ImGui.Unindent();
                }
            }

            // String parameters
            if (node.OutputParameters.String != null && node.OutputParameters.String.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.6f, 0.8f, 1.0f), "String:");
                for (int i = 0; i < node.OutputParameters.String.Count; i++)
                {
                    var param = node.OutputParameters.String[i];
                    string paramName = param.Name ?? "";
                    ImGui.Indent();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText($"##OutputString{i}", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();
                    ImGui.Unindent();
                }
            }

            // UserDefined parameters
            if (node.OutputParameters.UserDefined != null && node.OutputParameters.UserDefined.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.5f, 0.0f, 1.0f), "UserDefined:");
                for (int i = 0; i < node.OutputParameters.UserDefined.Count; i++)
                {
                    var param = node.OutputParameters.UserDefined[i];
                    ImGui.PushID($"OutputUserDef{i}");
                    ImGui.Indent();

                    string paramName = param.Name ?? "";
                    ImGui.Text("Name:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Name", ref paramName, 256))
                        param.Name = paramName;
                    ImGui.PopItemWidth();

                    string paramClass = param.Class ?? "";
                    ImGui.Text("Class:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Class", ref paramClass, 256))
                        param.Class = paramClass;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            if (!hasAnyParams)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No output parameters");
            }
        }

        /// <summary>
        /// Draws linked nodes for a node - editable.
        /// </summary>
        private void DrawLinkedNodes(AINB.LogicNode node)
        {
            if (node.LinkedNodes == null)
            {
                ImGui.Text("No linked nodes");
                return;
            }

            if (node.LinkedNodes.BoolFloatInputLinkAndOutputLink != null &&
                node.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count > 0)
            {
                ImGui.Text("Output Links:");
                for (int i = 0; i < node.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count; i++)
                {
                    var link = node.LinkedNodes.BoolFloatInputLinkAndOutputLink[i];
                    ImGui.PushID($"Link{i}");
                    ImGui.Indent();

                    // Parameter name
                    string paramName = link.Parameter ?? "";
                    ImGui.Text("Parameter:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Param", ref paramName, 256))
                        link.Parameter = paramName;
                    ImGui.PopItemWidth();

                    // Node index
                    int nodeIndex = link.NodeIndex;
                    ImGui.Text("Target Node:");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(80);
                    if (ImGui.InputInt("##NodeIdx", ref nodeIndex))
                        link.NodeIndex = nodeIndex;
                    ImGui.PopItemWidth();

                    // Show target node name
                    ImGui.SameLine();
                    string targetName = "-> Disconnected";
                    if (link.NodeIndex >= 0 && AinbData?.Nodes != null)
                    {
                        var target = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == link.NodeIndex);
                        if (target != null)
                            targetName = $"-> {target.Name ?? $"Node {link.NodeIndex}"}";
                    }
                    ImGui.TextColored(new Vector4(0.6f, 0.8f, 0.6f, 1.0f), targetName);

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }
            else
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No output links");
            }
        }

        /// <summary>
        /// Draws internal parameters for a node - contains InstanceName for object linking.
        /// </summary>
        private void DrawInternalParameters(AINB.LogicNode node)
        {
            if (node.InternalParameters == null)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No internal parameters");
                return;
            }

            bool hasAnyParams = false;

            // String parameters (includes InstanceName)
            if (node.InternalParameters.String != null && node.InternalParameters.String.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.6f, 0.8f, 1.0f), "String:");
                for (int i = 0; i < node.InternalParameters.String.Count; i++)
                {
                    var param = node.InternalParameters.String[i];
                    ImGui.PushID($"InternalString{i}");
                    ImGui.Indent();

                    // Highlight InstanceName specially since it's the object link
                    bool isInstanceName = param.Name == "InstanceName";
                    if (isInstanceName)
                        ImGui.TextColored(new Vector4(0.2f, 1.0f, 0.4f, 1.0f), "Name: InstanceName (Object Link)");
                    else
                        ImGui.Text($"Name: {param.Name}");

                    string paramValue = param.Value ?? "";
                    ImGui.PushItemWidth(-1);
                    if (ImGui.InputText("##Value", ref paramValue, 1024))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // Int parameters
            if (node.InternalParameters.Int != null && node.InternalParameters.Int.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Int:");
                for (int i = 0; i < node.InternalParameters.Int.Count; i++)
                {
                    var param = node.InternalParameters.Int[i];
                    ImGui.PushID($"InternalInt{i}");
                    ImGui.Indent();

                    ImGui.Text($"Name: {param.Name}");
                    int paramValue = param.Value;
                    ImGui.PushItemWidth(100);
                    if (ImGui.InputInt("##Value", ref paramValue))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // Bool parameters
            if (node.InternalParameters.Bool != null && node.InternalParameters.Bool.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(1.0f, 0.8f, 0.4f, 1.0f), "Bool:");
                for (int i = 0; i < node.InternalParameters.Bool.Count; i++)
                {
                    var param = node.InternalParameters.Bool[i];
                    ImGui.PushID($"InternalBool{i}");
                    ImGui.Indent();

                    ImGui.Text($"Name: {param.Name}");
                    bool paramValue = param.Value;
                    if (ImGui.Checkbox("##Value", ref paramValue))
                        param.Value = paramValue;

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            // Float parameters
            if (node.InternalParameters.Float != null && node.InternalParameters.Float.Count > 0)
            {
                hasAnyParams = true;
                ImGui.TextColored(new Vector4(0.4f, 1.0f, 0.4f, 1.0f), "Float:");
                for (int i = 0; i < node.InternalParameters.Float.Count; i++)
                {
                    var param = node.InternalParameters.Float[i];
                    ImGui.PushID($"InternalFloat{i}");
                    ImGui.Indent();

                    ImGui.Text($"Name: {param.Name}");
                    float paramValue = param.Value;
                    ImGui.PushItemWidth(100);
                    if (ImGui.InputFloat("##Value", ref paramValue))
                        param.Value = paramValue;
                    ImGui.PopItemWidth();

                    ImGui.Unindent();
                    ImGui.PopID();
                    ImGui.Separator();
                }
            }

            if (!hasAnyParams)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No internal parameters");
            }
        }

        /// <summary>
        /// Auto-layouts nodes in a hierarchical arrangement.
        /// </summary>
        private void AutoLayoutNodes()
        {
            if (AinbData?.Nodes == null)
                return;

            // Simple grid layout
            int col = 0;
            int row = 0;
            int maxCols = 4;

            foreach (var node in AinbData.Nodes)
            {
                float x = 50 + col * (NODE_WIDTH + 80);
                float y = 50 + row * 120;
                _nodePositions[node.NodeIndex] = new Vector2(x, y);

                col++;
                if (col >= maxCols)
                {
                    col = 0;
                    row++;
                }
            }
        }

        public List<NodeBase> GetSelected()
        {
            var selected = new List<NodeBase>();
            foreach (var child in Root.Children)
            {
                if (child.IsSelected)
                    selected.Add(child);
            }
            return selected;
        }

        public void ReloadEditor()
        {
            ReloadNodeTree();
        }

        /// <summary>
        /// Adds a SplLogicUIStartAnnounce node to the chain connected to the given primary node.
        /// This allows playing announcement sounds when the chain triggers.
        /// </summary>
        private void AddUIStartAnnounceNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find the upstream nodes that connect to the primary node
            var upstreamNodes = FindUpstreamNodesOrdered(primaryNode.NodeIndex);

            // Find the node that directly provides the Pulse input to the primary node
            // This is typically a BoolToPulse or PulseDelay node
            int sourceNodeIndex = -1;
            int sourceParamIndex = 0;

            // Check PreconditionNodes of the primary node
            if (primaryNode.PreconditionNodes != null && primaryNode.PreconditionNodes.Count > 0)
            {
                sourceNodeIndex = primaryNode.PreconditionNodes[0];
            }

            // If no precondition, check the Input Parameters for a Pulse source
            if (sourceNodeIndex < 0 && primaryNode.InputParameters?.UserDefined != null)
            {
                var pulseParam = primaryNode.InputParameters.UserDefined.FirstOrDefault(p =>
                    p.Name == "Start" || p.Name == "Pulse" || p.Class?.Contains("Pulse") == true);
                if (pulseParam != null && pulseParam.NodeIndex >= 0)
                {
                    sourceNodeIndex = pulseParam.NodeIndex;
                    sourceParamIndex = pulseParam.ParameterIndex;
                }
            }

            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for UIStartAnnounce. No Pulse connection found.");
                return;
            }

            // Get the next available node index
            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            // Create the new SplLogicUIStartAnnounce node
            var announceNode = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = newNodeIndex,
                Name = "SplLogicUIStartAnnounce",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter
                        {
                            Name = "AnnounceMsgType",
                            Value = 0 // Default announcement type
                        }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Pulse",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        }
                    }
                }
            };

            // Add the new node to the AINB
            AinbData.Nodes.Add(announceNode);

            // Update the node positions for the graph view
            if (_nodePositions.TryGetValue(primaryNode.NodeIndex, out Vector2 primaryPos))
            {
                // Place the new node above the primary node
                _nodePositions[newNodeIndex] = primaryPos + new Vector2(-50, -100);
            }
            else
            {
                _nodePositions[newNodeIndex] = new Vector2(100, 100);
            }

            // Reload the tree view to show the new node
            ReloadNodeTree();

            // Mark AINB as modified
            if (MapEditor?.MapLoader?.stageDefinition != null)
                MapEditor.MapLoader.stageDefinition.AINBModified = true;

            Console.WriteLine($"[AINB] Added SplLogicUIStartAnnounce node (index {newNodeIndex}) connected to node {sourceNodeIndex}");
        }

        /// <summary>
        /// Adds a SplLogicActor node to activate/sleep an actor when triggered.
        /// Use this to make objects appear (Logic_Activate) or disappear (Logic_Sleep).
        /// </summary>
        private void AddActorNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find last node in chain (smart chaining)
            var (sourceNodeIndex, sourceParamIndex) = FindLastChainNode(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for Actor.");
                return;
            }

            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            var actorNode = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = newNodeIndex,
                Name = "SplLogicActor",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter { Name = "InstanceName", Value = "SET_ACTOR_INSTANCE_NAME" }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0
                        }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>
                    {
                        new AINB.OutputBoolParameter { Name = "Logic_IsActive" }
                    },
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" }
                    }
                }
            };

            AinbData.Nodes.Add(actorNode);
            PositionNewNode(newNodeIndex, primaryNode.NodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added SplLogicActor node (index {newNodeIndex}). Set InstanceName to link to an actor!");
        }

        /// <summary>
        /// Adds a GameFlowPulseDelay node to add a delay before triggering.
        /// </summary>
        private void AddPulseDelayNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find last node in chain (smart chaining)
            var (sourceNodeIndex, sourceParamIndex) = FindLastChainNode(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for PulseDelay.");
                return;
            }

            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            var delayNode = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = newNodeIndex,
                Name = "GameFlowPulseDelay",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter { Name = "Delay", Value = 60 }, // Delay in frames (60 = 1 second at 60fps)
                        new AINB.InternalIntParameter { Name = "QueueSize", Value = 1 }
                    },
                    Bool = new List<AINB.InternalBoolParameter>
                    {
                        new AINB.InternalBoolParameter { Name = "CanSave", Value = false },
                        new AINB.InternalBoolParameter { Name = "NeedToNetSync", Value = true }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Input",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Int = new List<AINB.OutputIntParameter>
                    {
                        new AINB.OutputIntParameter { Name = "StockNum" }
                    },
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Output", Class = "game::ai::Pulse" }
                    }
                }
            };

            AinbData.Nodes.Add(delayNode);
            PositionNewNode(newNodeIndex, primaryNode.NodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added GameFlowPulseDelay node (index {newNodeIndex}) connected to node {sourceNodeIndex}");
        }

        /// <summary>
        /// Adds a SplLogicLftBlitzCompatibles node to control another object/lift.
        /// The user needs to set the InstanceName to match an object in the map.
        /// </summary>
        private void AddLiftControlNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find last node in chain (smart chaining)
            var (sourceNodeIndex, sourceParamIndex) = FindLastChainNode(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for LiftControl.");
                return;
            }

            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            var liftNode = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = newNodeIndex,
                Name = "SplLogicLftBlitzCompatibles",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter { Name = "InstanceName", Value = "SET_INSTANCE_NAME" }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Start",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Stop",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = 0,
                            Value = 0
                        }
                    }
                }
            };

            AinbData.Nodes.Add(liftNode);
            PositionNewNode(newNodeIndex, primaryNode.NodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added SplLogicLftBlitzCompatibles node (index {newNodeIndex}). Set InstanceName to link to a map object!");
        }

        /// <summary>
        /// Adds a SplLogicChangeOceanSimulation node to enable/disable water effects.
        /// </summary>
        private void AddOceanSimulationNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find last node in chain (smart chaining)
            var (sourceNodeIndex, sourceParamIndex) = FindLastChainNode(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for OceanSimulation.");
                return;
            }

            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            var oceanNode = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = newNodeIndex,
                Name = "SplLogicChangeOceanSimulation",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    Bool = new List<AINB.InternalBoolParameter>
                    {
                        new AINB.InternalBoolParameter { Name = "IsActive", Value = true }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Pulse",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        }
                    }
                }
            };

            AinbData.Nodes.Add(oceanNode);
            PositionNewNode(newNodeIndex, primaryNode.NodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added SplLogicChangeOceanSimulation node (index {newNodeIndex}) connected to node {sourceNodeIndex}");
        }

        /// <summary>
        /// Deletes a node from the AINB and cleans up references to it.
        /// </summary>
        private void DeleteNode(AINB.LogicNode nodeToDelete)
        {
            if (AinbData?.Nodes == null || nodeToDelete == null)
                return;

            int deletedIndex = nodeToDelete.NodeIndex;

            // Remove the node from the list
            AinbData.Nodes.Remove(nodeToDelete);

            // Clean up references to this node in other nodes
            foreach (var node in AinbData.Nodes)
            {
                // Remove from PreconditionNodes
                if (node.PreconditionNodes != null)
                {
                    node.PreconditionNodes.RemoveAll(p => p == deletedIndex);
                }

                // Clean up InputParameters references
                if (node.InputParameters?.UserDefined != null)
                {
                    foreach (var param in node.InputParameters.UserDefined)
                    {
                        if (param.NodeIndex == deletedIndex)
                        {
                            param.NodeIndex = -1;
                            param.ParameterIndex = -1;
                        }
                    }
                }
            }

            // Remove from node positions
            _nodePositions.Remove(deletedIndex);

            // Clear selection if this was the selected node
            if (_selectedNode == nodeToDelete)
                _selectedNode = null;

            // Reload the tree view
            ReloadNodeTree();
            MarkAINBModified();

            Console.WriteLine($"[AINB] Deleted node {deletedIndex} ({nodeToDelete.Name})");
        }

        #region Preset Methods (Delay + Action)

        /// <summary>
        /// Creates a PulseDelay + SplLogicActor chain, properly linked with LinkedNodes.
        /// </summary>
        private void AddDelayAndActorNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find source - the node that feeds the primary node
            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for Delay+Actor.");
                return;
            }

            Console.WriteLine($"[AINB] Creating Delay+Actor chain: Source={sourceNodeIndex}, Primary={primaryNode.NodeIndex}");

            // Create PulseDelay connected to source (NOT to primary!)
            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);
            Console.WriteLine($"[AINB] Created Delay [{delayNodeIndex}] with Precondition={sourceNodeIndex}, Input={sourceNodeIndex}");

            // Create Actor connected to delay's output
            int actorNodeIndex = delayNodeIndex + 1;
            var actorNode = CreateActorNode(actorNodeIndex, delayNodeIndex, 0);
            AinbData.Nodes.Add(actorNode);
            Console.WriteLine($"[AINB] Created Actor [{actorNodeIndex}] with Precondition={delayNodeIndex}, Input={delayNodeIndex}");

            // Add LinkedNodes to delay node pointing to actor node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = actorNodeIndex, Parameter = "Output" }
                }
            };

            // Also update source node's LinkedNodes to include the delay
            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            PositionNewNode(delayNodeIndex, primaryNode.NodeIndex);
            PositionNewNode(actorNodeIndex, delayNodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Chain complete: [{sourceNodeIndex}] → [{delayNodeIndex}] Delay → [{actorNodeIndex}] Actor with LinkedNodes");
        }

        /// <summary>
        /// Creates a PulseDelay + SplLogicLftBlitzCompatibles chain, properly linked with LinkedNodes.
        /// </summary>
        private void AddDelayAndLiftNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for Delay+Lift.");
                return;
            }

            Console.WriteLine($"[AINB] Creating Delay+Lift chain: Source={sourceNodeIndex}, Primary={primaryNode.NodeIndex}");

            // Create PulseDelay connected to source (NOT to primary!)
            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);
            Console.WriteLine($"[AINB] Created Delay [{delayNodeIndex}] with Precondition={sourceNodeIndex}, Input={sourceNodeIndex}");

            // Create Lift connected to delay's output
            int liftNodeIndex = delayNodeIndex + 1;
            var liftNode = CreateLiftNode(liftNodeIndex, delayNodeIndex, 0);
            AinbData.Nodes.Add(liftNode);
            Console.WriteLine($"[AINB] Created Lift [{liftNodeIndex}] with Precondition={delayNodeIndex}, Input={delayNodeIndex}");

            // Add LinkedNodes to delay node pointing to lift node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = liftNodeIndex, Parameter = "Output" }
                }
            };

            // Also update source node's LinkedNodes to include the delay
            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            PositionNewNode(delayNodeIndex, primaryNode.NodeIndex);
            PositionNewNode(liftNodeIndex, delayNodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Chain complete: [{sourceNodeIndex}] → [{delayNodeIndex}] Delay → [{liftNodeIndex}] Lift with LinkedNodes");
        }

        /// <summary>
        /// Creates a PulseDelay + SplLogicUIStartAnnounce chain, properly linked with LinkedNodes.
        /// </summary>
        private void AddDelayAndAnnounceNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for Delay+Announce.");
                return;
            }

            // Create PulseDelay first
            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            // Create Announce connected to delay's output
            int announceNodeIndex = delayNodeIndex + 1;
            var announceNode = CreateAnnounceNode(announceNodeIndex, delayNodeIndex, 0);
            AinbData.Nodes.Add(announceNode);

            // Add LinkedNodes to delay node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = announceNodeIndex, Parameter = "Output" }
                }
            };

            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            PositionNewNode(delayNodeIndex, primaryNode.NodeIndex);
            PositionNewNode(announceNodeIndex, delayNodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added Delay [{delayNodeIndex}] → Announce [{announceNodeIndex}] chain with LinkedNodes");
        }

        /// <summary>
        /// Creates a PulseDelay + SplLogicChangeOceanSimulation chain, properly linked with LinkedNodes.
        /// </summary>
        private void AddDelayAndOceanNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for Delay+Ocean.");
                return;
            }

            // Create PulseDelay first
            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            // Create Ocean connected to delay's output
            int oceanNodeIndex = delayNodeIndex + 1;
            var oceanNode = CreateOceanNode(oceanNodeIndex, delayNodeIndex, 0);
            AinbData.Nodes.Add(oceanNode);

            // Add LinkedNodes to delay node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = oceanNodeIndex, Parameter = "Output" }
                }
            };

            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            PositionNewNode(delayNodeIndex, primaryNode.NodeIndex);
            PositionNewNode(oceanNodeIndex, delayNodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added Delay [{delayNodeIndex}] → Ocean [{oceanNodeIndex}] chain with LinkedNodes");
        }

        /// <summary>
        /// Adds a SplLogicActor node specifically for SnakeBlock actors.
        /// SnakeBlocks extend along rails when activated via Logic_Activate.
        /// </summary>
        private void AddSnakeBlockNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find last node in chain (smart chaining)
            var (sourceNodeIndex, sourceParamIndex) = FindLastChainNode(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for SnakeBlock.");
                return;
            }

            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            var snakeBlockNode = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = newNodeIndex,
                Name = "SplLogicActor",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter { Name = "InstanceName", Value = "SnakeBlock_XXXX" }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0
                        }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>
                    {
                        new AINB.OutputBoolParameter { Name = "Logic_IsActive" }
                    },
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" }
                    }
                }
            };

            AinbData.Nodes.Add(snakeBlockNode);

            // Add LinkedNodes to source
            AddLinkedNodeToSource(sourceNodeIndex, newNodeIndex, "OnEnter");

            PositionNewNode(newNodeIndex, primaryNode.NodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added SnakeBlock node (index {newNodeIndex}). Set InstanceName to your SnakeBlock's name!");
        }

        /// <summary>
        /// Creates a PulseDelay + SnakeBlock chain, properly linked with LinkedNodes.
        /// </summary>
        private void AddDelayAndSnakeBlockNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0)
            {
                Console.WriteLine("[AINB] Could not find source node for Delay+SnakeBlock.");
                return;
            }

            // Create PulseDelay first
            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            // Create SnakeBlock connected to delay's output
            int snakeBlockNodeIndex = delayNodeIndex + 1;
            var snakeBlockNode = CreateSnakeBlockNode(snakeBlockNodeIndex, delayNodeIndex, 0);
            AinbData.Nodes.Add(snakeBlockNode);

            // Add LinkedNodes to delay node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = snakeBlockNodeIndex, Parameter = "Output" }
                }
            };

            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            PositionNewNode(delayNodeIndex, primaryNode.NodeIndex);
            PositionNewNode(snakeBlockNodeIndex, delayNodeIndex);
            ReloadNodeTree();
            MarkAINBModified();
            Console.WriteLine($"[AINB] Added Delay [{delayNodeIndex}] → SnakeBlock [{snakeBlockNodeIndex}] chain with LinkedNodes");
        }

        /// <summary>
        /// Adds a linked node entry to the source node's LinkedNodes section.
        /// </summary>
        private void AddLinkedNodeToSource(int sourceNodeIndex, int targetNodeIndex, string parameter)
        {
            var sourceNode = AinbData?.Nodes?.FirstOrDefault(n => n.NodeIndex == sourceNodeIndex);
            if (sourceNode == null) return;

            if (sourceNode.LinkedNodes == null)
            {
                sourceNode.LinkedNodes = new AINB.LinkedNodes();
            }

            if (sourceNode.LinkedNodes.BoolFloatInputLinkAndOutputLink == null)
            {
                sourceNode.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>();
            }

            // Check if already linked
            if (!sourceNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Any(ln => ln.NodeIndex == targetNodeIndex))
            {
                sourceNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
                {
                    NodeIndex = targetNodeIndex,
                    Parameter = parameter
                });
            }
        }

        #endregion

        #region Node Factory Methods

        private AINB.LogicNode CreatePulseDelayNode(int nodeIndex, int sourceNodeIndex, int sourceParamIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowPulseDelay",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter { Name = "Delay", Value = 60 }, // Delay in frames (60 = 1 second at 60fps)
                        new AINB.InternalIntParameter { Name = "QueueSize", Value = 1 }
                    },
                    Bool = new List<AINB.InternalBoolParameter>
                    {
                        new AINB.InternalBoolParameter { Name = "CanSave", Value = false },
                        new AINB.InternalBoolParameter { Name = "NeedToNetSync", Value = true }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Input",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Int = new List<AINB.OutputIntParameter>
                    {
                        new AINB.OutputIntParameter { Name = "StockNum" }
                    },
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Output", Class = "game::ai::Pulse" }
                    }
                }
            };
        }

        private AINB.LogicNode CreateActorNode(int nodeIndex, int sourceNodeIndex, int sourceParamIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicActor",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter { Name = "InstanceName", Value = "SET_ACTOR_INSTANCE_NAME" }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0
                        }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>
                    {
                        new AINB.OutputBoolParameter { Name = "Logic_IsActive" }
                    },
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" }
                    }
                }
            };
        }

        private AINB.LogicNode CreateLiftNode(int nodeIndex, int sourceNodeIndex, int sourceParamIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicLftBlitzCompatibles",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter { Name = "InstanceName", Value = "SET_INSTANCE_NAME" }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Start",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Stop",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0
                        }
                    }
                }
            };
        }

        private AINB.LogicNode CreateAnnounceNode(int nodeIndex, int sourceNodeIndex, int sourceParamIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicUIStartAnnounce",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter { Name = "AnnounceMsgType", Value = 0 }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Pulse",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        }
                    }
                }
            };
        }

        private AINB.LogicNode CreateOceanNode(int nodeIndex, int sourceNodeIndex, int sourceParamIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicChangeOceanSimulation",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    Bool = new List<AINB.InternalBoolParameter>
                    {
                        new AINB.InternalBoolParameter { Name = "IsActive", Value = true }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Pulse",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        }
                    }
                }
            };
        }

        private AINB.LogicNode CreateSnakeBlockNode(int nodeIndex, int sourceNodeIndex, int sourceParamIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicActor",
                GUID = Guid.NewGuid().ToString(),
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { sourceNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter { Name = "InstanceName", Value = "SnakeBlock_XXXX" }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate",
                            Class = "const game::ai::Pulse",
                            NodeIndex = sourceNodeIndex,
                            ParameterIndex = sourceParamIndex,
                            Value = 0
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0
                        }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>
                    {
                        new AINB.OutputBoolParameter { Name = "Logic_IsActive" }
                    },
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" }
                    }
                }
            };
        }

        #endregion

        /// <summary>
        /// Helper method to find the source node index for connecting new nodes.
        /// </summary>
        private int FindSourceNodeIndex(AINB.LogicNode primaryNode)
        {
            int sourceNodeIndex = -1;

            // Check PreconditionNodes of the primary node
            if (primaryNode.PreconditionNodes != null && primaryNode.PreconditionNodes.Count > 0)
            {
                sourceNodeIndex = primaryNode.PreconditionNodes[0];
            }

            // If no precondition, check the Input Parameters for a Pulse source
            if (sourceNodeIndex < 0 && primaryNode.InputParameters?.UserDefined != null)
            {
                var pulseParam = primaryNode.InputParameters.UserDefined.FirstOrDefault(p =>
                    p.Name == "Start" || p.Name == "Pulse" || p.Class?.Contains("Pulse") == true);
                if (pulseParam != null && pulseParam.NodeIndex >= 0)
                {
                    sourceNodeIndex = pulseParam.NodeIndex;
                }
            }

            return sourceNodeIndex;
        }

        /// <summary>
        /// Finds the last node in the chain that has a Pulse output.
        /// This is used to automatically chain new nodes after the last added node.
        /// </summary>
        private (int nodeIndex, int paramIndex) FindLastChainNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return (-1, 0);

            // Get the upstream chain for this primary node
            var upstreamNodes = FindUpstreamNodesOrdered(primaryNode.NodeIndex);
            var upstreamIndices = new HashSet<int>(upstreamNodes.Select(n => n.NodeIndex));
            upstreamIndices.Add(primaryNode.NodeIndex);

            // Find all sibling nodes (nodes that depend on the upstream chain)
            var siblingNodes = AinbData.Nodes.Where(n =>
                n.NodeIndex != primaryNode.NodeIndex &&
                !upstreamIndices.Contains(n.NodeIndex) &&
                (
                    (n.PreconditionNodes != null && n.PreconditionNodes.Any(p => upstreamIndices.Contains(p))) ||
                    (n.InputParameters?.UserDefined != null && n.InputParameters.UserDefined.Any(p => p.NodeIndex >= 0 && upstreamIndices.Contains(p.NodeIndex)))
                )
            ).ToList();

            // Find the sibling with the highest index that has a Pulse output (like PulseDelay)
            AINB.LogicNode lastNodeWithOutput = null;
            foreach (var sibling in siblingNodes.OrderByDescending(n => n.NodeIndex))
            {
                // Check if this node has a Pulse output
                if (sibling.OutputParameters?.UserDefined != null)
                {
                    var pulseOutput = sibling.OutputParameters.UserDefined.FirstOrDefault(p =>
                        p.Class?.Contains("Pulse") == true || p.Name == "Output");
                    if (pulseOutput != null)
                    {
                        lastNodeWithOutput = sibling;
                        break;
                    }
                }
            }

            if (lastNodeWithOutput != null)
            {
                // Find the parameter index for the Output
                int paramIndex = 0;
                if (lastNodeWithOutput.OutputParameters?.UserDefined != null)
                {
                    for (int i = 0; i < lastNodeWithOutput.OutputParameters.UserDefined.Count; i++)
                    {
                        var p = lastNodeWithOutput.OutputParameters.UserDefined[i];
                        if (p.Class?.Contains("Pulse") == true || p.Name == "Output")
                        {
                            paramIndex = i;
                            break;
                        }
                    }
                }
                Console.WriteLine($"[AINB] Smart chain: Found last node with output: {lastNodeWithOutput.NodeIndex} ({lastNodeWithOutput.Name})");
                return (lastNodeWithOutput.NodeIndex, paramIndex);
            }

            // No sibling with output found, fall back to normal source detection
            int sourceIndex = FindSourceNodeIndex(primaryNode);
            Console.WriteLine($"[AINB] Smart chain: No sibling with output, using source: {sourceIndex}");
            return (sourceIndex, 0);
        }

        /// <summary>
        /// Helper method to position a new node near the reference node.
        /// </summary>
        private void PositionNewNode(int newNodeIndex, int referenceNodeIndex)
        {
            if (_nodePositions.TryGetValue(referenceNodeIndex, out Vector2 refPos))
            {
                _nodePositions[newNodeIndex] = refPos + new Vector2(-50, -100);
            }
            else
            {
                _nodePositions[newNodeIndex] = new Vector2(100, 100);
            }
        }

        /// <summary>
        /// Helper method to mark AINB as modified.
        /// </summary>
        private void MarkAINBModified()
        {
            if (MapEditor?.MapLoader?.stageDefinition != null)
                MapEditor.MapLoader.stageDefinition.AINBModified = true;
        }

        /// <summary>
        /// Clears all AINB nodes and commands.
        /// </summary>
        private void ClearAllAINBNodes()
        {
            if (AinbData == null)
                return;

            int nodeCount = AinbData.Nodes?.Count ?? 0;
            int commandCount = AinbData.Commands?.Count ?? 0;

            // Clear all nodes
            if (AinbData.Nodes != null)
                AinbData.Nodes.Clear();

            // Clear all commands
            if (AinbData.Commands != null)
                AinbData.Commands.Clear();

            // Clear node positions
            _nodePositions.Clear();

            // Reload the tree view
            ReloadNodeTree();

            // Mark AINB as modified
            if (MapEditor?.MapLoader?.stageDefinition != null)
                MapEditor.MapLoader.stageDefinition.AINBModified = true;

            Console.WriteLine($"[AINB] Cleared all - removed {nodeCount} nodes and {commandCount} commands");
        }

        public void DrawHelpWindow()
        {
            if (ImGui.CollapsingHeader("AINB Editor Controls", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiHelper.BoldTextLabel("Left Drag", "Pan the node graph");
                ImGuiHelper.BoldTextLabel("Scroll Wheel", "Zoom in/out");
                ImGuiHelper.BoldTextLabel("Click Node", "Select node in tree");
            }
        }

        public void DrawEditMenuBar()
        {
            // Add menu items if needed
        }

        public void RemoveSelected()
        {
            // TODO: Implement node deletion
        }

        public void OnMouseMove(MouseEventInfo mouseInfo) { }
        public void OnMouseDown(MouseEventInfo mouseInfo) { }
        public void OnMouseUp(MouseEventInfo mouseInfo) { }
        public void OnKeyDown(KeyEventInfo keyInfo) { }

        public void OnSave(StageDefinition stage)
        {
            // AINB data is saved through StageDefinition automatically
            // The AinbData reference is the same object
        }
    }
}
