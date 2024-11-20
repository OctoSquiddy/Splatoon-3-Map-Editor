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
	public class Mu_spl__EnemyCleanerBancParam
	{
		public Mu_spl__EnemyCleanerBancParam()
		{
		}

		public Mu_spl__EnemyCleanerBancParam(Mu_spl__EnemyCleanerBancParam other)
		{
		}

		public Mu_spl__EnemyCleanerBancParam Clone()
		{
			return new Mu_spl__EnemyCleanerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyCleanerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyCleanerBancParam" };

			if (SerializedActor["spl__EnemyCleanerBancParam"] != null)
			{
				spl__EnemyCleanerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyCleanerBancParam"];
			}


			if (SerializedActor["spl__EnemyCleanerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyCleanerBancParam);
			}
		}
	}
}
