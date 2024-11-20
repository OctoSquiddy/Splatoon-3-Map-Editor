using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyTakolienBounceShieldVehicle : MuObj
	{
		[BindGUI("ToRailPoint", Category = "EnemyTakolienBounceShieldVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsCancel", Category = "EnemyTakolienBounceShieldVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PaintFrame", Category = "EnemyTakolienBounceShieldVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Width", Category = "EnemyTakolienBounceShieldVehicle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__AttentionTargetingBancParam")]
		public Mu_spl__AttentionTargetingBancParam spl__AttentionTargetingBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		[ByamlMember("spl__RailPaintHelperBancParam")]
		public Mu_spl__RailPaintHelperBancParam spl__RailPaintHelperBancParam { get; set; }

		public EnemyTakolienBounceShieldVehicle() : base()
		{
			spl__AttentionTargetingBancParam = new Mu_spl__AttentionTargetingBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();
			spl__RailPaintHelperBancParam = new Mu_spl__RailPaintHelperBancParam();

			Links = new List<Link>();
		}

		public EnemyTakolienBounceShieldVehicle(EnemyTakolienBounceShieldVehicle other) : base(other)
		{
			spl__AttentionTargetingBancParam = other.spl__AttentionTargetingBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
			spl__RailPaintHelperBancParam = other.spl__RailPaintHelperBancParam.Clone();
		}

		public override EnemyTakolienBounceShieldVehicle Clone()
		{
			return new EnemyTakolienBounceShieldVehicle(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AttentionTargetingBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailPaintHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
