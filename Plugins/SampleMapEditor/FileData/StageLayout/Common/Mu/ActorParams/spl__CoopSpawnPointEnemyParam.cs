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
	public class Mu_spl__CoopSpawnPointEnemyParam
	{
		[ByamlMember]
		public bool IsSpawnBoss { get; set; }

		public Mu_spl__CoopSpawnPointEnemyParam()
		{
			IsSpawnBoss = false;
		}

		public Mu_spl__CoopSpawnPointEnemyParam(Mu_spl__CoopSpawnPointEnemyParam other)
		{
			IsSpawnBoss = other.IsSpawnBoss;
		}

		public Mu_spl__CoopSpawnPointEnemyParam Clone()
		{
			return new Mu_spl__CoopSpawnPointEnemyParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopSpawnPointEnemyParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSpawnPointEnemyParam" };

			if (SerializedActor["spl__CoopSpawnPointEnemyParam"] != null)
			{
				spl__CoopSpawnPointEnemyParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSpawnPointEnemyParam"];
			}


			if (this.IsSpawnBoss)
			{
				spl__CoopSpawnPointEnemyParam.AddNode("IsSpawnBoss", this.IsSpawnBoss);
			}

			if (SerializedActor["spl__CoopSpawnPointEnemyParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopSpawnPointEnemyParam);
			}
		}
	}
}
