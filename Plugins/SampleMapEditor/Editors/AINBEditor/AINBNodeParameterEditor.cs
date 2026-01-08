using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using SampleMapEditor.Ainb;
using MapStudio.UI;

namespace SampleMapEditor.AINBEditor
{
    /// <summary>
    /// UI for editing AINB node parameters in the property window.
    /// </summary>
    public class AINBNodeParameterEditor
    {
        private AINBEditorLoader _editorLoader;
        private AINB.LogicNode _selectedNode;

        public AINBNodeParameterEditor(AINBEditorLoader editorLoader)
        {
            _editorLoader = editorLoader;
        }

        /// <summary>
        /// Sets the node to edit.
        /// </summary>
        public void SetSelectedNode(AINB.LogicNode node)
        {
            _selectedNode = node;
        }

        /// <summary>
        /// Renders the parameter editing UI.
        /// </summary>
        public void Render()
        {
            if (_selectedNode == null)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No node selected");
                return;
            }

            // Node header
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.9f, 0.3f, 1.0f));
            ImGui.Text($"Node: [{_selectedNode.NodeIndex}] {_selectedNode.Name}");
            ImGui.PopStyleColor();
            ImGui.Separator();

            // Node properties
            if (ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawNodeProperties();
            }

            // Input parameters
            if (ImGui.CollapsingHeader("Input Parameters", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawInputParameters();
            }

            // Output parameters
            if (ImGui.CollapsingHeader("Output Parameters"))
            {
                DrawOutputParameters();
            }

            // Linked nodes
            if (ImGui.CollapsingHeader("Linked Nodes"))
            {
                DrawLinkedNodes();
            }

            // Preconditions
            if (ImGui.CollapsingHeader("Preconditions"))
            {
                DrawPreconditions();
            }
        }

        /// <summary>
        /// Draws the basic node properties.
        /// </summary>
        private void DrawNodeProperties()
        {
            string name = _selectedNode.Name ?? "";
            if (ImGui.InputText("Name", ref name, 256))
            {
                _selectedNode.Name = name;
            }

            ImGui.Text($"Node Type: {_selectedNode.NodeType ?? "Unknown"}");
            ImGui.Text($"Node Index: {_selectedNode.NodeIndex}");
            ImGui.Text($"GUID: {_selectedNode.GUID ?? "None"}");

            // Flags
            if (_selectedNode.Flags != null && _selectedNode.Flags.Count > 0)
            {
                ImGui.Text("Flags:");
                foreach (var flag in _selectedNode.Flags)
                {
                    ImGui.BulletText(flag);
                }
            }
        }

        /// <summary>
        /// Draws input parameter editors.
        /// </summary>
        private void DrawInputParameters()
        {
            if (_selectedNode.InputParameters == null)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No input parameters");
                return;
            }

            // Int inputs
            if (_selectedNode.InputParameters.Int?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 0.8f, 0.3f, 1.0f));
                ImGui.Text("int");
                ImGui.PopStyleColor();
                for (int i = 0; i < _selectedNode.InputParameters.Int.Count; i++)
                {
                    var param = _selectedNode.InputParameters.Int[i];
                    int value = param.Value;
                    if (ImGui.InputInt($"{param.Name}##{i}", ref value))
                    {
                        param.Value = value;
                    }
                }
            }

            // Bool inputs
            if (_selectedNode.InputParameters.Bool?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.3f, 0.3f, 1.0f));
                ImGui.Text("bool");
                ImGui.PopStyleColor();
                for (int i = 0; i < _selectedNode.InputParameters.Bool.Count; i++)
                {
                    var param = _selectedNode.InputParameters.Bool[i];
                    bool value = param.Value;
                    if (ImGui.Checkbox($"{param.Name}##{i}", ref value))
                    {
                        param.Value = value;
                    }
                }
            }

            // Float inputs
            if (_selectedNode.InputParameters.Float?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 0.6f, 0.9f, 1.0f));
                ImGui.Text("float");
                ImGui.PopStyleColor();
                for (int i = 0; i < _selectedNode.InputParameters.Float.Count; i++)
                {
                    var param = _selectedNode.InputParameters.Float[i];
                    float value = param.Value;
                    if (ImGui.InputFloat($"{param.Name}##{i}", ref value))
                    {
                        param.Value = value;
                    }
                }
            }

            // String inputs
            if (_selectedNode.InputParameters.String?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.6f, 0.3f, 1.0f));
                ImGui.Text("string");
                ImGui.PopStyleColor();
                for (int i = 0; i < _selectedNode.InputParameters.String.Count; i++)
                {
                    var param = _selectedNode.InputParameters.String[i];
                    string value = param.Value ?? "";
                    if (ImGui.InputText($"{param.Name}##{i}", ref value, 1024))
                    {
                        param.Value = value;
                    }
                }
            }

            // User-defined inputs
            if (_selectedNode.InputParameters.UserDefined?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.3f, 0.9f, 1.0f));
                ImGui.Text("userdefined");
                ImGui.PopStyleColor();
                foreach (var param in _selectedNode.InputParameters.UserDefined)
                {
                    ImGui.Text($"  {param.Name} ({param.Class})");
                }
            }
        }

        /// <summary>
        /// Draws output parameter info.
        /// </summary>
        private void DrawOutputParameters()
        {
            if (_selectedNode.OutputParameters == null)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No output parameters");
                return;
            }

            // Int outputs
            if (_selectedNode.OutputParameters.Int?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 0.8f, 0.3f, 1.0f));
                ImGui.Text("int outputs:");
                ImGui.PopStyleColor();
                foreach (var param in _selectedNode.OutputParameters.Int)
                {
                    ImGui.BulletText(param.Name);
                }
            }

            // Bool outputs
            if (_selectedNode.OutputParameters.Bool?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.3f, 0.3f, 1.0f));
                ImGui.Text("bool outputs:");
                ImGui.PopStyleColor();
                foreach (var param in _selectedNode.OutputParameters.Bool)
                {
                    ImGui.BulletText(param.Name);
                }
            }

            // Float outputs
            if (_selectedNode.OutputParameters.Float?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 0.6f, 0.9f, 1.0f));
                ImGui.Text("float outputs:");
                ImGui.PopStyleColor();
                foreach (var param in _selectedNode.OutputParameters.Float)
                {
                    ImGui.BulletText(param.Name);
                }
            }

            // String outputs
            if (_selectedNode.OutputParameters.String?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.6f, 0.3f, 1.0f));
                ImGui.Text("string outputs:");
                ImGui.PopStyleColor();
                foreach (var param in _selectedNode.OutputParameters.String)
                {
                    ImGui.BulletText(param.Name);
                }
            }

            // User-defined outputs
            if (_selectedNode.OutputParameters.UserDefined?.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.3f, 0.9f, 1.0f));
                ImGui.Text("userdefined outputs:");
                ImGui.PopStyleColor();
                foreach (var param in _selectedNode.OutputParameters.UserDefined)
                {
                    ImGui.BulletText($"{param.Name} ({param.Class})");
                }
            }
        }

        /// <summary>
        /// Draws linked nodes information.
        /// </summary>
        private void DrawLinkedNodes()
        {
            if (_selectedNode.LinkedNodes == null)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No linked nodes");
                return;
            }

            if (_selectedNode.LinkedNodes.BoolFloatInputLinkAndOutputLink?.Count > 0)
            {
                ImGui.Text("Flow Links:");
                for (int i = 0; i < _selectedNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count; i++)
                {
                    var link = _selectedNode.LinkedNodes.BoolFloatInputLinkAndOutputLink[i];
                    string label = string.IsNullOrEmpty(link.Parameter) ? $"Output {i}" : link.Parameter;

                    ImGui.PushID(i);
                    ImGui.Text($"  {label} -> ");
                    ImGui.SameLine();

                    // Node selector
                    int nodeIndex = link.NodeIndex;
                    if (ImGui.InputInt("##nodeIndex", ref nodeIndex))
                    {
                        link.NodeIndex = nodeIndex;
                    }

                    // Jump to node button
                    ImGui.SameLine();
                    if (ImGui.SmallButton("Go"))
                    {
                        // TODO: Navigate to linked node
                    }
                    ImGui.PopID();
                }
            }

            if (_selectedNode.LinkedNodes.IntInputLink?.Count > 0)
            {
                ImGui.Text("Int Links:");
                for (int i = 0; i < _selectedNode.LinkedNodes.IntInputLink.Count; i++)
                {
                    var link = _selectedNode.LinkedNodes.IntInputLink[i];
                    string label = string.IsNullOrEmpty(link.Parameter) ? $"Int Link {i}" : link.Parameter;

                    ImGui.PushID(100 + i);
                    ImGui.Text($"  {label} -> ");
                    ImGui.SameLine();

                    int nodeIndex = link.NodeIndex;
                    if (ImGui.InputInt("##nodeIndex", ref nodeIndex))
                    {
                        link.NodeIndex = nodeIndex;
                    }

                    ImGui.SameLine();
                    if (ImGui.SmallButton("Go"))
                    {
                        // TODO: Navigate to linked node
                    }
                    ImGui.PopID();
                }
            }
        }

        /// <summary>
        /// Draws precondition nodes.
        /// </summary>
        private void DrawPreconditions()
        {
            if (_selectedNode.PreconditionNodes == null || _selectedNode.PreconditionNodes.Count == 0)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "No preconditions");
                return;
            }

            ImGui.Text("Precondition Nodes:");
            for (int i = 0; i < _selectedNode.PreconditionNodes.Count; i++)
            {
                int nodeIndex = _selectedNode.PreconditionNodes[i];
                var targetNode = _editorLoader.AinbData?.Nodes?.FirstOrDefault(n => n.NodeIndex == nodeIndex);
                string nodeName = targetNode?.Name ?? "Unknown";

                ImGui.BulletText($"[{nodeIndex}] {nodeName}");
            }
        }

        /// <summary>
        /// Static helper to draw parameter editor for a node in the property window.
        /// </summary>
        public static void DrawPropertyUI(object tag, AINBEditorLoader editorLoader)
        {
            if (tag is AINB.LogicNode node)
            {
                var editor = new AINBNodeParameterEditor(editorLoader);
                editor.SetSelectedNode(node);
                editor.Render();
            }
        }
    }
}
