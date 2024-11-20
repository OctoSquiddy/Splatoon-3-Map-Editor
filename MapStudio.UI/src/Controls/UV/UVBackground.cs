using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class UVBackground
    {
        static PlaneRenderer QuadRender;

        public static void Init()
        {
            if (QuadRender == null)
                QuadRender = new PlaneRenderer();
        }

        public static void Draw(GenericRenderer.TextureView texture, float brightness,
            STGenericTextureMap textureMap,  Vector2 aspectScale, Viewport2D.Camera2D camera)
        {
            Vector2 bgscale = new Vector2(1000, 1000);

            Init();

            GL.Disable(EnableCap.CullFace);

            var shader = GlobalShaders.GetShader("UV_WINDOW");
            shader.Enable();

            var cameraMtx = camera.ViewMatrix * camera.ProjectionMatrix;
            shader.SetMatrix4x4("mtxCam", ref cameraMtx);

            GL.ActiveTexture(TextureUnit.Texture1);
            BindTexture(texture, textureMap);
            shader.SetInt("uvTexture", 1);
            shader.SetInt("hasTexture", 1);
            shader.SetVector2("scale", bgscale * aspectScale);
            shader.SetVector2("texCoordScale", bgscale);
            shader.SetVector4("uColor", new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
            shader.SetFloat("brightness", brightness);
            shader.SetBoolToInt("isBC5S", false);

            if (texture != null) {
                shader.SetBoolToInt("isSRGB", texture.IsSRGB);
                shader.SetBoolToInt("isBC5S", texture.Format == TexFormat.BC5_SNORM);
            }

            //Draw background
            QuadRender.UpdatePrimitiveType(PrimitiveType.TriangleStrip);
            QuadRender.Draw(shader);

            //Draw main texture quad inside boundings (0, 1)
            shader.SetVector2("scale", aspectScale);
            shader.SetVector2("texCoordScale", new Vector2(1));
            shader.SetVector4("uColor", new Vector4(1));

            QuadRender.UpdatePrimitiveType(PrimitiveType.TriangleStrip);
            QuadRender.Draw(shader);

            //Draw outline of boundings (0, 1)
            shader.SetInt("hasTexture", 0);
            shader.SetVector2("scale", aspectScale);
            shader.SetVector2("texCoordScale", new Vector2(1));
            shader.SetVector4("uColor", new Vector4(0,0,0,1));

            QuadRender.UpdatePrimitiveType(PrimitiveType.LineLoop);
            QuadRender.Draw(shader);

            GL.Enable(EnableCap.CullFace);
        }

        static void BindTexture(GenericRenderer.TextureView tex, STGenericTextureMap texMap)
        {
            if (tex == null)
                return;

            if (tex.RenderTexture == null)
                return;

            var target = ((GLTexture)tex.RenderTexture).Target;
            var texID = tex.RenderTexture.ID;

            GL.BindTexture(target, texID);
            GL.TexParameter(target, TextureParameterName.TextureWrapS, (float)OpenGLHelper.WrapMode[texMap.WrapU]);
            GL.TexParameter(target, TextureParameterName.TextureWrapT, (float)OpenGLHelper.WrapMode[texMap.WrapV]);
            GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)OpenGLHelper.MinFilter[texMap.MinFilter]);
            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)OpenGLHelper.MagFilter[texMap.MagFilter]);

            int[] mask = new int[4]
              {
                        GetSwizzle(tex.RedChannel),
                        GetSwizzle(tex.GreenChannel),
                        GetSwizzle(tex.BlueChannel),
                        GetSwizzle(tex.AlphaChannel),
              };
            GL.TexParameter(target, TextureParameterName.TextureSwizzleRgba, mask);
        }

        static int GetSwizzle(STChannelType channel)
        {
            switch (channel)
            {
                case STChannelType.Red: return (int)All.Red;
                case STChannelType.Green: return (int)All.Green;
                case STChannelType.Blue: return (int)All.Blue;
                case STChannelType.Alpha: return (int)All.Alpha;
                case STChannelType.One: return (int)All.One;
                case STChannelType.Zero: return (int)All.Zero;
                default: return 0;
            }
        }
    }
}
