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
	public class Mu_spl__CoopSpawnGeyserBancParam
	{
		[ByamlMember]
		public bool FlgFarBank { get; set; }

		public Mu_spl__CoopSpawnGeyserBancParam()
		{
			FlgFarBank = false;
		}

		public Mu_spl__CoopSpawnGeyserBancParam(Mu_spl__CoopSpawnGeyserBancParam other)
		{
			FlgFarBank = other.FlgFarBank;
		}

		public Mu_spl__CoopSpawnGeyserBancParam Clone()
		{
			return new Mu_spl__CoopSpawnGeyserBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopSpawnGeyserBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSpawnGeyserBancParam" };

			if (SerializedActor["spl__CoopSpawnGeyserBancParam"] != null)
			{
				spl__CoopSpawnGeyserBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSpawnGeyserBancParam"];
			}


			if (this.FlgFarBank)
			{
				spl__CoopSpawnGeyserBancParam.AddNode("FlgFarBank", this.FlgFarBank);
			}

			if (SerializedActor["spl__CoopSpawnGeyserBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopSpawnGeyserBancParam);
			}
		}
	}
}
