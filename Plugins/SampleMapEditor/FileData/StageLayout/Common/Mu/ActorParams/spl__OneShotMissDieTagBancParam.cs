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
	public class Mu_spl__OneShotMissDieTagBancParam
	{
		public Mu_spl__OneShotMissDieTagBancParam()
		{
		}

		public Mu_spl__OneShotMissDieTagBancParam(Mu_spl__OneShotMissDieTagBancParam other)
		{
		}

		public Mu_spl__OneShotMissDieTagBancParam Clone()
		{
			return new Mu_spl__OneShotMissDieTagBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__OneShotMissDieTagBancParam = new BymlNode.DictionaryNode() { Name = "spl__OneShotMissDieTagBancParam" };

			if (SerializedActor["spl__OneShotMissDieTagBancParam"] != null)
			{
				spl__OneShotMissDieTagBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__OneShotMissDieTagBancParam"];
			}


			if (SerializedActor["spl__OneShotMissDieTagBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__OneShotMissDieTagBancParam);
			}
		}
	}
}
