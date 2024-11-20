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
	public class Mu_spl__PropellerOnlineDecorationBancParam
	{
		public Mu_spl__PropellerOnlineDecorationBancParam()
		{
		}

		public Mu_spl__PropellerOnlineDecorationBancParam(Mu_spl__PropellerOnlineDecorationBancParam other)
		{
		}

		public Mu_spl__PropellerOnlineDecorationBancParam Clone()
		{
			return new Mu_spl__PropellerOnlineDecorationBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PropellerOnlineDecorationBancParam = new BymlNode.DictionaryNode() { Name = "spl__PropellerOnlineDecorationBancParam" };

			if (SerializedActor["spl__PropellerOnlineDecorationBancParam"] != null)
			{
				spl__PropellerOnlineDecorationBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PropellerOnlineDecorationBancParam"];
			}


			if (SerializedActor["spl__PropellerOnlineDecorationBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PropellerOnlineDecorationBancParam);
			}
		}
	}
}
