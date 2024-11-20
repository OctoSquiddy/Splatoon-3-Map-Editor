using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SighterTarget : MuObj
	{
		[BindGUI("AttCalcType", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("InterpolationType", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveSpeed", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveTime", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PatrolType", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpeedCalcType", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "SighterTarget Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__RailMovableSequentialHelperBancParam")]
		public Mu_spl__RailMovableSequentialHelperBancParam spl__RailMovableSequentialHelperBancParam { get; set; }

		public SighterTarget() : base()
		{
			game__RailMovableSequentialParam = new Mu_game__RailMovableSequentialParam();
			spl__RailMovableSequentialHelperBancParam = new Mu_spl__RailMovableSequentialHelperBancParam();

			Links = new List<Link>();
		}

		public SighterTarget(SighterTarget other) : base(other)
		{
			game__RailMovableSequentialParam = other.game__RailMovableSequentialParam.Clone();
			spl__RailMovableSequentialHelperBancParam = other.spl__RailMovableSequentialHelperBancParam.Clone();
		}

		public override SighterTarget Clone()
		{
			return new SighterTarget(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__RailMovableSequentialParam.SaveParameterBank(SerializedActor);
			this.spl__RailMovableSequentialHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
