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
	public class Mu_spl__EnemyTakopterBancParam
	{
		[ByamlMember]
		public bool IsFixedMode { get; set; }

		public Mu_spl__EnemyTakopterBancParam()
		{
			IsFixedMode = false;
		}

		public Mu_spl__EnemyTakopterBancParam(Mu_spl__EnemyTakopterBancParam other)
		{
			IsFixedMode = other.IsFixedMode;
		}

		public Mu_spl__EnemyTakopterBancParam Clone()
		{
			return new Mu_spl__EnemyTakopterBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyTakopterBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyTakopterBancParam" };

			if (SerializedActor["spl__EnemyTakopterBancParam"] != null)
			{
				spl__EnemyTakopterBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyTakopterBancParam"];
			}


			if (this.IsFixedMode)
			{
				spl__EnemyTakopterBancParam.AddNode("IsFixedMode", this.IsFixedMode);
			}

			if (SerializedActor["spl__EnemyTakopterBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyTakopterBancParam);
			}
		}
	}
}
