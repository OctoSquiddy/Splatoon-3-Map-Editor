using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyRock : MuObj
	{
		[BindGUI("ConstraintType", Category = "EnemyRock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ConstraintType
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.ConstraintType;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.ConstraintType = value;
			}
		}

		[BindGUI("FreeAxis", Category = "EnemyRock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _FreeAxis
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.FreeAxis;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.FreeAxis = value;
			}
		}

		[BindGUI("IsBindToWorld", Category = "EnemyRock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsBindToWorld
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.IsBindToWorld;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.IsBindToWorld = value;
			}
		}

		[BindGUI("IsNoHitBetweenBodies", Category = "EnemyRock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsNoHitBetweenBodies
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.IsNoHitBetweenBodies;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.IsNoHitBetweenBodies = value;
			}
		}

		[BindGUI("IsStop", Category = "EnemyRock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsStop
		{
			get
			{
				return this.spl__EnemyRockBancParam.IsStop;
			}

			set
			{
				this.spl__EnemyRockBancParam.IsStop = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "EnemyRock Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__ConstraintBindableHelperBancParam")]
		public Mu_spl__ConstraintBindableHelperBancParam spl__ConstraintBindableHelperBancParam { get; set; }

		[ByamlMember("spl__EnemyRockBancParam")]
		public Mu_spl__EnemyRockBancParam spl__EnemyRockBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		public EnemyRock() : base()
		{
			spl__ConstraintBindableHelperBancParam = new Mu_spl__ConstraintBindableHelperBancParam();
			spl__EnemyRockBancParam = new Mu_spl__EnemyRockBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();

			Links = new List<Link>();
		}

		public EnemyRock(EnemyRock other) : base(other)
		{
			spl__ConstraintBindableHelperBancParam = other.spl__ConstraintBindableHelperBancParam.Clone();
			spl__EnemyRockBancParam = other.spl__EnemyRockBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
		}

		public override EnemyRock Clone()
		{
			return new EnemyRock(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ConstraintBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyRockBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
