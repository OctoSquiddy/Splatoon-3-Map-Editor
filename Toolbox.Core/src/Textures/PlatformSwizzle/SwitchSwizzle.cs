using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Core.Switch;

namespace Toolbox.Core.Imaging
{
    public class SwitchSwizzle : IPlatformSwizzle
    {
        public TexFormat OutputFormat { get; set; } = TexFormat.RGBA8_UNORM;

        //Required settings
        public uint BlockHeightLog2;
        public uint Alignment;
        public uint TileMode;
        public int Target = 1; //Platform PC or NX

        //Adjusted on encode
        public uint ReadTextureLayout;
        public uint ImageSize;
        public uint[] MipOffsets;

        //Quick check for linear tiling
        public bool LinearMode => TileMode == 1;

        public SwitchSwizzle(TexFormat format) {
            OutputFormat = format;
        }

        public override string ToString() {
            return OutputFormat.ToString();
        }

        public byte[] DecodeImage(byte[] data, uint width, uint height, uint arrayCount, uint mipCount, int array, int mip) {

            if (data.Length == 0)
                throw new Exception("Data is empty! Failed to swizzle image!");

            if (BlockHeightLog2 == 0)
            {
                uint blkHeight = TextureFormatHelper.GetBlockHeight(OutputFormat);
                uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight));
                BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length ;

                if (OutputFormat != TexFormat.ASTC_8x5_UNORM)
                    BlockHeightLog2 -= 1;
            }

            return TegraX1Swizzle.GetImageData(OutputFormat, width, height, arrayCount, mipCount, 1, data, array, mip, 0, BlockHeightLog2, Target, LinearMode);
        }

        public byte[] EncodeImage(byte[] data, uint width, uint height, uint arrayCount, uint mipCount, int array, int mip) {
            uint imageOffset = 0;
            List<byte[]> mipmaps = SwizzleSurfaceMipMaps(data, width, height, arrayCount, mipCount, ref imageOffset);
            //Combine mip map data
            return ByteUtils.CombineArray(mipmaps.ToArray());
        }

        public uint[] GenerateMipOffsets(STGenericTexture tex, uint imageSize)
        {
            return TegraX1Swizzle.GenerateMipSizes(OutputFormat,
                 tex.Width, tex.Height, tex.Depth, tex.ArrayCount, tex.MipCount, imageSize)[0]; 
        }

        public List<byte[]> SwizzleSurfaceMipMaps(byte[] data, uint width, uint height, uint depth, uint MipCount, ref uint imageOffset)
        {
            uint SurfaceSize = 0;
            int blockHeightShift = 0;
            int target = 1;
            uint Pitch = 0;
            uint blockHeight = 0;
            uint blkWidth = TextureFormatHelper.GetBlockWidth(this.OutputFormat);
            uint blkHeight = TextureFormatHelper.GetBlockHeight(this.OutputFormat);
            uint blkDepth = TextureFormatHelper.GetBlockDepth(this.OutputFormat);
            uint bpp = TextureFormatHelper.GetBytesPerPixel(this.OutputFormat);

            MipOffsets = new uint[MipCount];

            uint linesPerBlockHeight = 0;

            if (LinearMode)
            {
                blockHeight = 1;
                BlockHeightLog2 = 0;
                Alignment = 1;

                linesPerBlockHeight = 1;
                ReadTextureLayout = 0;
            }
            else
            {
                blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight));
                BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
                Alignment = 512;
                ReadTextureLayout = 1;

                linesPerBlockHeight = blockHeight * 8;
            }

            List<byte[]> mipmaps = new List<byte[]>();
            for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
            {
                var result = WiiU.TextureHelper.GetCurrentMipSize(width, height, blkWidth, blkHeight, bpp, mipLevel);
                uint offset = result.Item1;
                uint size = result.Item2;
                byte[] data_ = ByteUtils.SubArray(data, offset, size);

                uint width_ = Math.Max(1, width >> mipLevel);
                uint height_ = Math.Max(1, height >> mipLevel);
                uint depth_ = Math.Max(1, depth >> mipLevel);

                uint width__ = TegraX1Swizzle.DIV_ROUND_UP(width_, blkWidth);
                uint height__ = TegraX1Swizzle.DIV_ROUND_UP(height_, blkHeight);
                uint depth__ = TegraX1Swizzle.DIV_ROUND_UP(depth_, blkDepth);

                byte[] AlignedData = new byte[(TegraX1Swizzle.round_up(imageOffset, Alignment) - imageOffset)];
                SurfaceSize += (uint)AlignedData.Length;

                MipOffsets[mipLevel] = SurfaceSize;
                if (LinearMode)
                {
                    Pitch = width__ * bpp;

                    if (target == 1)
                        Pitch = TegraX1Swizzle.round_up(width__ * bpp, 32);

                    SurfaceSize += Pitch * height__;
                }
                else
                {
                    if (TegraX1Swizzle.pow2_round_up(height__) < linesPerBlockHeight)
                        blockHeightShift += 1;

                    Pitch = TegraX1Swizzle.round_up(width__ * bpp, 64);
                    SurfaceSize += Pitch * TegraX1Swizzle.round_up(height__, Math.Max(1, blockHeight >> blockHeightShift) * 8);
                }

                Span<byte> SwizzledData = TegraX1Swizzle.swizzle(width_, height_, depth_, blkWidth, blkHeight, blkDepth, target, bpp, (uint)TileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), data_);
                mipmaps.Add(AlignedData.Concat(SwizzledData.ToArray()).ToArray());
            }
            ImageSize = SurfaceSize;
            imageOffset = SurfaceSize;


            Console.WriteLine($"ImageSize {ImageSize}");

            return mipmaps;
        }
    }
}
