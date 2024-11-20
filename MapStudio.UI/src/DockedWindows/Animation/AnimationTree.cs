using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UIFramework;
using Toolbox.Core.Animations;
using CurveEditorLibrary;
using ImGuiNET;
using Toolbox.Core;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents a tree for loading and editing animation data.
    /// </summary>
    public class AnimationTree
    {
        static bool UpdateValue;
        static bool UpdateFrameCount;
        static bool UpdateRemoved;

        public EventHandler OnRemoved;

        /// <summary>
        /// Runs when a key frame or value has been altered.
        /// </summary>
        public EventHandler OnValueUpdated;

        /// <summary>
        /// Runs when the frame count has been altered.
        /// </summary>
        public EventHandler OnFrameCountUpdated;

        /// <summary>
        /// The tree view UI to render animation nodes.
        /// </summary>
        public TreeView TreeView = new TreeView();

        //timeline
        AnimationTimelineControl CurveEditor;

        //Keyed valur color
        private static readonly Vector4 KEY_COLOR = new Vector4(0.602f, 0.569f, 0.240f, 1.000f);

        public AnimationTree(AnimationProperties propertyWindow, AnimationTimelineControl curveEditor) {
            CurveEditor = curveEditor;
            //2 columns. Animation tree and current track value
            TreeView.ColumnCount = 2;
            TreeView.DisplaySearchBox = false;
            TreeView.OnSelectionChanged += (o, e) =>
            {
                var node = o as TreeNode;
                if (node == null)
                    return;

                propertyWindow.SelectedObject = node;
            };
            TreeView.CanDisplayNode = (node) =>
            {
                if (node.Tag is STAnimationTrack)
                {
                    //UI filters
                    if (!AnimationGraphWindow.ShowConstants &&
                        ((STAnimationTrack)node.Tag).InterpolationType == STInterpoaltionType.Constant)
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        public void Load(List<STAnimation> anims)
        {
            TreeView.Nodes.Clear();
            foreach (var anim in anims)
                LoadAnimation(anim);
        }

        private void LoadAnimation(STAnimation anim)
        {
            if (!(anim is IEditableAnimation))
                return;

            var editableAnim = anim as IEditableAnimation;
            TreeView.Nodes.Add(editableAnim.Root);
        }

        public void Render() {
            if (UpdateValue)
            {
                OnValueUpdated?.Invoke(this, EventArgs.Empty);
                UpdateValue = false;
            }
            if (UpdateFrameCount)
            {
                OnFrameCountUpdated?.Invoke(this, EventArgs.Empty);
                UpdateFrameCount = false;
            }
            if (UpdateRemoved)
            {
                OnRemoved?.Invoke(this, EventArgs.Empty);
                UpdateRemoved = false;
            }

            TreeView.Render();
        }

        public void Dispose()
        {
            TreeView.Nodes.Clear();
        }

        public class AnimNode : TreeNode
        {
            public STAnimation Animation;

            public AnimNode(STAnimation anim)
            {
                Animation = anim;
                ContextMenus.Add(new MenuItem("Resize", () =>
                {
                    float frameCount = Animation.FrameCount;

                    DialogHandler.Show("Resize Animation", 250, 80, () =>
                    {
                        ImGui.InputFloat("Frame Count", ref frameCount);

                        if (ImGui.Button(TranslationSource.GetText("OK"), new Vector2(ImGui.GetWindowWidth(), 24)))
                            DialogHandler.ClosePopup(true);
                    }, (ok) =>
                    {
                        if (!ok)
                            return;

                        Animation.Resize(frameCount);
                        Animation.IsEdited = true;

                        UpdateFrameCount = true;
                    });
                }));
            }
        }

        /// <summary>
        /// Represents a tree node that stores an animation group.
        /// </summary>
        public class GroupNode : TreeNode
        {
            /// <summary>
            /// The target animation group.
            /// </summary>
            public STAnimGroup Group => (STAnimGroup)this.Tag;

            /// <summary>
            /// The parent animation.
            /// </summary>
            public STAnimation Anim { get; private set; }

            public EventHandler OnGroupRemoved;

            public GroupNode(STAnimation anim, STAnimGroup group, STAnimGroup parent)
            {
                Tag = group;
                Anim = anim;
                Header = group.Name;
                //Removing animation group
                ContextMenus.Add(new MenuItem("Remove Property", () =>
                {
                    //Remove from group
                    if (parent != null)
                        parent.SubAnimGroups.Remove(group);
                    else
                        anim.AnimGroups.Remove(group);

                    //Remove from UI
                    this.Parent.Children.Remove(this);
                    UpdateRemoved = true;

                    OnGroupRemoved?.Invoke(this, EventArgs.Empty);
                    anim.IsEdited = true;
                }));
                //Drawing the node in a custom rennder override.
                RenderOverride += delegate
                {
                    RenderNode();
                };
            }

            public virtual void RenderNode()
            {
                //Simple text display. Adjust columns for entire tree.
                ImGui.Text(this.Header);
                for (int i = 0; i < 2; i++)
                    ImGui.NextColumn();
            }
        }

        /// <summary>
        /// Represents a tree node that stores an animation group.
        /// This node will display an RGB editor for color editing given RGB tracks.
        /// </summary>
        public class ColorGroupNode : GroupNode
        {
            public GLFrameworkEngine.GLTexture2D ColorSheet = null;

            public ColorGroupNode(STAnimation anim, STAnimGroup group, STAnimGroup parent) : base(anim, group, parent)
            {

            }

            public override void RenderNode()
            {
                ImGui.Text(this.Header);
                ImGui.NextColumn();

                //Display a color picker for editing track values on the current frame.
                Vector4 color = GetTrackColor(Anim.Frame);

                var width = ImGui.GetColumnWidth() - 3;
                var height = ImGui.GetFrameHeight();
                if (ImGui.ColorButton($"##{this.ID}_clr", color, ImGuiColorEditFlags.HDR, new Vector2(width, height))) {
                    ImGui.OpenPopup($"##{this.ID}_picker");
                }
                if (ImGui.BeginPopup($"##{this.ID}_picker"))
                {
                    if (ImGui.ColorPicker4($"##{this.ID}_clrp", ref color)) {
                        SetTrackColor(color);
                    }
                    ImGui.EndPopup();
                }
                ImGui.NextColumn();
            }

            /// <summary>
            /// Gets the track color of the current frame.
            /// </summary>
            public virtual Vector4 GetTrackColor(float frame)
            {
                Vector4 color = Vector4.One;
                var tracks = Group.GetTracks();
                if (tracks.Count > 0)
                    color.X = tracks[0].GetFrameValue(frame);
                if (tracks.Count > 1)
                    color.Y = tracks[1].GetFrameValue(frame);
                if (tracks.Count > 2)
                    color.Z = tracks[2].GetFrameValue(frame);
                if (tracks.Count > 3)
                    color.W = tracks[3].GetFrameValue(frame);
                return color;
            }

            /// <summary>
            /// Sets the track color of the current frame.
            /// </summary>
            public virtual void SetTrackColor(Vector4 color)
            {

            }
        }

        /// <summary>
        /// Represents a track node for textures.
        /// </summary>
        public class TextureTrackNode : TrackNode
        {
            public virtual List<string> TextureList { get; set; }

            public TextureTrackNode(STAnimation anim, STAnimationTrack track) : base(anim, track)
            {
            }
        }

        /// <summary>
        /// Represents a track node for editing values as degrees.
        /// The given track should have the values output as radians.
        /// </summary>
        public class TrackNodeDegreesConversion : TrackNode
        {
            public TrackNodeDegreesConversion(STAnimation anim, STAnimationTrack track) : base(anim, track)
            {

            }

            public override void RenderNode()
            {
                ImGui.Text(this.Header);

                ImGui.NextColumn();

                var color = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
                //Display keyed values differently
                bool isKeyed = Track.KeyFrames.Any(x => x.Frame == Anim.Frame);
                if (isKeyed)
                    color = KEY_COLOR;

                ImGui.PushStyleColor(ImGuiCol.Text, color);

                //Display the current track value
                float value = Track.GetFrameValue(Anim.Frame) * Toolbox.Core.STMath.Rad2Deg;
                //Span the whole column
                ImGui.PushItemWidth(ImGui.GetColumnWidth() - 3);
                bool edited = ImGui.DragFloat($"##{Track.Name}_frame", ref value);
                bool isActive = ImGui.IsItemDeactivated();
                ImGui.PopItemWidth();

                //Insert key value with degrees back to radians
                if (edited || (isActive && ImGui.IsKeyDown((int)ImGuiKey.Enter)))
                    InsertOrUpdateKeyValue(value * Toolbox.Core.STMath.Deg2Rad);

                ImGui.PopStyleColor();

                ImGui.NextColumn();
            }
        }

        /// <summary>
        /// Represents a tree node for editing animation tracks.
        /// </summary>
        public class TrackNode : TreeNode
        {
            /// <summary>
            /// The target animation track.
            /// </summary>
            public STAnimationTrack Track => (STAnimationTrack)this.Tag;

            /// <summary>
            /// A list of key nodes to display on the dope sheet.
            /// </summary>
            public List<KeyNode> Keys = new List<KeyNode>();

            /// <summary>
            /// The parent animation.
            /// </summary>
            public STAnimation Anim { get; private set; }

            public TrackNode(STAnimation anim, STAnimationTrack track) {
                Header = track.Name;
                Tag = track;
                Anim = anim;

                //Clearable keys
                this.ContextMenus.Add(new MenuItem("Clear Keys", () =>
                {
                    Track.KeyFrames.Clear();
                    Keys.Clear();
                    UpdateValue = true;
                    UpdateRemoved = true;
                    Anim.IsEdited = true;
                }));

                //Add key nodes for editing in the dope sheet
                Keys.Clear();
                foreach (var key in track.KeyFrames)
                    Keys.Add(new KeyNode(this, key));

                //Operation when the key is inserted from the track
                //Update the dope sheet UI
                track.OnKeyInserted += (o, e) =>
                {
                    var kf = o as STKeyFrame;
                    if (!Keys.Any(x => x.KeyFrame == kf))
                        Keys.Add(new KeyNode(this, kf));

                    UpdateValue = true;
                };

                //Draw the track node for editing the current frame value
                RenderOverride += delegate
                {
                    RenderNode();
                };
            }

            public virtual void RenderKeyTableUI()
            {
                ImGui.BeginColumns("##keyList", 2);
                for (int i = 0; i < Keys.Count; i++)
                {
                    float frame = Keys[i].Frame;
                    float value = Keys[i].KeyFrame.Value;

                    bool edited = false;
                    edited |= ImGui.DragFloat($"Frame##{i}", ref frame, 1, 0, Anim.FrameCount);
                    ImGui.NextColumn();
                    edited |= ImGui.DragFloat($"Value##{i}", ref value);
                    ImGui.NextColumn();

                    if (edited)
                    {
                        Keys[i].Frame = frame;
                        Keys[i].KeyFrame.Value = value;
                        if (Keys[i].KeyFrame is STLinearKeyFrame)
                            AdjustLinearDeltas(Track);
                    }
                }
                ImGui.EndColumns();
            }

            public virtual void RenderNode()
            {
                ImGui.Text(this.Header);

                ImGui.NextColumn();

                var color = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
                //Display keyed values differently
                bool isKeyed = Track.KeyFrames.Any(x => x.Frame == Anim.Frame);
                if (isKeyed)
                    color = KEY_COLOR;

                ImGui.PushStyleColor(ImGuiCol.Text, color);

                //Display the current track value
                float value = Track.GetFrameValue(Anim.Frame);
                //Span the whole column
                ImGui.PushItemWidth(ImGui.GetColumnWidth() - 3);
                bool edited = ImGui.DragFloat($"##{Track.Name}_frame", ref value);
                bool isActive = ImGui.IsItemDeactivated();

                ImGui.PopItemWidth();

                //Insert key value from current frame
                if (edited || (isActive && ImGui.IsKeyDown((int)ImGuiKey.Enter)))
                    InsertOrUpdateKeyValue(value);

                ImGui.PopStyleColor();

                ImGui.NextColumn();
            }

            public void InsertOrUpdateKeyValue(float value) {
                InsertOrUpdateKeyValue(Anim.Frame, value);

                UpdateValue = true;
            }

            public void InsertOrUpdateKeyValue(float frame, float value)
            {
                Anim.IsEdited = true;

                bool isKeyed = Track.KeyFrames.Any(x => x.Frame == frame);

                if (!isKeyed)
                {
                    STKeyFrame keyFrame = null;

                    //Either insert a key or edit existing key
                    switch (Track.InterpolationType)
                    {
                        case STInterpoaltionType.Hermite:
                            keyFrame = new STHermiteKeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                                TangentIn = 0,
                                TangentOut = 0,
                            };
                            break;
                        case STInterpoaltionType.Linear:
                            keyFrame = new STLinearKeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                            };
                            break;
                        case STInterpoaltionType.Step:
                            keyFrame = new STKeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                            };
                            break;
                        case STInterpoaltionType.Constant:
                            keyFrame = new STLinearKeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                            };
                            //Switch type to linear if frame is > 0
                            if (frame > 0)
                                Track.InterpolationType = STInterpoaltionType.Linear;
                            break;
                        default:
                            throw new Exception($"Unsupported interpolation type! {Track.InterpolationType}");
                    }
                    if (keyFrame == null)
                        return;

                    Track.Insert(keyFrame);
                    if (keyFrame is STLinearKeyFrame)
                        AdjustLinearDeltas((STLinearKeyFrame)keyFrame, Anim.Frame);

                    Keys.Add(new KeyNode(this, keyFrame));
                }
                else
                {
                    var key = Track.KeyFrames.FirstOrDefault(x => x.Frame == frame);
                    key.Value = value;
                    if (key is STLinearKeyFrame)
                        AdjustLinearDeltas((STLinearKeyFrame)key, Anim.Frame);
                }
            }

            private void AdjustLinearDeltas(STAnimationTrack track)
            {
                for (int i = 0; i < track.KeyFrames.Count; i++)
                {
                    if (track.KeyFrames[i] is STLinearKeyFrame && i + 1 < track.KeyFrames.Count)
                    {
                        var leftKey = track.KeyFrames[i + 1];
                        ((STLinearKeyFrame)track.KeyFrames[i]).Delta = leftKey.Value - track.KeyFrames[i].Value;
                    }
                }
            }

            //Linear deltas are auto calculated. These keys can use custom deltas so the current viewer uses them directly
            private void AdjustLinearDeltas(STLinearKeyFrame keyFrame, float frame)
            {
                var leftKey = Track.KeyFrames.LastOrDefault(x => (int)x.Frame < frame);
                var rightKey = Track.KeyFrames.FirstOrDefault(x => (int)x.Frame > frame);
                if (leftKey != null && leftKey is STLinearKeyFrame) {
                    ((STLinearKeyFrame)leftKey).Delta = keyFrame.Value - ((STLinearKeyFrame)leftKey).Value;
                }
                if (rightKey != null)
                    keyFrame.Delta = rightKey.Value - keyFrame.Value;
            }
        }

        public class KeyNode : ISelectableElement
        {
            /// <summary>
            /// The animation key data.
            /// </summary>
            public STKeyFrame KeyFrame { get; set; }

            /// <summary>
            /// Current frame of the key.
            /// </summary>
            public float Frame
            {
                get { return KeyFrame.Frame; }
                set { KeyFrame.Frame = value; }
            }

            //Min/max UI element size
            public Vector2 Max = new Vector2();
            public Vector2 Min = new Vector2();

            /// <summary>
            /// Determines if the key was selected by a selection box or not.
            /// This is needed for invert selection.
            /// </summary>
            public bool SelectedByBox { get; set; }

            /// <summary>
            /// Determines if the key node is selected or not.
            /// </summary>
            public bool IsSelected { get; set; }

            //parent track node
            private TrackNode TrackNode;
            //parent track data
            private STAnimationTrack Track => TrackNode.Track;
           
            /// <summary>
            /// Gets the parent track data.
            /// </summary>
            public STAnimationTrack GetTrack() => Track;

            public KeyNode(TrackNode track, STKeyFrame key)
            {
                TrackNode = track;
                KeyFrame = key;
            }

            /// <summary>
            /// Creates a copy of the key node.
            /// </summary>
            public KeyNode Copy()
            {
                //Create new key as copy
                return new KeyNode(TrackNode, KeyFrame.Clone());
            }

            /// <summary>
            /// Removes the key node from the parent track.
            /// </summary>
            public void TryRemoveFromTrack()
            {
                //Remove from animation data
                if (Track.KeyFrames.Contains(KeyFrame))
                    Track.KeyFrames.Remove(KeyFrame);
                //Remove from GUI
                if (TrackNode.Keys.Contains(this))
                    TrackNode.Keys.Remove(this);
                UpdateValue = true;
            }

            /// <summary>
            /// Adds the key node back to the parent track.
            /// </summary>
            public void TryAddFromTrack()
            {
                //Remove from animation data
                if (!Track.KeyFrames.Contains(KeyFrame))
                    Track.KeyFrames.Add(KeyFrame);
                //Remove from GUI
                if (!TrackNode.Keys.Contains(this))
                    TrackNode.Keys.Add(this);
                UpdateValue = true;
            }
        }
    }
}
