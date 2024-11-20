using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GfxPointLight_Info : MuObj
	{
		[BindGUI("DampParam", Category = "GfxPointLight_Info Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DebugDisplay", Category = "GfxPointLight_Info Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DiffuseColor", Category = "GfxPointLight_Info Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Intensity", Category = "GfxPointLight_Info Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnable", Category = "GfxPointLight_Info Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("TurnOnType", Category = "GfxPointLight_Info Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__gfx__LocatorPointLightBancParam")]
		public Mu_spl__gfx__LocatorPointLightBancParam spl__gfx__LocatorPointLightBancParam { get; set; }

		public GfxPointLight_Info() : base()
		{
			spl__gfx__LocatorPointLightBancParam = new Mu_spl__gfx__LocatorPointLightBancParam();

			Links = new List<Link>();
		}

		public GfxPointLight_Info(GfxPointLight_Info other) : base(other)
		{
			spl__gfx__LocatorPointLightBancParam = other.spl__gfx__LocatorPointLightBancParam.Clone();
		}

		public override GfxPointLight_Info Clone()
		{
			return new GfxPointLight_Info(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorPointLightBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
