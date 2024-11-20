using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Npc_JerryFest_HeadBand02_DObj_SAND : MuObj
	{
		[BindGUI("AttCalcType", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("InterpolationType", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveSpeed", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MoveTime", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PatrolType", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpeedCalcType", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("WaitTime", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsFreeY", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("AnimName", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _AnimName
		{
			get
			{
				return this.spl__DesignerObjBancParam.AnimName;
			}

			set
			{
				this.spl__DesignerObjBancParam.AnimName = value;
			}
		}

		[BindGUI("IsEventOnly", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsOceanBind", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsOceanBind
		{
			get
			{
				return this.spl__OceanBindableHelperBancParam.IsOceanBind;
			}

			set
			{
				this.spl__OceanBindableHelperBancParam.IsOceanBind = value;
			}
		}

		[BindGUI("Ratio", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Ratio
		{
			get
			{
				return this.spl__OceanBindableHelperBancParam.Ratio;
			}

			set
			{
				this.spl__OceanBindableHelperBancParam.Ratio = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "Npc_JerryFest_HeadBand02_DObj_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__DesignerObjBancParam")]
		public Mu_spl__DesignerObjBancParam spl__DesignerObjBancParam { get; set; }

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__OceanBindableHelperBancParam")]
		public Mu_spl__OceanBindableHelperBancParam spl__OceanBindableHelperBancParam { get; set; }

		[ByamlMember("spl__RailMovableSequentialHelperBancParam")]
		public Mu_spl__RailMovableSequentialHelperBancParam spl__RailMovableSequentialHelperBancParam { get; set; }

		public Npc_JerryFest_HeadBand02_DObj_SAND() : base()
		{
			game__RailMovableSequentialParam = new Mu_game__RailMovableSequentialParam();
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__DesignerObjBancParam = new Mu_spl__DesignerObjBancParam();
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__OceanBindableHelperBancParam = new Mu_spl__OceanBindableHelperBancParam();
			spl__RailMovableSequentialHelperBancParam = new Mu_spl__RailMovableSequentialHelperBancParam();

			Links = new List<Link>();
		}

		public Npc_JerryFest_HeadBand02_DObj_SAND(Npc_JerryFest_HeadBand02_DObj_SAND other) : base(other)
		{
			game__RailMovableSequentialParam = other.game__RailMovableSequentialParam.Clone();
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__DesignerObjBancParam = other.spl__DesignerObjBancParam.Clone();
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__OceanBindableHelperBancParam = other.spl__OceanBindableHelperBancParam.Clone();
			spl__RailMovableSequentialHelperBancParam = other.spl__RailMovableSequentialHelperBancParam.Clone();
		}

		public override Npc_JerryFest_HeadBand02_DObj_SAND Clone()
		{
			return new Npc_JerryFest_HeadBand02_DObj_SAND(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__RailMovableSequentialParam.SaveParameterBank(SerializedActor);
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__DesignerObjBancParam.SaveParameterBank(SerializedActor);
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__OceanBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailMovableSequentialHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
