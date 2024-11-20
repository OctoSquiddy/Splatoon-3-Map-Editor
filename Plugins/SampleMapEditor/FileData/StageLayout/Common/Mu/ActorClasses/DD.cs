using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DD : MuObj
	{
		[BindGUI("IsFreeY", Category = "DD Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableRejectGroundCol", Category = "DD Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnableRejectGroundCol
		{
			get
			{
				return this.spl__ItemIkuraBancParam.IsEnableRejectGroundCol;
			}

			set
			{
				this.spl__ItemIkuraBancParam.IsEnableRejectGroundCol = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__ItemIkuraBancParam")]
		public Mu_spl__ItemIkuraBancParam spl__ItemIkuraBancParam { get; set; }

		public DD() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__ItemIkuraBancParam = new Mu_spl__ItemIkuraBancParam();

			Links = new List<Link>();
		}

		public DD(DD other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__ItemIkuraBancParam = other.spl__ItemIkuraBancParam.Clone();
		}

		public override DD Clone()
		{
			return new DD(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemIkuraBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
