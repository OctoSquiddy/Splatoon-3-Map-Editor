using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class PropellerUpSdodr : MuObj
	{
		[BindGUI("ImpulseOnDamage", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _ImpulseOnDamage
		{
			get
			{
				return this.spl__PropellerBancParam.ImpulseOnDamage;
			}

			set
			{
				this.spl__PropellerBancParam.ImpulseOnDamage = value;
			}
		}

		[BindGUI("MoveAcc", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveAcc
		{
			get
			{
				return this.spl__PropellerBancParam.MoveAcc;
			}

			set
			{
				this.spl__PropellerBancParam.MoveAcc = value;
			}
		}

		[BindGUI("MoveReturnAcc", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveReturnAcc
		{
			get
			{
				return this.spl__PropellerBancParam.MoveReturnAcc;
			}

			set
			{
				this.spl__PropellerBancParam.MoveReturnAcc = value;
			}
		}

		[BindGUI("MoveReturnSpeedMax", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveReturnSpeedMax
		{
			get
			{
				return this.spl__PropellerBancParam.MoveReturnSpeedMax;
			}

			set
			{
				this.spl__PropellerBancParam.MoveReturnSpeedMax = value;
			}
		}

		[BindGUI("MoveSpeedMax", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveSpeedMax
		{
			get
			{
				return this.spl__PropellerBancParam.MoveSpeedMax;
			}

			set
			{
				this.spl__PropellerBancParam.MoveSpeedMax = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__RailFollowBancParam.ToRailPoint;
			}

			set
			{
				this.spl__RailFollowBancParam.ToRailPoint = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint1", Category = "PropellerUpSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint1
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

		[ByamlMember("spl__PropellerBancParam")]
		public Mu_spl__PropellerBancParam spl__PropellerBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public PropellerUpSdodr() : base()
		{
			spl__PropellerBancParam = new Mu_spl__PropellerBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public PropellerUpSdodr(PropellerUpSdodr other) : base(other)
		{
			spl__PropellerBancParam = other.spl__PropellerBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override PropellerUpSdodr Clone()
		{
			return new PropellerUpSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PropellerBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
