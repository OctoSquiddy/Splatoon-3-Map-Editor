using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DObj_FldObj_TransparentActorLarge : MuObj
	{
		[BindGUI("VariationId", Category = "DObj_FldObj_TransparentActorLarge Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _VariationId
		{
			get
			{
				return this.spl__PlazaConditionalActorBancParam.VariationId;
			}

			set
			{
				this.spl__PlazaConditionalActorBancParam.VariationId = value;
			}
		}

		[ByamlMember("spl__PlazaConditionalActorBancParam")]
		public Mu_spl__PlazaConditionalActorBancParam spl__PlazaConditionalActorBancParam { get; set; }

		public DObj_FldObj_TransparentActorLarge() : base()
		{
			spl__PlazaConditionalActorBancParam = new Mu_spl__PlazaConditionalActorBancParam();

			Links = new List<Link>();
		}

		public DObj_FldObj_TransparentActorLarge(DObj_FldObj_TransparentActorLarge other) : base(other)
		{
			spl__PlazaConditionalActorBancParam = other.spl__PlazaConditionalActorBancParam.Clone();
		}

		public override DObj_FldObj_TransparentActorLarge Clone()
		{
			return new DObj_FldObj_TransparentActorLarge(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PlazaConditionalActorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
