using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class ExclamationMarkEmitObj : MuObj
	{
		[BindGUI("IsFreeY", Category = "ExclamationMarkEmitObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__ExclamationMarkEmitObjBancParam")]
		public Mu_spl__ExclamationMarkEmitObjBancParam spl__ExclamationMarkEmitObjBancParam { get; set; }

		public ExclamationMarkEmitObj() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__ExclamationMarkEmitObjBancParam = new Mu_spl__ExclamationMarkEmitObjBancParam();

			Links = new List<Link>();
		}

		public ExclamationMarkEmitObj(ExclamationMarkEmitObj other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__ExclamationMarkEmitObjBancParam = other.spl__ExclamationMarkEmitObjBancParam.Clone();
		}

		public override ExclamationMarkEmitObj Clone()
		{
			return new ExclamationMarkEmitObj(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ExclamationMarkEmitObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
