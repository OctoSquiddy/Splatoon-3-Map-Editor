using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.Imaging
{
    public interface IPlatformSwizzle
    {
        TexFormat OutputFormat { get; set; }

        byte[] DecodeImage(byte[] data, uint width, uint height, uint arrayCount, uint mipCount, int array, int mip);
        byte[] EncodeImage(byte[] data, uint width, uint height, uint arrayCount, uint mipCount, int array, int mip);
    }
}
