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
	public class Mu_spl__EnemyUtsuboxBancParam
	{
		public Mu_spl__EnemyUtsuboxBancParam()
		{
		}

		public Mu_spl__EnemyUtsuboxBancParam(Mu_spl__EnemyUtsuboxBancParam other)
		{
		}

		public Mu_spl__EnemyUtsuboxBancParam Clone()
		{
			return new Mu_spl__EnemyUtsuboxBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyUtsuboxBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyUtsuboxBancParam" };

			if (SerializedActor["spl__EnemyUtsuboxBancParam"] != null)
			{
				spl__EnemyUtsuboxBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyUtsuboxBancParam"];
			}


			if (SerializedActor["spl__EnemyUtsuboxBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyUtsuboxBancParam);
			}
		}
	}
}
