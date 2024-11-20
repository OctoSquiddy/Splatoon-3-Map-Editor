using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorTricolMatoiLandingPoint : MuObj
	{
		[BindGUI("Height", Category = "LocatorTricolMatoiLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Height
		{
			get
			{
				return this.spl__LocatorTricolMatoiLandingPointBancParam.Height;
			}

			set
			{
				this.spl__LocatorTricolMatoiLandingPointBancParam.Height = value;
			}
		}

		[BindGUI("TargetPaintRadius", Category = "LocatorTricolMatoiLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _TargetPaintRadius
		{
			get
			{
				return this.spl__LocatorTricolMatoiLandingPointBancParam.TargetPaintRadius;
			}

			set
			{
				this.spl__LocatorTricolMatoiLandingPointBancParam.TargetPaintRadius = value;
			}
		}

		[ByamlMember("spl__LocatorTricolMatoiLandingPointBancParam")]
		public Mu_spl__LocatorTricolMatoiLandingPointBancParam spl__LocatorTricolMatoiLandingPointBancParam { get; set; }

		public LocatorTricolMatoiLandingPoint() : base()
		{
			spl__LocatorTricolMatoiLandingPointBancParam = new Mu_spl__LocatorTricolMatoiLandingPointBancParam();

			Links = new List<Link>();
		}

		public LocatorTricolMatoiLandingPoint(LocatorTricolMatoiLandingPoint other) : base(other)
		{
			spl__LocatorTricolMatoiLandingPointBancParam = other.spl__LocatorTricolMatoiLandingPointBancParam.Clone();
		}

		public override LocatorTricolMatoiLandingPoint Clone()
		{
			return new LocatorTricolMatoiLandingPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorTricolMatoiLandingPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
