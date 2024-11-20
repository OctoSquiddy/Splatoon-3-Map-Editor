using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DemoLightC : MuObj
	{
		[BindGUI("DampParam", Category = "DemoLightC Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DebugDisplay", Category = "DemoLightC Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DiffuseColor", Category = "DemoLightC Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Intensity", Category = "DemoLightC Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnable", Category = "DemoLightC Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__gfx__LocatorPointLightBancParam")]
		public Mu_spl__gfx__LocatorPointLightBancParam spl__gfx__LocatorPointLightBancParam { get; set; }

		public DemoLightC() : base()
		{
			spl__gfx__LocatorPointLightBancParam = new Mu_spl__gfx__LocatorPointLightBancParam();

			Links = new List<Link>();
		}

		public DemoLightC(DemoLightC other) : base(other)
		{
			spl__gfx__LocatorPointLightBancParam = other.spl__gfx__LocatorPointLightBancParam.Clone();
		}

		public override DemoLightC Clone()
		{
			return new DemoLightC(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorPointLightBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
