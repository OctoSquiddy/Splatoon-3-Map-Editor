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
	public class Mu_spl__EnemyTowerKingBancParamSdodr
	{
		public Mu_spl__EnemyTowerKingBancParamSdodr()
		{
		}

		public Mu_spl__EnemyTowerKingBancParamSdodr(Mu_spl__EnemyTowerKingBancParamSdodr other)
		{
		}

		public Mu_spl__EnemyTowerKingBancParamSdodr Clone()
		{
			return new Mu_spl__EnemyTowerKingBancParamSdodr(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyTowerKingBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__EnemyTowerKingBancParamSdodr" };

			if (SerializedActor["spl__EnemyTowerKingBancParamSdodr"] != null)
			{
				spl__EnemyTowerKingBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyTowerKingBancParamSdodr"];
			}


			if (SerializedActor["spl__EnemyTowerKingBancParamSdodr"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyTowerKingBancParamSdodr);
			}
		}
	}
}
