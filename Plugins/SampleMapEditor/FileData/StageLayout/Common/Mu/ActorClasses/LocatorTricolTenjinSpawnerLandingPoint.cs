using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorTricolTenjinSpawnerLandingPoint : MuObj
	{
		[BindGUI("PitchDegree", Category = "LocatorTricolTenjinSpawnerLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _PitchDegree
		{
			get
			{
				return this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.PitchDegree;
			}

			set
			{
				this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.PitchDegree = value;
			}
		}

		[BindGUI("SpanFrame", Category = "LocatorTricolTenjinSpawnerLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _SpanFrame
		{
			get
			{
				return this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SpanFrame;
			}

			set
			{
				this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SpanFrame = value;
			}
		}

		[BindGUI("SpeedBase", Category = "LocatorTricolTenjinSpawnerLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _SpeedBase
		{
			get
			{
				return this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SpeedBase;
			}

			set
			{
				this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SpeedBase = value;
			}
		}

		[BindGUI("SpeedRandom", Category = "LocatorTricolTenjinSpawnerLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _SpeedRandom
		{
			get
			{
				return this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SpeedRandom;
			}

			set
			{
				this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SpeedRandom = value;
			}
		}

		[BindGUI("WaitFrame", Category = "LocatorTricolTenjinSpawnerLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _WaitFrame
		{
			get
			{
				return this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.WaitFrame;
			}

			set
			{
				this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.WaitFrame = value;
			}
		}

		[BindGUI("YawDegreeRandom", Category = "LocatorTricolTenjinSpawnerLandingPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _YawDegreeRandom
		{
			get
			{
				return this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.YawDegreeRandom;
			}

			set
			{
				this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.YawDegreeRandom = value;
			}
		}

		[ByamlMember("spl__LocatorTricolTenjinSpawnerLandingPointBancParam")]
		public Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam spl__LocatorTricolTenjinSpawnerLandingPointBancParam { get; set; }

		public LocatorTricolTenjinSpawnerLandingPoint() : base()
		{
			spl__LocatorTricolTenjinSpawnerLandingPointBancParam = new Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam();

			Links = new List<Link>();
		}

		public LocatorTricolTenjinSpawnerLandingPoint(LocatorTricolTenjinSpawnerLandingPoint other) : base(other)
		{
			spl__LocatorTricolTenjinSpawnerLandingPointBancParam = other.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.Clone();
		}

		public override LocatorTricolTenjinSpawnerLandingPoint Clone()
		{
			return new LocatorTricolTenjinSpawnerLandingPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorTricolTenjinSpawnerLandingPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
