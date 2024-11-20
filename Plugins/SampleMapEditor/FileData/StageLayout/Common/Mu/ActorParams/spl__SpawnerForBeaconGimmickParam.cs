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
	public class Mu_spl__SpawnerForBeaconGimmickParam
	{
		public Mu_spl__SpawnerForBeaconGimmickParam()
		{
		}

		public Mu_spl__SpawnerForBeaconGimmickParam(Mu_spl__SpawnerForBeaconGimmickParam other)
		{
		}

		public Mu_spl__SpawnerForBeaconGimmickParam Clone()
		{
			return new Mu_spl__SpawnerForBeaconGimmickParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpawnerForBeaconGimmickParam = new BymlNode.DictionaryNode() { Name = "spl__SpawnerForBeaconGimmickParam" };

			if (SerializedActor["spl__SpawnerForBeaconGimmickParam"] != null)
			{
				spl__SpawnerForBeaconGimmickParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpawnerForBeaconGimmickParam"];
			}


			if (SerializedActor["spl__SpawnerForBeaconGimmickParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpawnerForBeaconGimmickParam);
			}
		}
	}
}
