using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using ImGuiNET;
using Toolbox.Core.Animations;
using CurveEditorLibrary;
using GLFrameworkEngine;
using UIFramework;

namespace MapStudio.UI
{
    public class DopeSheetEditor
    {
        //Copy
        List<AnimationTree.KeyNode> CopiedKeys = new List<AnimationTree.KeyNode>();

        //Selection
        public List<AnimationTree.KeyNode> SelectedKeys = new List<AnimationTree.KeyNode>();
        public List<float> PreDragFrames = new List<float>();

        //Undo/redo
        private UndoStack UndoStack = new UndoStack();

        //The boundings of the current selection
        private BoundingBox2D SelectionBounding;
        private float SelectedFrameMin;
        private float SelectedFrameMax;

        //The selection box tool
        private UIFramework.SelectionBox SelectionBox = new UIFramework.SelectionBox();

        //Dope sheet editor comprised of a tree and a timeline
        private AnimationTimelineControl Timeline;
        private AnimationTree Tree;

        private ResizeMode _ResizeMode = ResizeMode.None;
        private ResizeOperation ResizeOp = null;

        private bool _IsMoving;

        //Movement
        private bool IsMoving
        {
            get { return _IsMoving; }
            set
            {
                if (_IsMoving != value)
                {
                    if (value)
                        SetupMoveOperation();

                    //A check for when the movement has just been set (for undo operations)
                    movementChanged = value;
                    _IsMoving = value;
                }
            }
        }

        Vector2 lastMousePos;
        Vector2 mouseDownPos;

        bool movementChanged = false;
        bool _mouseDown = false;

        public DopeSheetEditor(AnimationTimelineControl timeline, AnimationTree tree) {
            Timeline = timeline;
            Tree = tree;
            tree.OnRemoved += delegate
            {
                DeselectAll();
            };
            SelectionBounding = new BoundingBox2D(new Vector2(), new Vector2());
            SelectionBox.OnSelectionStart += delegate
            {
                DeselectAll();
            };
        }

        //deselects all keys currently selected
        private void DeselectAll()
        {
            foreach (var key in SelectedKeys)
            {
                key.SelectedByBox = false;
                key.IsSelected = false;
            }
            SelectedKeys.Clear();
        }

        /// <summary>
        /// Clears all selected keys from the dope sheet
        /// </summary>
        public void Reset()
        {
            SelectedKeys.Clear();
        }

        /// <summary>
        /// Draw the dope sheet.
        /// </summary>
        public void Render()
        {
            //Enable selection box unless moving
            SelectionBox.Enabled = IsMoving ? false : true;

            if (SelectedKeys.Count > 0)
            {
                var pos = ImGui.GetCursorPos();
                var screenPos = ImGui.GetCursorScreenPos();

                //Hit detection via the bounds of the dope key
                var minBox = SelectionBounding.Min;
                var maxBox = SelectionBounding.Max;

                var frameMin = SelectedFrameMin;
                var frameMax = SelectedFrameMax;

                //Draw current selected frame
                ImGui.SetCursorScreenPos(new Vector2(minBox.X, screenPos.Y + 23));

                ImGuiHelper.BeginBoldText();

                //Draw displays for the smallest frame during a selection
                var p = ImGui.GetCursorScreenPos();
                var size = ImGui.CalcTextSize($"{frameMin}");
                ImGui.GetWindowDrawList().AddRectFilled(
                         new Vector2(p.X, p.Y),
                         new Vector2(p.X + size.X, p.Y + size.Y),
                         ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

                ImGui.Text($"{frameMin}");

                //Draw displays for the largest frame during a selection if more that 2 keys are selected
                if (frameMin != frameMax) {
                    size = ImGui.CalcTextSize($"{frameMax}");

                    ImGui.SetCursorScreenPos(new Vector2(maxBox.X, screenPos.Y + 23));
                     p = ImGui.GetCursorScreenPos();
                    ImGui.GetWindowDrawList().AddRectFilled(
                        new Vector2(p.X, p.Y),
                        new Vector2(p.X + size.X, p.Y + size.Y),
                        ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

                    ImGui.Text($"{frameMax}");
                }
                ImGuiHelper.EndBoldText();

                ImGui.SetCursorPos(pos);
            }

            //Current window position - scroll of the tree view
            float cursorPosY = ImGui.GetCursorPosY() - Tree.TreeView.GetScrollY();
            //Draw through the node hiearchy and draw the keyed tracks
            foreach (var node in Tree.TreeView.Nodes)
                DrawDopeSheet(node, ref cursorPosY);

            //Draw selection boundary
            DrawSelectionBounds();
            //Draw selection box
            if (Timeline.IsHoverOverCurrentFrameHandle && !SelectionBox.IsActive)
                return;

            if ((ImGui.IsWindowHovered() || SelectionBox.IsActive) && !ImGui.IsAnyItemHovered())
                SelectionBox.Render();
        }

        //Draws a box around the selected dope keys for transforming
        private void DrawSelectionBounds()
        {
            if (SelectedKeys.Count == 0)
                return;

            CalculateSelection();

            var minBox = SelectionBounding.Min;
            var maxBox = SelectionBounding.Max;

            ImGui.GetWindowDrawList().AddRectFilled(minBox, maxBox,
               ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
        }

        //Reset previous mouse position
        public void ResetMouse(MouseEventInfo mouse)
        {
            lastMousePos = new Vector2(mouse.X, mouse.Y);
        }

        private void DrawDopeSheet(UIFramework.TreeNode node, ref float cursorPosY)
        {
            //Draw the dope sheet according to the tree hiearchy
            if (node is AnimationTree.TrackNode)
            {
                //Track node drawing
                var trackNode = node as AnimationTree.TrackNode;
                //Node filtering
                if (!AnimationGraphWindow.ShowConstants && 
                    trackNode.Track.InterpolationType == STInterpoaltionType.Constant)
                {
                    return;
                }

                DrawDopeSheet(trackNode.Keys, cursorPosY);
            }
            //Draw color groups using a colored bar around the sheet
            if (node is AnimationTree.ColorGroupNode)
            {
                var colorNode = node as AnimationTree.ColorGroupNode;
                DrawColorSheet(colorNode, cursorPosY);
            }
            //Shift each position to match the current tree node hiearchy
            cursorPosY += ImGui.GetFrameHeight() + 1;
            if (node.IsExpanded)
            {
                foreach (var c in node.Children)
                    DrawDopeSheet(c, ref cursorPosY);
            }
        }

        private void DrawColorSheet(AnimationTree.ColorGroupNode colorNode, float cursorPosY)
        {
            //Timeline resized too small, don't display
            if (Timeline.Height - 40 <= 0)
                return;

            //Starting cursor pos
            var curPos = ImGui.GetCursorPos();

            int height = 1;
            int width = (int)(this.Timeline.Width);

            //Prepare color texture. Use floating format for values higher than 1.0
            if (colorNode.ColorSheet == null)
                colorNode.ColorSheet = GLTexture2D.CreateUncompressedTexture(width, height,
                     OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba32f,
                      OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                      OpenTK.Graphics.OpenGL.PixelType.Float);

            //Resize the color sheet to span the timeline
            if (colorNode.ColorSheet.Width != width)
                colorNode.ColorSheet.Width = width;

            //Color sheet to input full color bar into a texture
            UpdateColorSheet(colorNode);

            //Move the cursor in range where the provided track tree node is at
            ImGui.SetCursorPosY(cursorPosY + 3);
            //Draw the color sheet
            ImGui.Image((IntPtr)colorNode.ColorSheet.ID, new Vector2(this.Timeline.Width, ImGui.GetFrameHeight() - 2));

            ImGui.SetCursorPos(curPos);
        }

        private void UpdateColorSheet(AnimationTree.ColorGroupNode colorNode)
        {
            int height = 1;
            int width = (int)(this.Timeline.Width);

            float[] data = new float[width * height * 4];
            //Create a 1D texture sheet from the span of the timeline covering all the colors
            int index = 0;
            for (int i = 0; i < Timeline.Width; i++)
            {
                float time = Timeline.XToFrame(i);
                var color = colorNode.GetTrackColor(time);
                data[index + 0] = color.X;
                data[index + 1] = color.Y;
                data[index + 2] = color.Z;
                data[index + 3] = color.W;
                index += 4;
            }
            colorNode.ColorSheet.Reload(width, height, data);
        }

        public void DrawDopeSheet(List<AnimationTree.KeyNode> keyFrames, float cursorPosY)
        {
            //Timeline resized too small, don't display
            if (Timeline.Height - 40 <= 0)
                return;

            //Check if the window has been clicked
            bool clicked = ImGui.IsMouseDown(0) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered() && !ImGui.IsAnyItemHovered();

            //Move the cursor in range where the provided track tree node is at
            ImGui.SetCursorPosY(cursorPosY);
            //Get the position in screen coordinates for custom drawing
            var screenPos = ImGui.GetCursorScreenPos();
            //Total width from a frame value
            float frameWidth = Timeline.FramesToPixelsX(1f);
            //Starting cursor pos
            var curPos = ImGui.GetCursorPos();

            //Offset of the min frame range
            float offset = Timeline.FramesToPixelsX(Timeline.frameRangeMin);

            //Draw the full bar representing the tracks ranges
            var minBox = screenPos;
            var maxBox = screenPos + new Vector2(Timeline.Width, ImGui.GetFrameHeight());

            ImGui.GetWindowDrawList().AddRectFilled(minBox, maxBox,
               ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 0.1f)));

            foreach (var keyFrame in keyFrames)
            {
                float height = ImGui.GetFrameHeight();
                var color = new Vector4(1);
                if (keyFrame.IsSelected)
                    color = new Vector4(1, 1, 0, 1.0f);

                //Draw at the frame position
                var pos = screenPos + new Vector2(keyFrame.Frame * frameWidth - offset, height / 2);
                //Circle to represent a keyed value
                ImGui.GetWindowDrawList().AddCircleFilled(
                    new Vector2(pos.X, pos.Y), 5,
                    ImGui.ColorConvertFloat4ToU32(color));
                //Add a border around the circle
                ImGui.GetWindowDrawList().AddCircle(new Vector2(pos.X, pos.Y), 5,
                ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 1)));
                //Hit detection for selecting keys
                float hitboxSize = 5;
                var min = new Vector2(pos.X - hitboxSize, pos.Y - hitboxSize);
                var max = new Vector2(pos.X + hitboxSize, pos.Y + hitboxSize);

                keyFrame.Min = min;
                keyFrame.Max = max;

                //Select via selection box
                if (SelectionBox.IsActive)
                {
                    SelectionBox.CheckFrameSelection(keyFrame, min, max);
                    if (keyFrame.IsSelected && !SelectedKeys.Contains(keyFrame))
                    {
                        keyFrame.SelectedByBox = true;
                        SelectedKeys.Add(keyFrame);
                        SelectionChanged();
                    }
                    if (!keyFrame.IsSelected && SelectedKeys.Contains(keyFrame))
                    {
                        SelectedKeys.Remove(keyFrame);
                        keyFrame.SelectedByBox = false;
                        SelectionChanged();
                    }
                } //Select via mouse click
                else if (!IsMoving && clicked && ImGui.IsMouseHoveringRect(min, max))
                {
                    if (!ImGui.GetIO().KeyCtrl && !keyFrame.SelectedByBox)
                    {
                        DeselectAll();
                        SelectionChanged();
                    }

                    SelectionBox.Enabled = false;

                    if (!SelectedKeys.Contains(keyFrame)) {
                        SelectedKeys.Add(keyFrame);
                        SelectionChanged();
                    }
                    keyFrame.IsSelected = true;
                    IsMoving = true;
                }
            }

            ImGui.SetCursorPos(curPos);
        }

        public void Undo()
        {
            UndoStack.Undo();
        }

        public void Redo()
        {
            UndoStack.Redo();
        }

        public void OnKeyDown(KeyEventInfo keyDown)
        {
            if (keyDown.IsKeyDown(InputSettings.INPUT.Scene.Delete)) 
                DeleteSelectedKeys();
            if (keyDown.IsKeyDown(InputSettings.INPUT.Scene.Undo)) 
                Undo();
            if (keyDown.IsKeyDown(InputSettings.INPUT.Scene.Redo))
                Redo();
            if (keyDown.IsKeyDown(InputSettings.INPUT.Scene.Copy))
                CopySelected();
            if (keyDown.IsKeyDown(InputSettings.INPUT.Scene.Paste))
                PasteSelected();
        }

        private void CopySelected()
        {
            CopiedKeys.Clear();
            foreach (var keyNode in this.SelectedKeys)
                CopiedKeys.Add(keyNode.Copy());
        }

        private void PasteSelected()
        {
            DeselectAll();
            foreach (var copy in CopiedKeys)
            {
                //Add to the parent track
                copy.TryAddFromTrack();
                //select new copy
                copy.IsSelected = true;
                SelectedKeys.Add(copy);
            }
        }

        public void OnMouseMove(MouseEventInfo mouseInfo)
        {
            const float edgeSelectionSize = 10;

            MouseEventInfo.MouseCursor = MouseEventInfo.Cursor.Arrow;
            if (_ResizeMode != ResizeMode.None)
                MouseEventInfo.MouseCursor = MouseEventInfo.Cursor.ResizeEW;

            if (SelectionBounding.Max.X - SelectionBounding.Min.X > 20)
            {
                //Selection box resizing (right side)
                if (ImGui.IsMouseHoveringRect(new Vector2(
                    SelectionBounding.Max.X - edgeSelectionSize, SelectionBounding.Min.Y),
                    new Vector2(SelectionBounding.Max.X, SelectionBounding.Max.Y)))
                {
                    MouseEventInfo.MouseCursor = MouseEventInfo.Cursor.ResizeEW;
                }
                //Selection box resizing (left side)
                if (ImGui.IsMouseHoveringRect(new Vector2(
                    SelectionBounding.Min.X, SelectionBounding.Min.Y),
                    new Vector2(SelectionBounding.Min.X + edgeSelectionSize, SelectionBounding.Max.Y)))
                {
                 //   MouseEventInfo.MouseCursor = MouseEventInfo.Cursor.ResizeEW;
                }
            }

            //Move selected
            if (IsMoving && mouseInfo.LeftButton == OpenTK.Input.ButtonState.Pressed)
            {
                float currentFrame = Timeline.XToFrame(mouseInfo.X);
                float previousFrame = Timeline.XToFrame(mouseDownPos.X);
                float diff = (int)Math.Round(currentFrame - previousFrame);

                if (movementChanged && diff != 0) {
                    movementChanged = false;
                    UndoStack.AddToUndo(new KeyMoveOperation(SelectedKeys.ToList()));
                }

                //Check if the edge is clicked on or not
                if(_ResizeMode != ResizeMode.None && diff != 0)
                {
                    if (ResizeOp == null)
                        ResizeOp = new ResizeOperation(previousFrame,
                            SelectedKeys.Min(x => x.Frame), 
                            SelectedKeys.Max(x => x.Frame), SelectedKeys);

                    ResizeOp.Resize(currentFrame, SelectedKeys, _ResizeMode);
                }
                else if (diff != 0)
                {
                    //Move selected
                    for (int i = 0; i < SelectedKeys.Count; i++)
                        SelectedKeys[i].Frame = PreDragFrames[i] + diff;
                }
            }
            lastMousePos = new Vector2(mouseInfo.X, mouseInfo.Y);
        }

        class ResizeOperation
        {
            private float PreviousFrame;
            private float LeftFrame;
            private float RightFrame;
            private List<float> previousFrames = new List<float>();

            public ResizeOperation(float previousFrame, float leftFrame, float rightFrame, List<AnimationTree.KeyNode> keys)
            {
                PreviousFrame = previousFrame;
                LeftFrame = leftFrame;
                RightFrame = rightFrame;
                previousFrames = keys.Select(x => x.Frame).ToList();
            }

            public void Resize(float currentFrame, List<AnimationTree.KeyNode> keys, ResizeMode mode)
            {
                float diff = (int)Math.Round(currentFrame - PreviousFrame);
                //Resize selected
                float newFrameCount = PreviousFrame + diff;

                if (mode == ResizeMode.Right)
                {
                    float ratio = (float)newFrameCount / (float)RightFrame;

                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i].Frame != LeftFrame)
                        {
                            int newFrame = (int)MathF.Max(((float)previousFrames[i] * ratio + 0.5f), 0);
                            if (newFrame > LeftFrame)
                                keys[i].Frame = newFrame;
                        }
                    }
                }
                if (mode == ResizeMode.Left)
                {
                    float ratio = LeftFrame == 0 ? newFrameCount / (float)1.0f : (float)newFrameCount / (float)LeftFrame;

                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i].Frame != RightFrame)
                        {
                            float value = previousFrames[i] == 0 ? newFrameCount : previousFrames[i] * ratio + 0.5f;
                            int newFrame = (int)MathF.Max((value), 0);
                            if (newFrame < RightFrame)
                                keys[i].Frame = newFrame;
                        }
                    }
                }
            }
        }

        public void OnMouseDown(MouseEventInfo mouseInfo)
        {
            mouseDownPos = new Vector2(mouseInfo.X, mouseInfo.Y);

            _mouseDown = true;
            //Selection box movement
            if(SelectedKeys.Count > 0)
            {
                if (ImGui.IsMouseHoveringRect(SelectionBounding.Min, SelectionBounding.Max))
                    IsMoving = true;

                const float edgeSelectionSize = 10;

                if (SelectionBounding.Max.X - SelectionBounding.Min.X > 20)
                {
                    //Selection box resizing (right side)
                    if (ImGui.IsMouseHoveringRect(new Vector2(
                    SelectionBounding.Max.X - edgeSelectionSize, SelectionBounding.Min.Y),
                    new Vector2(SelectionBounding.Max.X, SelectionBounding.Max.Y)))
                    {
                        _ResizeMode = ResizeMode.Right;
                    }
                    //Selection box resizing (left side)
                    if (ImGui.IsMouseHoveringRect(new Vector2(
                        SelectionBounding.Min.X, SelectionBounding.Min.Y),
                        new Vector2(SelectionBounding.Min.X + edgeSelectionSize, SelectionBounding.Max.Y)))
                    {
                     //   _ResizeMode = ResizeMode.Left;
                    }
                }
            }
        }

        private void SetupMoveOperation()
        {
            PreDragFrames.Clear();
            for (int i = 0; i < SelectedKeys.Count; i++)
                PreDragFrames.Add(SelectedKeys[i].Frame);
        }

        public void OnMouseUp(MouseEventInfo mouseInfo)
        {
            _ResizeMode = ResizeMode.None;
            IsMoving = false;
            _mouseDown = false;
            movementChanged = false;
            ResizeOp = null;
        }

        private void DeleteSelectedKeys()
        {
            UndoStack.AddToUndo(new KeyDeleteOperation(SelectedKeys.ToList()));

            foreach (var key in SelectedKeys)
                key.TryRemoveFromTrack();

            SelectedKeys.Clear();
        }

        public void SelectionChanged() 
        {
        }

        public void CalculateSelection()
        {
            if (SelectedKeys.Count == 0)
                return;

            SelectionBounding.Max = new Vector2(float.MinValue);
            SelectionBounding.Min = new Vector2(float.MaxValue);
            SelectedFrameMin = float.MaxValue;
            SelectedFrameMax = float.MinValue;

            //Calculate min/max
            foreach (var item in SelectedKeys)
            {
                SelectedFrameMin = MathF.Min(SelectedFrameMin, item.Frame);
                SelectedFrameMax = MathF.Max(SelectedFrameMax, item.Frame);

                SelectionBounding.Max = new Vector2(
                    MathF.Max(SelectionBounding.Max.X, item.Max.X),
                    MathF.Max(SelectionBounding.Max.Y, item.Max.Y));
                SelectionBounding.Min = new Vector2(
                    MathF.Min(SelectionBounding.Min.X, item.Min.X),
                    MathF.Min(SelectionBounding.Min.Y, item.Min.Y));
            }
        }

        class KeyMoveOperation : IRevertable
        {
            List<KeyEntry> Keys = new List<KeyEntry>();

            public KeyMoveOperation(List<KeyEntry> keys) {
                Keys = keys;
            }

            public KeyMoveOperation(List<AnimationTree.KeyNode> keyFrames)
            {
                foreach (var key in keyFrames)
                    Keys.Add(new KeyEntry()
                    {
                        KeyData = key,
                        Frame = key.Frame,
                    });
            }

            public IRevertable Revert()
            {
                var prev = new KeyMoveOperation(Keys.ToList());

                //Add back to the tracks
                for (int i = 0; i < Keys.Count; i++)
                    Keys[i].KeyData.Frame = Keys[i].Frame;

                return prev;
            }

            public class KeyEntry
            {
                public AnimationTree.KeyNode KeyData;
                public float Frame;
            }
        }

        enum ResizeMode
        {
            None,
            Left,
            Right,
        }

        class KeyDeleteOperation : IRevertable
        {
            List<AnimationTree.KeyNode> RemovedKeys;

            public KeyDeleteOperation(List<AnimationTree.KeyNode> keyFrames) {
                RemovedKeys = keyFrames;
            }

            public IRevertable Revert()
            {
                //Add back to the tracks
                foreach (var key in RemovedKeys)
                    key.TryAddFromTrack();

                return new KeyAddOperation(RemovedKeys);
            }
        }

        class KeyAddOperation : IRevertable
        {
            List<AnimationTree.KeyNode> RemovedKeys;

            public KeyAddOperation(List<AnimationTree.KeyNode> keyFrames) {
                RemovedKeys = keyFrames;
            }

            public IRevertable Revert()
            {
                //Remove from the tracks
                foreach (var key in RemovedKeys)
                    key.TryRemoveFromTrack();

                return new KeyAddOperation(RemovedKeys);
            }
        }
    }
}
