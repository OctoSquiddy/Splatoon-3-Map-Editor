using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplPlayerMediationForLogic : MuObj
	{
		[ByamlMember("spl__PlayerMediationForLogicBancParam")]
		public Mu_spl__PlayerMediationForLogicBancParam spl__PlayerMediationForLogicBancParam { get; set; }

		public SplPlayerMediationForLogic() : base()
		{
			spl__PlayerMediationForLogicBancParam = new Mu_spl__PlayerMediationForLogicBancParam();

			Links = new List<Link>();
		}

		public SplPlayerMediationForLogic(SplPlayerMediationForLogic other) : base(other)
		{
			spl__PlayerMediationForLogicBancParam = other.spl__PlayerMediationForLogicBancParam.Clone();
		}

		public override SplPlayerMediationForLogic Clone()
		{
			return new SplPlayerMediationForLogic(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PlayerMediationForLogicBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
