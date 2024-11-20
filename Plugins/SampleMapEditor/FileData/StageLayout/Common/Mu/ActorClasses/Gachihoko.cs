using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Gachihoko : MuObj
	{
		[BindGUI("ConstraintType", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("FreeAxis", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsBindToWorld", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsNoHitBetweenBodies", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("HikikomoriDetectionBaseRate", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HikikomoriDetectionBaseRate
		{
			get
			{
				return this.spl__GachihokoBancParam.HikikomoriDetectionBaseRate;
			}

			set
			{
				this.spl__GachihokoBancParam.HikikomoriDetectionBaseRate = value;
			}
		}

		[BindGUI("HikikomoriDetectionDist", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HikikomoriDetectionDist
		{
			get
			{
				return this.spl__GachihokoBancParam.HikikomoriDetectionDist;
			}

			set
			{
				this.spl__GachihokoBancParam.HikikomoriDetectionDist = value;
			}
		}

		[BindGUI("HikikomoriDetectionRate", Category = "Gachihoko Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HikikomoriDetectionRate
		{
			get
			{
				return this.spl__GachihokoBancParam.HikikomoriDetectionRate;
			}

			set
			{
				this.spl__GachihokoBancParam.HikikomoriDetectionRate = value;
			}
		}

		[ByamlMember("spl__ConstraintBindableHelperBancParam")]
		public Mu_spl__ConstraintBindableHelperBancParam spl__ConstraintBindableHelperBancParam { get; set; }

		[ByamlMember("spl__GachihokoBancParam")]
		public Mu_spl__GachihokoBancParam spl__GachihokoBancParam { get; set; }

		public Gachihoko() : base()
		{
			spl__ConstraintBindableHelperBancParam = new Mu_spl__ConstraintBindableHelperBancParam();
			spl__GachihokoBancParam = new Mu_spl__GachihokoBancParam();

			Links = new List<Link>();
		}

		public Gachihoko(Gachihoko other) : base(other)
		{
			spl__ConstraintBindableHelperBancParam = other.spl__ConstraintBindableHelperBancParam.Clone();
			spl__GachihokoBancParam = other.spl__GachihokoBancParam.Clone();
		}

		public override Gachihoko Clone()
		{
			return new Gachihoko(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ConstraintBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__GachihokoBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
