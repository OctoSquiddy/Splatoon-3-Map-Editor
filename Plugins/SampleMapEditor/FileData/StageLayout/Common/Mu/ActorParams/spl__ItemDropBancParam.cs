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
	public class Mu_spl__ItemDropBancParam
	{
		public Mu_spl__ItemDropBancParam()
		{
		}

		public Mu_spl__ItemDropBancParam(Mu_spl__ItemDropBancParam other)
		{
		}

		public Mu_spl__ItemDropBancParam Clone()
		{
			return new Mu_spl__ItemDropBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ItemDropBancParam = new BymlNode.DictionaryNode() { Name = "spl__ItemDropBancParam" };

			if (SerializedActor["spl__ItemDropBancParam"] != null)
			{
				spl__ItemDropBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ItemDropBancParam"];
			}


			if (SerializedActor["spl__ItemDropBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ItemDropBancParam);
			}
		}
	}
}
