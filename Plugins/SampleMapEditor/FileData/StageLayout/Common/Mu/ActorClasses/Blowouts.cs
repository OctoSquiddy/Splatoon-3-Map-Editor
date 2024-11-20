using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Blowouts : MuObj
	{
		[BindGUI("IsFreeY", Category = "Blowouts Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MaxLength", Category = "Blowouts Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxLength
		{
			get
			{
				return this.spl__BlowoutsBancParam.MaxLength;
			}

			set
			{
				this.spl__BlowoutsBancParam.MaxLength = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__BlowoutsBancParam")]
		public Mu_spl__BlowoutsBancParam spl__BlowoutsBancParam { get; set; }

		public Blowouts() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__BlowoutsBancParam = new Mu_spl__BlowoutsBancParam();

			Links = new List<Link>();
		}

		public Blowouts(Blowouts other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__BlowoutsBancParam = other.spl__BlowoutsBancParam.Clone();
		}

		public override Blowouts Clone()
		{
			return new Blowouts(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__BlowoutsBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
