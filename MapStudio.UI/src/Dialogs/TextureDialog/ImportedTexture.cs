using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Toolbox.Core;
using Toolbox.Core.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace MapStudio.UI
{
    public class ImportedTexture
    {
        /// <summary>
        /// The texture format of the imported texture.
        /// </summary>
        public TexFormat Format
        {
            get { return _format; }
            set
            {
                if (_format != value) {
                    _format = value;
                    OnFormatUpdated();
                }
            }
        }

        private TexFormat _format;

        /// <summary>
        /// The file path of the imported texture.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The name of the texture.
        /// </summary>
        public string Name { get; set; }

        public bool IsArrayType { get; set; } = true;

        /// <summary>
        /// The time it took to encode the image.
        /// </summary>
        public string EncodingTime = "";

        /// <summary>
        /// The width of the image.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the image.
        /// </summary>
        public int Height { get; private set; }

        private uint _mipCount;

        /// <summary>
        /// The mip count of the image.
        /// </summary>
        public uint MipCount
        {
            get { return _mipCount; }
            set
            {
                value = Math.Max(value, 1);
                value = Math.Min(value, MaxMipCount);
                _mipCount = value;
                //re encode all surfaces to adjust the mip count
                foreach (var surface in Surfaces)
                    surface.Encoded = false;
            }
        }

        private uint MaxMipCount = 13;

        /// <summary>
        /// The red channel output of the image.
        /// </summary>
        public STChannelType ChannelRed { get; set; } = STChannelType.Red;

        /// <summary>
        /// The green channel output of the image.
        /// </summary>
        public STChannelType ChannelGreen { get; set; } = STChannelType.Green;

        /// <summary>
        /// The blue channel output of the image.
        /// </summary>
        public STChannelType ChannelBlue { get; set; } = STChannelType.Blue;

        /// <summary>
        /// The alpha channel output of the image.
        /// </summary>
        public STChannelType ChannelAlpha { get; set; } = STChannelType.Alpha;

        /// <summary>
        /// 
        /// </summary>
        public int ActiveArrayIndex = 0;

        /// <summary>
        /// Determines if the texture has been encoded in the "Format" or not.
        /// </summary>
        public bool Encoded
        {
            get { return Surfaces[ActiveArrayIndex].Encoded; }
            set
            {
                Surfaces[ActiveArrayIndex].Encoded = value;
            }
        }

        //Cache the encoded data so it can be applied when dialog is finished.
        //This prevents re encoding again saving time.
        public List<Surface> Surfaces = new List<Surface>();

        public IPlatformSwizzle PlatformSwizzle { get; set; }

        public void OnFormatUpdated()
        {
            if (Format == TexFormat.BC4_UNORM || Format == TexFormat.BC4_SNORM)
            {
                ChannelRed = STChannelType.Red;
                ChannelGreen = STChannelType.Red;
                ChannelBlue = STChannelType.Red;
                ChannelAlpha = STChannelType.Red;
            }
            else if (Format == TexFormat.BC5_UNORM || Format == TexFormat.BC5_SNORM)
            {
                ChannelRed = STChannelType.Red;
                ChannelGreen = STChannelType.Green;
                ChannelBlue = STChannelType.Zero;
                ChannelAlpha = STChannelType.One;
            }
            else
            {
                ChannelRed = STChannelType.Red;
                ChannelBlue = STChannelType.Blue;
                ChannelGreen = STChannelType.Green;
                ChannelAlpha = STChannelType.Alpha;
            }
        }

        public ImportedTexture(string fileName) {
            FilePath = fileName;
            Name = Path.GetFileNameWithoutExtension(fileName);
            Format = TexFormat.BC1_SRGB;

            Surfaces.Add(new Surface(fileName));
            Reload(0);
        }

        public ImportedTexture(string name, byte[] rgbaData, uint width, uint height, uint mipCount, TexFormat format)
        {
            Name = name;
            Format = format;
            Width = (int)width;
            Height = (int)height;
            MipCount = mipCount;
            Surfaces.Add(new Surface(rgbaData, Width, Height));
        }

        /// <summary>
        /// Adds a surface to the imported texture.
        /// </summary>
        public void AddSurface(string filePath)
        {
            var surface = new Surface(filePath);
            surface.Reload(Width, Height);
            Surfaces.Add(surface);
        }

        /// <summary>
        /// Removes a surface from the imported texture.
        /// </summary>
        public void RemoveSurface(int index)
        {
            var surface = Surfaces[index];
            surface.Dispose();
            Surfaces.RemoveAt(index);
        }

        /// <summary>
        /// Reloads the surface from its loaded file path.
        /// </summary>
        public void Reload(int index) {
            var surface = Surfaces[index];
            surface.Reload();
            Width = surface.ImageFile.Width;
            Height = surface.ImageFile.Height;
            MaxMipCount = CalculateMipCount();
            MipCount = MaxMipCount;
        }

        /// <summary>
        /// Disposes all surfaces in the imported texture.
        /// </summary>
        public void Dispose()
        {
            foreach (var surface in Surfaces)
                surface?.Dispose();
        }

        /// <summary>
        /// Decodes the current surface.
        /// </summary>
        public byte[] DecodeTexture(int index)
        {
            //Get the encoded data and turn back into raw rgba data for previewing purposes
            var surface = Surfaces[index];
            var encoded = surface.Mipmaps[0]; //Only need first mip level

            if (Format == TexFormat.BC5_SNORM) {
                return Toolbox.Core.TextureDecoding.DXT.DecompressBC5(encoded, Width, Height, true);
            }

            byte[] decoded = new byte[Width * Height * 4];
            foreach (var decoder in FileManager.GetTextureDecoders())
            {
                if (decoder.Decode(Format, encoded, Width, Height, out decoded))
                    return decoded;
            }
            return decoded;
        }

        /// <summary>
        /// Encodes the current surface.
        /// </summary>
        public void EncodeTexture(int index)
        {
            var surface = Surfaces[index];

            //raw
            if (Format == TexFormat.RGBA8_SRGB || Format == TexFormat.RGBA8_UNORM)
            {
                surface.Mipmaps = surface.GenerateMipMaps(MipCount);
                Encoded = true;
                EncodingTime = "0ms";
                return;
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var decoders = FileManager.GetTextureDecoders();
            //Force DirectXTexLibrary to be used first (if the OS and hardware supports it)
            foreach (var decoder in decoders)
            {
                if (decoder.ToString().Contains("DirectXTexLibrary"))
                    EncodeSurface(decoder, surface);
            }
            //Use normal software decoders (slower) if it hasn't been encoded yet
            foreach (var decoder in decoders) {
                if (!Encoded && decoder.CanEncode(Format))
                    EncodeSurface(decoder, surface);
            }
            //Failed to encode :(
            if (!Encoded)
            {
                surface.Mipmaps = surface.GenerateMipMaps(MipCount);
                return;
            }


            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            EncodingTime = string.Format("{0:00}ms", ts.Milliseconds);
        }

        private void EncodeSurface(ITextureDecoder decoder, Surface surface)
        {
            var mipMaps = surface.GenerateMipMaps(MipCount);

            surface.Mipmaps.Clear();
            for (int i = 0; i < MipCount; i++)
            {
                int mipWidth = Math.Max(1, Width >> i);
                int mipHeight = Math.Max(1, Height >> i);

                byte[] encoded;
                decoder.Encode(Format, mipMaps[i], mipWidth, mipHeight, out encoded);
                if (encoded == null)
                    throw new Exception($"Failed to encode image! {Format}");

                surface.Mipmaps.Add(encoded);
            }
            Encoded = true;
        }

        private uint CalculateMipCount() {
            return 1 + (uint)Math.Floor(Math.Log(Math.Max(Width, Height), 2));
        }
        
        /// <summary>
        /// Attempts to detect what format might be best suited based on the image contents.
        /// </summary>
        public TexFormat TryDetectFormat(TexFormat defaultFormat)
        {
            var imageData = Surfaces[0].ImageFile.GetSourceInBytes();
            int index = 0;
            int stride = 4;
            bool isAlphaTranslucent = false;
            bool hasAlpha = false;

            bool isGrayscale = true;

            for (int w = 0; w < Width; w++) {
                for (int h = 0; h < Height; h++) {
                    byte red   = imageData[index];
                    byte green = imageData[index+1];
                    byte blue  = imageData[index+2];
                    byte alpha = imageData[index+3];

                    //not grayscale
                    if (red != green && red != blue)
                        isGrayscale = false;

                    //alpha supported
                    if (alpha != 255)
                        hasAlpha = true;

                    //alpha values > 0 and < 255 supported
                    if (alpha != 255 && alpha != 0)
                        isAlphaTranslucent = true;

                    index += stride;
                }
            }

            //Red only
            if (isGrayscale)
                return TexFormat.BC4_UNORM;
            //Has transparency
            if (isAlphaTranslucent)
                return TexFormat.BC3_SRGB;
            //Has alpha
            if (hasAlpha)
                return TexFormat.BC1_SRGB;

            return defaultFormat;
        }

        public class Surface
        {
            /// <summary>
            /// Determines if the texture has been encoded in the "Format" or not.
            /// </summary>
            public bool Encoded { get; set; }

            /// <summary>
            /// The encoded mip map data of the surface.
            /// </summary>
            public List<byte[]> Mipmaps = new List<byte[]>();

            /// <summary>
            /// The original file path of the image imported.
            /// </summary>
            public string SourceFilePath { get; set; }

            /// <summary>
            /// The raw image file data.
            /// </summary>
            public Image<Rgba32> ImageFile;

            public Surface(string fileName) {
                SourceFilePath = fileName;
            }

            public Surface(byte[] rgba, int width, int height) {
                ImageFile = Image.LoadPixelData<Rgba32>(rgba, width, height);
            }

            public void Reload() {
                if (ImageFile != null)
                    ImageFile.Dispose();

                if (SourceFilePath.EndsWith(".exr"))
                    ImageFile = ImageSharpTextureHelper.LoadAlphaEncodedExr(SourceFilePath);
                else if (SourceFilePath.EndsWith(".tiff") || SourceFilePath.EndsWith(".tif"))
                {
                    var bitmap = new System.Drawing.Bitmap(SourceFilePath);
                    var rgba = BitmapExtension.ImageToByte(bitmap);
                    ImageFile = Image.LoadPixelData<Rgba32>(rgba, bitmap.Width, bitmap.Height);
                    bitmap.Dispose();
                }
                else
                    ImageFile = Image.Load<Rgba32>(SourceFilePath);
            }

            public void Reload(int width, int height) {
                ImageFile = Image.Load<Rgba32>(SourceFilePath);
                if (ImageFile.Width != width || ImageFile.Height != height) 
                    ImageSharpTextureHelper.Resize(ImageFile, width, height);
            }

            public List<byte[]> GenerateMipMaps(uint mipCount)
            {
                var mipmaps = ImageSharpTextureHelper.GenerateMipmaps(ImageFile, mipCount);

                List<byte[]> output = new List<byte[]>();
                for (int i = 0; i < mipCount; i++)
                {
                    output.Add(mipmaps[i].GetSourceInBytes());
                    //Dispose base images after if not the base image
                    if (i != 0)
                        mipmaps[i].Dispose();
                }
                return output;
            }

            public void Dispose() {
                ImageFile?.Dispose();
            }
        }
    }
}
