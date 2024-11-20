using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class AerialRing : MuObj
	{
		[BindGUI("IsFreeY", Category = "AerialRing Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IkuraNum", Category = "AerialRing Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IkuraNum
		{
			get
			{
				return this.spl__AerialRingBancParam.IkuraNum;
			}

			set
			{
				this.spl__AerialRingBancParam.IkuraNum = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__AerialRingBancParam")]
		public Mu_spl__AerialRingBancParam spl__AerialRingBancParam { get; set; }

		public AerialRing() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__AerialRingBancParam = new Mu_spl__AerialRingBancParam();

			Links = new List<Link>();
		}

		public AerialRing(AerialRing other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__AerialRingBancParam = other.spl__AerialRingBancParam.Clone();
		}

		public override AerialRing Clone()
		{
			return new AerialRing(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__AerialRingBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
