using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class DeferredRenderQuad
    {
        public static GLTexture LUTTexture = null;

        static Plane2DRenderer PlaneRender;
 
        public static void Draw(GLContext control, GLTexture colorPass, GLTexture highlightPass,
            GLTexture bloomPass, RenderFrameArgs frameArgs)
        {
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.CullFace);

            var shader = GlobalShaders.GetShader("FINALHDR");
            control.CurrentShader = shader;

            shader.SetInt("ENABLE_FBO_ALPHA", frameArgs.DisplayAlpha ? 1 : 0);
            shader.SetInt("ENABLE_BLOOM", 0);
            shader.SetInt("ENABLE_LUT", 0);
            shader.SetInt("ENABLE_BACKGROUND", 0);
            shader.SetBoolToInt("ENABLE_SRGB", control.UseSRBFrameBuffer);
            shader.SetVector2("pixelSize", new Vector2(
                1.0f / control.Width,
                1.0f / control.Height));
            shader.SetVector4("highlight_color", new Vector4(GLConstants.SelectColor));
            shader.SetVector4("outline_color", GLConstants.SelectOutlineColor);

            GL.ActiveTexture(TextureUnit.Texture1);
            colorPass.Bind();
            shader.SetInt("uColorTex", 1);

            if (frameArgs.DisplayBackground && DrawableBackground.Display)
            {
                shader.SetInt("ENABLE_BACKGROUND", 1);
             }
            if (highlightPass != null)
            {
                shader.SetTexture(highlightPass, "uHighlightTex", 25);
            }
            if (bloomPass != null && control.EnableBloom)
            {
                shader.SetInt("ENABLE_BLOOM", 1);
                shader.SetTexture(bloomPass, "uBloomTex", 24);
            }
            if (LUTTexture != null)
            {
                shader.SetInt("ENABLE_LUT", 1);
                shader.SetTexture(LUTTexture, "uLutTex", 26);
            }

            if (PlaneRender == null)
                PlaneRender = new Plane2DRenderer(1.0f, true);

            PlaneRender.Draw(control);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.UseProgram(0);
        }
    }
}
        