using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Animations
{
    public class STBoneAnimGroup : STAnimGroup
    {
        public STAnimationTrack TranslateX { get; set; }
        public STAnimationTrack TranslateY { get; set; }
        public STAnimationTrack TranslateZ { get; set; }

        public STAnimationTrack RotateX { get; set; }
        public STAnimationTrack RotateY { get; set; }
        public STAnimationTrack RotateZ { get; set; }

        //Used if the rotation mode is set to quat
        public STAnimationTrack RotateW { get; set; }

        public STAnimationTrack ScaleX { get; set; }
        public STAnimationTrack ScaleY { get; set; }
        public STAnimationTrack ScaleZ { get; set; }

        public bool UseSegmentScaleCompensate { get; set; }

        public bool UseQuaternion = false;

        public STBoneAnimGroup()
        {
            TranslateX = new STAnimationTrack("TranslateX");
            TranslateY = new STAnimationTrack("TranslateY");
            TranslateZ = new STAnimationTrack("TranslateZ");
            RotateX = new STAnimationTrack("RotateX");
            RotateY = new STAnimationTrack("RotateY");
            RotateZ = new STAnimationTrack("RotateZ");
            RotateW = new STAnimationTrack("RotateW");
            ScaleX = new STAnimationTrack("ScaleX");
            ScaleY = new STAnimationTrack("ScaleY");
            ScaleZ = new STAnimationTrack("ScaleZ");
        }

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            tracks.Add(ScaleX);
            tracks.Add(ScaleY);
            tracks.Add(ScaleZ);
            tracks.Add(RotateX);
            tracks.Add(RotateY);
            tracks.Add(RotateZ);
            tracks.Add(RotateW);
            tracks.Add(TranslateX);
            tracks.Add(TranslateY);
            tracks.Add(TranslateZ);
            return tracks;
        }
    }
}
