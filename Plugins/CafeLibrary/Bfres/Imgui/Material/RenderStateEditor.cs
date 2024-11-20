using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using BfresLibrary.GX2;
using BfresLibrary;
using MapStudio.UI;

namespace CafeLibrary
{
    public class RenderStateEditor
    {
        static string CreateBlendMethod(
         GX2BlendFunction src,
         GX2BlendCombine op,
         GX2BlendFunction dst)
        {
            string source = ConvertFunc(src);
            string dest = ConvertFunc(dst);
            if (op == GX2BlendCombine.Add) return $"{source} + {dest}";
            else if (op == GX2BlendCombine.SourceMinusDestination) return $"{source} - {dest}";
            else if (op == GX2BlendCombine.DestinationMinusSource) return $"{dest} - {source}";
            else if (op == GX2BlendCombine.Maximum) return $"min({source}, {dest})";
            else if (op == GX2BlendCombine.Minimum) return $"max({source}, {dest})";

            return $"";
        }

        static string ConvertFunc(GX2BlendFunction func)
        {
            switch (func)
            {
                case GX2BlendFunction.OneMinusDestinationAlpha: return "(1 - Dst.A)";
                case GX2BlendFunction.OneMinusDestinationColor: return "(1 - Dst.RGB)";
                case GX2BlendFunction.OneMinusConstantAlpha: return "(1 - Const.A)";
                case GX2BlendFunction.OneMinusConstantColor: return "(1 - Const.RGB)";
                case GX2BlendFunction.OneMinusSource1Color: return "(1 - Src1.RGB)";
                case GX2BlendFunction.OneMinusSource1Alpha: return "(1 - Src1.A)";
                case GX2BlendFunction.OneMinusSourceColor: return "(1 - Src.RGB)";
                case GX2BlendFunction.OneMinusSourceAlpha: return "(1 - Src.A)";
                case GX2BlendFunction.ConstantAlpha: return "Const.A";
                case GX2BlendFunction.ConstantColor: return "Const.RGB";
                case GX2BlendFunction.DestinationColor: return "Dst.RGB";
                case GX2BlendFunction.DestinationAlpha: return "Dst.A";
                case GX2BlendFunction.Source1Alpha: return "Src1.A";
                case GX2BlendFunction.Source1Color: return "Src1.RGB";
                case GX2BlendFunction.SourceColor: return "Src.RGB";
                case GX2BlendFunction.SourceAlpha: return "Src.A";
                case GX2BlendFunction.SourceAlphaSaturate: return "(Saturate(Src.A))";
                case GX2BlendFunction.One: return "1";
                case GX2BlendFunction.Zero: return "0";
                default:
                    return "(Unk Op)";
            }
        }
    }
}
