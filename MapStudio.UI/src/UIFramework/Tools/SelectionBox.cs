using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using ImGuiNET;
using Toolbox.Core;

namespace UIFramework
{
    public class SelectionBox
    {
        public EventHandler OnSelectionStart;
        public EventHandler OnSelectionApplied;

        /// <summary>
        /// Checks if the selection box start/end points are not the same.
        /// </summary>
        public bool HasChange => selectionStartPosition != selectionEndPosition;

        /// <summary>
        /// Determines if the selection box is enabled or not.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// Determines if the selection box is currently active or not.
        /// </summary>
        public bool IsActive { get; private set; } = false;
        
        //Elements to track overlapping
        List<ISelectableElement> overlappedElements = new List<ISelectableElement>();

        //Selection box
        private Vector2 selectionStartPosition;
        private Vector2 selectionEndPosition;

        private BoundingBox2D Bounding;

        public void HandleInputs()
        {
            bool clicked = ImGui.IsMouseClicked(0);
            bool down = ImGui.IsMouseDown(0);
            bool released = ImGui.IsMouseReleased(0);
            //Start selection
            if (clicked && !IsActive && Enabled)
            {
                selectionStartPosition = ImGui.GetMousePos();
                selectionEndPosition = ImGui.GetMousePos();
                IsActive = true;
            }
            else if (down && IsActive) //Drag selection point
            {
                bool previousMatch = selectionStartPosition == selectionEndPosition;
                selectionEndPosition = ImGui.GetMousePos();

                //Event for when the selection starts and changes at first
                if (previousMatch && HasChange)
                    OnSelectionStart?.Invoke(this, EventArgs.Empty);

                //Calculate bounding
                Vector2 min = new Vector2(
                    MathF.Min(selectionStartPosition.X, selectionEndPosition.X),
                    MathF.Min(selectionStartPosition.Y, selectionEndPosition.Y));
                Vector2 max = new Vector2(
                    MathF.Max(selectionStartPosition.X, selectionEndPosition.X),
                    MathF.Max(selectionStartPosition.Y, selectionEndPosition.Y));
                Bounding = new BoundingBox2D(min, max);
            }
            //Stop selection box
            if ((!down || released) && IsActive)
            {
                OnSelectionApplied?.Invoke(this, EventArgs.Empty);
                Reset();
            }
        }

        /// <summary>
        /// Resets the selection box back to the default state.
        /// </summary>
        public void Reset()
        {
            if (!IsActive)
                return;

            overlappedElements.Clear();
            selectionStartPosition = Vector2.Zero;
            selectionEndPosition = Vector2.Zero;
            IsActive = false;
            Bounding = null;
        }

        /// <summary>
        /// Checks if the UI elements frame currently intersects.
        /// This method must be called after a UI element has been drawn.
        /// </summary>
        public void CheckFrameSelection(ISelectableElement node)
        {
            //Don't activate if not active or the box hasn't been changed yet
            if (!IsActive || Bounding == null || !HasChange)
                return;

            CheckFrameSelection(node, ImGui.GetItemRectMin(), ImGui.GetItemRectMax());
        }

        /// <summary>
        /// Checks if the UI elements frame currently intersects.
        /// This method must be called after a UI element has been drawn.
        /// </summary>
        public void CheckFrameSelection(ISelectableElement node, Vector2 min, Vector2 max)
        {
            //Don't activate if not active or the box hasn't been changed yet
            if (!IsActive || Bounding == null || !HasChange)
                return;

            bool isInvertedSelection = ImGui.GetIO().KeyCtrl;
            bool isOverlapping = Bounding.Overlaps(new BoundingBox2D(min, max));
            bool hasBeenOverlapped = overlappedElements.Contains(node);

            //Check if the item gets an overlap and hasn't been overlapped yet
            if (isOverlapping && !hasBeenOverlapped)
            {
                overlappedElements.Add(node);
                //Invert or select the node
                if (isInvertedSelection)
                    node.IsSelected = !node.IsSelected;
                else
                    node.IsSelected = true;
            } //Node has been overlapped previously so determine what state it should be when losing overlap
            else if (!isOverlapping && hasBeenOverlapped)
            {
                //Only change the non overlapping state once
                overlappedElements.Remove(node);
                //Invert or deselect the node
                if (isInvertedSelection)
                    node.IsSelected = !node.IsSelected;
                else
                    node.IsSelected = false;
            }
        }

        public void Render()
        {
            if (!Enabled && IsActive) {
                Reset();
                return;
            }

            HandleInputs();

            if (!IsActive)
                return;

            var color = ImGui.GetStyle().Colors[(int)ImGuiCol.TextSelectedBg];
            ImGui.GetWindowDrawList().AddRectFilled(selectionStartPosition,
                selectionEndPosition, ImGui.ColorConvertFloat4ToU32(new Vector4(color.X, color.Y, color.Z, 0.5f)));
            //border
            ImGui.GetWindowDrawList().AddRect(selectionStartPosition,
                selectionEndPosition, ImGui.ColorConvertFloat4ToU32(new Vector4(color.X, color.Y, color.Z, 1)));
        }
    }
}
