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
	public class Mu_spl__EnemySpinnerBancParam
	{
		public Mu_spl__EnemySpinnerBancParam()
		{
		}

		public Mu_spl__EnemySpinnerBancParam(Mu_spl__EnemySpinnerBancParam other)
		{
		}

		public Mu_spl__EnemySpinnerBancParam Clone()
		{
			return new Mu_spl__EnemySpinnerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemySpinnerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySpinnerBancParam" };

			if (SerializedActor["spl__EnemySpinnerBancParam"] != null)
			{
				spl__EnemySpinnerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySpinnerBancParam"];
			}


			if (SerializedActor["spl__EnemySpinnerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemySpinnerBancParam);
			}
		}
	}
}
