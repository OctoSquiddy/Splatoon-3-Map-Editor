using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;
using CurveEditorLibrary;
using Toolbox.Core.Animations;
using UIFramework;

namespace MapStudio.UI
{
    public class AnimationGraphWindow : DockWindow
    {
        public override string Name => "ANIMATION_GRAPH";
        //Only use child window scrolling
        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollWithMouse;

        public static bool ShowConstants = true;

        public bool IsActive => AnimationPlayer.IsPlaying;
        public bool IsRecordMode = false;

        AnimationPlayer AnimationPlayer { get; set; }
        AnimationTimelineControl CurveEditor { get; set; }
        AnimationTree AnimationHierarchy { get; set; }
        DopeSheetEditor DopeSheet;
        public AnimationProperties PropertyWindow;

        public bool IsEditorDopeSheet = true;

        public STAnimation ActiveAnimation;

        float hierachySize = 0;

        private bool _mouseDown;
        private bool onEnter = false;

        private bool updateTimelineRender = true;

        public AnimationGraphWindow(DockSpaceWindow parent) : base(parent)
        {
            Init();
        }

        private void Init()
        {
            hierachySize = 200;
            PropertyWindow = new AnimationProperties();
            AnimationPlayer = new AnimationPlayer();
            CurveEditor = new AnimationTimelineControl();
            AnimationHierarchy = new AnimationTree(PropertyWindow, CurveEditor);
            PropertyWindow.Opened = false;

            CurveEditor.OnLoad();
            CurveEditor.BackColor = System.Drawing.Color.FromArgb(40, 40, 40, 40);
            AnimationPlayer.OnFrameChanged += delegate
            {
                if (CurveEditor.CurrentFrame != (int)AnimationPlayer.CurrentFrame)
                    CurveEditor.CurrentFrame = (int)AnimationPlayer.CurrentFrame;
            };

            CurveEditor.OnFrameChanged += delegate {
                if (AnimationPlayer.CurrentFrame != CurveEditor.CurrentFrame)
                    AnimationPlayer.SetFrame(CurveEditor.CurrentFrame);
            };
            CurveEditor.OnFrameCountChanged += delegate {
                AnimationPlayer.FrameCount = CurveEditor.FrameCount;
                if (ActiveAnimation != null)
                    ActiveAnimation.FrameCount = CurveEditor.FrameCount;
            };
            DopeSheet = new DopeSheetEditor(CurveEditor, AnimationHierarchy);
            AnimationHierarchy.OnValueUpdated += delegate
            {
                AnimationPlayer.SetFrame(AnimationPlayer.CurrentFrame);
            };
            AnimationHierarchy.OnFrameCountUpdated += delegate
            {
                if (ActiveAnimation != null)
                {
                    CurveEditor.FrameCount = (int)ActiveAnimation.FrameCount;
                    AnimationPlayer.FrameCount = ActiveAnimation.FrameCount;
                    CurveEditor.SetFrameRange(ActiveAnimation.FrameCount);
                    //Update frame display
                    CurveEditor.CurrentFrame = (int)ActiveAnimation.Frame;
                    AnimationPlayer.SetFrame(ActiveAnimation.Frame);
                    updateTimelineRender = true;
                }
            };

            //Dock settings
            this.DockDirection = ImGuiDir.Down;
            this.SplitRatio = 0.3f;
            this.Opened = true;
        }

        public void ResetAnimations()
        {
            AnimationPlayer.Reset();
            updateTimelineRender = true;
            AnimationHierarchy.TreeView.Nodes.Clear();
            ActiveAnimation = null;

            GLFrameworkEngine.GLContext.ActiveContext.Camera.ResetAnimations();
        }

        public void ClearAnimations()
        {
            AnimationPlayer.CurrentAnimations.Clear();
            AnimationPlayer.StartFrame = 0;
            AnimationPlayer.CurrentFrame = 0;
            AnimationPlayer.ResetModels();

            updateTimelineRender = true;
        }

        public void SetFrame(float frame)
        {
            AnimationPlayer.SetFrame(frame);
        }

        public void Reset()
        {
            CurveEditor.CurrentFrame = 0;
            CurveEditor.FrameCount = 1;
            AnimationPlayer.Reset(true);

            updateTimelineRender = true;
        }

        public void AddAnimation(STAnimation animation, bool reset = true)
        {
            ActiveAnimation = animation;

            AnimationPlayer.AddAnimation(animation, "", reset);

            DopeSheet.Reset();
            AnimationHierarchy.Load(new List<STAnimation>() { animation });
            //Todo, high frame counts can cause freeze issues atm
            CurveEditor.SetFrameRange(AnimationPlayer.FrameCount, 10);
            AnimationPlayer.SetFrame(AnimationPlayer.CurrentFrame);

            updateTimelineRender = true;
        }

        public override void Render()
        {
            if (ImGui.BeginMenuBar())
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4());

                float height = ImGui.GetFrameHeight();
                Vector2 menuSize = new Vector2(height, height);

                if (IsRecordMode)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ThemeHandler.Theme.Error);
                    if (ImGui.Button($"  {'\uf8d9'}  ", menuSize))
                        IsRecordMode = !IsRecordMode;
                    ImGui.PopStyleColor();
                }
                else
                {
                    if (ImGui.Button($"  {'\uf8d9'}  ", menuSize))
                        IsRecordMode = !IsRecordMode;
                }
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("Records edited keyable values to activate animation");

                if (ImGui.Button($"  {IconManager.RESET_ICON}  ", menuSize))
                    ResetAnimations();

                if (ImGui.IsItemHovered()) ImGui.SetTooltip(TranslationSource.GetText("RESET"));

                if (ImGui.Button($"  {IconManager.PREV_FAST_FORWARD_ICON}  ", menuSize))
                    AdvanceFirstKeyFrame();

                if (ImGui.IsItemHovered()) ImGui.SetTooltip("First frame");

                if (ImGui.Button($"  {IconManager.PREV_STEP_ICON}  ", menuSize))
                    AdvancePreviousKeyFrame();

                if (ImGui.IsItemHovered()) ImGui.SetTooltip("Previous frame");

                if (!AnimationPlayer.IsPlaying)
                {
                    if (ImGui.Button($"  {IconManager.PLAY_ICON}  ", menuSize))
                        AnimationPlayer.Play();
                }
                else
                {
                    if (ImGui.Button($"  {IconManager.PAUSE_ICON}  ", menuSize))
                        AnimationPlayer.Pause();
                }

                if (ImGui.IsItemHovered()) ImGui.SetTooltip($"{(AnimationPlayer.IsPlaying ? "Stop" : "Play")}");

                if (ImGui.Button($"  {IconManager.NEXT_STEP_ICON}  ", menuSize))
                    AdvanceNextKeyFrame();

                if (ImGui.IsItemHovered()) ImGui.SetTooltip("Next frame");

                if (ImGui.Button($"  {IconManager.NEXT_FAST_FORWARD_ICON}  ", menuSize))
                    AdvanceLastKeyFrame();

                if (ImGui.IsItemHovered()) ImGui.SetTooltip("Last frame");

                ImGui.PopStyleColor();

                ImGui.SetNextItemWidth(100);

                float frame = AnimationPlayer.CurrentFrame;
                if (ImGui.DragFloat(TranslationSource.GetText("##FRAME"), ref frame, 1, 0)) {
                    UpdateCurrentFrame(frame);
                }

                if (ActiveAnimation != null)
                {
                    float frameCount = ActiveAnimation.FrameCount;

                    ImGui.SetNextItemWidth(100);
                    if (ImGui.DragFloat(TranslationSource.GetText("##FRAME_COUNT"), ref frameCount, 1, 0))
                    {
                        frameCount = MathF.Max(frameCount, 1.0f);

                        ActiveAnimation.FrameCount = frameCount;
                        AnimationPlayer.FrameCount = frameCount;
                        CurveEditor.SetFrameRange(frameCount);
                        updateTimelineRender = true;
                    }
                    bool loop = ActiveAnimation.Loop;

                    ImGui.SetNextItemWidth(100);
                    if (ImGui.Checkbox(TranslationSource.GetText("LOOP"), ref loop)) {
                        ActiveAnimation.Loop = loop;
                    }
                }

                if (ImGui.Checkbox(TranslationSource.GetText("SHOW_CONSTANTS"), ref AnimationGraphWindow.ShowConstants))
                {
                }
                
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 170);

                ImGui.SetNextItemWidth(120);
                if (ImGui.DragFloat(TranslationSource.GetText("FPS"), ref AnimationPlayer.FrameRate, 1, 1, 240)) {
                    AnimationPlayer.UpdateFramerate();
                }

                ImGui.EndMenuBar();
            }

            var size = ImGui.GetWindowSize();
            var posY = ImGui.GetCursorPosY();

            ImGui.Columns(2);
            hierachySize = ImGui.GetColumnWidth();

            if (ImGui.BeginChild("animation_hierarchy1", new System.Numerics.Vector2(hierachySize, size.Y - posY - 25), true))
            {
                AnimationHierarchy.Render();
            }
            ImGui.EndChild();

            DrawTimelineEditorSwitch();

            ImGui.NextColumn();
            if (ImGui.BeginChild("timeline_child1", new System.Numerics.Vector2(size.X - hierachySize, size.Y - posY), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                DrawDopeSheetTimeline();
            }
            ImGui.EndChild();

            ImGui.NextColumn();

           // DrawProperties();

           // ImGui.NextColumn();
        }

        private void DrawTimelineEditorSwitch()
        {
            //Place the cursor bottom right of the tree
            // ImGui.SetCursorPos(new Vector2(ImGui.GetColumnWidth() - 100, posY - size.Y - 23));
            //Draw buttons to switch editors
            var selectColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
            var buttonColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
            var textColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
            var disabledColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];

            var buttonWidth = hierachySize / 2;

            bool DrawButton(string name, bool selected)
            {
                var color = selected ? selectColor : buttonColor;

                ImGui.PushStyleColor(ImGuiCol.Text, selected ? textColor : disabledColor);
                ImGui.PushStyleColor(ImGuiCol.Button, color);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 1);

                bool clicked = ImGui.Button(name, new Vector2(buttonWidth, 21));

                ImGui.PopStyleColor(4);
                ImGui.PopStyleVar();

                return clicked;
            }

            if (DrawButton("Dope Sheet", IsEditorDopeSheet))
                IsEditorDopeSheet = true;

            ImGui.SameLine();

            if (DrawButton("Curve Editor", !IsEditorDopeSheet))
                IsEditorDopeSheet = false; 
        }

        private void DrawProperties()
        {
            if (DopeSheet.SelectedKeys.Count == 0)
                return;

            var selectedKey = DopeSheet.SelectedKeys.FirstOrDefault();
            float frame = selectedKey.Frame;
            float value = selectedKey.KeyFrame.Value;
            var track = selectedKey.GetTrack();
            var keyFrame = selectedKey.KeyFrame;

            ImGuiHelper.ComboFromEnum<STInterpoaltionType>("Interpolation", track, "InterpolationType");
            ImGuiHelper.ComboFromEnum<STLoopMode>("Wrap", track, "WrapMode");

            if (ImGui.DragFloat($"Frame", ref frame, 1))  
                selectedKey.Frame = frame;
            
            if (ImGui.DragFloat($"Value", ref value, 1))
                selectedKey.KeyFrame.Value = value;

            if (keyFrame is STHermiteKeyFrame)
            {
                var tangentIn = ((STHermiteKeyFrame)keyFrame).TangentIn;
                var tangentOut = ((STHermiteKeyFrame)keyFrame).TangentOut;

                if (ImGui.DragFloat($"Slope In", ref tangentIn))
                    ((STHermiteKeyFrame)keyFrame).TangentIn = tangentIn;

                if (ImGui.DragFloat($"Slope Out", ref tangentOut))
                    ((STHermiteKeyFrame)keyFrame).TangentIn = tangentOut;
            }
            if (keyFrame is STLinearKeyFrame)
            {
                var delta = ((STLinearKeyFrame)keyFrame).Delta;

                if (ImGui.DragFloat($"Delta", ref delta))
                    ((STLinearKeyFrame)keyFrame).Delta = delta;
            }
        }

        private void AdvanceLastKeyFrame()
        {
            if (ActiveAnimation == null)
                return;

            List<STKeyFrame> keys = GetAllKeys();
            var prevKey = keys.LastOrDefault();
            if (prevKey != null)
                UpdateCurrentFrame(prevKey.Frame);
            else
                UpdateCurrentFrame(ActiveAnimation.FrameCount);
        }

        private void AdvanceFirstKeyFrame()
        {
            if (ActiveAnimation == null)
                return;

            List<STKeyFrame> keys = GetAllKeys();
            //Advance to previous key in track
            var nextKey = keys.FirstOrDefault();
            if (nextKey != null)
                UpdateCurrentFrame(nextKey.Frame);
            else
                UpdateCurrentFrame(0);
        }

        private List<STKeyFrame> GetAllKeys()
        {
            List<STKeyFrame> keys = new List<STKeyFrame>();
            foreach (var group in ActiveAnimation.AnimGroups)
                GetAllKeys(group, ref keys);
            return keys.OrderBy(x => x.Frame).ToList();
        }

        static void GetAllKeys(STAnimGroup group, ref List<STKeyFrame> keys)
        {
            foreach (var track in group.GetTracks()) {
                keys.AddRange(track.KeyFrames);
            }

            foreach (var subGroup in group.SubAnimGroups)
                GetAllKeys(subGroup, ref keys);
        }

        private void AdvancePreviousKeyFrame()
        {
            if (ActiveAnimation == null)
                return;

            List<STKeyFrame> keys = GetAllKeys();
            var frame = AnimationPlayer.CurrentFrame;
            var prevKey = keys.LastOrDefault(x => (int)x.Frame < frame);
            if (prevKey != null)
                UpdateCurrentFrame(prevKey.Frame);
        }

        private void AdvanceNextKeyFrame()
        {
            if (ActiveAnimation == null)
                return;

            List<STKeyFrame> keys = GetAllKeys();

            var frame = AnimationPlayer.CurrentFrame;
            var nextKey = keys.FirstOrDefault(x => (int)x.Frame > frame);
            if (nextKey != null)
                UpdateCurrentFrame(nextKey.Frame);
        }

        private void UpdateCurrentFrame(float frame)
        {
            AnimationPlayer.SetFrame(frame);
            CurveEditor.CurrentFrame = (int)frame;
            updateTimelineRender = true;
        }

        private void DrawDopeSheetTimeline()
        {
            var size = ImGui.GetWindowSize();
            var pos = ImGui.GetCursorPos();
            var viewerSize = size - pos;
            if (CurveEditor.Width != viewerSize.X || CurveEditor.Height != viewerSize.Y)
            {
                CurveEditor.Width = (int)viewerSize.X;
                CurveEditor.Height = (int)viewerSize.Y;
                CurveEditor.Resize();
                updateTimelineRender = true;
            }

            if (CurveEditor.FrameCount != AnimationPlayer.FrameCount)
                CurveEditor.FrameCount = (int)AnimationPlayer.FrameCount;

            var backgroundColor = ImGui.GetStyle().Colors[(int)ImGuiCol.MenuBarBg];
            CurveEditor.BGColor = new Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);

            if (ImGui.IsWindowHovered() && ImGui.IsWindowFocused() || _mouseDown)
                UpdateCurveEvents();
            else
            {
                previousMouseWheel = 0;
                onEnter = true;
            }

            if (updateTimelineRender || AnimationPlayer.IsPlaying)
            {
                CurveEditor.Render();
                updateTimelineRender = false;
            }

            var id = CurveEditor.GetTextureID();
            ImGui.Image((IntPtr)id, viewerSize,
                new System.Numerics.Vector2(0, 1),
                new System.Numerics.Vector2(1, 0));

            ImGui.SetCursorPos(pos);
            CurveEditor.DrawText();
            ImGui.SetCursorPos(pos);

            DopeSheet.Render();
        }

        public void OnKeyDown(GLFrameworkEngine.KeyEventInfo state)
        {
            DopeSheet.OnKeyDown(state);
        }

        private float previousMouseWheel;

        private void UpdateCurveEvents()
        {
            var mouseInfo = ImGuiHelper.CreateMouseState();

            bool controlDown = ImGui.GetIO().KeyCtrl;
            bool shiftDown = ImGui.GetIO().KeyShift;

            if (onEnter)
            {
                DopeSheet.ResetMouse(mouseInfo);
                CurveEditor.ResetMouse(mouseInfo);
                onEnter = false;
            }

            if (ImGui.IsAnyMouseDown() && !_mouseDown)
            {
                DopeSheet.OnMouseDown(mouseInfo);
                CurveEditor.OnMouseDown(mouseInfo);
                previousMouseWheel = 0;
                _mouseDown = true;
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) ||
               ImGui.IsMouseReleased(ImGuiMouseButton.Right) ||
               ImGui.IsMouseReleased(ImGuiMouseButton.Middle))
            {
                DopeSheet.OnMouseUp(mouseInfo);
                CurveEditor.OnMouseUp(mouseInfo);
                _mouseDown = false;
            }

            if (previousMouseWheel == 0)
                previousMouseWheel = mouseInfo.WheelPrecise;

            mouseInfo.Delta = mouseInfo.WheelPrecise - previousMouseWheel;
            previousMouseWheel = mouseInfo.WheelPrecise;

            //  if (_mouseDown)
            CurveEditor.OnMouseMove(mouseInfo);
            DopeSheet.OnMouseMove(mouseInfo);

            CurveEditor.OnMouseWheel(mouseInfo, controlDown, shiftDown);

            updateTimelineRender = true;
        }

        public void Dispose()
        {
            AnimationHierarchy.Dispose();
            AnimationPlayer.Stop();
            AnimationPlayer.Dispose();
            ClearAnimations();
        }
    }
}
