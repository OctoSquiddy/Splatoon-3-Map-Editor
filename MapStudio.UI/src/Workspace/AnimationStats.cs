using System;
using System.Collections.Generic;
using System.Text;

namespace MapStudio.UI
{
    public class AnimationStats
    {
        public static int SkeletalAnims = 0;
        public static int MaterialAnims = 0;

        public static void Reset()
        {
            SkeletalAnims = 0;
            MaterialAnims = 0;
        }
    }
}
