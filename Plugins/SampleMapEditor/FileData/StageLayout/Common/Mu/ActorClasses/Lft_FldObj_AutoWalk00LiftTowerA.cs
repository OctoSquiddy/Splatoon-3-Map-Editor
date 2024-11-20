using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Lft_FldObj_AutoWalk00LiftTowerA : MuObj
	{
		[BindGUI("IsEventOnly", Category = "Lft_FldObj_AutoWalk00LiftTowerA Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsNotReplaceMapPart", Category = "Lft_FldObj_AutoWalk00LiftTowerA Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "Lft_FldObj_AutoWalk00LiftTowerA Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Lft_FldObj_AutoWalk00LiftTowerA Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "Lft_FldObj_AutoWalk00LiftTowerA Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__LiftBancParam")]
		public Mu_spl__LiftBancParam spl__LiftBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public Lft_FldObj_AutoWalk00LiftTowerA() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__LiftBancParam = new Mu_spl__LiftBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public Lft_FldObj_AutoWalk00LiftTowerA(Lft_FldObj_AutoWalk00LiftTowerA other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__LiftBancParam = other.spl__LiftBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override Lft_FldObj_AutoWalk00LiftTowerA Clone()
		{
			return new Lft_FldObj_AutoWalk00LiftTowerA(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__LiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
