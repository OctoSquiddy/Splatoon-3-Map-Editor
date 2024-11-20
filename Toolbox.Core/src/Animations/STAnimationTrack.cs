using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core.Animations
{
    /// <summary>
    /// Represents a animation track used to store key frames
    /// </summary>
    public class STAnimationTrack
    {
        /// <summary>
        /// The name of the track displayed in the timeline
        /// </summary>
        [BindGUI("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The interpolation type used to interpoalte key frames.
        /// </summary>
        [BindGUI("InterpolationType")]
        public STInterpoaltionType InterpolationType { get; set; }

        public STLoopMode WrapMode { get; set; } = STLoopMode.Clamp;

        /// <summary>
        /// A list of key frames used for the track.
        /// </summary>
        public List<STKeyFrame> KeyFrames = new List<STKeyFrame>();

        /// <summary>
        /// Determines if the track has key data used.
        /// </summary>
        public bool HasKeys => KeyFrames.Count > 0;

        public EventHandler OnKeyInserted;

        public virtual int ChannelIndex { get; set; }

        public STAnimationTrack() { }

        public STAnimationTrack(string name) {
            Name = name;
        }

        public STAnimationTrack(STInterpoaltionType interpolation)
        {
            InterpolationType = interpolation;
        }

        public void Resize(float frameCount, float minFrame, float maxFrame)
        {
            float ratio = (float)frameCount / (float)maxFrame;
            foreach (var key in this.KeyFrames)
            {
                int newFrame = (int)((float)key.Frame * ratio + 0.5f);
                float frameRatio = newFrame == 0 ? 0 : (float)key.Frame / (float)newFrame;

                key.Frame = key.Frame <= minFrame ? key.Frame : newFrame;
                if (key is STHermiteKeyFrame)
                {
                    var hermiteKey = key as STHermiteKeyFrame;
                    hermiteKey.TangentIn *= float.IsNaN(frameRatio) ? 1 : frameRatio;
                    hermiteKey.TangentOut *= float.IsNaN(frameRatio) ? 1 : frameRatio;
                }
            }
        }

        public void RemoveKey(float frame)
        {
            var keyed = KeyFrames.FindIndex(x => x.Frame == frame);
            if (keyed != -1)
                KeyFrames.RemoveAt(keyed);
        }

        public void Insert(STKeyFrame keyFrame)
        {
            var keyed = KeyFrames.FindIndex(x => x.Frame == keyFrame.Frame);
            if (keyed != -1)
                KeyFrames.RemoveAt(keyed);

            KeyFrames.Add(keyFrame);
            KeyFrames = KeyFrames.OrderBy(x => x.Frame).ToList();

            OnKeyInserted?.Invoke(keyFrame, EventArgs.Empty);
        }

        /// <summary>
        /// Determines if a keyframe is keyed at the given frame value
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool IsKeyed(float frame)
        {
            var matches = KeyFrames.Where(p => p.Frame == frame);
            return matches != null && matches.Count() > 0;
        }

        /// <summary>
        /// Optimizes the key frames in the animation track by removing unecessary keys.
        /// </summary>
        public void OptimizeKeyFrames()
        {
            List<STKeyFrame> keysToRemove = new List<STKeyFrame>();
            for (int i = 0; i < KeyFrames.Count; i++)
            {
                if (i != 0)
                {
                    //Remove duplicate values if they return the same from the previous key.
                    var previous = KeyFrames[i - 1];
                    if (previous.Value == KeyFrames[i].Value)
                        keysToRemove.Add(previous);
                }
            }
            foreach (var key in keysToRemove)
                KeyFrames.Remove(key);

            keysToRemove.Clear();
        }

        /// <summary>
        /// Gets the left and right key frame given the frame value.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public STKeyFrame[] GetFrame(float frame)
        {
            if (KeyFrames.Count == 0) return null;
            STKeyFrame k1 = (STKeyFrame)KeyFrames[0], k2 = (STKeyFrame)KeyFrames[0];
            foreach (STKeyFrame k in KeyFrames)
            {
                if (k.Frame < frame)
                {
                    k1 = k;
                }
                else
                {
                    k2 = k;
                    break;
                }
            }

            return new STKeyFrame[] { k1, k2 };
        }

        private float GetWrapFrame(float frame)
        {
            var lastFrame = KeyFrames.Last().Frame;
            if (frame < 0 || lastFrame < 0)
                return frame;

            if (WrapMode == STLoopMode.Clamp)
            {
                if (frame > lastFrame)
                    return lastFrame;
                else
                    return frame;
            }
            else if (WrapMode == STLoopMode.Repeat)
            {
                while (frame > lastFrame)
                    frame -= lastFrame;
                return frame;
            }
            else if (WrapMode == STLoopMode.Cumulative)
            {
                while (frame > lastFrame)
                    frame -= lastFrame;
                return frame;
            }
            return frame;
        }

        public virtual STKeyFrame[] GetFrames(float frame, float startFrame = 0)
        {
            STKeyFrame LK = KeyFrames.First();
            STKeyFrame RK = KeyFrames.Last();

            float Frame = GetWrapFrame(frame - startFrame);
            foreach (STKeyFrame keyFrame in KeyFrames)
            {
                if (keyFrame.Frame <= Frame) LK = keyFrame;
                if (keyFrame.Frame >= Frame && keyFrame.Frame < RK.Frame) RK = keyFrame;
            }

            return new STKeyFrame[2] { LK, RK };
        }

        //Key frame setup based on
        //https://github.com/gdkchan/SPICA/blob/42c4181e198b0fd34f0a567345ee7e75b54cb58b/SPICA/Formats/CtrH3D/Animation/H3DFloatKeyFrameGroup.cs

        /// <summary>
        /// Gets the current value at the given frame based on the interpolation the track uses and the keyframes.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <returns></returns>
        public virtual float GetFrameValue(float frame, float startFrame = 0)
        {
            if (KeyFrames.Count == 0) return 0;
            if (KeyFrames.Count == 1) return KeyFrames[0].Value;
            STKeyFrame LK = KeyFrames.First();
            STKeyFrame RK = KeyFrames.Last();

            float Frame = GetWrapFrame(frame - startFrame);
            foreach (STKeyFrame keyFrame in KeyFrames)
            {
                if (keyFrame.Frame <= Frame) LK = keyFrame;
                if (keyFrame.Frame >= Frame && keyFrame.Frame < RK.Frame) RK = keyFrame;
            }
            if (LK == RK)
                return LK.Value;

            return GetInterpolatedValue(Frame, LK, RK);
        }

        private float GetInterpolatedValue(float Frame, STKeyFrame LK, STKeyFrame RK)
        {
            float FrameDiff = Frame - LK.Frame;
            float Weight = FrameDiff / (RK.Frame - LK.Frame);
            if (LK is STLinearKeyFrame)
            {
                //Get the right value from the linear delta value
                float rightValue = LK.Value + ((STLinearKeyFrame)LK).Delta;
                return InterpolationHelper.Lerp(LK.Value, rightValue, Weight);
            }

            if (LK is STBezierKeyFrame)
            {
                var p0 = LK.Value;
                var p1 = ((STBezierKeyFrame)LK).SlopeOut;
                var p2 = ((STBezierKeyFrame)RK).SlopeIn;
                var p3 = RK.Value;
                return InterpolationHelper.BezierInterpolate(Frame,
                    LK.Frame, RK.Frame,
                    p1, p2,
                    p0, p3);
            }

            switch (InterpolationType)
            {
                case STInterpoaltionType.Constant: return LK.Value;
                case STInterpoaltionType.Step: return LK.Value; 
                case STInterpoaltionType.Linear: return InterpolationHelper.Lerp(LK.Value, RK.Value, Weight);
                case STInterpoaltionType.Bezier:
                    {
                        float length = RK.Frame - LK.Frame;
                        if (LK is STBezierKeyFrame)
                        {
                            var p0 = LK.Value;
                            var p1 = ((STBezierKeyFrame)LK).SlopeOut;
                            var p2 = ((STBezierKeyFrame)RK).SlopeIn;
                            var p3 = RK.Value;
                            return InterpolationHelper.BezierInterpolate(Frame,
                                LK.Frame, RK.Frame,
                                p1, p2,
                                p0, p3);
                        }

                        //Default to no control point usage for this key
                        return InterpolationHelper.BezierInterpolate(Frame,
                            LK.Frame, RK.Frame, 0, 0, LK.Value, RK.Value);
                    }
                case STInterpoaltionType.Hermite:

                    if (LK is STHermiteCubicKeyFrame)
                    {
                        if (!(RK is STHermiteCubicKeyFrame))
                        {
                            RK = new STHermiteCubicKeyFrame() { Value = RK.Value };
                        }

                        STHermiteCubicKeyFrame hermiteKeyLK = (STHermiteCubicKeyFrame)LK;
                        STHermiteCubicKeyFrame hermiteKeyRK = (STHermiteCubicKeyFrame)RK;

                        return InterpolationHelper.CubicHermiteInterpolate(Frame,
                             hermiteKeyLK.Frame,
                             hermiteKeyRK.Frame,
                             hermiteKeyLK.Value,
                             hermiteKeyLK.Coef1,
                             hermiteKeyLK.Coef2,
                             hermiteKeyLK.Coef3);
                    }
                    if (LK is STHermiteKeyFrame)
                    {
                        if (!(RK is STHermiteKeyFrame))
                        {
                            RK = new STHermiteKeyFrame() { Value = RK.Value };
                        }

                        STHermiteKeyFrame hermiteKeyLK = (STHermiteKeyFrame)LK;
                        STHermiteKeyFrame hermiteKeyRK = (STHermiteKeyFrame)RK;

                        float length = RK.Frame - LK.Frame;

                        return InterpolationHelper.HermiteInterpolate(Frame,
                         hermiteKeyLK.Frame,
                         hermiteKeyRK.Frame,
                         hermiteKeyLK.Value,
                         hermiteKeyRK.Value,
                         hermiteKeyLK.TangentOut * length,
                         hermiteKeyRK.TangentIn * length);
                    }
                   return InterpolationHelper.Herp(
                        LK.Value, RK.Value,
                        LK.Slope, RK.Slope,
                        FrameDiff,
                        Weight);
            }
            return LK.Value;
        }

        public override string ToString()
        {
            return $"{this.Name}_{KeyFrames.Count}";
        }
    }
}
