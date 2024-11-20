using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__PlayerMediationForLogicBancParam
	{
		public Mu_spl__PlayerMediationForLogicBancParam()
		{
		}

		public Mu_spl__PlayerMediationForLogicBancParam(Mu_spl__PlayerMediationForLogicBancParam other)
		{
		}

		public Mu_spl__PlayerMediationForLogicBancParam Clone()
		{
			return new Mu_spl__PlayerMediationForLogicBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PlayerMediationForLogicBancParam = new BymlNode.DictionaryNode() { Name = "spl__PlayerMediationForLogicBancParam" };

			if (SerializedActor["spl__PlayerMediationForLogicBancParam"] != null)
			{
				spl__PlayerMediationForLogicBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PlayerMediationForLogicBancParam"];
			}


			if (SerializedActor["spl__PlayerMediationForLogicBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PlayerMediationForLogicBancParam);
			}
		}
	}
}
