using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GachihokoCheckPoint : MuObj
	{
		[BindGUI("IsFreeY", Category = "GachihokoCheckPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public GachihokoCheckPoint() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();

			Links = new List<Link>();
		}

		public GachihokoCheckPoint(GachihokoCheckPoint other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
		}

		public override GachihokoCheckPoint Clone()
		{
			return new GachihokoCheckPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}