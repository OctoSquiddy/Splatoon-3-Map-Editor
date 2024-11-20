using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GfxPointLightDynamic_D100 : MuObj
	{
		[BindGUI("DampParam", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DampParam
		{
			get
			{
				return this.spl__gfx__LocatorPointLightBancParam.DampParam;
			}

			set
			{
				this.spl__gfx__LocatorPointLightBancParam.DampParam = value;
			}
		}

		[BindGUI("DebugDisplay", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _DebugDisplay
		{
			get
			{
				return this.spl__gfx__LocatorPointLightBancParam.DebugDisplay;
			}

			set
			{
				this.spl__gfx__LocatorPointLightBancParam.DebugDisplay = value;
			}
		}

		[BindGUI("DiffuseColor", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public System.Numerics.Vector4 _DiffuseColor
		{
			get
			{
				return new System.Numerics.Vector4(
					this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.R,
					this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.G,
					this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.B,
					this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.A);
			}

			set
			{
				this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.R = (float)value.X;
				this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.G = (float)value.Y;
				this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.B = (float)value.Z;
				this.spl__gfx__LocatorPointLightBancParam.DiffuseColor.A = (float)value.W;
			}
		}

		[BindGUI("Intensity", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Intensity
		{
			get
			{
				return this.spl__gfx__LocatorPointLightBancParam.Intensity;
			}

			set
			{
				this.spl__gfx__LocatorPointLightBancParam.Intensity = value;
			}
		}

		[BindGUI("IsEnable", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnable
		{
			get
			{
				return this.spl__gfx__LocatorPointLightBancParam.IsEnable;
			}

			set
			{
				this.spl__gfx__LocatorPointLightBancParam.IsEnable = value;
			}
		}

		[BindGUI("TurnOnType", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _TurnOnType
		{
			get
			{
				return this.spl__gfx__LocatorPointLightBancParam.TurnOnType;
			}

			set
			{
				this.spl__gfx__LocatorPointLightBancParam.TurnOnType = value;
			}
		}

		[BindGUI("AngleDamp", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DiffuseColor1", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public System.Numerics.Vector4 _DiffuseColor1
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

		[BindGUI("DistDamp", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Intensity1", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Intensity1
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

		[BindGUI("IsEnable1", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnable1
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

		[BindGUI("TurnOnType1", Category = "GfxPointLightDynamic_D100 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _TurnOnType1
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

		[ByamlMember("spl__gfx__LocatorPointLightBancParam")]
		public Mu_spl__gfx__LocatorPointLightBancParam spl__gfx__LocatorPointLightBancParam { get; set; }

		[ByamlMember("spl__gfx__LocatorSpotLightBancParam")]
		public Mu_spl__gfx__LocatorSpotLightBancParam spl__gfx__LocatorSpotLightBancParam { get; set; }

		public GfxPointLightDynamic_D100() : base()
		{
			spl__gfx__LocatorPointLightBancParam = new Mu_spl__gfx__LocatorPointLightBancParam();
			spl__gfx__LocatorSpotLightBancParam = new Mu_spl__gfx__LocatorSpotLightBancParam();

			Links = new List<Link>();
		}

		public GfxPointLightDynamic_D100(GfxPointLightDynamic_D100 other) : base(other)
		{
			spl__gfx__LocatorPointLightBancParam = other.spl__gfx__LocatorPointLightBancParam.Clone();
			spl__gfx__LocatorSpotLightBancParam = other.spl__gfx__LocatorSpotLightBancParam.Clone();
		}

		public override GfxPointLightDynamic_D100 Clone()
		{
			return new GfxPointLightDynamic_D100(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorPointLightBancParam.SaveParameterBank(SerializedActor);
			this.spl__gfx__LocatorSpotLightBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
