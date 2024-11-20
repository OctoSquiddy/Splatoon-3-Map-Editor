using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class IntervalSpawnerForMovePainter : MuObj
	{
		[BindGUI("DelaySec", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DelaySec
		{
			get
			{
				return this.spl__IntervalSpawnerBancParam.DelaySec;
			}

			set
			{
				this.spl__IntervalSpawnerBancParam.DelaySec = value;
			}
		}

		[BindGUI("DropIkuraEnemyLimitNum", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _DropIkuraEnemyLimitNum
		{
			get
			{
				return this.spl__IntervalSpawnerBancParam.DropIkuraEnemyLimitNum;
			}

			set
			{
				this.spl__IntervalSpawnerBancParam.DropIkuraEnemyLimitNum = value;
			}
		}

		[BindGUI("IntervalSec", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _IntervalSec
		{
			get
			{
				return this.spl__IntervalSpawnerBancParam.IntervalSec;
			}

			set
			{
				this.spl__IntervalSpawnerBancParam.IntervalSec = value;
			}
		}

		[BindGUI("IsAlwaysOpen", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsAlwaysOpen
		{
			get
			{
				return this.spl__IntervalSpawnerBancParam.IsAlwaysOpen;
			}

			set
			{
				this.spl__IntervalSpawnerBancParam.IsAlwaysOpen = value;
			}
		}

		[BindGUI("LinkToRailForMove", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRailForMove
		{
			get
			{
				return this.spl__IntervalSpawnerBancParam.LinkToRailForMove;
			}

			set
			{
				this.spl__IntervalSpawnerBancParam.LinkToRailForMove = value;
			}
		}

		[BindGUI("ReserveNum", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ReserveNum
		{
			get
			{
				return this.spl__IntervalSpawnerBancParam.ReserveNum;
			}

			set
			{
				this.spl__IntervalSpawnerBancParam.ReserveNum = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "IntervalSpawnerForMovePainter Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__IntervalSpawnerBancParam")]
		public Mu_spl__IntervalSpawnerBancParam spl__IntervalSpawnerBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public IntervalSpawnerForMovePainter() : base()
		{
			spl__IntervalSpawnerBancParam = new Mu_spl__IntervalSpawnerBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public IntervalSpawnerForMovePainter(IntervalSpawnerForMovePainter other) : base(other)
		{
			spl__IntervalSpawnerBancParam = other.spl__IntervalSpawnerBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override IntervalSpawnerForMovePainter Clone()
		{
			return new IntervalSpawnerForMovePainter(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__IntervalSpawnerBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
