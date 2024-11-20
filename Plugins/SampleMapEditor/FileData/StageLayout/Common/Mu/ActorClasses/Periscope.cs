using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Periscope : MuObj
	{
		[BindGUI("LinkToRail", Category = "Periscope Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRail
		{
			get
			{
				return this.spl__PeriscopeBancParam.LinkToRail;
			}

			set
			{
				this.spl__PeriscopeBancParam.LinkToRail = value;
			}
		}

		[BindGUI("PitchAngleDegMax", Category = "Periscope Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _PitchAngleDegMax
		{
			get
			{
				return this.spl__PeriscopeBancParam.PitchAngleDegMax;
			}

			set
			{
				this.spl__PeriscopeBancParam.PitchAngleDegMax = value;
			}
		}

		[BindGUI("PitchAngleDegMin", Category = "Periscope Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _PitchAngleDegMin
		{
			get
			{
				return this.spl__PeriscopeBancParam.PitchAngleDegMin;
			}

			set
			{
				this.spl__PeriscopeBancParam.PitchAngleDegMin = value;
			}
		}

		[BindGUI("YawAngleDegMax", Category = "Periscope Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _YawAngleDegMax
		{
			get
			{
				return this.spl__PeriscopeBancParam.YawAngleDegMax;
			}

			set
			{
				this.spl__PeriscopeBancParam.YawAngleDegMax = value;
			}
		}

		[BindGUI("YawAngleDegMin", Category = "Periscope Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _YawAngleDegMin
		{
			get
			{
				return this.spl__PeriscopeBancParam.YawAngleDegMin;
			}

			set
			{
				this.spl__PeriscopeBancParam.YawAngleDegMin = value;
			}
		}

		[ByamlMember("spl__PeriscopeBancParam")]
		public Mu_spl__PeriscopeBancParam spl__PeriscopeBancParam { get; set; }

		public Periscope() : base()
		{
			spl__PeriscopeBancParam = new Mu_spl__PeriscopeBancParam();

			Links = new List<Link>();
		}

		public Periscope(Periscope other) : base(other)
		{
			spl__PeriscopeBancParam = other.spl__PeriscopeBancParam.Clone();
		}

		public override Periscope Clone()
		{
			return new Periscope(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PeriscopeBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
