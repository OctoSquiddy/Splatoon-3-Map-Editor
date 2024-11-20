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
	public class Mu_spl__EnemyBombBlowSdodrBancParam
	{
		public Mu_spl__EnemyBombBlowSdodrBancParam()
		{
		}

		public Mu_spl__EnemyBombBlowSdodrBancParam(Mu_spl__EnemyBombBlowSdodrBancParam other)
		{
		}

		public Mu_spl__EnemyBombBlowSdodrBancParam Clone()
		{
			return new Mu_spl__EnemyBombBlowSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyBombBlowSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBombBlowSdodrBancParam" };

			if (SerializedActor["spl__EnemyBombBlowSdodrBancParam"] != null)
			{
				spl__EnemyBombBlowSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBombBlowSdodrBancParam"];
			}


			if (SerializedActor["spl__EnemyBombBlowSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyBombBlowSdodrBancParam);
			}
		}
	}
}
