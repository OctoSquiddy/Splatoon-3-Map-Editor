using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NavigateAirBall_Tutorial : MuObj
	{
		[BindGUI("AttCalcType", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("InterpolationType", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveSpeed", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveTime", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PatrolType", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpeedCalcType", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsFreeY", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsFreeY
		{
			get
			{
				return this.spl__ActorMatrixBindableHelperBancParam.IsFreeY;
			}

			set
			{
				this.spl__ActorMatrixBindableHelperBancParam.IsFreeY = value;
			}
		}

		[BindGUI("IkuraNum", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IkuraNum
		{
			get
			{
				return this.spl__AirBallBancParam.IkuraNum;
			}

			set
			{
				this.spl__AirBallBancParam.IkuraNum = value;
			}
		}

		[BindGUI("IkuraValue", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IkuraValue
		{
			get
			{
				return this.spl__AirBallBancParam.IkuraValue;
			}

			set
			{
				this.spl__AirBallBancParam.IkuraValue = value;
			}
		}

		[BindGUI("TimeLimit", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _TimeLimit
		{
			get
			{
				return this.spl__AirBallBancParam.TimeLimit;
			}

			set
			{
				this.spl__AirBallBancParam.TimeLimit = value;
			}
		}

		[BindGUI("HideMode", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _HideMode
		{
			get
			{
				return this.spl__InkHideHelperBancParam.HideMode;
			}

			set
			{
				this.spl__InkHideHelperBancParam.HideMode = value;
			}
		}

		[BindGUI("DisappearFrame", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _DisappearFrame
		{
			get
			{
				return this.spl__NavigateAirBallBancParam.DisappearFrame;
			}

			set
			{
				this.spl__NavigateAirBallBancParam.DisappearFrame = value;
			}
		}

		[BindGUI("IsFirst", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsFirst
		{
			get
			{
				return this.spl__NavigateAirBallBancParam.IsFirst;
			}

			set
			{
				this.spl__NavigateAirBallBancParam.IsFirst = value;
			}
		}

		[BindGUI("MoveFrame", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MoveFrame
		{
			get
			{
				return this.spl__NavigateAirBallBancParam.MoveFrame;
			}

			set
			{
				this.spl__NavigateAirBallBancParam.MoveFrame = value;
			}
		}

		[BindGUI("WaitFrame", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _WaitFrame
		{
			get
			{
				return this.spl__NavigateAirBallBancParam.WaitFrame;
			}

			set
			{
				this.spl__NavigateAirBallBancParam.WaitFrame = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "NavigateAirBall_Tutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__RailMovableSequentialHelperBancParam.ToRailPoint;
			}

			set
			{
				this.spl__RailMovableSequentialHelperBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("game__RailMovableSequentialParam")]
		public Mu_game__RailMovableSequentialParam game__RailMovableSequentialParam { get; set; }

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__AirBallBancParam")]
		public Mu_spl__AirBallBancParam spl__AirBallBancParam { get; set; }

		[ByamlMember("spl__InkHideHelperBancParam")]
		public Mu_spl__InkHideHelperBancParam spl__InkHideHelperBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__NavigateAirBallBancParam")]
		public Mu_spl__NavigateAirBallBancParam spl__NavigateAirBallBancParam { get; set; }

		[ByamlMember("spl__RailMovableSequentialHelperBancParam")]
		public Mu_spl__RailMovableSequentialHelperBancParam spl__RailMovableSequentialHelperBancParam { get; set; }

		public NavigateAirBall_Tutorial() : base()
		{
			game__RailMovableSequentialParam = new Mu_game__RailMovableSequentialParam();
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__AirBallBancParam = new Mu_spl__AirBallBancParam();
			spl__InkHideHelperBancParam = new Mu_spl__InkHideHelperBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__NavigateAirBallBancParam = new Mu_spl__NavigateAirBallBancParam();
			spl__RailMovableSequentialHelperBancParam = new Mu_spl__RailMovableSequentialHelperBancParam();

			Links = new List<Link>();
		}

		public NavigateAirBall_Tutorial(NavigateAirBall_Tutorial other) : base(other)
		{
			game__RailMovableSequentialParam = other.game__RailMovableSequentialParam.Clone();
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__AirBallBancParam = other.spl__AirBallBancParam.Clone();
			spl__InkHideHelperBancParam = other.spl__InkHideHelperBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__NavigateAirBallBancParam = other.spl__NavigateAirBallBancParam.Clone();
			spl__RailMovableSequentialHelperBancParam = other.spl__RailMovableSequentialHelperBancParam.Clone();
		}

		public override NavigateAirBall_Tutorial Clone()
		{
			return new NavigateAirBall_Tutorial(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__RailMovableSequentialParam.SaveParameterBank(SerializedActor);
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__AirBallBancParam.SaveParameterBank(SerializedActor);
			this.spl__InkHideHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__NavigateAirBallBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailMovableSequentialHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
