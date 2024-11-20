using System;
using System.Collections.Generic;
using Toolbox.Core.ViewModels;
using GLFrameworkEngine;
using ImGuiNET;
using MapStudio.UI;
using UIFramework;

namespace MapStudio.UI
{
    public class PropertyWindow : DockWindow
    {
        public override string Name => "PROPERTIES";

        public object SelectedObject = null;

        NodeBase ActiveNode = null;

        private object ActiveEditor = null;

       public PropertyWindow(DockSpaceWindow parent) : base(parent)
        {

        }

        public override void Render()
        {
            if (SelectedObject is NodeBase)
                Render(SelectedObject as NodeBase);
            if (SelectedObject is AssetItem)
                Render(SelectedObject as AssetItem);
        }

        public void Render(AssetItem asset)
        {
            if (asset.Tag != null)
                ImguiBinder.LoadProperties(asset.Tag, (sender, e) =>
                {
                    var handler = (ImguiBinder.PropertyChangedCustomArgs)e;

                    var type = asset.Tag.GetType().GetProperty(handler.Name);
                    type.SetValue(asset.Tag, sender);
                });
        }

        public void Render(NodeBase node)
        {
            if (node == null)
                return;

            bool valueChanged = ActiveNode != node;
            if (valueChanged)
                ActiveNode = node;

            //Draw overrides
            node.TagUI.UIDrawer?.Invoke(this, EventArgs.Empty);
            //Node properties
            TryLoadNodeUI(node, valueChanged);
            //Property based UI applied to the tags.
            TryLoadPropertyUI(node.Tag, valueChanged);
        }

        private void TryLoadPropertyUI(object obj, bool valueChanged)
        {
            //A UI type that can display rendered IMGUI code.
            if (obj is IPropertyUI)
            {
                var propertyUI = (IPropertyUI)obj;
                if (ActiveEditor == null || ActiveEditor.GetType() != propertyUI.GetTypeUI())
                {
                    var instance = Activator.CreateInstance(propertyUI.GetTypeUI());
                    ActiveEditor = instance;
                }
                if (valueChanged)
                    propertyUI.OnLoadUI(ActiveEditor);

                propertyUI.OnRenderUI(ActiveEditor);
            }
            if (obj is Toolbox.Core.STGenericTexture) {
                ImageEditor.LoadEditor((Toolbox.Core.STGenericTexture)obj);
            }
        }

        private void TryLoadNodeUI(NodeBase obj, bool valueChanged)
        {
            //A UI type that can display rendered IMGUI code.
            if (obj is IPropertyUI)
            {
                var propertyUI = (IPropertyUI)obj;
                if (ActiveEditor == null || ActiveEditor.GetType() != propertyUI.GetTypeUI())
                {
                    var instance = Activator.CreateInstance(propertyUI.GetTypeUI());
                    ActiveEditor = instance;
                }
                if (valueChanged)
                    propertyUI.OnLoadUI(ActiveEditor);

                propertyUI.OnRenderUI(ActiveEditor);
            }

            //Editable object properties
            if (obj is EditableObjectNode)
            {
                DrawEditableObjectProperties((EditableObjectNode)obj);
            }
            else if (obj is RenderablePath.PathNode)
            {
                // DrawEditablePathProperties((RenderablePath.PathNode)obj);
            }
            else if (obj.TagUI.Tag != null) //Generated UI properties using attributes
            {
                ImguiBinder.LoadProperties(obj.TagUI.Tag, OnPropertyChanged);
            }
            else if (obj.Tag != null) //Generated UI properties using attributes
            {
                ImguiBinder.LoadProperties(obj.Tag, OnPropertyChanged);
            }
        }

        private void DrawEditableObjectProperties(NodeBase node)
        {
            var n = node as EditableObjectNode;
            var transform = n.Object.Transform;

            ImguiBinder.LoadProperties(transform, (sender, e) =>
            {
                var handler = (ImguiBinder.PropertyChangedCustomArgs)e;
                var type = transform.GetType().GetProperty(handler.Name);

                List<IRevertable> revertables = new List<IRevertable>();

                //Batch editing
                var selected = Workspace.ActiveWorkspace.GetSelected();
                foreach (var node in selected)
                {
                    if (node is EditableObjectNode)
                    {
                        var editTransform = ((EditableObjectNode)node).Object.Transform;
                        revertables.Add(new TransformUndo(new TransformInfo(editTransform)));

                        type.SetValue(editTransform, sender);
                        editTransform.UpdateMatrix(true);

                        GLContext.ActiveContext.TransformTools.UpdateOrigin();
                        GLContext.ActiveContext.TransformTools.UpdateBoundingBox();
                    }
                }
                GLContext.ActiveContext.Scene.AddToUndo(revertables);
                GLContext.ActiveContext.UpdateViewport = true;
            });

            if (n.UIProperyDrawer != null)
            {
                n.UIProperyDrawer.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (node.Tag != null)
                    ImguiBinder.LoadProperties(node.Tag, OnPropertyChanged);
            }
        }

        private void DrawEditablePathProperties(NodeBase node)
        {
            var n = node as RenderablePath.PathNode;

            if (n.UIProperyDrawer != null)
            {
                n.UIProperyDrawer.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (node.Tag != null)
                    ImguiBinder.LoadProperties(node.Tag, OnPropertyChanged);
            }
        }

        private void OnPropertyChanged(object sender, EventArgs e)
        {
            var handler = (ImguiBinder.PropertyChangedCustomArgs)e;
            //Apply the property
            handler.PropertyInfo.SetValue(handler.Object, sender);

            //Batch editing for selected
            var selected = Workspace.ActiveWorkspace.GetSelected();
            foreach (var node in selected)
            {
                if (node.Tag.GetType() != handler.Object.GetType())
                    continue;

                var tag = node.Tag;
                var type = tag.GetType().GetProperty(handler.Name);
                type.SetValue(tag, sender);
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }
    }
}
