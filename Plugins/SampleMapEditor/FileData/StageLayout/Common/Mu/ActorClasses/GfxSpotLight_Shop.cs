using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GfxSpotLight_Shop : MuObj
	{
		[BindGUI("AngleDamp", Category = "GfxSpotLight_Shop Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _AngleDamp
		{
			get
			{
				return this.spl__gfx__LocatorSpotLightBancParam.AngleDamp;
			}

			set
			{
				this.spl__gfx__LocatorSpotLightBancParam.AngleDamp = value;
			}
		}

		[BindGUI("DiffuseColor", Category = "GfxSpotLight_Shop Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public System.Numerics.Vector4 _DiffuseColor
		{
			get
			{
				return new System.Numerics.Vector4(
					this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.R,
					this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.G,
					this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.B,
					this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.A);
			}

			set
			{
				this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.R = (float)value.X;
				this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.G = (float)value.Y;
				this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.B = (float)value.Z;
				this.spl__gfx__LocatorSpotLightBancParam.DiffuseColor.A = (float)value.W;
			}
		}

		[BindGUI("DistDamp", Category = "GfxSpotLight_Shop Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DistDamp
		{
			get
			{
				return this.spl__gfx__LocatorSpotLightBancParam.DistDamp;
			}

			set
			{
				this.spl__gfx__LocatorSpotLightBancParam.DistDamp = value;
			}
		}

		[BindGUI("Intensity", Category = "GfxSpotLight_Shop Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Intensity
		{
			get
			{
				return this.spl__gfx__LocatorSpotLightBancParam.Intensity;
			}

			set
			{
				this.spl__gfx__LocatorSpotLightBancParam.Intensity = value;
			}
		}

		[BindGUI("IsEnable", Category = "GfxSpotLight_Shop Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnable
		{
			get
			{
				return this.spl__gfx__LocatorSpotLightBancParam.IsEnable;
			}

			set
			{
				this.spl__gfx__LocatorSpotLightBancParam.IsEnable = value;
			}
		}

		[BindGUI("TurnOnType", Category = "GfxSpotLight_Shop Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _TurnOnType
		{
			get
			{
				return this.spl__gfx__LocatorSpotLightBancParam.TurnOnType;
			}

			set
			{
				this.spl__gfx__LocatorSpotLightBancParam.TurnOnType = value;
			}
		}

		[ByamlMember("spl__gfx__LocatorSpotLightBancParam")]
		public Mu_spl__gfx__LocatorSpotLightBancParam spl__gfx__LocatorSpotLightBancParam { get; set; }

		public GfxSpotLight_Shop() : base()
		{
			spl__gfx__LocatorSpotLightBancParam = new Mu_spl__gfx__LocatorSpotLightBancParam();

			Links = new List<Link>();
		}

		public GfxSpotLight_Shop(GfxSpotLight_Shop other) : base(other)
		{
			spl__gfx__LocatorSpotLightBancParam = other.spl__gfx__LocatorSpotLightBancParam.Clone();
		}

		public override GfxSpotLight_Shop Clone()
		{
			return new GfxSpotLight_Shop(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorSpotLightBancParam.SaveParameterBank(SerializedActor);
		}
	}
}