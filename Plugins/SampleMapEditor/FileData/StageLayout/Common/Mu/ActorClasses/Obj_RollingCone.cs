using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_RollingCone : MuObj
	{
		[ByamlMember("spl__WallaObjBancParam")]
		public Mu_spl__WallaObjBancParam spl__WallaObjBancParam { get; set; }

		public Obj_RollingCone() : base()
		{
			spl__WallaObjBancParam = new Mu_spl__WallaObjBancParam();

			Links = new List<Link>();
		}

		public Obj_RollingCone(Obj_RollingCone other) : base(other)
		{
			spl__WallaObjBancParam = other.spl__WallaObjBancParam.Clone();
		}

		public override Obj_RollingCone Clone()
		{
			return new Obj_RollingCone(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__WallaObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
