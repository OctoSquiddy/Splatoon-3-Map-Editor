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
	public class Mu_spl__EnemySharkKingBancParam
	{
		public Mu_spl__EnemySharkKingBancParam()
		{
		}

		public Mu_spl__EnemySharkKingBancParam(Mu_spl__EnemySharkKingBancParam other)
		{
		}

		public Mu_spl__EnemySharkKingBancParam Clone()
		{
			return new Mu_spl__EnemySharkKingBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemySharkKingBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySharkKingBancParam" };

			if (SerializedActor["spl__EnemySharkKingBancParam"] != null)
			{
				spl__EnemySharkKingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySharkKingBancParam"];
			}


			if (SerializedActor["spl__EnemySharkKingBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemySharkKingBancParam);
			}
		}
	}
}
