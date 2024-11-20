using System;
using System.Collections.Generic;
using System.IO;
using Toolbox.Core.IO;

namespace Toolbox.Core
{
    public class DDSHelper
    {
        public static List<STGenericTexture.Surface> GetArrayFaces(DDS dds)
        {
            using (FileReader reader = new FileReader(dds.ImageData)) {
                var format = dds.Platform.OutputFormat;

                var Surfaces = new List<STGenericTexture.Surface>();
                uint formatSize = TextureFormatHelper.GetBytesPerPixel(format);

                bool isBlock = TextureFormatHelper.IsBCNCompressed(format);
                uint Offset = 0;

                for (byte i = 0; i < dds.ArrayCount; ++i)
                {
                    var Surface = new STGenericTexture.Surface();

                    for (int j = 0; j < dds.MipCount; ++j)
                    {
                        uint MipWidth = (uint)Math.Max(1, dds.Width >> j);
                        uint MipHeight = (uint)Math.Max(1, dds.Height >> j);

                        uint size = (MipWidth * MipHeight); //Total pixels
                        if (isBlock)
                        {
                            size = ((MipWidth + 3) >> 2) * ((MipHeight + 3) >> 2) * formatSize;
                            if (size < formatSize)
                                size = formatSize;
                        }
                        else
                        {
                            size = (uint)(size * (TextureFormatHelper.GetBytesPerPixel(format))); //Bytes per pixel
                        }

                        Surface.mipmaps.Add(reader.getSection((int)Offset, (int)size));
                        Offset += size;
                    }

                    Surfaces.Add(Surface);
                }
                return Surfaces;
            }
        }
    }
}
