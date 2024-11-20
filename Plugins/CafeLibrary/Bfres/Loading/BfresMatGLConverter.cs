using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BfresLibrary;
using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.GX2;
using Toolbox.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using GLFrameworkEngine;

namespace CafeLibrary.Rendering
{
    public class BfresMatGLConverter
    {
        public static void ConvertRenderState(FMAT fmat, Syroot.NintenTools.NSW.Bfres.Material mat)
        {
            //Switch support
            if (mat.RenderInfos.Any(x => x.Name == "gsys_render_state_mode"))
                ConvertSwitchRenderState(fmat, mat);
        }

        static void ConvertSwitchRenderState(FMAT fmat, Syroot.NintenTools.NSW.Bfres.Material mat)
        {
            string blend = GetRenderInfo(mat, "gsys_render_state_blend_mode");
            //Alpha test
            string alphaTest = GetRenderInfo(mat, "gsys_alpha_test_enable");
            string alphaFunc = GetRenderInfo(mat, "gsys_alpha_test_func");
            float alphaValue = GetRenderInfo(mat, "gsys_alpha_test_value");

            string colorOp = GetRenderInfo(mat, "gsys_color_blend_rgb_op");
            string colorDst = GetRenderInfo(mat, "gsys_color_blend_rgb_dst_func");
            string colorSrc = GetRenderInfo(mat, "gsys_color_blend_rgb_src_func");
            float[] blendColorF32 = mat.RenderInfos.First(x => x.Name == "gsys_color_blend_const_color").GetValueSingles();

            string alphaOp = GetRenderInfo(mat, "gsys_color_blend_alpha_op");
            string alphaDst = GetRenderInfo(mat, "gsys_color_blend_alpha_dst_func");
            string alphaSrc = GetRenderInfo(mat, "gsys_color_blend_alpha_src_func");

            string depthTest = GetRenderInfo(mat, "gsys_depth_test_enable");
            string depthTestFunc = GetRenderInfo(mat, "gsys_depth_test_func");
            string depthWrite = GetRenderInfo(mat, "gsys_depth_test_write");
            string state = GetRenderInfo(mat, "gsys_render_state_mode");

            if (state == "opaque")
                fmat.BlendState.State = GLMaterialBlendState.BlendState.Opaque;
            else if (state == "translucent")
                fmat.BlendState.State = GLMaterialBlendState.BlendState.Translucent;
            else if (state == "mask")
                fmat.BlendState.State = GLMaterialBlendState.BlendState.Mask;
            else
                fmat.BlendState.State = GLMaterialBlendState.BlendState.Custom;

            string displayFace = GetRenderInfo(mat, "gsys_render_state_display_face");
            if (displayFace == "front")
            {
                fmat.CullFront = false;
                fmat.CullBack = true;
            }
            if (displayFace == "back")
            {
                fmat.CullFront = true;
                fmat.CullBack = false;
            }
            if (displayFace == "both")
            {
                fmat.CullFront = false;
                fmat.CullBack = false;
            }
            if (displayFace == "none")
            {
                fmat.CullFront = true;
                fmat.CullBack = true;
            }

            if (!string.IsNullOrEmpty(state) && state != "opaque")
                fmat.IsTransparent = true;

            fmat.BlendState.Color = new OpenTK.Vector4(blendColorF32[0], blendColorF32[1], blendColorF32[2], blendColorF32[3]);
            fmat.BlendState.BlendColor = blend == "color";
            fmat.BlendState.DepthTest = depthTest == "true";
            fmat.BlendState.DepthWrite = depthWrite == "true";
            fmat.BlendState.AlphaTest = alphaTest == "true";
            fmat.BlendState.AlphaValue = alphaValue;

            if (alphaFunc == "always")
                fmat.BlendState.AlphaFunction = AlphaFunction.Always;
            if (alphaFunc == "equal")
                fmat.BlendState.AlphaFunction = AlphaFunction.Equal;
            if (alphaFunc == "lequal")
                fmat.BlendState.AlphaFunction = AlphaFunction.Lequal;
            if (alphaFunc == "gequal")
                fmat.BlendState.AlphaFunction = AlphaFunction.Gequal;
            if (alphaFunc == "less")
                fmat.BlendState.AlphaFunction = AlphaFunction.Less;
            if (alphaFunc == "greater")
                fmat.BlendState.AlphaFunction = AlphaFunction.Greater;
            if (alphaFunc == "never")
                fmat.BlendState.AlphaFunction = AlphaFunction.Never;
        }

        public static dynamic GetRenderInfo(Syroot.NintenTools.NSW.Bfres.Material mat, string name, int index = 0)
        {
            if (mat.RenderInfos.Any(x => x.Name == name))
            {
                if (mat.RenderInfos.First(x => x.Name == name)._value == null)
                    return null;

                switch (mat.RenderInfos.First(x => x.Name == name).Type)
                {
                    case Syroot.NintenTools.NSW.Bfres.RenderInfoType.Int32: 
                        return mat.RenderInfos.First(x => x.Name == name).GetValueInt32s()[index];
                    case Syroot.NintenTools.NSW.Bfres.RenderInfoType.String:
                        if (mat.RenderInfos.First(x => x.Name == name).GetValueStrings().Length > index)
                            return mat.RenderInfos.First(x => x.Name == name).GetValueStrings()[index];
                        else
                            return null;
                    case Syroot.NintenTools.NSW.Bfres.RenderInfoType.Single: 
                        return mat.RenderInfos.First(x => x.Name == name).GetValueSingles()[index];
                }
            }
            return null;
        }

        static BlendEquationMode ConvertOp(GX2BlendCombine func)
        {
            switch (func)
            {
                case GX2BlendCombine.Add: return BlendEquationMode.FuncAdd;
                case GX2BlendCombine.SourceMinusDestination: return BlendEquationMode.FuncSubtract;
                case GX2BlendCombine.DestinationMinusSource: return BlendEquationMode.FuncReverseSubtract;
                case GX2BlendCombine.Maximum: return BlendEquationMode.Max;
                case GX2BlendCombine.Minimum: return BlendEquationMode.Min;
                default: return BlendEquationMode.FuncAdd;

            }
        }


        static BlendingFactor ConvertBlend(GX2BlendFunction func)
        {
            switch (func)
            {
                case GX2BlendFunction.ConstantAlpha: return BlendingFactor.ConstantAlpha;
                case GX2BlendFunction.ConstantColor: return BlendingFactor.ConstantColor;
                case GX2BlendFunction.DestinationColor: return BlendingFactor.DstColor;
                case GX2BlendFunction.DestinationAlpha: return BlendingFactor.DstAlpha;
                case GX2BlendFunction.One: return BlendingFactor.One;
                case GX2BlendFunction.OneMinusConstantAlpha: return BlendingFactor.OneMinusConstantAlpha;
                case GX2BlendFunction.OneMinusConstantColor: return BlendingFactor.OneMinusConstantColor;
                case GX2BlendFunction.OneMinusDestinationAlpha: return BlendingFactor.OneMinusDstAlpha;
                case GX2BlendFunction.OneMinusDestinationColor: return BlendingFactor.OneMinusDstColor;
                case GX2BlendFunction.OneMinusSourceAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case GX2BlendFunction.OneMinusSourceColor: return BlendingFactor.OneMinusSrcColor;
                case GX2BlendFunction.OneMinusSource1Alpha: return (BlendingFactor)BlendingFactorSrc.OneMinusSrc1Alpha;
                case GX2BlendFunction.OneMinusSource1Color: return (BlendingFactor)BlendingFactorSrc.OneMinusSrc1Color;
                case GX2BlendFunction.SourceAlpha: return BlendingFactor.SrcAlpha;
                case GX2BlendFunction.SourceAlphaSaturate: return BlendingFactor.SrcAlphaSaturate;
                case GX2BlendFunction.Source1Alpha: return BlendingFactor.Src1Alpha;
                case GX2BlendFunction.SourceColor: return BlendingFactor.SrcColor;
                case GX2BlendFunction.Source1Color: return BlendingFactor.Src1Color;
                case GX2BlendFunction.Zero: return BlendingFactor.Zero;
                default: return BlendingFactor.One;
            }
        }

        public static STTextureMinFilter ConvertMinFilter(GX2TexMipFilterType mip, GX2TexXYFilterType wrap)
        {
            if (mip == GX2TexMipFilterType.Linear)
            {
                switch (wrap)
                {
                    case GX2TexXYFilterType.Bilinear: return STTextureMinFilter.LinearMipmapLinear;
                    case GX2TexXYFilterType.Point: return STTextureMinFilter.NearestMipmapLinear;
                    default: return STTextureMinFilter.LinearMipmapNearest;
                }
            }
            else if (mip == GX2TexMipFilterType.Point)
            {
                switch (wrap)
                {
                    case GX2TexXYFilterType.Bilinear: return STTextureMinFilter.LinearMipmapNearest;
                    case GX2TexXYFilterType.Point: return STTextureMinFilter.NearestMipmapNearest;
                    default: return STTextureMinFilter.NearestMipmapLinear;
                }
            }
            else
            {
                switch (wrap)
                {
                    case GX2TexXYFilterType.Bilinear: return STTextureMinFilter.Linear;
                    case GX2TexXYFilterType.Point: return STTextureMinFilter.Nearest;
                    default: return STTextureMinFilter.Linear;
                }
            }
        }

        public static STTextureMagFilter ConvertMagFilter(GX2TexXYFilterType wrap)
        {
            switch (wrap)
            {
                case GX2TexXYFilterType.Bilinear: return STTextureMagFilter.Linear;
                case GX2TexXYFilterType.Point: return STTextureMagFilter.Nearest;
                default: return STTextureMagFilter.Linear;
            }
        }

        public static STTextureWrapMode ConvertWrapMode(Syroot.NintenTools.NSW.Bfres.GFX.TexClamp wrap)
        {
            switch (wrap)
            {
                case Syroot.NintenTools.NSW.Bfres.GFX.TexClamp.Repeat: return STTextureWrapMode.Repeat;
                case Syroot.NintenTools.NSW.Bfres.GFX.TexClamp.Clamp: return STTextureWrapMode.Clamp;
                case Syroot.NintenTools.NSW.Bfres.GFX.TexClamp.ClampToEdge: return STTextureWrapMode.Clamp;
                case Syroot.NintenTools.NSW.Bfres.GFX.TexClamp.Mirror: return STTextureWrapMode.Mirror;
                case Syroot.NintenTools.NSW.Bfres.GFX.TexClamp.MirrorOnce: return STTextureWrapMode.Mirror;
                case Syroot.NintenTools.NSW.Bfres.GFX.TexClamp.MirrorOnceClampToEdge: return STTextureWrapMode.Mirror;
                default: return STTextureWrapMode.Clamp;
            }
        }
    }
}
