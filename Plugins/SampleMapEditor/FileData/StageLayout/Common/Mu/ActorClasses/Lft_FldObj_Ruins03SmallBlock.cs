using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Lft_FldObj_Ruins03SmallBlock : MuObj
	{
		[BindGUI("IsEventOnly", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsNotReplaceMapPart", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Accel", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Accel
		{
			get
			{
				return this.spl__ailift__AccDecBancParam.Accel;
			}

			set
			{
				this.spl__ailift__AccDecBancParam.Accel = value;
			}
		}

		[BindGUI("Decel", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Decel
		{
			get
			{
				return this.spl__ailift__AccDecBancParam.Decel;
			}

			set
			{
				this.spl__ailift__AccDecBancParam.Decel = value;
			}
		}

		[BindGUI("MaxSpeed", Category = "Lft_FldObj_Ruins03SmallBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxSpeed
		{
			get
			{
				return this.spl__ailift__AccDecBancParam.MaxSpeed;
			}

			set
			{
				this.spl__ailift__AccDecBancParam.MaxSpeed = value;
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

		[ByamlMember("spl__ailift__AccDecBancParam")]
		public Mu_spl__ailift__AccDecBancParam spl__ailift__AccDecBancParam { get; set; }

		public Lft_FldObj_Ruins03SmallBlock() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__LiftBancParam = new Mu_spl__LiftBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();
			spl__ailift__AccDecBancParam = new Mu_spl__ailift__AccDecBancParam();

			Links = new List<Link>();
		}

		public Lft_FldObj_Ruins03SmallBlock(Lft_FldObj_Ruins03SmallBlock other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__LiftBancParam = other.spl__LiftBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
			spl__ailift__AccDecBancParam = other.spl__ailift__AccDecBancParam.Clone();
		}

		public override Lft_FldObj_Ruins03SmallBlock Clone()
		{
			return new Lft_FldObj_Ruins03SmallBlock(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__LiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AccDecBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
