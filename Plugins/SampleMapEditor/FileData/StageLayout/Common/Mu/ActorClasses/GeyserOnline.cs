using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GeyserOnline : MuObj
	{
		[BindGUI("KeepMaxHeightSec", Category = "GeyserOnline Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _KeepMaxHeightSec
		{
			get
			{
				return this.spl__GeyserOnlineBancParam.KeepMaxHeightSec;
			}

			set
			{
				this.spl__GeyserOnlineBancParam.KeepMaxHeightSec = value;
			}
		}

		[BindGUI("MaxHeight", Category = "GeyserOnline Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxHeight
		{
			get
			{
				return this.spl__GeyserOnlineBancParam.MaxHeight;
			}

			set
			{
				this.spl__GeyserOnlineBancParam.MaxHeight = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "GeyserOnline Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnableMoveChatteringPrevent
		{
			get
			{
				return this.spl__ailift__AILiftBancParam.IsEnableMoveChatteringPrevent;
			}

			set
			{
				this.spl__ailift__AILiftBancParam.IsEnableMoveChatteringPrevent = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "GeyserOnline Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__ailift__AILiftBancParam.ToRailPoint;
			}

			set
			{
				this.spl__ailift__AILiftBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__GeyserOnlineBancParam")]
		public Mu_spl__GeyserOnlineBancParam spl__GeyserOnlineBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public GeyserOnline() : base()
		{
			spl__GeyserOnlineBancParam = new Mu_spl__GeyserOnlineBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public GeyserOnline(GeyserOnline other) : base(other)
		{
			spl__GeyserOnlineBancParam = other.spl__GeyserOnlineBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override GeyserOnline Clone()
		{
			return new GeyserOnline(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__GeyserOnlineBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
