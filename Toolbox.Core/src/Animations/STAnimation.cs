using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.ViewModels;

namespace Toolbox.Core.Animations
{
    /// <summary>
    /// Represents a class for animating
    /// </summary>
    public class STAnimation
    {
        /// <summary>
        /// The name of the animation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The frame to start playing the animation.
        /// </summary>
        public float StartFrame { get; set; }

        /// <summary>
        /// The current frame of the animation.
        /// </summary>
        public float Frame { get; set; }

        /// <summary>
        /// The total amount of frames to play in the animation.
        /// </summary>
        public float FrameCount { get; set; }

        /// <summary>
        /// The rate of frames per second to play back in the animation.
        /// </summary>
        public float FrameRate { get; set; } = 60.0f;

        /// <summary>
        /// Whether the animation will loop or not after
        /// the playback rearches the total frame count.
        /// </summary>
        public bool Loop { get; set; } = true;

        /// <summary>
        /// The step value when a frame advances.
        /// </summary>
        public float Step { get; set; }

        /// <summary>
        /// Toggles playabily in the animation player.
        /// </summary>
        public bool CanPlay { get; set; } = true;

        public bool IsEdited { get; set; } = false;

        /// <summary>
        /// A list of groups that store the animation data.
        /// </summary>
        public List<STAnimGroup> AnimGroups = new List<STAnimGroup>();

        public void SetFrame(float frame) {
            Frame = frame;
        }

        public void UpdateFrame(int frame) {
            SetFrame(frame);
            NextFrame();
        }

        public virtual void NextFrame() {
            if (Frame < StartFrame || Frame > FrameCount) return;
        }

        public void Resize(float frameCount)
        {
            foreach (var group in this.AnimGroups)
                group.Resize(frameCount, this.FrameCount);

            this.FrameCount = frameCount;
        }


        /// <summary>
        /// Resets the animation group values
        /// This should clear values from tracks, or reset them to base values.
        /// </summary>
        public virtual void Reset()
        {

        }

        public virtual NodeBase CreateNodeHierachy()
        {
            var animNode = new NodeBase(this.Name);
            animNode.Tag = this;
            foreach (var group in this.AnimGroups)
                animNode.AddChild(CreateGroupHierachy(group));
            return animNode;
        }

        private NodeBase CreateGroupHierachy(STAnimGroup group)
        {
            var node = new NodeBase(group.Name);
            node.Tag = group;

            foreach (var child in group.SubAnimGroups)
                node.AddChild(CreateGroupHierachy(child));

            foreach (var child in group.GetTracks())
                node.AddChild(CreateTrackNode(group, child));

            return node;
        }

        private NodeBase CreateTrackNode(STAnimGroup group, STAnimationTrack track)
        {
            var node = new NodeBase($"{group.Name}.{track.Name}");
            node.Tag = track;
            return node;
        }
    }
}
