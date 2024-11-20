using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;

namespace MapStudio.UI
{
    public static class ImageSharpExtension
    {
        public static byte[] GetSourceInBytes(this Image<Rgba32> image)
        {
            var _IMemoryGroup = image.GetPixelMemoryGroup();
            var _MemoryGroup = _IMemoryGroup.ToArray()[0];
            return MemoryMarshal.AsBytes(_MemoryGroup.Span).ToArray();
        }
    }
}
