using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class ShootingBoxS : MuObj
	{
		[BindGUI("IsFreeY", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ConstraintType", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("FreeAxis", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsBindToWorld", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsNoHitBetweenBodies", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IkuraNum", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IkuraNum
		{
			get
			{
				return this.spl__WoodenBoxBancParam.IkuraNum;
			}

			set
			{
				this.spl__WoodenBoxBancParam.IkuraNum = value;
			}
		}

		[BindGUI("IkuraValue", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IkuraValue
		{
			get
			{
				return this.spl__WoodenBoxBancParam.IkuraValue;
			}

			set
			{
				this.spl__WoodenBoxBancParam.IkuraValue = value;
			}
		}

		[BindGUI("IsEnemyBreakMiss", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnemyBreakMiss
		{
			get
			{
				return this.spl__WoodenBoxBancParam.IsEnemyBreakMiss;
			}

			set
			{
				this.spl__WoodenBoxBancParam.IsEnemyBreakMiss = value;
			}
		}

		[BindGUI("IsKinematic", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsKinematic
		{
			get
			{
				return this.spl__WoodenBoxBancParam.IsKinematic;
			}

			set
			{
				this.spl__WoodenBoxBancParam.IsKinematic = value;
			}
		}

		[BindGUI("TimeLimit", Category = "ShootingBoxS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _TimeLimit
		{
			get
			{
				return this.spl__WoodenBoxBancParam.TimeLimit;
			}

			set
			{
				this.spl__WoodenBoxBancParam.TimeLimit = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__ConstraintBindableHelperBancParam")]
		public Mu_spl__ConstraintBindableHelperBancParam spl__ConstraintBindableHelperBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__WoodenBoxBancParam")]
		public Mu_spl__WoodenBoxBancParam spl__WoodenBoxBancParam { get; set; }

		public ShootingBoxS() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__ConstraintBindableHelperBancParam = new Mu_spl__ConstraintBindableHelperBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__WoodenBoxBancParam = new Mu_spl__WoodenBoxBancParam();

			Links = new List<Link>();
		}

		public ShootingBoxS(ShootingBoxS other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__ConstraintBindableHelperBancParam = other.spl__ConstraintBindableHelperBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__WoodenBoxBancParam = other.spl__WoodenBoxBancParam.Clone();
		}

		public override ShootingBoxS Clone()
		{
			return new ShootingBoxS(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ConstraintBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__WoodenBoxBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
