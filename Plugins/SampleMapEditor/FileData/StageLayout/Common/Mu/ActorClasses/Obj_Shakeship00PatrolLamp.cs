using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_Shakeship00PatrolLamp : MuObj
	{
		[ByamlMember("spl__PropellerOnlineDecorationBancParam")]
		public Mu_spl__PropellerOnlineDecorationBancParam spl__PropellerOnlineDecorationBancParam { get; set; }

		public Obj_Shakeship00PatrolLamp() : base()
		{
			spl__PropellerOnlineDecorationBancParam = new Mu_spl__PropellerOnlineDecorationBancParam();

			Links = new List<Link>();
		}

		public Obj_Shakeship00PatrolLamp(Obj_Shakeship00PatrolLamp other) : base(other)
		{
			spl__PropellerOnlineDecorationBancParam = other.spl__PropellerOnlineDecorationBancParam.Clone();
		}

		public override Obj_Shakeship00PatrolLamp Clone()
		{
			return new Obj_Shakeship00PatrolLamp(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PropellerOnlineDecorationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
