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
	public class Mu_spl__EnemyRockBancParam
	{
		[ByamlMember]
		public bool IsStop { get; set; }

		public Mu_spl__EnemyRockBancParam()
		{
			IsStop = false;
		}

		public Mu_spl__EnemyRockBancParam(Mu_spl__EnemyRockBancParam other)
		{
			IsStop = other.IsStop;
		}

		public Mu_spl__EnemyRockBancParam Clone()
		{
			return new Mu_spl__EnemyRockBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyRockBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyRockBancParam" };

			if (SerializedActor["spl__EnemyRockBancParam"] != null)
			{
				spl__EnemyRockBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyRockBancParam"];
			}


			if (this.IsStop)
			{
				spl__EnemyRockBancParam.AddNode("IsStop", this.IsStop);
			}

			if (SerializedActor["spl__EnemyRockBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyRockBancParam);
			}
		}
	}
}
