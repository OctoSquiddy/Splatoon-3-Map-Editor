using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using ImGuiNET;
using UIFramework;
using SampleMapEditor.Ainb;
using MapStudio.UI;

namespace SampleMapEditor.AINBEditor
{
    /// <summary>
    /// A docked window that displays the AINB node graph editor.
    /// Provides visual editing of AI logic nodes with drag-and-drop connections.
    /// </summary>
    public class AINBNodeGraphWindow : DockWindow
    {
        public override string Name => "AINB_NODE_GRAPH";
        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        // Reference to the editor loader
        private AINBEditorLoader _editorLoader;

        // View state
        private Vector2 _viewOffset = Vector2.Zero;
        private float _zoom = 1.0f;
        private const float MIN_ZOOM = 0.1f;
        private const float MAX_ZOOM = 3.0f;

        // Node visual state
        private Dictionary<int, Vector2> _nodePositions = new Dictionary<int, Vector2>();
        private List<int> _selectedNodes = new List<int>();

        // Interaction state
        private bool _isPanning = false;
        private bool _isDraggingNode = false;
        private Vector2 _lastMousePos;
        private Vector2 _dragStartPos;
        private int _hoveredNode = -1;
        private int _hoveredPort = -1;
        private bool _isHoveringOutput = false;

        // Connection creation state
        private bool _isCreatingConnection = false;
        private int _connectionSourceNode = -1;
        private int _connectionSourcePort = -1;
        private bool _connectionSourceIsOutput = false;

        // Node appearance
        private const float NODE_WIDTH = 200.0f;
        private const float NODE_HEADER_HEIGHT = 25.0f;
        private const float NODE_PORT_HEIGHT = 20.0f;
        private const float NODE_PORT_RADIUS = 6.0f;
        private const float NODE_PADDING = 8.0f;
        private const float NODE_ROUNDING = 5.0f;

        // Colors
        private readonly Vector4 COLOR_NODE_BG = new Vector4(0.15f, 0.15f, 0.15f, 0.95f);
        private readonly Vector4 COLOR_NODE_HEADER = new Vector4(0.3f, 0.5f, 0.7f, 1.0f);
        private readonly Vector4 COLOR_NODE_HEADER_SELECTED = new Vector4(0.4f, 0.6f, 0.9f, 1.0f);
        private readonly Vector4 COLOR_NODE_BORDER = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
        private readonly Vector4 COLOR_NODE_BORDER_SELECTED = new Vector4(1.0f, 0.8f, 0.2f, 1.0f);
        private readonly Vector4 COLOR_CONNECTION = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        private readonly Vector4 COLOR_CONNECTION_CREATING = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        private readonly Vector4 COLOR_GRID = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        private readonly Vector4 COLOR_PORT_INT = new Vector4(0.3f, 0.8f, 0.3f, 1.0f);
        private readonly Vector4 COLOR_PORT_BOOL = new Vector4(0.8f, 0.3f, 0.3f, 1.0f);
        private readonly Vector4 COLOR_PORT_FLOAT = new Vector4(0.3f, 0.6f, 0.9f, 1.0f);
        private readonly Vector4 COLOR_PORT_STRING = new Vector4(0.9f, 0.6f, 0.3f, 1.0f);
        private readonly Vector4 COLOR_PORT_FLOW = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        public AINBNodeGraphWindow(DockSpaceWindow parent, AINBEditorLoader editorLoader) : base(parent)
        {
            _editorLoader = editorLoader;
            DockDirection = ImGuiDir.None;
            SplitRatio = 0.7f;
            Opened = true;

            InitializeNodePositions();
        }

        /// <summary>
        /// Initializes node positions in a grid layout.
        /// </summary>
        private void InitializeNodePositions()
        {
            if (_editorLoader?.AinbData?.Nodes == null)
                return;

            int nodesPerRow = 4;
            float xSpacing = NODE_WIDTH + 80;
            float ySpacing = 200;

            for (int i = 0; i < _editorLoader.AinbData.Nodes.Count; i++)
            {
                var node = _editorLoader.AinbData.Nodes[i];
                int row = i / nodesPerRow;
                int col = i % nodesPerRow;

                _nodePositions[node.NodeIndex] = new Vector2(col * xSpacing + 100, row * ySpacing + 100);
            }
        }

        /// <summary>
        /// Centers the view on all nodes.
        /// </summary>
        public void CenterView()
        {
            if (_nodePositions.Count == 0)
            {
                _viewOffset = Vector2.Zero;
                return;
            }

            Vector2 min = new Vector2(float.MaxValue);
            Vector2 max = new Vector2(float.MinValue);

            foreach (var pos in _nodePositions.Values)
            {
                min = Vector2.Min(min, pos);
                max = Vector2.Max(max, pos + new Vector2(NODE_WIDTH, 150));
            }

            Vector2 center = (min + max) / 2;
            var windowSize = ImGui.GetWindowSize();
            _viewOffset = windowSize / 2 - center * _zoom;
        }

        /// <summary>
        /// Automatically lays out nodes based on their connections.
        /// </summary>
        public void AutoLayoutNodes()
        {
            if (_editorLoader?.AinbData?.Nodes == null)
                return;

            // Simple hierarchical layout
            var visited = new HashSet<int>();
            var levels = new Dictionary<int, int>();

            // Find root nodes (nodes that are not linked to by others)
            var linkedTo = new HashSet<int>();
            foreach (var node in _editorLoader.AinbData.Nodes)
            {
                if (node.LinkedNodes != null)
                {
                    foreach (var link in node.LinkedNodes.BoolFloatInputLinkAndOutputLink ?? Enumerable.Empty<AINB.LinkedNode>())
                        linkedTo.Add(link.NodeIndex);
                    foreach (var link in node.LinkedNodes.IntInputLink ?? Enumerable.Empty<AINB.IntLinkedNode>())
                        linkedTo.Add(link.NodeIndex);
                }
            }

            // Calculate levels
            foreach (var node in _editorLoader.AinbData.Nodes)
            {
                if (!linkedTo.Contains(node.NodeIndex))
                    CalculateLevel(node.NodeIndex, 0, levels, visited);
            }

            // Position nodes by level
            var levelCounts = new Dictionary<int, int>();
            float xSpacing = NODE_WIDTH + 100;
            float ySpacing = 150;

            foreach (var node in _editorLoader.AinbData.Nodes)
            {
                int level = levels.ContainsKey(node.NodeIndex) ? levels[node.NodeIndex] : 0;
                if (!levelCounts.ContainsKey(level))
                    levelCounts[level] = 0;

                int yIndex = levelCounts[level]++;
                _nodePositions[node.NodeIndex] = new Vector2(level * xSpacing + 100, yIndex * ySpacing + 100);
            }
        }

        private void CalculateLevel(int nodeIndex, int level, Dictionary<int, int> levels, HashSet<int> visited)
        {
            if (visited.Contains(nodeIndex))
            {
                if (levels.ContainsKey(nodeIndex))
                    levels[nodeIndex] = Math.Max(levels[nodeIndex], level);
                return;
            }

            visited.Add(nodeIndex);
            levels[nodeIndex] = level;

            var node = _editorLoader.AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
            if (node?.LinkedNodes != null)
            {
                foreach (var link in node.LinkedNodes.BoolFloatInputLinkAndOutputLink ?? Enumerable.Empty<AINB.LinkedNode>())
                    CalculateLevel(link.NodeIndex, level + 1, levels, visited);
                foreach (var link in node.LinkedNodes.IntInputLink ?? Enumerable.Empty<AINB.IntLinkedNode>())
                    CalculateLevel(link.NodeIndex, level + 1, levels, visited);
            }
        }

        public override void Render()
        {
            var drawList = ImGui.GetWindowDrawList();
            var windowPos = ImGui.GetCursorScreenPos();
            var windowSize = ImGui.GetWindowSize();

            // Draw background
            drawList.AddRectFilled(windowPos, windowPos + windowSize, ImGui.ColorConvertFloat4ToU32(new Vector4(0.1f, 0.1f, 0.1f, 1.0f)));

            // Draw grid
            DrawGrid(drawList, windowPos, windowSize);

            // Handle input
            HandleInput();

            // Draw connections first (behind nodes)
            DrawConnections(drawList, windowPos);

            // Draw connection being created
            if (_isCreatingConnection)
            {
                var sourceNode = _editorLoader?.AinbData?.Nodes?.FirstOrDefault(n => n.NodeIndex == _connectionSourceNode);
                if (sourceNode != null)
                {
                    Vector2 startPos = GetPortPosition(sourceNode, _connectionSourcePort, _connectionSourceIsOutput, windowPos);
                    Vector2 endPos = ImGui.GetMousePos();
                    DrawBezierConnection(drawList, startPos, endPos, COLOR_CONNECTION_CREATING, 3.0f);
                }
            }

            // Draw nodes
            DrawNodes(drawList, windowPos);

            // Draw selection box if multi-selecting
            // TODO: Implement selection box

            // Draw mini-map (optional)
            // TODO: Implement mini-map
        }

        /// <summary>
        /// Draws the background grid.
        /// </summary>
        private void DrawGrid(ImDrawListPtr drawList, Vector2 windowPos, Vector2 windowSize)
        {
            float gridSize = 50.0f * _zoom;
            float startX = (_viewOffset.X % gridSize);
            float startY = (_viewOffset.Y % gridSize);

            uint gridColor = ImGui.ColorConvertFloat4ToU32(COLOR_GRID);

            for (float x = startX; x < windowSize.X; x += gridSize)
                drawList.AddLine(windowPos + new Vector2(x, 0), windowPos + new Vector2(x, windowSize.Y), gridColor);

            for (float y = startY; y < windowSize.Y; y += gridSize)
                drawList.AddLine(windowPos + new Vector2(0, y), windowPos + new Vector2(windowSize.X, y), gridColor);
        }

        /// <summary>
        /// Handles mouse and keyboard input.
        /// </summary>
        private void HandleInput()
        {
            if (!ImGui.IsWindowHovered())
                return;

            Vector2 mousePos = ImGui.GetMousePos();
            var io = ImGui.GetIO();

            // Zoom with scroll wheel
            if (Math.Abs(io.MouseWheel) > 0.01f)
            {
                float zoomDelta = io.MouseWheel * 0.1f;
                float newZoom = Math.Clamp(_zoom + zoomDelta, MIN_ZOOM, MAX_ZOOM);

                // Zoom toward mouse position
                Vector2 windowPos = ImGui.GetCursorScreenPos();
                Vector2 mouseLocal = (mousePos - windowPos - _viewOffset) / _zoom;
                _zoom = newZoom;
                _viewOffset = mousePos - windowPos - mouseLocal * _zoom;
            }

            // Pan with middle mouse or left mouse on empty space
            if (ImGui.IsMouseDown(ImGuiMouseButton.Middle) ||
                (ImGui.IsMouseDown(ImGuiMouseButton.Left) && _hoveredNode == -1 && !_isCreatingConnection))
            {
                if (!_isPanning)
                {
                    _isPanning = true;
                    _lastMousePos = mousePos;
                }
                else
                {
                    Vector2 delta = mousePos - _lastMousePos;
                    _viewOffset += delta;
                    _lastMousePos = mousePos;
                }
            }
            else
            {
                _isPanning = false;
            }

            // Node selection
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && _hoveredNode >= 0)
            {
                if (!io.KeyCtrl)
                    _selectedNodes.Clear();

                if (_selectedNodes.Contains(_hoveredNode))
                    _selectedNodes.Remove(_hoveredNode);
                else
                    _selectedNodes.Add(_hoveredNode);

                _isDraggingNode = true;
                _dragStartPos = mousePos;
            }

            // Node dragging
            if (_isDraggingNode && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                Vector2 delta = (mousePos - _lastMousePos) / _zoom;
                foreach (int nodeIndex in _selectedNodes)
                {
                    if (_nodePositions.ContainsKey(nodeIndex))
                        _nodePositions[nodeIndex] += delta;
                }
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                _isDraggingNode = false;

                // End connection creation
                if (_isCreatingConnection && _hoveredPort >= 0 && _hoveredNode != _connectionSourceNode)
                {
                    // TODO: Create the actual connection in the AINB data
                    Console.WriteLine($"Create connection: {_connectionSourceNode}[{_connectionSourcePort}] -> {_hoveredNode}[{_hoveredPort}]");
                }
                _isCreatingConnection = false;
            }

            // Delete selected nodes
            if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Delete)) && _selectedNodes.Count > 0)
            {
                DeleteSelectedNodes();
            }

            // Right-click context menu
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("NodeGraphContextMenu");
            }

            if (ImGui.BeginPopup("NodeGraphContextMenu"))
            {
                if (_hoveredNode >= 0)
                {
                    if (ImGui.MenuItem("Delete Node"))
                    {
                        DeleteNode(_hoveredNode);
                    }
                    if (ImGui.MenuItem("Duplicate Node"))
                    {
                        DuplicateNode(_hoveredNode);
                    }
                    ImGui.Separator();
                    if (ImGui.BeginMenu("Add Output Link"))
                    {
                        if (ImGui.MenuItem("Add Flow Output"))
                        {
                            AddOutputLinkToNode(_hoveredNode);
                        }
                        ImGui.EndMenu();
                    }
                }
                else
                {
                    if (ImGui.BeginMenu("Add Node"))
                    {
                        if (ImGui.MenuItem("Action Node"))
                        {
                            AddActionNode();
                        }
                        if (ImGui.MenuItem("Condition Node"))
                        {
                            AddConditionNode();
                        }
                        if (ImGui.MenuItem("Selector Node"))
                        {
                            AddSelectorNode();
                        }
                        if (ImGui.MenuItem("Sequence Node"))
                        {
                            AddSequenceNode();
                        }
                        ImGui.Separator();
                        if (ImGui.MenuItem("Empty Node"))
                        {
                            AddEmptyNode();
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Auto Layout"))
                    {
                        AutoLayoutNodes();
                    }
                    if (ImGui.MenuItem("Center View"))
                    {
                        CenterView();
                    }
                }
                ImGui.EndPopup();
            }

            _lastMousePos = mousePos;
        }

        /// <summary>
        /// Draws all connections between nodes.
        /// </summary>
        private void DrawConnections(ImDrawListPtr drawList, Vector2 windowPos)
        {
            if (_editorLoader?.AinbData?.Nodes == null)
                return;

            foreach (var node in _editorLoader.AinbData.Nodes)
            {
                if (node.LinkedNodes == null)
                    continue;

                // Draw flow connections (BoolFloatInputLinkAndOutputLink)
                if (node.LinkedNodes.BoolFloatInputLinkAndOutputLink != null)
                {
                    for (int i = 0; i < node.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count; i++)
                    {
                        var link = node.LinkedNodes.BoolFloatInputLinkAndOutputLink[i];
                        var targetNode = _editorLoader.AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == link.NodeIndex);
                        if (targetNode != null)
                        {
                            Vector2 startPos = GetOutputFlowPortPosition(node, i, windowPos);
                            Vector2 endPos = GetInputFlowPortPosition(targetNode, 0, windowPos);
                            DrawBezierConnection(drawList, startPos, endPos, COLOR_PORT_FLOW, 2.0f);
                        }
                    }
                }

                // Draw int connections
                if (node.LinkedNodes.IntInputLink != null)
                {
                    for (int i = 0; i < node.LinkedNodes.IntInputLink.Count; i++)
                    {
                        var link = node.LinkedNodes.IntInputLink[i];
                        var targetNode = _editorLoader.AinbData.Nodes.FirstOrDefault(n => n.NodeIndex == link.NodeIndex);
                        if (targetNode != null)
                        {
                            Vector2 startPos = GetOutputFlowPortPosition(node, i + (node.LinkedNodes.BoolFloatInputLinkAndOutputLink?.Count ?? 0), windowPos);
                            Vector2 endPos = GetInputFlowPortPosition(targetNode, 0, windowPos);
                            DrawBezierConnection(drawList, startPos, endPos, COLOR_PORT_INT, 2.0f);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a bezier curve connection.
        /// </summary>
        private void DrawBezierConnection(ImDrawListPtr drawList, Vector2 start, Vector2 end, Vector4 color, float thickness)
        {
            float tangentDistance = Math.Abs(end.X - start.X) * 0.5f;
            tangentDistance = Math.Max(tangentDistance, 50.0f);

            Vector2 cp1 = start + new Vector2(tangentDistance, 0);
            Vector2 cp2 = end - new Vector2(tangentDistance, 0);

            drawList.AddBezierCubic(start, cp1, cp2, end, ImGui.ColorConvertFloat4ToU32(color), thickness);
        }

        /// <summary>
        /// Draws all nodes.
        /// </summary>
        private void DrawNodes(ImDrawListPtr drawList, Vector2 windowPos)
        {
            if (_editorLoader?.AinbData?.Nodes == null)
                return;

            _hoveredNode = -1;
            _hoveredPort = -1;

            foreach (var node in _editorLoader.AinbData.Nodes)
            {
                DrawNode(drawList, windowPos, node);
            }
        }

        /// <summary>
        /// Draws a single node.
        /// </summary>
        private void DrawNode(ImDrawListPtr drawList, Vector2 windowPos, AINB.LogicNode node)
        {
            if (!_nodePositions.ContainsKey(node.NodeIndex))
                _nodePositions[node.NodeIndex] = new Vector2(100, 100);

            Vector2 nodePos = windowPos + _viewOffset + _nodePositions[node.NodeIndex] * _zoom;
            float scaledWidth = NODE_WIDTH * _zoom;

            // Calculate node height based on content
            int inputCount = CountInputPorts(node);
            int outputCount = CountOutputPorts(node);
            int maxPorts = Math.Max(inputCount, outputCount);
            float nodeHeight = (NODE_HEADER_HEIGHT + NODE_PADDING * 2 + maxPorts * NODE_PORT_HEIGHT) * _zoom;

            Vector2 nodeSize = new Vector2(scaledWidth, nodeHeight);
            Vector2 nodeEnd = nodePos + nodeSize;

            // Check if hovered
            Vector2 mousePos = ImGui.GetMousePos();
            bool isHovered = mousePos.X >= nodePos.X && mousePos.X <= nodeEnd.X &&
                            mousePos.Y >= nodePos.Y && mousePos.Y <= nodeEnd.Y;
            if (isHovered)
                _hoveredNode = node.NodeIndex;

            bool isSelected = _selectedNodes.Contains(node.NodeIndex);

            // Draw node background
            uint bgColor = ImGui.ColorConvertFloat4ToU32(COLOR_NODE_BG);
            uint borderColor = ImGui.ColorConvertFloat4ToU32(isSelected ? COLOR_NODE_BORDER_SELECTED : COLOR_NODE_BORDER);
            uint headerColor = ImGui.ColorConvertFloat4ToU32(isSelected ? COLOR_NODE_HEADER_SELECTED : GetNodeHeaderColor(node.NodeType));

            float rounding = NODE_ROUNDING * _zoom;
            drawList.AddRectFilled(nodePos, nodeEnd, bgColor, rounding);
            drawList.AddRectFilled(nodePos, nodePos + new Vector2(scaledWidth, NODE_HEADER_HEIGHT * _zoom), headerColor, rounding);
            drawList.AddRect(nodePos, nodeEnd, borderColor, rounding);

            // Draw node title
            string title = $"[{node.NodeIndex}] {node.Name}";
            if (title.Length > 25)
                title = title.Substring(0, 22) + "...";

            Vector2 textPos = nodePos + new Vector2(NODE_PADDING * _zoom, (NODE_HEADER_HEIGHT / 2 - 6) * _zoom);
            var curPos = ImGui.GetCursorPos();
            ImGui.SetCursorScreenPos(textPos);
            ImGui.TextColored(new Vector4(1, 1, 1, 1), title);
            ImGui.SetCursorPos(curPos);

            // Draw input ports (left side)
            DrawInputPorts(drawList, windowPos, node, nodePos, nodeHeight);

            // Draw output ports (right side)
            DrawOutputPorts(drawList, windowPos, node, nodePos, nodeEnd, nodeHeight);
        }

        /// <summary>
        /// Draws input ports for a node.
        /// </summary>
        private void DrawInputPorts(ImDrawListPtr drawList, Vector2 windowPos, AINB.LogicNode node, Vector2 nodePos, float nodeHeight)
        {
            float portY = nodePos.Y + NODE_HEADER_HEIGHT * _zoom + NODE_PADDING * _zoom;
            float portRadius = NODE_PORT_RADIUS * _zoom;
            int portIndex = 0;

            // Flow input
            Vector2 flowPortPos = new Vector2(nodePos.X, portY + portRadius);
            DrawPort(drawList, flowPortPos, portRadius, COLOR_PORT_FLOW, "In", true, node.NodeIndex, portIndex++);
            portY += NODE_PORT_HEIGHT * _zoom;

            // Parameter inputs
            if (node.InputParameters != null)
            {
                // Int inputs
                foreach (var param in node.InputParameters.Int ?? Enumerable.Empty<AINB.InputIntParameter>())
                {
                    Vector2 portPos = new Vector2(nodePos.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_INT, param.Name, true, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                // Bool inputs
                foreach (var param in node.InputParameters.Bool ?? Enumerable.Empty<AINB.InputBoolParameter>())
                {
                    Vector2 portPos = new Vector2(nodePos.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_BOOL, param.Name, true, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                // Float inputs
                foreach (var param in node.InputParameters.Float ?? Enumerable.Empty<AINB.InputFloatParameter>())
                {
                    Vector2 portPos = new Vector2(nodePos.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_FLOAT, param.Name, true, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                // String inputs
                foreach (var param in node.InputParameters.String ?? Enumerable.Empty<AINB.InputStringParameter>())
                {
                    Vector2 portPos = new Vector2(nodePos.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_STRING, param.Name, true, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }
            }
        }

        /// <summary>
        /// Draws output ports for a node.
        /// </summary>
        private void DrawOutputPorts(ImDrawListPtr drawList, Vector2 windowPos, AINB.LogicNode node, Vector2 nodePos, Vector2 nodeEnd, float nodeHeight)
        {
            float portY = nodePos.Y + NODE_HEADER_HEIGHT * _zoom + NODE_PADDING * _zoom;
            float portRadius = NODE_PORT_RADIUS * _zoom;
            int portIndex = 0;

            // Linked node outputs
            if (node.LinkedNodes != null)
            {
                foreach (var link in node.LinkedNodes.BoolFloatInputLinkAndOutputLink ?? Enumerable.Empty<AINB.LinkedNode>())
                {
                    Vector2 portPos = new Vector2(nodeEnd.X, portY + portRadius);
                    string label = string.IsNullOrEmpty(link.Parameter) ? "Out" : link.Parameter;
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_FLOW, label, false, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                foreach (var link in node.LinkedNodes.IntInputLink ?? Enumerable.Empty<AINB.IntLinkedNode>())
                {
                    Vector2 portPos = new Vector2(nodeEnd.X, portY + portRadius);
                    string label = string.IsNullOrEmpty(link.Parameter) ? "Out" : link.Parameter;
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_INT, label, false, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }
            }

            // Output parameters
            if (node.OutputParameters != null)
            {
                foreach (var param in node.OutputParameters.Int ?? Enumerable.Empty<AINB.OutputIntParameter>())
                {
                    Vector2 portPos = new Vector2(nodeEnd.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_INT, param.Name, false, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                foreach (var param in node.OutputParameters.Bool ?? Enumerable.Empty<AINB.OutputBoolParameter>())
                {
                    Vector2 portPos = new Vector2(nodeEnd.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_BOOL, param.Name, false, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                foreach (var param in node.OutputParameters.Float ?? Enumerable.Empty<AINB.OutputFloatParameter>())
                {
                    Vector2 portPos = new Vector2(nodeEnd.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_FLOAT, param.Name, false, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }

                foreach (var param in node.OutputParameters.String ?? Enumerable.Empty<AINB.OutputStringParameter>())
                {
                    Vector2 portPos = new Vector2(nodeEnd.X, portY + portRadius);
                    DrawPort(drawList, portPos, portRadius, COLOR_PORT_STRING, param.Name, false, node.NodeIndex, portIndex++);
                    portY += NODE_PORT_HEIGHT * _zoom;
                }
            }
        }

        /// <summary>
        /// Draws a single port circle.
        /// </summary>
        private void DrawPort(ImDrawListPtr drawList, Vector2 position, float radius, Vector4 color, string label, bool isInput, int nodeIndex, int portIndex)
        {
            uint portColor = ImGui.ColorConvertFloat4ToU32(color);
            uint borderColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));

            // Check if hovered
            Vector2 mousePos = ImGui.GetMousePos();
            float distance = Vector2.Distance(mousePos, position);
            bool isHovered = distance <= radius * 1.5f;

            if (isHovered)
            {
                _hoveredPort = portIndex;
                _isHoveringOutput = !isInput;

                // Start connection on click
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    _isCreatingConnection = true;
                    _connectionSourceNode = nodeIndex;
                    _connectionSourcePort = portIndex;
                    _connectionSourceIsOutput = !isInput;
                }
            }

            // Draw port circle
            if (isHovered)
                radius *= 1.3f;

            drawList.AddCircleFilled(position, radius, portColor);
            drawList.AddCircle(position, radius, borderColor, 12, 1.5f);

            // Draw label
            float labelOffset = (radius + 5) * (isInput ? 1 : -1);
            Vector2 textPos = position + new Vector2(labelOffset, -6);

            if (!isInput)
            {
                // Right-align for output labels
                var textSize = ImGui.CalcTextSize(label);
                textPos.X -= textSize.X + radius + 5;
            }

            if (!string.IsNullOrEmpty(label) && label.Length <= 15)
            {
                var savedCurPos = ImGui.GetCursorPos();
                ImGui.SetCursorScreenPos(textPos);
                ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.8f, 1.0f), label);
                ImGui.SetCursorPos(savedCurPos);
            }
        }

        /// <summary>
        /// Gets the header color based on node type.
        /// </summary>
        private Vector4 GetNodeHeaderColor(string nodeType)
        {
            if (string.IsNullOrEmpty(nodeType))
                return COLOR_NODE_HEADER;

            if (nodeType.Contains("Condition"))
                return new Vector4(0.7f, 0.3f, 0.3f, 1.0f);
            if (nodeType.Contains("Action") || nodeType.Contains("Do"))
                return new Vector4(0.3f, 0.7f, 0.3f, 1.0f);
            if (nodeType.Contains("Selector"))
                return new Vector4(0.7f, 0.5f, 0.2f, 1.0f);
            if (nodeType.Contains("Sequence"))
                return new Vector4(0.2f, 0.5f, 0.7f, 1.0f);

            return COLOR_NODE_HEADER;
        }

        /// <summary>
        /// Counts input ports for a node.
        /// </summary>
        private int CountInputPorts(AINB.LogicNode node)
        {
            int count = 1; // Flow input
            if (node.InputParameters != null)
            {
                count += (node.InputParameters.Int?.Count ?? 0);
                count += (node.InputParameters.Bool?.Count ?? 0);
                count += (node.InputParameters.Float?.Count ?? 0);
                count += (node.InputParameters.String?.Count ?? 0);
            }
            return count;
        }

        /// <summary>
        /// Counts output ports for a node.
        /// </summary>
        private int CountOutputPorts(AINB.LogicNode node)
        {
            int count = 0;
            if (node.LinkedNodes != null)
            {
                count += (node.LinkedNodes.BoolFloatInputLinkAndOutputLink?.Count ?? 0);
                count += (node.LinkedNodes.IntInputLink?.Count ?? 0);
            }
            if (node.OutputParameters != null)
            {
                count += (node.OutputParameters.Int?.Count ?? 0);
                count += (node.OutputParameters.Bool?.Count ?? 0);
                count += (node.OutputParameters.Float?.Count ?? 0);
                count += (node.OutputParameters.String?.Count ?? 0);
            }
            return Math.Max(count, 1);
        }

        /// <summary>
        /// Gets the screen position of a port.
        /// </summary>
        private Vector2 GetPortPosition(AINB.LogicNode node, int portIndex, bool isOutput, Vector2 windowPos)
        {
            if (!_nodePositions.ContainsKey(node.NodeIndex))
                return Vector2.Zero;

            Vector2 nodePos = windowPos + _viewOffset + _nodePositions[node.NodeIndex] * _zoom;
            float scaledWidth = NODE_WIDTH * _zoom;
            float portY = nodePos.Y + NODE_HEADER_HEIGHT * _zoom + NODE_PADDING * _zoom + portIndex * NODE_PORT_HEIGHT * _zoom + NODE_PORT_RADIUS * _zoom;

            if (isOutput)
                return new Vector2(nodePos.X + scaledWidth, portY);
            else
                return new Vector2(nodePos.X, portY);
        }

        private Vector2 GetOutputFlowPortPosition(AINB.LogicNode node, int linkIndex, Vector2 windowPos)
        {
            if (!_nodePositions.ContainsKey(node.NodeIndex))
                return Vector2.Zero;

            Vector2 nodePos = windowPos + _viewOffset + _nodePositions[node.NodeIndex] * _zoom;
            float scaledWidth = NODE_WIDTH * _zoom;
            float portY = nodePos.Y + NODE_HEADER_HEIGHT * _zoom + NODE_PADDING * _zoom + linkIndex * NODE_PORT_HEIGHT * _zoom + NODE_PORT_RADIUS * _zoom;

            return new Vector2(nodePos.X + scaledWidth, portY);
        }

        private Vector2 GetInputFlowPortPosition(AINB.LogicNode node, int portIndex, Vector2 windowPos)
        {
            if (!_nodePositions.ContainsKey(node.NodeIndex))
                return Vector2.Zero;

            Vector2 nodePos = windowPos + _viewOffset + _nodePositions[node.NodeIndex] * _zoom;
            float portY = nodePos.Y + NODE_HEADER_HEIGHT * _zoom + NODE_PADDING * _zoom + NODE_PORT_RADIUS * _zoom;

            return new Vector2(nodePos.X, portY);
        }

        #region Node Operations

        /// <summary>
        /// Gets the current mouse position in graph space.
        /// </summary>
        private Vector2 GetMouseGraphPosition()
        {
            Vector2 mousePos = ImGui.GetMousePos();
            Vector2 windowPos = ImGui.GetCursorScreenPos();
            return (mousePos - windowPos - _viewOffset) / _zoom;
        }

        /// <summary>
        /// Deletes selected nodes.
        /// </summary>
        private void DeleteSelectedNodes()
        {
            foreach (int nodeIndex in _selectedNodes.ToList())
            {
                DeleteNode(nodeIndex);
            }
            _selectedNodes.Clear();
        }

        /// <summary>
        /// Deletes a single node.
        /// </summary>
        private void DeleteNode(int nodeIndex)
        {
            if (_editorLoader?.AinbData == null)
                return;

            AINBNodeOperations.DeleteNode(_editorLoader.AinbData, nodeIndex);
            _nodePositions.Remove(nodeIndex);
            _selectedNodes.Remove(nodeIndex);
            Console.WriteLine($"Deleted node {nodeIndex}");
        }

        /// <summary>
        /// Duplicates a node.
        /// </summary>
        private void DuplicateNode(int sourceNodeIndex)
        {
            if (_editorLoader?.AinbData == null)
                return;

            var newNode = AINBNodeOperations.DuplicateNode(_editorLoader.AinbData, sourceNodeIndex);
            if (newNode != null)
            {
                // Position the new node offset from the original
                if (_nodePositions.ContainsKey(sourceNodeIndex))
                {
                    _nodePositions[newNode.NodeIndex] = _nodePositions[sourceNodeIndex] + new Vector2(50, 50);
                }
                else
                {
                    _nodePositions[newNode.NodeIndex] = GetMouseGraphPosition();
                }
                Console.WriteLine($"Duplicated node {sourceNodeIndex} -> {newNode.NodeIndex}");
            }
        }

        /// <summary>
        /// Adds an empty node at the current mouse position.
        /// </summary>
        private void AddEmptyNode()
        {
            if (_editorLoader?.AinbData == null)
                return;

            var newNode = AINBNodeOperations.CreateNode(_editorLoader.AinbData, "NewNode");
            _nodePositions[newNode.NodeIndex] = GetMouseGraphPosition();
            Console.WriteLine($"Created empty node {newNode.NodeIndex}");
        }

        /// <summary>
        /// Adds an action node at the current mouse position.
        /// </summary>
        private void AddActionNode()
        {
            if (_editorLoader?.AinbData == null)
                return;

            var newNode = AINBNodeOperations.CreateActionNode(_editorLoader.AinbData, "NewAction");
            _nodePositions[newNode.NodeIndex] = GetMouseGraphPosition();
            Console.WriteLine($"Created action node {newNode.NodeIndex}");
        }

        /// <summary>
        /// Adds a condition node at the current mouse position.
        /// </summary>
        private void AddConditionNode()
        {
            if (_editorLoader?.AinbData == null)
                return;

            var newNode = AINBNodeOperations.CreateConditionNode(_editorLoader.AinbData, "NewCondition");
            _nodePositions[newNode.NodeIndex] = GetMouseGraphPosition();
            Console.WriteLine($"Created condition node {newNode.NodeIndex}");
        }

        /// <summary>
        /// Adds a selector node at the current mouse position.
        /// </summary>
        private void AddSelectorNode()
        {
            if (_editorLoader?.AinbData == null)
                return;

            var newNode = AINBNodeOperations.CreateSelectorNode(_editorLoader.AinbData, "NewSelector");
            _nodePositions[newNode.NodeIndex] = GetMouseGraphPosition();
            Console.WriteLine($"Created selector node {newNode.NodeIndex}");
        }

        /// <summary>
        /// Adds a sequence node at the current mouse position.
        /// </summary>
        private void AddSequenceNode()
        {
            if (_editorLoader?.AinbData == null)
                return;

            var newNode = AINBNodeOperations.CreateSequenceNode(_editorLoader.AinbData, "NewSequence");
            _nodePositions[newNode.NodeIndex] = GetMouseGraphPosition();
            Console.WriteLine($"Created sequence node {newNode.NodeIndex}");
        }

        /// <summary>
        /// Adds an output link to a node.
        /// </summary>
        private void AddOutputLinkToNode(int nodeIndex)
        {
            var node = _editorLoader?.AinbData?.Nodes?.FirstOrDefault(n => n.NodeIndex == nodeIndex);
            if (node == null)
                return;

            if (node.LinkedNodes == null)
                node.LinkedNodes = new AINB.LinkedNodes();
            if (node.LinkedNodes.BoolFloatInputLinkAndOutputLink == null)
                node.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>();

            int index = node.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count;
            node.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = -1,
                Parameter = $"Output {index}"
            });
            Console.WriteLine($"Added output link to node {nodeIndex}");
        }

        #endregion
    }
}

