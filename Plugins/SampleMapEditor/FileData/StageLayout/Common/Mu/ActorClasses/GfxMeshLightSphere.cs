using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GfxMeshLightSphere : MuObj
	{
		[BindGUI("Color", Category = "GfxMeshLightSphere Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public System.Numerics.Vector4 _Color
		{
			get
			{
				return new System.Numerics.Vector4(
					this.game__gfx__LocatorPrimitiveLightBancParam.Color.R,
					this.game__gfx__LocatorPrimitiveLightBancParam.Color.G,
					this.game__gfx__LocatorPrimitiveLightBancParam.Color.B,
					this.game__gfx__LocatorPrimitiveLightBancParam.Color.A);
			}

			set
			{
				this.game__gfx__LocatorPrimitiveLightBancParam.Color.R = (float)value.X;
				this.game__gfx__LocatorPrimitiveLightBancParam.Color.G = (float)value.Y;
				this.game__gfx__LocatorPrimitiveLightBancParam.Color.B = (float)value.Z;
				this.game__gfx__LocatorPrimitiveLightBancParam.Color.A = (float)value.W;
			}
		}

		[BindGUI("Intensity", Category = "GfxMeshLightSphere Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Intensity
		{
			get
			{
				return this.game__gfx__LocatorPrimitiveLightBancParam.Intensity;
			}

			set
			{
				this.game__gfx__LocatorPrimitiveLightBancParam.Intensity = value;
			}
		}

		[ByamlMember("game__gfx__LocatorPrimitiveLightBancParam")]
		public Mu_game__gfx__LocatorPrimitiveLightBancParam game__gfx__LocatorPrimitiveLightBancParam { get; set; }

		public GfxMeshLightSphere() : base()
		{
			game__gfx__LocatorPrimitiveLightBancParam = new Mu_game__gfx__LocatorPrimitiveLightBancParam();

			Links = new List<Link>();
		}

		public GfxMeshLightSphere(GfxMeshLightSphere other) : base(other)
		{
			game__gfx__LocatorPrimitiveLightBancParam = other.game__gfx__LocatorPrimitiveLightBancParam.Clone();
		}

		public override GfxMeshLightSphere Clone()
		{
			return new GfxMeshLightSphere(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__gfx__LocatorPrimitiveLightBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
