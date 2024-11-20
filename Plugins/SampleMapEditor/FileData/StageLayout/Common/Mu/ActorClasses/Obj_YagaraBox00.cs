using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_YagaraBox00 : MuObj
	{
		[ByamlMember("spl__WallaObjBancParam")]
		public Mu_spl__WallaObjBancParam spl__WallaObjBancParam { get; set; }

		public Obj_YagaraBox00() : base()
		{
			spl__WallaObjBancParam = new Mu_spl__WallaObjBancParam();

			Links = new List<Link>();
		}

		public Obj_YagaraBox00(Obj_YagaraBox00 other) : base(other)
		{
			spl__WallaObjBancParam = other.spl__WallaObjBancParam.Clone();
		}

		public override Obj_YagaraBox00 Clone()
		{
			return new Obj_YagaraBox00(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__WallaObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
