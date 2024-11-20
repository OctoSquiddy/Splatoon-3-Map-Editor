using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class PropellerOnline2PopOut : MuObj
	{
		[BindGUI("MoveAcc", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveAcc
		{
			get
			{
				return this.spl__PropellerOnlineBancParam.MoveAcc;
			}

			set
			{
				this.spl__PropellerOnlineBancParam.MoveAcc = value;
			}
		}

		[BindGUI("MoveReturnAcc", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveReturnAcc
		{
			get
			{
				return this.spl__PropellerOnlineBancParam.MoveReturnAcc;
			}

			set
			{
				this.spl__PropellerOnlineBancParam.MoveReturnAcc = value;
			}
		}

		[BindGUI("MoveReturnSpeedMax", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveReturnSpeedMax
		{
			get
			{
				return this.spl__PropellerOnlineBancParam.MoveReturnSpeedMax;
			}

			set
			{
				this.spl__PropellerOnlineBancParam.MoveReturnSpeedMax = value;
			}
		}

		[BindGUI("MoveSpeedMax", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveSpeedMax
		{
			get
			{
				return this.spl__PropellerOnlineBancParam.MoveSpeedMax;
			}

			set
			{
				this.spl__PropellerOnlineBancParam.MoveSpeedMax = value;
			}
		}

		[BindGUI("MoveVelChargeFull", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveVelChargeFull
		{
			get
			{
				return this.spl__PropellerOnlineBancParam.MoveVelChargeFull;
			}

			set
			{
				this.spl__PropellerOnlineBancParam.MoveVelChargeFull = value;
			}
		}

		[BindGUI("WaitFrameEndRail", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _WaitFrameEndRail
		{
			get
			{
				return this.spl__PropellerOnlineBancParam.WaitFrameEndRail;
			}

			set
			{
				this.spl__PropellerOnlineBancParam.WaitFrameEndRail = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint1", Category = "PropellerOnline2PopOut Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__PropellerOnlineBancParam")]
		public Mu_spl__PropellerOnlineBancParam spl__PropellerOnlineBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public PropellerOnline2PopOut() : base()
		{
			spl__PropellerOnlineBancParam = new Mu_spl__PropellerOnlineBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public PropellerOnline2PopOut(PropellerOnline2PopOut other) : base(other)
		{
			spl__PropellerOnlineBancParam = other.spl__PropellerOnlineBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override PropellerOnline2PopOut Clone()
		{
			return new PropellerOnline2PopOut(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PropellerOnlineBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
