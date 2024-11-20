using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_Shakeship00Gear : MuObj
	{
		[ByamlMember("spl__PropellerOnlineDecorationBancParam")]
		public Mu_spl__PropellerOnlineDecorationBancParam spl__PropellerOnlineDecorationBancParam { get; set; }

		public Obj_Shakeship00Gear() : base()
		{
			spl__PropellerOnlineDecorationBancParam = new Mu_spl__PropellerOnlineDecorationBancParam();

			Links = new List<Link>();
		}

		public Obj_Shakeship00Gear(Obj_Shakeship00Gear other) : base(other)
		{
			spl__PropellerOnlineDecorationBancParam = other.spl__PropellerOnlineDecorationBancParam.Clone();
		}

		public override Obj_Shakeship00Gear Clone()
		{
			return new Obj_Shakeship00Gear(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PropellerOnlineDecorationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
