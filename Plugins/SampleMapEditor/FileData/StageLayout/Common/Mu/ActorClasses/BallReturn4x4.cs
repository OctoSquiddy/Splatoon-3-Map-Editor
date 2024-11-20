using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class BallReturn4x4 : MuObj
	{
		[BindGUI("AttCalcType", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("InterpolationType", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveSpeed", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveTime", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PatrolType", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpeedCalcType", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("FreeRotateVelDegPerSec", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeBreakTime", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeReverseBreakTime", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeRotateAngleDeg", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeRotateTime", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeRotateType", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime1", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("CycleSec", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _CycleSec
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.CycleSec;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.CycleSec = value;
			}
		}

		[BindGUI("IsTotem", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsTotem
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.IsTotem;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.IsTotem = value;
			}
		}

		[BindGUI("LinkToRail", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRail
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.LinkToRail;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.LinkToRail = value;
			}
		}

		[BindGUI("NormalTimingTable0", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _NormalTimingTable0
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.NormalTimingTable0;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.NormalTimingTable0 = value;
			}
		}

		[BindGUI("NormalTimingTable1", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _NormalTimingTable1
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.NormalTimingTable1;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.NormalTimingTable1 = value;
			}
		}

		[BindGUI("NormalTimingTable2", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _NormalTimingTable2
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.NormalTimingTable2;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.NormalTimingTable2 = value;
			}
		}

		[BindGUI("PauseSecAfterKilled", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _PauseSecAfterKilled
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.PauseSecAfterKilled;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.PauseSecAfterKilled = value;
			}
		}

		[BindGUI("ReserveNum", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ReserveNum
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.ReserveNum;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.ReserveNum = value;
			}
		}

		[BindGUI("SpeedRate", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _SpeedRate
		{
			get
			{
				return this.spl__IntervalSpawnerForEnemyRockBancParam.SpeedRate;
			}

			set
			{
				this.spl__IntervalSpawnerForEnemyRockBancParam.SpeedRate = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "BallReturn4x4 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__IntervalSpawnerForEnemyRockBancParam")]
		public Mu_spl__IntervalSpawnerForEnemyRockBancParam spl__IntervalSpawnerForEnemyRockBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public BallReturn4x4() : base()
		{
			game__RailMovableSequentialParam = new Mu_game__RailMovableSequentialParam();
			game__SequentialRotateParam = new Mu_game__SequentialRotateParam();
			spl__IntervalSpawnerForEnemyRockBancParam = new Mu_spl__IntervalSpawnerForEnemyRockBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public BallReturn4x4(BallReturn4x4 other) : base(other)
		{
			game__RailMovableSequentialParam = other.game__RailMovableSequentialParam.Clone();
			game__SequentialRotateParam = other.game__SequentialRotateParam.Clone();
			spl__IntervalSpawnerForEnemyRockBancParam = other.spl__IntervalSpawnerForEnemyRockBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override BallReturn4x4 Clone()
		{
			return new BallReturn4x4(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__RailMovableSequentialParam.SaveParameterBank(SerializedActor);
			this.game__SequentialRotateParam.SaveParameterBank(SerializedActor);
			this.spl__IntervalSpawnerForEnemyRockBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
