using System;
using System.Collections.Generic;
using System.Linq;
using SampleMapEditor.Ainb;

namespace SampleMapEditor.AINBEditor
{
    /// <summary>
    /// Helper class for AINB node creation, deletion, and manipulation operations.
    /// </summary>
    public static class AINBNodeOperations
    {
        /// <summary>
        /// Creates a new AINB node with a unique index.
        /// </summary>
        public static AINB.LogicNode CreateNode(AINB ainb, string name, string nodeType = "UserDefined")
        {
            int newIndex = GetNextNodeIndex(ainb);

            var node = new AINB.LogicNode
            {
                Name = name,
                NodeType = nodeType,
                NodeIndex = newIndex,
                GUID = GenerateGUID(),
                Flags = new List<string>(),
                PreconditionNodes = new List<int>(),
                InputParameters = new AINB.InputParameters(),
                OutputParameters = new AINB.OutputParameters(),
                LinkedNodes = new AINB.LinkedNodes()
            };

            if (ainb.Nodes == null)
                ainb.Nodes = new List<AINB.LogicNode>();

            ainb.Nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Creates a condition node.
        /// </summary>
        public static AINB.LogicNode CreateConditionNode(AINB ainb, string name)
        {
            var node = CreateNode(ainb, name, "Element_BoolSelector");

            // Add default bool output
            node.OutputParameters.Bool = new List<AINB.OutputBoolParameter>
            {
                new AINB.OutputBoolParameter { Name = "Result" }
            };

            // Add default flow outputs
            node.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
            {
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "True" },
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "False" }
            };

            return node;
        }

        /// <summary>
        /// Creates an action node.
        /// </summary>
        public static AINB.LogicNode CreateActionNode(AINB ainb, string name)
        {
            var node = CreateNode(ainb, name, "Element_Action");

            // Add default flow output
            node.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
            {
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "Done" }
            };

            return node;
        }

        /// <summary>
        /// Creates a selector node.
        /// </summary>
        public static AINB.LogicNode CreateSelectorNode(AINB ainb, string name)
        {
            var node = CreateNode(ainb, name, "Element_Selector");

            // Add initial child slots
            node.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
            {
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "Child 0" },
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "Child 1" }
            };

            return node;
        }

        /// <summary>
        /// Creates a sequence node.
        /// </summary>
        public static AINB.LogicNode CreateSequenceNode(AINB ainb, string name)
        {
            var node = CreateNode(ainb, name, "Element_Sequence");

            // Add initial child slots
            node.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
            {
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "Step 0" },
                new AINB.LinkedNode { NodeIndex = -1, Parameter = "Step 1" }
            };

            return node;
        }

        /// <summary>
        /// Deletes a node and updates all references.
        /// </summary>
        public static bool DeleteNode(AINB ainb, int nodeIndex)
        {
            if (ainb?.Nodes == null)
                return false;

            var node = ainb.Nodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
            if (node == null)
                return false;

            // Remove all links pointing to this node
            foreach (var otherNode in ainb.Nodes)
            {
                if (otherNode.LinkedNodes?.BoolFloatInputLinkAndOutputLink != null)
                {
                    foreach (var link in otherNode.LinkedNodes.BoolFloatInputLinkAndOutputLink)
                    {
                        if (link.NodeIndex == nodeIndex)
                            link.NodeIndex = -1;
                    }
                }

                if (otherNode.LinkedNodes?.IntInputLink != null)
                {
                    foreach (var link in otherNode.LinkedNodes.IntInputLink)
                    {
                        if (link.NodeIndex == nodeIndex)
                            link.NodeIndex = -1;
                    }
                }

                // Remove from precondition lists
                otherNode.PreconditionNodes?.RemoveAll(p => p == nodeIndex);
            }

            // Remove the node itself
            ainb.Nodes.Remove(node);
            return true;
        }

        /// <summary>
        /// Duplicates a node.
        /// </summary>
        public static AINB.LogicNode DuplicateNode(AINB ainb, int sourceNodeIndex)
        {
            var source = ainb?.Nodes?.FirstOrDefault(n => n.NodeIndex == sourceNodeIndex);
            if (source == null)
                return null;

            int newIndex = GetNextNodeIndex(ainb);

            var duplicate = new AINB.LogicNode
            {
                Name = source.Name + "_Copy",
                NodeType = source.NodeType,
                NodeIndex = newIndex,
                GUID = GenerateGUID(),
                Flags = source.Flags?.ToList() ?? new List<string>(),
                PreconditionNodes = new List<int>(), // Don't copy preconditions
                InputParameters = CloneInputParameters(source.InputParameters),
                OutputParameters = CloneOutputParameters(source.OutputParameters),
                LinkedNodes = CloneLinkedNodes(source.LinkedNodes)
            };

            // Clear links in duplicated node (they would create invalid connections)
            if (duplicate.LinkedNodes?.BoolFloatInputLinkAndOutputLink != null)
            {
                foreach (var link in duplicate.LinkedNodes.BoolFloatInputLinkAndOutputLink)
                    link.NodeIndex = -1;
            }
            if (duplicate.LinkedNodes?.IntInputLink != null)
            {
                foreach (var link in duplicate.LinkedNodes.IntInputLink)
                    link.NodeIndex = -1;
            }

            ainb.Nodes.Add(duplicate);
            return duplicate;
        }

        /// <summary>
        /// Creates a connection between two nodes.
        /// </summary>
        public static bool CreateConnection(AINB ainb, int sourceNode, int sourcePort, int targetNode, int targetPort)
        {
            var source = ainb?.Nodes?.FirstOrDefault(n => n.NodeIndex == sourceNode);
            var target = ainb?.Nodes?.FirstOrDefault(n => n.NodeIndex == targetNode);

            if (source == null || target == null)
                return false;

            // Ensure LinkedNodes exists
            if (source.LinkedNodes == null)
                source.LinkedNodes = new AINB.LinkedNodes();
            if (source.LinkedNodes.BoolFloatInputLinkAndOutputLink == null)
                source.LinkedNodes.BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>();

            // If the port index is valid, update the existing link
            if (sourcePort < source.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count)
            {
                source.LinkedNodes.BoolFloatInputLinkAndOutputLink[sourcePort].NodeIndex = targetNode;
            }
            else
            {
                // Add a new link
                source.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
                {
                    NodeIndex = targetNode,
                    Parameter = ""
                });
            }

            return true;
        }

        /// <summary>
        /// Removes a connection from a node.
        /// </summary>
        public static bool RemoveConnection(AINB ainb, int sourceNode, int linkIndex)
        {
            var source = ainb?.Nodes?.FirstOrDefault(n => n.NodeIndex == sourceNode);
            if (source?.LinkedNodes?.BoolFloatInputLinkAndOutputLink == null)
                return false;

            if (linkIndex >= 0 && linkIndex < source.LinkedNodes.BoolFloatInputLinkAndOutputLink.Count)
            {
                source.LinkedNodes.BoolFloatInputLinkAndOutputLink[linkIndex].NodeIndex = -1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an input parameter to a node.
        /// </summary>
        public static void AddInputParameter(AINB.LogicNode node, string name, Type paramType)
        {
            if (node.InputParameters == null)
                node.InputParameters = new AINB.InputParameters();

            if (paramType == typeof(int))
            {
                if (node.InputParameters.Int == null)
                    node.InputParameters.Int = new List<AINB.InputIntParameter>();
                node.InputParameters.Int.Add(new AINB.InputIntParameter { Name = name, Value = 0 });
            }
            else if (paramType == typeof(bool))
            {
                if (node.InputParameters.Bool == null)
                    node.InputParameters.Bool = new List<AINB.InputBoolParameter>();
                node.InputParameters.Bool.Add(new AINB.InputBoolParameter { Name = name, Value = false });
            }
            else if (paramType == typeof(float))
            {
                if (node.InputParameters.Float == null)
                    node.InputParameters.Float = new List<AINB.InputFloatParameter>();
                node.InputParameters.Float.Add(new AINB.InputFloatParameter { Name = name, Value = 0.0f });
            }
            else if (paramType == typeof(string))
            {
                if (node.InputParameters.String == null)
                    node.InputParameters.String = new List<AINB.InputStringParameter>();
                node.InputParameters.String.Add(new AINB.InputStringParameter { Name = name, Value = "" });
            }
        }

        /// <summary>
        /// Adds an output parameter to a node.
        /// </summary>
        public static void AddOutputParameter(AINB.LogicNode node, string name, Type paramType)
        {
            if (node.OutputParameters == null)
                node.OutputParameters = new AINB.OutputParameters();

            if (paramType == typeof(int))
            {
                if (node.OutputParameters.Int == null)
                    node.OutputParameters.Int = new List<AINB.OutputIntParameter>();
                node.OutputParameters.Int.Add(new AINB.OutputIntParameter { Name = name });
            }
            else if (paramType == typeof(bool))
            {
                if (node.OutputParameters.Bool == null)
                    node.OutputParameters.Bool = new List<AINB.OutputBoolParameter>();
                node.OutputParameters.Bool.Add(new AINB.OutputBoolParameter { Name = name });
            }
            else if (paramType == typeof(float))
            {
                if (node.OutputParameters.Float == null)
                    node.OutputParameters.Float = new List<AINB.OutputFloatParameter>();
                node.OutputParameters.Float.Add(new AINB.OutputFloatParameter { Name = name });
            }
            else if (paramType == typeof(string))
            {
                if (node.OutputParameters.String == null)
                    node.OutputParameters.String = new List<AINB.OutputStringParameter>();
                node.OutputParameters.String.Add(new AINB.OutputStringParameter { Name = name });
            }
        }

        /// <summary>
        /// Gets the next available node index.
        /// </summary>
        private static int GetNextNodeIndex(AINB ainb)
        {
            if (ainb?.Nodes == null || ainb.Nodes.Count == 0)
                return 0;

            return ainb.Nodes.Max(n => n.NodeIndex) + 1;
        }

        /// <summary>
        /// Generates a new GUID string.
        /// </summary>
        private static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Clones input parameters.
        /// </summary>
        private static AINB.InputParameters CloneInputParameters(AINB.InputParameters source)
        {
            if (source == null)
                return new AINB.InputParameters();

            return new AINB.InputParameters
            {
                Int = source.Int?.Select(p => new AINB.InputIntParameter
                    { Name = p.Name, Value = p.Value, NodeIndex = p.NodeIndex, ParameterIndex = p.ParameterIndex }).ToList(),
                Bool = source.Bool?.Select(p => new AINB.InputBoolParameter
                    { Name = p.Name, Value = p.Value, NodeIndex = p.NodeIndex, ParameterIndex = p.ParameterIndex }).ToList(),
                Float = source.Float?.Select(p => new AINB.InputFloatParameter
                    { Name = p.Name, Value = p.Value, NodeIndex = p.NodeIndex, ParameterIndex = p.ParameterIndex }).ToList(),
                String = source.String?.Select(p => new AINB.InputStringParameter
                    { Name = p.Name, Value = p.Value, NodeIndex = p.NodeIndex, ParameterIndex = p.ParameterIndex }).ToList(),
                UserDefined = source.UserDefined?.Select(p => new AINB.UserDefinedParameter
                    { Name = p.Name, Class = p.Class, Value = p.Value, NodeIndex = p.NodeIndex, ParameterIndex = p.ParameterIndex }).ToList(),
                Sources = source.Sources?.Select(s => new AINB.InputParameterSource
                    { NodeIndex = s.NodeIndex, ParameterIndex = s.ParameterIndex }).ToList()
            };
        }

        /// <summary>
        /// Clones output parameters.
        /// </summary>
        private static AINB.OutputParameters CloneOutputParameters(AINB.OutputParameters source)
        {
            if (source == null)
                return new AINB.OutputParameters();

            return new AINB.OutputParameters
            {
                Int = source.Int?.Select(p => new AINB.OutputIntParameter { Name = p.Name }).ToList(),
                Bool = source.Bool?.Select(p => new AINB.OutputBoolParameter { Name = p.Name }).ToList(),
                Float = source.Float?.Select(p => new AINB.OutputFloatParameter { Name = p.Name }).ToList(),
                String = source.String?.Select(p => new AINB.OutputStringParameter { Name = p.Name }).ToList(),
                UserDefined = source.UserDefined?.Select(p => new AINB.OutputUserDefinedParameter { Name = p.Name, Class = p.Class }).ToList()
            };
        }

        /// <summary>
        /// Clones linked nodes.
        /// </summary>
        private static AINB.LinkedNodes CloneLinkedNodes(AINB.LinkedNodes source)
        {
            if (source == null)
                return new AINB.LinkedNodes();

            return new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = source.BoolFloatInputLinkAndOutputLink?
                    .Select(l => new AINB.LinkedNode { NodeIndex = l.NodeIndex, Parameter = l.Parameter }).ToList(),
                IntInputLink = source.IntInputLink?
                    .Select(l => new AINB.IntLinkedNode { NodeIndex = l.NodeIndex, Parameter = l.Parameter }).ToList()
            };
        }
    }
}
