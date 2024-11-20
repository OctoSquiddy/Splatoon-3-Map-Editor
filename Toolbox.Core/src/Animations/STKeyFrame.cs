using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core.Animations
{
    public class STKeyFrame
    {
        /// <summary>
        /// The frame of the key.
        /// </summary>
        public virtual float Frame { get; set; }

        /// <summary>
        /// The value of the key
        /// </summary>
        public virtual float Value { get; set; }

        /// <summary>
        /// The slope used for the key used for interpolation.
        /// </summary>
        public virtual float Slope { get; set; }

        //UI elements

        public bool IsSelected = false;

        public STKeyFrame() { }

        public STKeyFrame(int frame, float value)
        {
            Frame = frame;
            Value = value;
        }

        public STKeyFrame(float frame, float value)
        {
            Frame = frame;
            Value = value;
        }

        public virtual STKeyFrame Clone()
        {
            return new STKeyFrame(Frame, Value);
        }
    }

    /// <summary>
    /// Represents a linear key frame used for linear interpolation
    /// This key frame provides the delta value to determine the weight between keys.
    /// </summary>
    public class STLinearKeyFrame : STKeyFrame
    {
        public bool StepDelta = false;

        /// <summary>
        /// The delta of linear key value.
        /// </summary>
        public virtual float Delta { get; set; }

        public STLinearKeyFrame() { }

        public STLinearKeyFrame(int frame, float value, float delta = 0)
        {
            Frame = frame;
            Value = value;
            Delta = delta;
        }
    }

    /// <summary>
    /// Represents a bezier key frame used for beizer interpolation
    /// </summary>
    public class STBezierKeyFrame : STKeyFrame
    {
        public float SlopeIn;
        public float SlopeOut;

        public STBezierKeyFrame() { }

        public STBezierKeyFrame(int frame, float value, float slopeIn, float slopeOut)
        {
            Frame = frame;
            Value = value;
            SlopeIn = slopeIn;
            SlopeOut = slopeOut;
        }
    }

    /// <summary>
    /// Represents a hermite cubic key frame used for hermite cubic interpolation
    /// </summary>
    public class STHermiteCubicKeyFrame : STHermiteKeyFrame
    {
        public float Coef1 { get; set; }
        public float Coef2 { get; set; }
        public float Coef3 { get; set; }

        /// <summary>
        /// Converts hermite key data to a cubic key.
        /// </summary>
        /// <param name="p0"> The current value in the key. </param>
        /// <param name="p1"> The next keyed value. </param>
        /// <param name="s0"> The current out slope in the key.</param>
        /// <param name="s1"> The next keyed in slope.</param>
        public void HermiteToCubicKey(float p0, float p1, float s0, float s1)
        {
            Coef3 = (p0 * 2) + (p1 * -2) + (s0 * 1) + (s1 * 1);
            Coef2 = (p0 * -3) + (p1 * 3) + (s0 * -2) + (s1 * -1);
            Coef1 = (p0 * 0) + (p1 * 0) + (s0 * 1) + (s1 * 0);
            Value = (p0 * 1) + (p1 * 0) + (s0 * 0) + (s1 * 0);
        }

        /// <summary>
        /// Converts a cubic key to a hermite key,
        /// </summary>
        /// <param name="time">The length from the current frame to the next frame.</param>
        /// <param name="delta">The difference between the next value and the current value.</param>
        /// <returns></returns>
        public STHermiteKeyFrame ToHermiteKey(float time, float delta)
        {
            var slopes = InterpolationHelper.GetCubicSlopes(time, delta,
                new float[4] {Value, Coef1, Coef2, Coef3 });
            return new STHermiteKeyFrame()
            {
                Value = this.Value,
                TangentIn = slopes[0],
                TangentOut = slopes[1],
            };
        }
    }

    /// <summary>
    /// Represents a hermite key frame used for hermite interpolation
    /// </summary>
    public class STHermiteKeyFrame : STKeyFrame
    {
        public virtual float TangentIn { get; set; }
        public virtual float TangentOut { get; set; }

        public STHermiteKeyFrame() { }

        public STHermiteKeyFrame(int frame, float value, float tangentIn, float tangentOut)
        {
            Frame = frame;
            Value = value;
            TangentIn = tangentIn;
            TangentOut = tangentOut;
        }
    }
}
