using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_HighLiftLoaderUnder04 : MuObj
	{
		[ByamlMember("spl__PropellerOnlineDecorationBancParam")]
		public Mu_spl__PropellerOnlineDecorationBancParam spl__PropellerOnlineDecorationBancParam { get; set; }

		public Obj_HighLiftLoaderUnder04() : base()
		{
			spl__PropellerOnlineDecorationBancParam = new Mu_spl__PropellerOnlineDecorationBancParam();

			Links = new List<Link>();
		}

		public Obj_HighLiftLoaderUnder04(Obj_HighLiftLoaderUnder04 other) : base(other)
		{
			spl__PropellerOnlineDecorationBancParam = other.spl__PropellerOnlineDecorationBancParam.Clone();
		}

		public override Obj_HighLiftLoaderUnder04 Clone()
		{
			return new Obj_HighLiftLoaderUnder04(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PropellerOnlineDecorationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
