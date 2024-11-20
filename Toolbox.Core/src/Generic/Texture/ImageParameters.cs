using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public class ImageParameters
    {
        public STRotateFlipType Rotation;

        /// <summary>
        /// To use the software decoder or not when rendering.
        /// </summary>
        public bool UseSoftwareDecoder { get; set; }

        //Flip the image on the Y axis
        public bool FlipY { get; set; }

        //Dont swap the red and green channels
        public bool DontSwapRG { get; set; }
    }
}
