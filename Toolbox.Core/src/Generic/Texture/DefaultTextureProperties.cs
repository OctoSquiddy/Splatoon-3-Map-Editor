using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public class DefaultTextureProperties
    {
        private STGenericTexture texture;

        [BindGUI("Width")]
        public uint Width => texture.Width;
        [BindGUI("Height")]
        public uint Height => texture.Height;
        [BindGUI("Depth")]
        public uint Depth => texture.Depth;
        [BindGUI("Format")]
        public string Format => $"{texture.Platform.ToString()}";
        [BindGUI("MipCount")]
        public uint MipCount => texture.MipCount;
        [BindGUI("ArrayCount")]
        public uint ArrayCount => texture.ArrayCount;
        [BindGUI("ImageSize")]
        public uint ImageSize => (uint)texture.DataSizeInBytes;
        [BindGUI("SurfaceType")]
        public STSurfaceType SurfaceType => texture.SurfaceType;

        public DefaultTextureProperties(STGenericTexture tex) {
            texture = tex; 
        }
    }
}
