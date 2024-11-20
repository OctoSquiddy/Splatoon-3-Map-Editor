using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using GLFrameworkEngine;
using OpenTK.Graphics.OpenGL;
using SharpEXR;

namespace MapStudio.UI
{
    public class ImageSharpTextureHelper
    {
        /// <summary>
        /// Exports a uncompressed rgba8 image given the file path, data, width and height
        /// </summary>
        public static void ExportFile(string filePath, byte[] data, int width, int height)
        {
            var file = Image.LoadPixelData<Rgba32>(data, width, height);
            file.Save(filePath);
        }

        public static byte[] DropShadows(byte[] data, int width, int height)
        {
            var file = Image.LoadPixelData<Rgba32>(data, width, height);
            file.Mutate(x => x.GaussianBlur());
            byte[] newData = file.GetSourceInBytes();
            //dispose file
            file.Dispose();
            return newData;
        }

        public static Image<Rgba32> LoadAlphaEncodedExr(string filePath)
        {
            var exrFile = EXRFile.FromFile(filePath);
            var part = exrFile.Parts[0];
            part.OpenParallel(filePath);

            float[] floats = part.GetFloats(ChannelConfiguration.RGB, true, GammaEncoding.Linear, true);
            var w = part.DataWindow.Width;
            var h = part.DataWindow.Height;
            part.Close();

            Console.WriteLine($"Encoding HDR image {filePath}");

            var hdrEncoded = CreateHdrAlpha(floats, w, h);
            return Image.LoadPixelData<Rgba32>(hdrEncoded, w, h);
        }

        /// <summary>
        /// Gets the uncompressed rgba8 image given the file path, width and height
        /// </summary>
        public static byte[] GetRgba(string filePath)
        {
            var file = Image.Load<Rgba32>(filePath);
            byte[] data = file.GetSourceInBytes();
            //dispose file
            file.Dispose();
            return data;
        }

        /// <summary>
        /// Gets the uncompressed rgba8 image given the file path.
        /// The given width and height will resize the output of the rgba data.
        /// </summary>
        public static byte[] GetRgba(string filePath, int width, int height)
        {
            var file = Image.Load<Rgba32>(filePath);
            if (file.Width != width || file.Height != height)
                Resize(file, width, height);

            var data = file.GetSourceInBytes();
            //dispose file
            file.Dispose();
            return data;
        }

        static float GetBrightestPixel(float[] data, int width, int height)
        {
            int index = 0;
            float max_intensity = 0;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    //gray scale
                    OpenTK.Vector3 rgb = new OpenTK.Vector3(
                        data[index + 0],
                        data[index + 1],
                        data[index + 2]);
                    float grayScale = OpenTK.Vector3.Dot(rgb, new OpenTK.Vector3(0.2125f, 0.7154f, 0.0721f));

                    max_intensity = MathF.Max(max_intensity, grayScale);
                    index += 4;
                }
            }
            return max_intensity;
        }

        public static byte[] CreateHdrAlpha(float[] data, int width, int height)
        {
            Framebuffer fbo = new Framebuffer(FramebufferTarget.Framebuffer, width, height);
            fbo.Bind();

            //Shader for hdr alpha baking
            var shader = GlobalShaders.GetShader("HDRBAKE", "Texture/TextureBakeHDR");
            shader.Enable();

            //Create the input texture
            var tex = GLTexture2D.CreateUncompressedTexture(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Bgra, 
                OpenTK.Graphics.OpenGL.PixelType.Float);

            tex.Bind();
            GL.TexImage2D(tex.Target, 0, tex.PixelInternalFormat, width, height, 0,
             tex.PixelFormat, tex.PixelType, data);
            tex.Unbind();

            //Find the brightest pixel
            float brightestPixel = GetBrightestPixel(data, width, height);
            Console.WriteLine($"Brightest pixel {brightestPixel}");

            //Bind texture
            shader.SetTexture(tex, "textureData", 1);
            shader.SetFloat("hdrIntensity", brightestPixel);

            GL.Viewport(0, 0, width, height);
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ScreenQuadRender.Draw();

            GL.Flush();

            var output =  fbo.ReadImageData(true);

            //Dispose
            tex.Dispose();
            fbo.Dispose();

            return output;
        }

      /*  public static byte[] EncodeHDRAlpha(RgbaVector[] data, int width, int height)
        {
            var file = Image.LoadPixelData<RgbaVector>(data, width, height);
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < width; h++)
                {
                    var pixel = file[w, h];
                    file.c
                }
            }
        }*/

        public static byte[] DecodeHdrAlpha(byte[] data, int width, int height)
        {
            Framebuffer fbo = new Framebuffer(FramebufferTarget.Framebuffer, width, height);
            fbo.Bind();

            //Shader for hdr alpha baking
            var shader = GlobalShaders.GetShader("HDRBAKE", "Texture/TextureBakeHDRDecode");
            shader.Enable();

            //Create the input texture
            var tex = GLTexture2D.CreateUncompressedTexture(width, height);
            tex.Reload(width, height, data);

            //Bind texture
            shader.SetTexture(tex, "textureData", 1);

            //Draw a screen quad

            GL.Viewport(0, 0, width, height);
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ScreenQuadRender.Draw();

            GL.Flush();

            var output = fbo.ReadImageData(true);

            //Dispose
            tex.Dispose();
            fbo.Dispose();

            return output;
        }

        /// <summary>
        /// Resizes the given image with a new width and height using the Lanczos3 resampler algoritim.
        /// </summary>
        public static void Resize(Image<Rgba32> baseImage, int newWidth, int newHeight) {
            baseImage.Mutate(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));
        }

        /// <summary>
        /// Generates mipmaps with the given mipmap count from the image provided.
        /// </summary>
        public static Image<Rgba32>[] GenerateMipmaps(Image<Rgba32> baseImage, uint mipLevelCount)
        {
            Image<Rgba32>[] mipLevels = new Image<Rgba32>[mipLevelCount];
            mipLevels[0] = baseImage;
            int i = 1;

            int currentWidth = baseImage.Width;
            int currentHeight = baseImage.Height;
            while ((currentWidth != 1 || currentHeight != 1) && i < mipLevelCount)
            {
                int newWidth = Math.Max(1, currentWidth / 2);
                int newHeight = Math.Max(1, currentHeight / 2);
                Image<Rgba32> newImage = baseImage.Clone(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));
                Debug.Assert(i < mipLevelCount);
                mipLevels[i] = newImage;

                i++;
                currentWidth = newWidth;
                currentHeight = newHeight;
            }

            Debug.Assert(i == mipLevelCount);

            return mipLevels;
        }
    }
}
