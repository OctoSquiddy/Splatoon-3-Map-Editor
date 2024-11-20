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
	public class Mu_spl__EnemySprinklerSdodrBancParam
	{
		public Mu_spl__EnemySprinklerSdodrBancParam()
		{
		}

		public Mu_spl__EnemySprinklerSdodrBancParam(Mu_spl__EnemySprinklerSdodrBancParam other)
		{
		}

		public Mu_spl__EnemySprinklerSdodrBancParam Clone()
		{
			return new Mu_spl__EnemySprinklerSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemySprinklerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySprinklerSdodrBancParam" };

			if (SerializedActor["spl__EnemySprinklerSdodrBancParam"] != null)
			{
				spl__EnemySprinklerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySprinklerSdodrBancParam"];
			}


			if (SerializedActor["spl__EnemySprinklerSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemySprinklerSdodrBancParam);
			}
		}
	}
}
