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
	public class Mu_spl__EnemyChargerTowerSdodrBancParam
	{
		public Mu_spl__EnemyChargerTowerSdodrBancParam()
		{
		}

		public Mu_spl__EnemyChargerTowerSdodrBancParam(Mu_spl__EnemyChargerTowerSdodrBancParam other)
		{
		}

		public Mu_spl__EnemyChargerTowerSdodrBancParam Clone()
		{
			return new Mu_spl__EnemyChargerTowerSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyChargerTowerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyChargerTowerSdodrBancParam" };

			if (SerializedActor["spl__EnemyChargerTowerSdodrBancParam"] != null)
			{
				spl__EnemyChargerTowerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyChargerTowerSdodrBancParam"];
			}


			if (SerializedActor["spl__EnemyChargerTowerSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyChargerTowerSdodrBancParam);
			}
		}
	}
}
