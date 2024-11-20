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
	public class Mu_spl__SpawnerForShieldGimmickParam
	{
		public Mu_spl__SpawnerForShieldGimmickParam()
		{
		}

		public Mu_spl__SpawnerForShieldGimmickParam(Mu_spl__SpawnerForShieldGimmickParam other)
		{
		}

		public Mu_spl__SpawnerForShieldGimmickParam Clone()
		{
			return new Mu_spl__SpawnerForShieldGimmickParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpawnerForShieldGimmickParam = new BymlNode.DictionaryNode() { Name = "spl__SpawnerForShieldGimmickParam" };

			if (SerializedActor["spl__SpawnerForShieldGimmickParam"] != null)
			{
				spl__SpawnerForShieldGimmickParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpawnerForShieldGimmickParam"];
			}


			if (SerializedActor["spl__SpawnerForShieldGimmickParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpawnerForShieldGimmickParam);
			}
		}
	}
}
