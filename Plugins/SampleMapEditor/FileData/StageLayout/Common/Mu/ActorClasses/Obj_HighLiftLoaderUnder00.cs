using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_HighLiftLoaderUnder00 : MuObj
	{
		[ByamlMember("spl__PropellerOnlineDecorationBancParam")]
		public Mu_spl__PropellerOnlineDecorationBancParam spl__PropellerOnlineDecorationBancParam { get; set; }

		public Obj_HighLiftLoaderUnder00() : base()
		{
			spl__PropellerOnlineDecorationBancParam = new Mu_spl__PropellerOnlineDecorationBancParam();

			Links = new List<Link>();
		}

		public Obj_HighLiftLoaderUnder00(Obj_HighLiftLoaderUnder00 other) : base(other)
		{
			spl__PropellerOnlineDecorationBancParam = other.spl__PropellerOnlineDecorationBancParam.Clone();
		}

		public override Obj_HighLiftLoaderUnder00 Clone()
		{
			return new Obj_HighLiftLoaderUnder00(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PropellerOnlineDecorationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}