using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyTakolienFixedVehicle : MuObj
	{
		[BindGUI("ToRailPoint", Category = "EnemyTakolienFixedVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsCancel", Category = "EnemyTakolienFixedVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsCancel
		{
			get
			{
				return this.spl__RailPaintHelperBancParam.IsCancel;
			}

			set
			{
				this.spl__RailPaintHelperBancParam.IsCancel = value;
			}
		}

		[BindGUI("PaintFrame", Category = "EnemyTakolienFixedVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _PaintFrame
		{
			get
			{
				return this.spl__RailPaintHelperBancParam.PaintFrame;
			}

			set
			{
				this.spl__RailPaintHelperBancParam.PaintFrame = value;
			}
		}

		[BindGUI("Width", Category = "EnemyTakolienFixedVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Width
		{
			get
			{
				return this.spl__RailPaintHelperBancParam.Width;
			}

			set
			{
				this.spl__RailPaintHelperBancParam.Width = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "EnemyTakolienFixedVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint1", Category = "EnemyTakolienFixedVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__AttentionTargetingBancParam")]
		public Mu_spl__AttentionTargetingBancParam spl__AttentionTargetingBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		[ByamlMember("spl__RailPaintHelperBancParam")]
		public Mu_spl__RailPaintHelperBancParam spl__RailPaintHelperBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public EnemyTakolienFixedVehicle() : base()
		{
			spl__AttentionTargetingBancParam = new Mu_spl__AttentionTargetingBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();
			spl__RailPaintHelperBancParam = new Mu_spl__RailPaintHelperBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public EnemyTakolienFixedVehicle(EnemyTakolienFixedVehicle other) : base(other)
		{
			spl__AttentionTargetingBancParam = other.spl__AttentionTargetingBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
			spl__RailPaintHelperBancParam = other.spl__RailPaintHelperBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override EnemyTakolienFixedVehicle Clone()
		{
			return new EnemyTakolienFixedVehicle(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AttentionTargetingBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailPaintHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
