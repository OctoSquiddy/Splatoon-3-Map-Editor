using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMapEditor
{
    public class MuTeamColorDataSet
    {
        public class MuTeamColorData
        {
            public float AlphaHueOffset { get; set; } = 0.0f;

            public float AlphaHueOffsetDetailBright { get; set; } = 0.0f;

            public float AlphaHueOffsetDetailDark { get; set; } = 0.0f;

            public bool AlphaHueOffsetDetailEnable { get; set; } = false;

            public float AlphaInkRimIntensity { get; set; } = 1.0f;

            public float AlphaInkRimIntensityNight { get; set; } = 1.0f;

            public float AlphaMapInkBrightnessOffset { get; set; } = 0.0f;

            public System.Numerics.Vector4 AlphaNightTeamColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 AlphaTeamColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 AlphaUIColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 AlphaUIEffectColor { get; set; } = new System.Numerics.Vector4();

            public float BravoHueOffset { get; set; } = 0.0f;

            public float BravoHueOffsetDetailBright { get; set; } = 0.0f;

            public float BravoHueOffsetDetailDark { get; set; } = 0.0f;

            public bool BravoHueOffsetDetailEnable { get; set; } = false;

            public float BravoInkRimIntensity { get; set; } = 1.0f;

            public float BravoInkRimIntensityNight { get; set; } = 1.0f;

            public float BravoMapInkBrightnessOffset { get; set; } = 0.0f;

            public System.Numerics.Vector4 BravoNightTeamColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 BravoTeamColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 BravoUIColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 BravoUIEffectColor { get; set; } = new System.Numerics.Vector4();

            public float CharlieHueOffset { get; set; } = 0.0f;

            public float CharlieHueOffsetDetailBright { get; set; } = 0.0f;

            public float CharlieHueOffsetDetailDark { get; set; } = 0.0f;

            public bool CharlieHueOffsetDetailEnable { get; set; } = false;

            public float CharlieInkRimIntensity { get; set; } = 1.0f;

            public float CharlieInkRimIntensityNight { get; set; } = 1.0f;

            public float CharlieMapInkBrightnessOffset { get; set; } = 0.0f;

            public System.Numerics.Vector4 CharlieNightTeamColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 CharlieTeamColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 CharlieUIColor { get; set; } = new System.Numerics.Vector4();

            public System.Numerics.Vector4 CharlieUIEffectColor { get; set; } = new System.Numerics.Vector4();

            public bool EnableInkRimIntensity { get; set; } = false;

            public bool EnableMapInkBrightnessOffset { get; set; } = false;

            public bool HueOffsetEnable { get; set; } = false;

            public bool IsSetNightTeamColor { get; set; } = false;

            public bool IsSetUIColor { get; set; } = false;

            public bool IsSetUIEffectColor { get; set; } = false;

            public System.Numerics.Vector4 NeutralColor { get; set; } = new System.Numerics.Vector4();

            public float NeutralHueOffset { get; set; } = 0.0f;

            public float NeutralHueOffsetDetailBright { get; set; } = 0.0f;

            public float NeutralHueOffsetDetailDark { get; set; } = 0.0f;

            public bool NeutralHueOffsetDetailEnable { get; set; } = false;

            public string Tag { get; set; } = "Mission";

            public string __RowId { get; set; } = "";

            public MuTeamColorData() { }

            public MuTeamColorData(MuTeamColorData other)
            {
                AlphaHueOffset = other.AlphaHueOffset;
                AlphaHueOffsetDetailBright = other.AlphaHueOffsetDetailBright;
                AlphaHueOffsetDetailDark = other.AlphaHueOffsetDetailDark;
                AlphaHueOffsetDetailEnable = other.AlphaHueOffsetDetailEnable;
                AlphaInkRimIntensity = other.AlphaInkRimIntensity;
                AlphaInkRimIntensityNight = other.AlphaInkRimIntensityNight;
                AlphaMapInkBrightnessOffset = other.AlphaMapInkBrightnessOffset;
                AlphaNightTeamColor = CloneVector4(other.AlphaNightTeamColor);
                AlphaTeamColor = CloneVector4(other.AlphaTeamColor);
                AlphaUIColor = CloneVector4(other.AlphaUIColor);
                AlphaUIEffectColor = CloneVector4(other.AlphaUIEffectColor);

                BravoHueOffset = other.BravoHueOffset;
                BravoHueOffsetDetailBright = other.BravoHueOffsetDetailBright;
                BravoHueOffsetDetailDark = other.BravoHueOffsetDetailDark;
                BravoHueOffsetDetailEnable = other.BravoHueOffsetDetailEnable;
                BravoInkRimIntensity = other.BravoInkRimIntensity;
                BravoInkRimIntensityNight = other.BravoInkRimIntensityNight;
                BravoMapInkBrightnessOffset = other.BravoMapInkBrightnessOffset;
                BravoNightTeamColor = CloneVector4(other.BravoNightTeamColor);
                BravoTeamColor = CloneVector4(other.BravoTeamColor);
                BravoUIColor = CloneVector4(other.BravoUIColor);
                BravoUIEffectColor = CloneVector4(other.BravoUIEffectColor);

                CharlieHueOffset = other.CharlieHueOffset;
                CharlieHueOffsetDetailBright = other.CharlieHueOffsetDetailBright;
                CharlieHueOffsetDetailDark = other.CharlieHueOffsetDetailDark;
                CharlieHueOffsetDetailEnable = other.CharlieHueOffsetDetailEnable;
                CharlieInkRimIntensity = other.CharlieInkRimIntensity;
                CharlieInkRimIntensityNight = other.CharlieInkRimIntensityNight;
                CharlieMapInkBrightnessOffset = other.CharlieMapInkBrightnessOffset;
                CharlieNightTeamColor = CloneVector4(other.CharlieNightTeamColor);
                CharlieTeamColor = CloneVector4(other.CharlieTeamColor);
                CharlieUIColor = CloneVector4(other.CharlieUIColor);
                CharlieUIEffectColor = CloneVector4(other.CharlieUIEffectColor);

                EnableInkRimIntensity = other.EnableInkRimIntensity;
                EnableMapInkBrightnessOffset = other.EnableMapInkBrightnessOffset;
                HueOffsetEnable = other.HueOffsetEnable;
                IsSetNightTeamColor = other.IsSetNightTeamColor;
                IsSetUIColor = other.IsSetUIColor;
                IsSetUIEffectColor = other.IsSetUIEffectColor;

                NeutralColor = CloneVector4(other.NeutralColor);
                NeutralHueOffset = other.NeutralHueOffset;
                NeutralHueOffsetDetailBright = other.NeutralHueOffsetDetailBright;
                NeutralHueOffsetDetailDark = other.NeutralHueOffsetDetailDark;
                NeutralHueOffsetDetailEnable = other.NeutralHueOffsetDetailEnable;

                Tag = other.Tag;
                __RowId = other.__RowId;
            }
            public MuTeamColorData Clone()
            {
                return new MuTeamColorData(this);
            }

            private System.Numerics.Vector4 CloneVector4(System.Numerics.Vector4 other)
            {
                return new System.Numerics.Vector4(other.X, other.Y, other.Z, other.W);
            }
        }

        public List<MuTeamColorData> ColorDataArray { get; set; } = new List<MuTeamColorData>();

        public bool IsZSTDCompressed { get; set; } = false;

        public string FileName { get; set; } = "";

        public bool IsFileFound { get; set; } = false;
    }
}
