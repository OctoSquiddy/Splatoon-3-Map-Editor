using System;
using System.Collections.Generic;
using System.Text;
using GLFrameworkEngine;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents a single screen view of a viewport space.
    /// </summary>
    public class ViewportScreen
    {
        public bool IsFocused;

        public string Name { get; set; }

        public Camera Camera { get; set; }

        private IDragDropPicking DragDroppedModel;

        private Framebuffer FinalBuffer;

        public ViewportScreen(string name, Camera camera) {
            Name = name;
            Camera = camera;

            FinalBuffer = new Framebuffer(FramebufferTarget.Framebuffer,
                1, 1, PixelInternalFormat.Rgb16f, 1);
        }

        public void RenderViewportDisplay(ViewportRenderer Pipeline)
        {
           // Pipeline._context.Camera = Camera;

            var size = ImGui.GetWindowSize();
            float pos = ImGui.GetCursorPosX(); ;
            if (Pipeline._context.Camera.UseSquareAspect)
            {
                pos = MathF.Max((size.X - size.Y) / 2, 0);
                size.X = size.Y;
            }

            Camera.Width = (int)size.X;
            Camera.Height = (int)size.Y;

            Pipeline._context.Camera.Width = (int)size.X;
            Pipeline._context.Camera.Height = (int)size.Y;

            if (Pipeline.Width != (int)size.X || Pipeline.Height != (int)size.Y)
            {
                Pipeline.Width = (int)size.X;
                Pipeline.Height = (int)size.Y;

                Pipeline.OnResize(FinalBuffer);
            }

            Pipeline.RenderScene(FinalBuffer);

            //Store the focus state for handling key events
            IsFocused = ImGui.IsWindowFocused();

            if (ImGui.IsAnyMouseDown() && ImGui.IsWindowHovered() && !IsFocused)
            {
                IsFocused = true;
                ImGui.FocusWindow(ImGui.GetCurrentWindow());
            }

            //Make sure the viewport is always focused during transforming
            var transformTools = Pipeline._context.TransformTools;
            bool isTransforming = false;
            bool isPicking = false;
            bool isImguiActive = ImGui.IsAnyItemActive();

            if (transformTools.ActiveActions.Count > 0 && transformTools.TransformSettings.ActiveAxis != TransformEngine.Axis.None)
                isTransforming = true;
            if (Pipeline._context.BoxCreationTool.IsActive)
                isTransforming = true;
            if (Pipeline._context.PickingTools.UseEyeDropper)
                isPicking = true;

            if ((IsFocused && _mouseDown) ||
                (ImGui.IsWindowHovered() && !isImguiActive) || _mouseDown || isTransforming || isPicking)
            {
                Pipeline._context.Focused = true;

                if (!onEnter)
                {
                    Pipeline._context.ResetPrevious();
                    onEnter = true;
                }

                //Only update scene when necessary
                if (ImGuiController.ApplicationHasFocus)
                    UpdateCamera(Pipeline._context);
            }
            else
            {
                onEnter = false;
                Pipeline._context.Focused = false;
            }

            var id = ((GLTexture2D)FinalBuffer.Attachments[0]).ID;

            if (Pipeline._context.Camera.UseSquareAspect)
                ImGui.SetCursorPosX(pos);

            ImGui.Image((IntPtr)id, size, new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            ImGui.SetItemAllowOverlap();

            DrawCustomCursors();

            if (ImGui.BeginDragDropTarget())
            {
                this.IsFocused = true;
                Pipeline._context.Focused = true;

                var mouseInfo = InputState.CreateMouseState();

                ImGuiPayloadPtr outlinerDrop = ImGui.AcceptDragDropPayload("OUTLINER_ITEM",
                    ImGuiDragDropFlags.AcceptNoDrawDefaultRect | ImGuiDragDropFlags.AcceptBeforeDelivery);
                ImGuiPayloadPtr assetDrop = ImGui.AcceptDragDropPayload("ASSET_ITEM",
                    ImGuiDragDropFlags.AcceptNoDrawDefaultRect);

                if (Workspace.ActiveWorkspace.AssetViewWindow.DraggedAsset != null)
                {
                    Pipeline._context.CurrentMousePoint = new OpenTK.Vector2(mouseInfo.X, mouseInfo.Y);
                    Pipeline._context.Scene.SpawnMarker.IsVisible = true;
                    Pipeline._context.Scene.SpawnMarker.SetCursor(false);

                    GLContext.ActiveContext.UpdateViewport = true;
                }

                //Dropping from asset window
                if (assetDrop.IsValid())
                {
                    //Hide spawn cursor during drop
                    Pipeline._context.Scene.SpawnMarker.IsVisible = false;
                    //Drop to workspace.
                    Workspace.ActiveWorkspace.OnAssetViewportDrop();
                }
                //Dropping from outliner window
                if (outlinerDrop.IsValid())
                {
                    GLContext.ActiveContext.UpdateViewport = true;

                    //Drag/drop things onto meshes
                    var picked = Pipeline.GetPickedObject(mouseInfo) as IDragDropPicking;
                    //Picking object changed.
                    if (DragDroppedModel != picked)
                    {
                        //Set exit drop event for previous model
                        if (DragDroppedModel != null)
                            DragDroppedModel.DragDroppedOnLeave();

                        DragDroppedModel = picked;

                        //Model has changed so call the enter event
                        if (picked != null)
                            picked.DragDroppedOnEnter();
                    }

                    if (picked != null)
                    {
                        //Set the drag/drop event
                        var node = Outliner.GetDragDropNode();
                        picked.DragDropped(node.Tag);
                    }
                    if (mouseInfo.LeftButton == OpenTK.Input.ButtonState.Released)
                        DragDroppedModel = null;
                }
                ImGui.EndDragDropTarget();
            }
            else
            {
                //Reset drag/dropped model data if mouse leaves the viewport during a drag event
                if (DragDroppedModel != null)
                {
                    DragDroppedModel.DragDroppedOnLeave();
                    DragDroppedModel = null;
                }
                Pipeline._context.Scene.SpawnMarker.IsVisible = false;
            }
        }

        private void DrawCustomCursors()
        {
            if (MouseEventInfo.MouseCursor == MouseEventInfo.Cursor.Eraser)
            {
                var image = IconManager.GetTextureIcon("ERASER");
                var p = ImGui.GetMousePos();
                p = new System.Numerics.Vector2(p.X - 5, p.Y - 5);

                var csize = new System.Numerics.Vector2(22, 22);
                ImGui.GetWindowDrawList().AddImage((IntPtr)image, p,
                    new System.Numerics.Vector2(p.X + csize.X, p.Y + csize.Y),
                    new System.Numerics.Vector2(0, 0),
                    new System.Numerics.Vector2(1, 1));
            }
        }

        public GLTexture2D SaveAsScreenshotGLTexture(ViewportRenderer renderer, int width, int height, bool enableAlpha = false)
        {
            //Save into an fbo that supports an alpha channel
            Framebuffer fbo = new Framebuffer(FramebufferTarget.Framebuffer,
             1, 1, PixelInternalFormat.Rgba16f, 1);

            var bitmap = renderer.SaveAsScreenshot(fbo, width, height, enableAlpha);
            fbo.Dispose();

            return GLTexture2D.FromBitmap(bitmap);
        }

        public System.Drawing.Bitmap SaveAsScreenshot(ViewportRenderer renderer, int width, int height, bool enableAlpha = false) {
            //Save into an fbo that supports an alpha channel
            Framebuffer fbo = new Framebuffer(FramebufferTarget.Framebuffer,
             1, 1, PixelInternalFormat.Rgba16f, 1);

            var bitmap = renderer.SaveAsScreenshot(fbo, width, height, enableAlpha);
            fbo.Dispose();

            return bitmap;
        }

        private bool onEnter = false;
        private bool _mouseDown = false;

        private void UpdateCamera(GLContext context)
        {
            var mouseInfo = InputState.CreateMouseState();
            var keyInfo = InputState.CreateKeyState();
            KeyEventInfo.State = keyInfo;

            if (ImGui.IsAnyMouseDown() && !_mouseDown)
            {
                context.OnMouseDown(mouseInfo, keyInfo);
                _mouseDown = true;

                Workspace.ActiveWorkspace?.OnMouseDown(mouseInfo);
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) ||
               ImGui.IsMouseReleased(ImGuiMouseButton.Right) ||
               ImGui.IsMouseReleased(ImGuiMouseButton.Middle))
            {
                context.OnMouseUp(mouseInfo);
                _mouseDown = false;
            }

            context.OnMouseMove(mouseInfo, keyInfo, _mouseDown);

            if (ImGuiController.ApplicationHasFocus && IsFocused)
                context.OnMouseWheel(mouseInfo, keyInfo);
            else
                context.ResetPrevious();

            if (this.IsFocused)
                context.Camera.Controller.KeyPress(keyInfo);
        }
    }
}
