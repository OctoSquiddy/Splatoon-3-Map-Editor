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
	public class Mu_spl__EnemyTreeSdodrBancParam
	{
		public Mu_spl__EnemyTreeSdodrBancParam()
		{
		}

		public Mu_spl__EnemyTreeSdodrBancParam(Mu_spl__EnemyTreeSdodrBancParam other)
		{
		}

		public Mu_spl__EnemyTreeSdodrBancParam Clone()
		{
			return new Mu_spl__EnemyTreeSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyTreeSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyTreeSdodrBancParam" };

			if (SerializedActor["spl__EnemyTreeSdodrBancParam"] != null)
			{
				spl__EnemyTreeSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyTreeSdodrBancParam"];
			}


			if (SerializedActor["spl__EnemyTreeSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyTreeSdodrBancParam);
			}
		}
	}
}
