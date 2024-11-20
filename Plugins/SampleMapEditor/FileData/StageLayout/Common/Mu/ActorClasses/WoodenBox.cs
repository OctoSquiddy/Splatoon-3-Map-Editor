using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class WoodenBox : MuObj
	{
		[BindGUI("IsFreeY", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

        [BindGUI("Constraint Type",
            new object[2] { "", "ConstraintSlider" },
            new string[2] { "Fixed", "Slider" },
            Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
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

        [BindGUI("Free Axis",
            new object[2] { "", "FreeAxis" },
            new string[2] { "Default", "Free" },
            Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
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

		[BindGUI("IsBindToWorld", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsNoHitBetweenBodies", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

        [BindGUI("Hide Mode",
            new object[3] { "", "HideGnd", "HideWall" },
            new string[3] { "None", "Hide in the Ground", "Hide in the Wall" },
            Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string _HideMode
		{
			get
			{
				return this.spl__InkHideHelperBancParam.HideMode;
			}

			set
			{
				this.spl__InkHideHelperBancParam.HideMode = value;
			}
		}

		[BindGUI("IkuraNum", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IkuraValue", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnemyBreakMiss", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsKinematic", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("TimeLimit", Category = "WoodenBox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__InkHideHelperBancParam")]
		public Mu_spl__InkHideHelperBancParam spl__InkHideHelperBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__WoodenBoxBancParam")]
		public Mu_spl__WoodenBoxBancParam spl__WoodenBoxBancParam { get; set; }

		public WoodenBox() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__ConstraintBindableHelperBancParam = new Mu_spl__ConstraintBindableHelperBancParam();
			spl__InkHideHelperBancParam = new Mu_spl__InkHideHelperBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__WoodenBoxBancParam = new Mu_spl__WoodenBoxBancParam();

			Links = new List<Link>();
		}

		public WoodenBox(WoodenBox other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__ConstraintBindableHelperBancParam = other.spl__ConstraintBindableHelperBancParam.Clone();
			spl__InkHideHelperBancParam = other.spl__InkHideHelperBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__WoodenBoxBancParam = other.spl__WoodenBoxBancParam.Clone();
		}

		public override WoodenBox Clone()
		{
			return new WoodenBox(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ConstraintBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__InkHideHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__WoodenBoxBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
