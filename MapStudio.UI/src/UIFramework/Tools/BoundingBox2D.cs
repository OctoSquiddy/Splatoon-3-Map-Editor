using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace UIFramework
{
    public class BoundingBox2D
    {
        /// <summary>
        /// The smallest point of the box.
        /// </summary>
        public Vector2 Min { get; set; }

        /// <summary>
        /// The largest point of the box.
        /// </summary>
        public Vector2 Max { get; set; }

        public BoundingBox2D(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Checks if this box overlaps another box.
        /// </summary>
        public bool Overlaps(BoundingBox2D box)
        {
            return (Min.X < box.Max.X &&
                    Min.Y < box.Max.Y &&
                    box.Min.X < Max.X &&
                    box.Min.Y < Max.Y);
        }

        public override string ToString() {
            return $"Min: {Min} Max: {Max}";
        }
    }
}
