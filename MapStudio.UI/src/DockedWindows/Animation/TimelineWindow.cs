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
    public class TimelineWindow : DockWindow
    {
        public override string Name => "TIMELINE";
        //Only use child window scrolling
        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoScrollbar;

        public bool IsActive => AnimationPlayer.IsPlaying;

        AnimationPlayer AnimationPlayer { get; set; }
        AnimationTimelineControl Timeline { get; set; }

        private bool _mouseDown;
        private bool onEnter = false;

        private bool updateTimelineRender = true;

        public TimelineWindow(DockSpaceWindow parent) : base(parent)
        {
           Init();
        }

        private void Init()
        {
            AnimationPlayer = new AnimationPlayer();
            Timeline = new AnimationTimelineControl();

            Timeline.OnLoad();
            Timeline.BackColor = System.Drawing.Color.FromArgb(40, 40, 40, 40);
            AnimationPlayer.OnFrameChanged += delegate
            {
                if (Timeline.CurrentFrame != (int)AnimationPlayer.CurrentFrame)
                    Timeline.CurrentFrame = (int)AnimationPlayer.CurrentFrame;
            };

            Timeline.OnFrameChanged += delegate {
                if (AnimationPlayer.CurrentFrame != Timeline.CurrentFrame)
                    AnimationPlayer.SetFrame(Timeline.CurrentFrame);
            };
            Timeline.OnFrameCountChanged += delegate {
                AnimationPlayer.FrameCount = Timeline.FrameCount;
            };

            //Dock settings
            this.DockDirection = ImGuiDir.Down;
            this.SplitRatio = 0.3f;
            this.Opened = true;
        }

        public void ResetAnimations()
        {
            AnimationPlayer.CurrentAnimations.Clear();
            AnimationPlayer.ResetModels();
        }

        public void ClearAnimations() {
            AnimationPlayer.CurrentAnimations.Clear();
            AnimationPlayer.StartFrame = 0;
            AnimationPlayer.CurrentFrame = 0;
            AnimationPlayer.ResetModels();
            updateTimelineRender = true;
        }

        public void SetFrame(float frame) {
            AnimationPlayer.SetFrame(frame);
        }

        public void Reset() {
            Timeline.CurrentFrame = 0;
            Timeline.FrameCount = 1;
            AnimationPlayer.Reset(true);

            updateTimelineRender = true;
        }

        public void AddAnimation(STAnimation animation, bool reset = true) {
            AnimationPlayer.AddAnimation(animation, "", reset);
            AnimationPlayer.SetFrame(0);
            Timeline.SetFrameRange(AnimationPlayer.FrameCount);

            updateTimelineRender = true;
        }

        public override void Render()
        {
            ImGui.PushItemWidth(200);
            if (ImGui.Button(TranslationSource.GetText("RESET"))) {
                Reset();
            }
            ImGui.SameLine();

            ImGui.PopItemWidth();
            ImGui.PushItemWidth(200);

            if (!AnimationPlayer.IsPlaying)
            {
                if (ImGui.Button($"  {IconManager.PLAY_ICON}  ")) {
                    AnimationPlayer.Play();
                }
            }
            else
            {
                if (ImGui.Button($"  {IconManager.PAUSE_ICON}  ")) {
                    AnimationPlayer.Pause();
                }
            }
            ImGui.PopItemWidth();

            ImGui.SameLine();
            ImGui.SetNextItemWidth(150);

            if (ImGui.DragFloat(TranslationSource.GetText("FRAMERATE"), ref AnimationPlayer.FrameRate, 1, 1, 240)) {
                AnimationPlayer.UpdateFramerate();
            }

            var size = ImGui.GetWindowSize();
            var pos = ImGui.GetCursorPosY();
            if (ImGui.BeginChild("timeline_child1", new Vector2(size.X, size.Y - pos - 5), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                DrawCurveTimeline();
            }
            ImGui.EndChild();
        }

        private void DrawCurveTimeline()
        {
            var size = ImGui.GetWindowSize();
            var pos = ImGui.GetCursorPos();
            var viewerSize = size - pos;
            if (Timeline.Width != viewerSize.X || Timeline.Height != viewerSize.Y)
            {
                Timeline.Width = (int)viewerSize.X;
                Timeline.Height = (int)viewerSize.Y;
                Timeline.Resize();
                updateTimelineRender = true;
            }

            if (Timeline.FrameCount != AnimationPlayer.FrameCount)
                Timeline.FrameCount = (int)AnimationPlayer.FrameCount;

            var backgroundColor = ImGui.GetStyle().Colors[(int)ImGuiCol.MenuBarBg];
            Timeline.BGColor = new Vector4(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);

            if (ImGui.IsWindowHovered() && ImGui.IsWindowFocused() || _mouseDown)
                UpdateCurveEvents();
            else
            {
                previousMouseWheel = 0;
                onEnter = true;
            }

            if (updateTimelineRender || AnimationPlayer.IsPlaying) {
                Timeline.Render();
                updateTimelineRender = false;
            }

            var id = Timeline.GetTextureID();
            ImGui.Image((IntPtr)id, viewerSize,
                new System.Numerics.Vector2(0, 1),
                new System.Numerics.Vector2(1, 0));

            ImGui.SetCursorPos(pos);
            Timeline.DrawText();
        }

        private float previousMouseWheel;

        private void UpdateCurveEvents()
        {
            var mouseInfo = ImGuiHelper.CreateMouseState();

            bool controlDown = ImGui.GetIO().KeyCtrl;
            bool shiftDown = ImGui.GetIO().KeyShift;

            if (onEnter)
            {
                Timeline.ResetMouse(mouseInfo);
                onEnter = false;
            }

            if (ImGui.IsAnyMouseDown() && !_mouseDown)
            {
                Timeline.OnMouseDown(mouseInfo);
                previousMouseWheel = 0;
                _mouseDown = true;
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) ||
               ImGui.IsMouseReleased(ImGuiMouseButton.Right) ||
               ImGui.IsMouseReleased(ImGuiMouseButton.Middle))
            {
                Timeline.OnMouseUp(mouseInfo);
                _mouseDown = false;
            }

            if (previousMouseWheel == 0)
                previousMouseWheel = mouseInfo.WheelPrecise;

            mouseInfo.Delta = mouseInfo.WheelPrecise - previousMouseWheel;
            previousMouseWheel = mouseInfo.WheelPrecise;

            //  if (_mouseDown)
            Timeline.OnMouseMove(mouseInfo);
            Timeline.OnMouseWheel(mouseInfo, controlDown, shiftDown);

            updateTimelineRender = true;
        }

        public void Dispose()
        {
            AnimationPlayer.Stop();
            AnimationPlayer.Dispose();
            ClearAnimations();
        }
    }
}
