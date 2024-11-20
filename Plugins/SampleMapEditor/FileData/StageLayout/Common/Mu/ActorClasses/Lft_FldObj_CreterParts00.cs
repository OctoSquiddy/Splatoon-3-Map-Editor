using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Lft_FldObj_CreterParts00 : MuObj
	{
		[BindGUI("AttCalcType", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("InterpolationType", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveSpeed", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveTime", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PatrolType", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpeedCalcType", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("FreeRotateVelDegPerSec", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeBreakTime", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeReverseBreakTime", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeRotateAngleDeg", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeRotateTime", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("OneTimeRotateType", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime1", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEventOnly", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEventOnly
		{
			get
			{
				return this.spl__EventActorStateBancParam.IsEventOnly;
			}

			set
			{
				this.spl__EventActorStateBancParam.IsEventOnly = value;
			}
		}

		[BindGUI("IsNotReplaceMapPart", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsNotReplaceMapPart
		{
			get
			{
				return this.spl__LiftBancParam.IsNotReplaceMapPart;
			}

			set
			{
				this.spl__LiftBancParam.IsNotReplaceMapPart = value;
			}
		}

		[BindGUI("IsIncludeVArea", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsIncludeVArea
		{
			get
			{
				return this.spl__PaintBancParam.IsIncludeVArea;
			}

			set
			{
				this.spl__PaintBancParam.IsIncludeVArea = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "Lft_FldObj_CreterParts00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__LiftBancParam")]
		public Mu_spl__LiftBancParam spl__LiftBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public Lft_FldObj_CreterParts00() : base()
		{
			game__RailMovableSequentialParam = new Mu_game__RailMovableSequentialParam();
			game__SequentialRotateParam = new Mu_game__SequentialRotateParam();
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__LiftBancParam = new Mu_spl__LiftBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public Lft_FldObj_CreterParts00(Lft_FldObj_CreterParts00 other) : base(other)
		{
			game__RailMovableSequentialParam = other.game__RailMovableSequentialParam.Clone();
			game__SequentialRotateParam = other.game__SequentialRotateParam.Clone();
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__LiftBancParam = other.spl__LiftBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override Lft_FldObj_CreterParts00 Clone()
		{
			return new Lft_FldObj_CreterParts00(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__RailMovableSequentialParam.SaveParameterBank(SerializedActor);
			this.game__SequentialRotateParam.SaveParameterBank(SerializedActor);
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__LiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
