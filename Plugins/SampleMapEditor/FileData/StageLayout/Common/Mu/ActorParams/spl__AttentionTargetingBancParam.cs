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
	public class Mu_spl__AttentionTargetingBancParam
	{
		public Mu_spl__AttentionTargetingBancParam()
		{
		}

		public Mu_spl__AttentionTargetingBancParam(Mu_spl__AttentionTargetingBancParam other)
		{
		}

		public Mu_spl__AttentionTargetingBancParam Clone()
		{
			return new Mu_spl__AttentionTargetingBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__AttentionTargetingBancParam = new BymlNode.DictionaryNode() { Name = "spl__AttentionTargetingBancParam" };

			if (SerializedActor["spl__AttentionTargetingBancParam"] != null)
			{
				spl__AttentionTargetingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AttentionTargetingBancParam"];
			}


			if (SerializedActor["spl__AttentionTargetingBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__AttentionTargetingBancParam);
			}
		}
	}
}
