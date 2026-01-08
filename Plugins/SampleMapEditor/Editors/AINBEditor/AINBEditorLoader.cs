using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Toolbox.Core;
using MapStudio.UI;
using GLFrameworkEngine;
using Toolbox.Core.IO;
using System.Collections.Generic;
using SampleMapEditor.Ainb;
using UIFramework;
using ImGuiNET;

namespace SampleMapEditor.AINBEditor
{
    /// <summary>
    /// File loader for AINB (AI Node Binary) files.
    /// Provides a visual node graph editor for editing AINB logic files.
    /// </summary>
    public class AINBEditorLoader : FileEditor, IFileFormat
    {
        /// <summary>
        /// The description of the file extension.
        /// </summary>
        public string[] Description => new string[] { "AI Node Binary" };

        /// <summary>
        /// The extension of the plugin.
        /// </summary>
        public string[] Extension => new string[] { "*.ainb" };

        /// <summary>
        /// Determines if the plugin can save or not.
        /// </summary>
        public bool CanSave { get; set; } = true;

        /// <summary>
        /// File info of the loaded file format.
        /// </summary>
        public File_Info FileInfo { get; set; }

        /// <summary>
        /// The loaded AINB data.
        /// </summary>
        public AINB AinbData { get; private set; }

        /// <summary>
        /// The node graph window for visual editing.
        /// </summary>
        public AINBNodeGraphWindow NodeGraphWindow { get; private set; }

        /// <summary>
        /// Raw file bytes for saving.
        /// </summary>
        private List<byte> _rawData;

        // Custom node list state (for tool window)
        private AINB.LogicNode _selectedNode = null;
        private HashSet<int> _expandedGroups = new HashSet<int>();

        public AINBEditorLoader()
        {
        }

        /// <summary>
        /// Determines when to use this editor from a given file.
        /// </summary>
        public bool Identify(File_Info fileInfo, Stream stream)
        {
            // Check file extension
            if (fileInfo.Extension == ".ainb")
                return true;

            // Also check magic bytes "AIB " (0x41 0x49 0x42 0x20)
            if (stream.Length >= 4)
            {
                byte[] magic = new byte[4];
                stream.Read(magic, 0, 4);
                stream.Position = 0;

                if (magic[0] == 0x41 && magic[1] == 0x49 && magic[2] == 0x42 && magic[3] == 0x20)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Loads the AINB file from a stream.
        /// </summary>
        public void Load(Stream stream)
        {
            Console.WriteLine("Loading AINB file...");

            // Read all bytes
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                _rawData = new List<byte>(ms.ToArray());
            }

            // Parse AINB data
            try
            {
                AinbData = AINB.LoadFromAINBData(_rawData);
                Console.WriteLine($"Loaded AINB: {AinbData?.Info?.Filename ?? "Unknown"}");
                Console.WriteLine($"  Category: {AinbData?.Info?.FileCategory ?? "Unknown"}");
                Console.WriteLine($"  Nodes: {AinbData?.Nodes?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing AINB: {ex.Message}");
                // Create empty AINB structure
                AinbData = new AINB();
            }

            // Setup the editor UI
            SetupEditor();
        }

        /// <summary>
        /// Sets up the editor UI - adds nodes to both Outliner and tool window.
        /// </summary>
        private void SetupEditor()
        {
            // Set up root node for outliner
            Root.Header = FileInfo?.FileName ?? "AINB File";
            Root.Icon = IconManager.MESH_ICON.ToString();
            Root.IsExpanded = false;

            // Add info node
            if (AinbData?.Info != null)
            {
                var infoNode = new Toolbox.Core.ViewModels.NodeBase($"Info: {AinbData.Info.FileCategory}");
                infoNode.Icon = IconManager.SETTINGS_ICON.ToString();
                Root.AddChild(infoNode);
            }

            // Add nodes to Outliner
            ReloadOutlinerNodes();
        }

        /// <summary>
        /// Adds AINB nodes to the Outliner as FLAT list (no hierarchy = no collapse on click).
        /// Visual grouping is done via label prefixes.
        /// </summary>
        private void ReloadOutlinerNodes()
        {
            // Remove existing node children (keep info node)
            var nodesToRemove = Root.Children.Where(c => c.Tag is AINB.LogicNode || c.Header.StartsWith("Nodes") || c.Header.StartsWith("Other") || c.Header.StartsWith("    ")).ToList();
            foreach (var n in nodesToRemove)
                Root.Children.Remove(n);

            if (AinbData?.Nodes == null)
                return;

            // Find primary nodes (with InstanceName)
            var primaryNodes = AinbData.Nodes.Where(n =>
                n.InternalParameters?.String?.Any(s => s.Name == "InstanceName" && !string.IsNullOrEmpty(s.Value)) == true
            ).OrderBy(n => n.NodeIndex).ToList();

            HashSet<int> addedNodes = new HashSet<int>();

            // Add ALL nodes FLAT to Root (no hierarchy = no collapse)
            foreach (var primaryNode in primaryNodes)
            {
                var instanceNameParam = primaryNode.InternalParameters?.String?.FirstOrDefault(s => s.Name == "InstanceName");
                string primaryLabel = $"[{primaryNode.NodeIndex}] {primaryNode.Name} ({instanceNameParam?.Value})";

                var primaryItem = new Toolbox.Core.ViewModels.NodeBase(primaryLabel);
                primaryItem.Tag = primaryNode;
                primaryItem.Icon = GetNodeIcon(primaryNode.NodeType);

                // Add context menu
                var capturedPrimary = primaryNode;
                var addNodeMenu = new Toolbox.Core.ViewModels.MenuItemModel("Add AINB Node");
                addNodeMenu.MenuItems = new List<Toolbox.Core.ViewModels.MenuItemModel>
                {
                    // Single nodes
                    new Toolbox.Core.ViewModels.MenuItemModel("GameFlowPulseDelay (Delay)", () => AddPulseDelayNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("SplLogicActor (Activate/Sleep)", () => AddActorNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("SplLogicUIStartAnnounce (Sound)", () => AddUIStartAnnounceNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("SplLogicLftBlitzCompatibles (Object)", () => AddLiftControlNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("SplLogicChangeOceanSimulation (Water)", () => AddOceanSimulationNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("SplLogicActor (SnakeBlock)", () => AddSnakeBlockNode(capturedPrimary)),
                    // Presets with automatic delay
                    new Toolbox.Core.ViewModels.MenuItemModel("--- With Delay ---", () => { }),
                    new Toolbox.Core.ViewModels.MenuItemModel("Delay + SplLogicActor", () => AddDelayAndActorNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("Delay + SplLogicLftBlitzCompatibles", () => AddDelayAndLiftNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("Delay + SplLogicUIStartAnnounce", () => AddDelayAndAnnounceNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("Delay + SplLogicChangeOceanSimulation", () => AddDelayAndOceanNode(capturedPrimary)),
                    new Toolbox.Core.ViewModels.MenuItemModel("Delay + SnakeBlock", () => AddDelayAndSnakeBlockNode(capturedPrimary))
                };
                primaryItem.ContextMenus.Add(addNodeMenu);

                Root.AddChild(primaryItem);
                addedNodes.Add(primaryNode.NodeIndex);

                // Find upstream nodes
                var upstreamNodes = FindUpstreamNodesOrdered(primaryNode.NodeIndex);
                var upstreamIndices = new HashSet<int>(upstreamNodes.Select(n => n.NodeIndex));

                // Add upstream nodes FLAT with visual indent
                foreach (var upstreamNode in upstreamNodes)
                {
                    if (addedNodes.Contains(upstreamNode.NodeIndex))
                        continue;

                    string childLabel = $"    [{upstreamNode.NodeIndex}] {upstreamNode.Name}";
                    var childItem = new Toolbox.Core.ViewModels.NodeBase(childLabel);
                    childItem.Tag = upstreamNode;
                    childItem.Icon = GetNodeIcon(upstreamNode.NodeType);

                    Root.AddChild(childItem); // FLAT
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
                    var siblingItem = new Toolbox.Core.ViewModels.NodeBase(siblingLabel);
                    siblingItem.Tag = siblingNode;
                    siblingItem.Icon = GetNodeIcon(siblingNode.NodeType);

                    // Add delete context menu for sibling nodes
                    var capturedSibling = siblingNode;
                    var deleteMenu = new Toolbox.Core.ViewModels.MenuItemModel($"Delete Node [{siblingNode.NodeIndex}]", () => DeleteNode(capturedSibling));
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
                var nodeItem = new Toolbox.Core.ViewModels.NodeBase(nodeLabel);
                nodeItem.Tag = node;
                nodeItem.Icon = GetNodeIcon(node.NodeType);

                // Add delete context menu for orphan nodes
                var capturedOrphan = node;
                var deleteMenu = new Toolbox.Core.ViewModels.MenuItemModel($"Delete Node [{node.NodeIndex}]", () => DeleteNode(capturedOrphan));
                nodeItem.ContextMenus.Add(deleteMenu);

                Root.AddChild(nodeItem);
            }
        }

        /// <summary>
        /// Draws the custom node list with proper right-click support.
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

                // Primary node as selectable
                Vector4 nodeColor = GetNodeColor(primaryNode.NodeType);
                ImGui.PushStyleColor(ImGuiCol.Text, nodeColor);

                bool isSelected = _selectedNode == primaryNode;
                if (ImGui.Selectable($"[{primaryNode.NodeIndex}] {groupLabel}##primary", isSelected))
                {
                    _selectedNode = primaryNode;
                }
                ImGui.PopStyleColor();

                // Right-click context menu
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

            // Draw orphan nodes
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
        /// Gets a color for a node type.
        /// </summary>
        private Vector4 GetNodeColor(string nodeType)
        {
            if (string.IsNullOrEmpty(nodeType))
                return new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

            if (nodeType.Contains("Condition") || nodeType.Contains("Bool"))
                return new Vector4(0.9f, 0.7f, 0.2f, 1.0f);
            if (nodeType.Contains("Selector"))
                return new Vector4(0.2f, 0.7f, 0.9f, 1.0f);
            if (nodeType.Contains("Sequence"))
                return new Vector4(0.2f, 0.9f, 0.4f, 1.0f);
            if (nodeType.Contains("Action"))
                return new Vector4(0.9f, 0.3f, 0.3f, 1.0f);

            return new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
        }

        /// <summary>
        /// Draws properties for a selected node.
        /// </summary>
        private void DrawSelectedNodeProperties()
        {
            if (_selectedNode == null)
                return;

            ImGui.Text($"Name: {_selectedNode.Name}");
            ImGui.Text($"Type: {_selectedNode.NodeType}");
            ImGui.Text($"Index: {_selectedNode.NodeIndex}");

            if (_selectedNode.InternalParameters?.String != null)
            {
                foreach (var param in _selectedNode.InternalParameters.String)
                {
                    ImGui.TextColored(new Vector4(0.4f, 0.8f, 0.4f, 1), $"{param.Name}: {param.Value}");
                }
            }

            if (_selectedNode.InternalParameters?.Int != null)
            {
                foreach (var param in _selectedNode.InternalParameters.Int)
                {
                    ImGui.TextColored(new Vector4(0.4f, 0.6f, 0.9f, 1), $"{param.Name}: {param.Value}");
                }
            }

            if (_selectedNode.PreconditionNodes != null && _selectedNode.PreconditionNodes.Count > 0)
            {
                ImGui.Text($"Preconditions: {string.Join(", ", _selectedNode.PreconditionNodes)}");
            }
        }

        /// <summary>
        /// Finds all upstream nodes by following PreconditionNodes and InputParameters recursively.
        /// </summary>
        private List<AINB.LogicNode> FindUpstreamNodesOrdered(int targetNodeIndex)
        {
            List<AINB.LogicNode> result = new List<AINB.LogicNode>();
            HashSet<int> visited = new HashSet<int>();

            var targetNode = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == targetNodeIndex);
            if (targetNode == null)
                return result;

            visited.Add(targetNodeIndex);
            Queue<int> toProcess = new Queue<int>();

            // Add precondition nodes
            if (targetNode.PreconditionNodes != null)
            {
                foreach (var idx in targetNode.PreconditionNodes)
                    if (!visited.Contains(idx)) toProcess.Enqueue(idx);
            }

            // Add input parameter node references
            if (targetNode.InputParameters?.UserDefined != null)
            {
                foreach (var p in targetNode.InputParameters.UserDefined)
                    if (p.NodeIndex >= 0 && !visited.Contains(p.NodeIndex)) toProcess.Enqueue(p.NodeIndex);
            }

            while (toProcess.Count > 0)
            {
                int idx = toProcess.Dequeue();
                if (visited.Contains(idx)) continue;
                visited.Add(idx);

                var node = AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == idx);
                if (node == null) continue;

                result.Add(node);

                if (node.PreconditionNodes != null)
                    foreach (var i in node.PreconditionNodes)
                        if (!visited.Contains(i)) toProcess.Enqueue(i);

                if (node.InputParameters?.UserDefined != null)
                    foreach (var p in node.InputParameters.UserDefined)
                        if (p.NodeIndex >= 0 && !visited.Contains(p.NodeIndex)) toProcess.Enqueue(p.NodeIndex);
            }

            return result;
        }

        /// <summary>
        /// Adds a SplLogicUIStartAnnounce node to the chain connected to the given primary node.
        /// This allows playing announcement sounds when the chain triggers.
        /// </summary>
        private void AddUIStartAnnounceNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

            // Find the node that directly provides the Pulse input to the primary node
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

            // Refresh the outliner to show the new node
            ReloadOutlinerNodes();

            Console.WriteLine($"[AINB] Added SplLogicUIStartAnnounce node (index {newNodeIndex}) connected to node {sourceNodeIndex}");
        }

        /// <summary>
        /// Adds a SplLogicActor node to activate/sleep an actor when triggered.
        /// </summary>
        private void AddActorNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

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
            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added SplLogicActor node (index {newNodeIndex}). Set InstanceName to link to an actor!");
        }

        /// <summary>
        /// Adds a GameFlowPulseDelay node to add a delay before triggering.
        /// </summary>
        private void AddPulseDelayNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

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
            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added GameFlowPulseDelay node (index {newNodeIndex}) connected to node {sourceNodeIndex}");
        }

        /// <summary>
        /// Adds a SplLogicLftBlitzCompatibles node to control another object/lift.
        /// </summary>
        private void AddLiftControlNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

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
            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added SplLogicLftBlitzCompatibles node (index {newNodeIndex}). Set InstanceName to link to a map object!");
        }

        /// <summary>
        /// Adds a SplLogicChangeOceanSimulation node to enable/disable water effects.
        /// </summary>
        private void AddOceanSimulationNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null)
                return;

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
            ReloadOutlinerNodes();
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

            // Clear selection if this was the selected node
            if (_selectedNode == nodeToDelete)
                _selectedNode = null;

            // Reload the outliner
            ReloadOutlinerNodes();

            Console.WriteLine($"[AINB] Deleted node {deletedIndex} ({nodeToDelete.Name})");
        }

        #region Preset Methods (Delay + Action)

        private void AddDelayAndActorNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null) return;
            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0) return;

            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            int actorNodeIndex = delayNodeIndex + 1;
            AinbData.Nodes.Add(CreateActorNode(actorNodeIndex, delayNodeIndex, 0));

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

            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added Delay [{delayNodeIndex}] → Actor [{actorNodeIndex}] chain with LinkedNodes");
        }

        private void AddDelayAndLiftNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null) return;
            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0) return;

            Console.WriteLine($"[AINB] Creating Delay+Lift: Source={sourceNodeIndex}, Primary={primaryNode.NodeIndex}");

            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);
            Console.WriteLine($"[AINB] Delay [{delayNodeIndex}]: Precondition={sourceNodeIndex}, Input={sourceNodeIndex}");

            int liftNodeIndex = delayNodeIndex + 1;
            AinbData.Nodes.Add(CreateLiftNode(liftNodeIndex, delayNodeIndex, 0));
            Console.WriteLine($"[AINB] Lift [{liftNodeIndex}]: Precondition={delayNodeIndex}, Input={delayNodeIndex}");

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

            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Chain: [{sourceNodeIndex}] → [{delayNodeIndex}] Delay → [{liftNodeIndex}] Lift with LinkedNodes");
        }

        private void AddDelayAndAnnounceNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null) return;
            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0) return;

            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            int announceNodeIndex = delayNodeIndex + 1;
            AinbData.Nodes.Add(CreateAnnounceNode(announceNodeIndex, delayNodeIndex, 0));

            // Add LinkedNodes to delay node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = announceNodeIndex, Parameter = "Output" }
                }
            };

            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added Delay [{delayNodeIndex}] → Announce [{announceNodeIndex}] chain with LinkedNodes");
        }

        private void AddDelayAndOceanNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null) return;
            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0) return;

            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            int oceanNodeIndex = delayNodeIndex + 1;
            AinbData.Nodes.Add(CreateOceanNode(oceanNodeIndex, delayNodeIndex, 0));

            // Add LinkedNodes to delay node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = oceanNodeIndex, Parameter = "Output" }
                }
            };

            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added Delay [{delayNodeIndex}] → Ocean [{oceanNodeIndex}] chain with LinkedNodes");
        }

        /// <summary>
        /// Adds a SplLogicActor node specifically for SnakeBlock actors.
        /// SnakeBlocks extend along rails when activated via Logic_Activate.
        /// </summary>
        private void AddSnakeBlockNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null) return;

            var (sourceNodeIndex, sourceParamIndex) = FindLastChainNode(primaryNode);
            if (sourceNodeIndex < 0) return;

            int newNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            AinbData.Nodes.Add(CreateSnakeBlockNode(newNodeIndex, sourceNodeIndex, sourceParamIndex));

            AddLinkedNodeToSource(sourceNodeIndex, newNodeIndex, "OnEnter");

            ReloadOutlinerNodes();
            Console.WriteLine($"[AINB] Added SnakeBlock node (index {newNodeIndex}). Set InstanceName to your SnakeBlock's name!");
        }

        /// <summary>
        /// Creates a PulseDelay + SnakeBlock chain, properly linked with LinkedNodes.
        /// </summary>
        private void AddDelayAndSnakeBlockNode(AINB.LogicNode primaryNode)
        {
            if (AinbData?.Nodes == null || primaryNode == null) return;
            int sourceNodeIndex = FindSourceNodeIndex(primaryNode);
            if (sourceNodeIndex < 0) return;

            int delayNodeIndex = AinbData.Nodes.Count > 0 ? AinbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;
            var delayNode = CreatePulseDelayNode(delayNodeIndex, sourceNodeIndex, 0);
            AinbData.Nodes.Add(delayNode);

            int snakeBlockNodeIndex = delayNodeIndex + 1;
            AinbData.Nodes.Add(CreateSnakeBlockNode(snakeBlockNodeIndex, delayNodeIndex, 0));

            // Add LinkedNodes to delay node
            delayNode.LinkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                {
                    new AINB.LinkedNode { NodeIndex = snakeBlockNodeIndex, Parameter = "Output" }
                }
            };

            AddLinkedNodeToSource(sourceNodeIndex, delayNodeIndex, "Output");

            ReloadOutlinerNodes();
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
                        new AINB.UserDefinedParameter { Name = "Input", Class = "const game::ai::Pulse", NodeIndex = sourceNodeIndex, ParameterIndex = sourceParamIndex, Value = 0 }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Int = new List<AINB.OutputIntParameter> { new AINB.OutputIntParameter { Name = "StockNum" } },
                    UserDefined = new List<AINB.OutputUserDefinedParameter> { new AINB.OutputUserDefinedParameter { Name = "Output", Class = "game::ai::Pulse" } }
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
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = "SET_ACTOR_INSTANCE_NAME" } }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Logic_Activate", Class = "const game::ai::Pulse", NodeIndex = sourceNodeIndex, ParameterIndex = sourceParamIndex, Value = 0 },
                        new AINB.UserDefinedParameter { Name = "Logic_Sleep", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0 }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "Logic_IsActive" } },
                    UserDefined = new List<AINB.OutputUserDefinedParameter> { new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" } }
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
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = "SET_INSTANCE_NAME" } }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Start", Class = "const game::ai::Pulse", NodeIndex = sourceNodeIndex, ParameterIndex = sourceParamIndex, Value = 0 },
                        new AINB.UserDefinedParameter { Name = "Stop", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0 }
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
                    Int = new List<AINB.InternalIntParameter> { new AINB.InternalIntParameter { Name = "AnnounceMsgType", Value = 0 } }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Pulse", Class = "const game::ai::Pulse", NodeIndex = sourceNodeIndex, ParameterIndex = sourceParamIndex, Value = 0 }
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
                    Bool = new List<AINB.InternalBoolParameter> { new AINB.InternalBoolParameter { Name = "IsActive", Value = true } }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Pulse", Class = "const game::ai::Pulse", NodeIndex = sourceNodeIndex, ParameterIndex = sourceParamIndex, Value = 0 }
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
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = "SnakeBlock_XXXX" } }
                },
                InputParameters = new AINB.InputParameters
                {
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Logic_Activate", Class = "const game::ai::Pulse", NodeIndex = sourceNodeIndex, ParameterIndex = sourceParamIndex, Value = 0 },
                        new AINB.UserDefinedParameter { Name = "Logic_Sleep", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0 }
                    }
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "Logic_IsActive" } },
                    UserDefined = new List<AINB.OutputUserDefinedParameter> { new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" } }
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

            if (primaryNode.PreconditionNodes != null && primaryNode.PreconditionNodes.Count > 0)
            {
                sourceNodeIndex = primaryNode.PreconditionNodes[0];
            }

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
        /// Gets an appropriate icon for a node type.
        /// </summary>
        private string GetNodeIcon(string nodeType)
        {
            if (string.IsNullOrEmpty(nodeType))
                return IconManager.MESH_ICON.ToString();

            if (nodeType.Contains("Condition") || nodeType.Contains("Bool"))
                return IconManager.FLAG_CHECKERED.ToString();
            if (nodeType.Contains("Action") || nodeType.Contains("Do"))
                return IconManager.PLAY_ICON.ToString();
            if (nodeType.Contains("Selector") || nodeType.Contains("Sequence"))
                return IconManager.FOLDER_ICON.ToString();

            return IconManager.MESH_ICON.ToString();
        }

        /// <summary>
        /// Saves the AINB file to a stream.
        /// </summary>
        public void Save(Stream stream)
        {
            Console.WriteLine("Saving AINB file...");

            try
            {
                // Get JSON from current data
                string jsonData = AinbData.getJSONData();

                // Convert back to binary
                List<byte> binaryData = AINB.json2ainb(jsonData);

                // Write to stream
                stream.Write(binaryData.ToArray(), 0, binaryData.Count);

                Console.WriteLine("AINB file saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving AINB: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Prepares the dock layouts for the AINB editor.
        /// </summary>
        public override List<DockWindow> PrepareDocks()
        {
            List<DockWindow> windows = new List<DockWindow>();

            // Add standard windows
            windows.Add(Workspace.Outliner);
            windows.Add(Workspace.PropertyWindow);
            windows.Add(Workspace.ConsoleWindow);

            // Create and add the node graph window
            NodeGraphWindow = new AINBNodeGraphWindow(Workspace.ActiveWorkspace, this);
            windows.Add(NodeGraphWindow);

            windows.Add(Workspace.ToolWindow);

            return windows;
        }

        /// <summary>
        /// Draws the tool window for AINB editing.
        /// </summary>
        public override void DrawToolWindow()
        {
            if (ImGui.CollapsingHeader("AINB Info", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (AinbData?.Info != null)
                {
                    ImGui.Text($"Filename: {AinbData.Info.Filename}");
                    ImGui.Text($"Category: {AinbData.Info.FileCategory}");
                    ImGui.Text($"Version: {AinbData.Info.Version}");
                    ImGui.Text($"Node Count: {AinbData.Nodes?.Count ?? 0}");
                }
            }

            // Custom node list with proper right-click support
            if (ImGui.CollapsingHeader($"Node List ({AinbData?.Nodes?.Count ?? 0})##NodeList", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.BeginChild("NodeListScroll", new Vector2(0, 200), true);
                DrawNodeList();
                ImGui.EndChild();
            }

            // Selected node properties
            if (_selectedNode != null)
            {
                if (ImGui.CollapsingHeader($"Selected: [{_selectedNode.NodeIndex}] {_selectedNode.Name}##SelectedNode", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawSelectedNodeProperties();
                }
            }

            if (ImGui.CollapsingHeader("Actions"))
            {
                if (ImGui.Button("Center View"))
                {
                    NodeGraphWindow?.CenterView();
                }

                if (ImGui.Button("Auto Layout"))
                {
                    NodeGraphWindow?.AutoLayoutNodes();
                }
            }
        }

        /// <summary>
        /// Draws help window content.
        /// </summary>
        public override void DrawHelpWindow()
        {
            if (ImGui.CollapsingHeader("AINB Editor Controls", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiHelper.BoldTextLabel("Left Click + Drag", "Pan the view");
                ImGuiHelper.BoldTextLabel("Scroll Wheel", "Zoom in/out");
                ImGuiHelper.BoldTextLabel("Click Node", "Select node");
                ImGuiHelper.BoldTextLabel("Ctrl + Click", "Multi-select");
                ImGuiHelper.BoldTextLabel("Drag Node", "Move selected nodes");
                ImGuiHelper.BoldTextLabel("Right Click", "Context menu");
                ImGuiHelper.BoldTextLabel("Delete", "Delete selected nodes");
            }
        }
    }
}
