using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Lft_AbstractDrawer : MuObj
	{
		[BindGUI("IsEventOnly", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsNotReplaceMapPart", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Accel", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Accel
		{
			get
			{
				return this.spl__ailift__AccDec2BancParam.Accel;
			}

			set
			{
				this.spl__ailift__AccDec2BancParam.Accel = value;
			}
		}

		[BindGUI("Decel", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Decel
		{
			get
			{
				return this.spl__ailift__AccDec2BancParam.Decel;
			}

			set
			{
				this.spl__ailift__AccDec2BancParam.Decel = value;
			}
		}

		[BindGUI("MaxSpeed", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxSpeed
		{
			get
			{
				return this.spl__ailift__AccDec2BancParam.MaxSpeed;
			}

			set
			{
				this.spl__ailift__AccDec2BancParam.MaxSpeed = value;
			}
		}

		[BindGUI("ReverseMaxSpeed", Category = "Lft_AbstractDrawer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _ReverseMaxSpeed
		{
			get
			{
				return this.spl__ailift__AccDec2BancParam.ReverseMaxSpeed;
			}

			set
			{
				this.spl__ailift__AccDec2BancParam.ReverseMaxSpeed = value;
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

		[ByamlMember("spl__ailift__AccDec2BancParam")]
		public Mu_spl__ailift__AccDec2BancParam spl__ailift__AccDec2BancParam { get; set; }

		public Lft_AbstractDrawer() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__LiftBancParam = new Mu_spl__LiftBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();
			spl__ailift__AccDec2BancParam = new Mu_spl__ailift__AccDec2BancParam();

			Links = new List<Link>();
		}

		public Lft_AbstractDrawer(Lft_AbstractDrawer other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__LiftBancParam = other.spl__LiftBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
			spl__ailift__AccDec2BancParam = other.spl__ailift__AccDec2BancParam.Clone();
		}

		public override Lft_AbstractDrawer Clone()
		{
			return new Lft_AbstractDrawer(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__LiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AccDec2BancParam.SaveParameterBank(SerializedActor);
		}
	}
}
