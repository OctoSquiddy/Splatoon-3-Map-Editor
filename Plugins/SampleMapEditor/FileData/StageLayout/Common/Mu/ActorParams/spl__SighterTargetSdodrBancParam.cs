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
	public class Mu_spl__SighterTargetSdodrBancParam
	{
		public Mu_spl__SighterTargetSdodrBancParam()
		{
		}

		public Mu_spl__SighterTargetSdodrBancParam(Mu_spl__SighterTargetSdodrBancParam other)
		{
		}

		public Mu_spl__SighterTargetSdodrBancParam Clone()
		{
			return new Mu_spl__SighterTargetSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SighterTargetSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__SighterTargetSdodrBancParam" };

			if (SerializedActor["spl__SighterTargetSdodrBancParam"] != null)
			{
				spl__SighterTargetSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SighterTargetSdodrBancParam"];
			}


			if (SerializedActor["spl__SighterTargetSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SighterTargetSdodrBancParam);
			}
		}
	}
}
