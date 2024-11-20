using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class BlackLight : MuObj
	{
		[BindGUI("AngleDamp", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DiffuseColor", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DistDamp", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Intensity", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnable", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DampParam", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DebugDisplay", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DiffuseColor1", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public System.Numerics.Vector4 _DiffuseColor1
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

		[BindGUI("Intensity1", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Intensity1
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

		[BindGUI("IsEnable1", Category = "BlackLight Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnable1
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

		[ByamlMember("spl__gfx__LocatorSpotLightBancParam")]
		public Mu_spl__gfx__LocatorSpotLightBancParam spl__gfx__LocatorSpotLightBancParam { get; set; }

		[ByamlMember("spl__gfx__LocatorPointLightBancParam")]
		public Mu_spl__gfx__LocatorPointLightBancParam spl__gfx__LocatorPointLightBancParam { get; set; }

		public BlackLight() : base()
		{
			spl__gfx__LocatorSpotLightBancParam = new Mu_spl__gfx__LocatorSpotLightBancParam();
			spl__gfx__LocatorPointLightBancParam = new Mu_spl__gfx__LocatorPointLightBancParam();

			Links = new List<Link>();
		}

		public BlackLight(BlackLight other) : base(other)
		{
			spl__gfx__LocatorSpotLightBancParam = other.spl__gfx__LocatorSpotLightBancParam.Clone();
			spl__gfx__LocatorPointLightBancParam = other.spl__gfx__LocatorPointLightBancParam.Clone();
		}

		public override BlackLight Clone()
		{
			return new BlackLight(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorSpotLightBancParam.SaveParameterBank(SerializedActor);
			this.spl__gfx__LocatorPointLightBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
