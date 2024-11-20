using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class IntervalSpawnerForEnemyFlyingHohei : MuObj
	{
		[BindGUI("AttCalcType", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _AttCalcType
		{
			get
			{
				return this.game__RailMovableSequentialParam.AttCalcType;
			}

			set
			{
				this.game__RailMovableSequentialParam.AttCalcType = value;
			}
		}

		[BindGUI("InterpolationType", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _InterpolationType
		{
			get
			{
				return this.game__RailMovableSequentialParam.InterpolationType;
			}

			set
			{
				this.game__RailMovableSequentialParam.InterpolationType = value;
			}
		}

		[BindGUI("MoveSpeed", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MoveSpeed
		{
			get
			{
				return this.game__RailMovableSequentialParam.MoveSpeed;
			}

			set
			{
				this.game__RailMovableSequentialParam.MoveSpeed = value;
			}
		}

		[BindGUI("MoveTime", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveTime
		{
			get
			{
				return this.game__RailMovableSequentialParam.MoveTime;
			}

			set
			{
				this.game__RailMovableSequentialParam.MoveTime = value;
			}
		}

		[BindGUI("PatrolType", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _PatrolType
		{
			get
			{
				return this.game__RailMovableSequentialParam.PatrolType;
			}

			set
			{
				this.game__RailMovableSequentialParam.PatrolType = value;
			}
		}

		[BindGUI("SpeedCalcType", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SpeedCalcType
		{
			get
			{
				return this.game__RailMovableSequentialParam.SpeedCalcType;
			}

			set
			{
				this.game__RailMovableSequentialParam.SpeedCalcType = value;
			}
		}

		[BindGUI("WaitTime", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _WaitTime
		{
			get
			{
				return this.game__RailMovableSequentialParam.WaitTime;
			}

			set
			{
				this.game__RailMovableSequentialParam.WaitTime = value;
			}
		}

		[BindGUI("FreeRotateVelDegPerSec", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _FreeRotateVelDegPerSec
		{
			get
			{
				return new OpenTK.Vector3(
					this.game__SequentialRotateParam.FreeRotateVelDegPerSec.X,
					this.game__SequentialRotateParam.FreeRotateVelDegPerSec.Y,
					this.game__SequentialRotateParam.FreeRotateVelDegPerSec.Z);
			}

			set
			{
				this.game__SequentialRotateParam.FreeRotateVelDegPerSec = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[BindGUI("OneTimeBreakTime", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OneTimeBreakTime
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeBreakTime;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeBreakTime = value;
			}
		}

		[BindGUI("OneTimeReverseBreakTime", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OneTimeReverseBreakTime
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeReverseBreakTime;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeReverseBreakTime = value;
			}
		}

		[BindGUI("OneTimeRotateAngleDeg", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _OneTimeRotateAngleDeg
		{
			get
			{
				return new OpenTK.Vector3(
					this.game__SequentialRotateParam.OneTimeRotateAngleDeg.X,
					this.game__SequentialRotateParam.OneTimeRotateAngleDeg.Y,
					this.game__SequentialRotateParam.OneTimeRotateAngleDeg.Z);
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeRotateAngleDeg = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[BindGUI("OneTimeRotateTime", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OneTimeRotateTime
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeRotateTime;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeRotateTime = value;
			}
		}

		[BindGUI("OneTimeRotateType", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _OneTimeRotateType
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeRotateType;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeRotateType = value;
			}
		}

		[BindGUI("WaitTime1", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _WaitTime1
		{
			get
			{
				return this.game__SequentialRotateParam.WaitTime;
			}

			set
			{
				this.game__SequentialRotateParam.WaitTime = value;
			}
		}

		[BindGUI("DelaySec", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DropIkuraEnemyLimitNum", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IntervalSec", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsAlwaysOpen", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("LinkToRailForMove", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ReserveNum", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "IntervalSpawnerForEnemyFlyingHohei Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("game__RailMovableSequentialParam")]
		public Mu_game__RailMovableSequentialParam game__RailMovableSequentialParam { get; set; }

		[ByamlMember("game__SequentialRotateParam")]
		public Mu_game__SequentialRotateParam game__SequentialRotateParam { get; set; }

		[ByamlMember("spl__IntervalSpawnerBancParam")]
		public Mu_spl__IntervalSpawnerBancParam spl__IntervalSpawnerBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public IntervalSpawnerForEnemyFlyingHohei() : base()
		{
			game__RailMovableSequentialParam = new Mu_game__RailMovableSequentialParam();
			game__SequentialRotateParam = new Mu_game__SequentialRotateParam();
			spl__IntervalSpawnerBancParam = new Mu_spl__IntervalSpawnerBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public IntervalSpawnerForEnemyFlyingHohei(IntervalSpawnerForEnemyFlyingHohei other) : base(other)
		{
			game__RailMovableSequentialParam = other.game__RailMovableSequentialParam.Clone();
			game__SequentialRotateParam = other.game__SequentialRotateParam.Clone();
			spl__IntervalSpawnerBancParam = other.spl__IntervalSpawnerBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override IntervalSpawnerForEnemyFlyingHohei Clone()
		{
			return new IntervalSpawnerForEnemyFlyingHohei(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__RailMovableSequentialParam.SaveParameterBank(SerializedActor);
			this.game__SequentialRotateParam.SaveParameterBank(SerializedActor);
			this.spl__IntervalSpawnerBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
