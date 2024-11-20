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
	public class Mu_spl__RandomDigUpPointSpawnAreaBancParam
	{
		[ByamlMember]
		public int MaxSpawnNum { get; set; }

		[ByamlMember]
		public int MinSpawnNum { get; set; }

		public Mu_spl__RandomDigUpPointSpawnAreaBancParam()
		{
			MaxSpawnNum = 0;
			MinSpawnNum = 0;
		}

		public Mu_spl__RandomDigUpPointSpawnAreaBancParam(Mu_spl__RandomDigUpPointSpawnAreaBancParam other)
		{
			MaxSpawnNum = other.MaxSpawnNum;
			MinSpawnNum = other.MinSpawnNum;
		}

		public Mu_spl__RandomDigUpPointSpawnAreaBancParam Clone()
		{
			return new Mu_spl__RandomDigUpPointSpawnAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RandomDigUpPointSpawnAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__RandomDigUpPointSpawnAreaBancParam" };

			if (SerializedActor["spl__RandomDigUpPointSpawnAreaBancParam"] != null)
			{
				spl__RandomDigUpPointSpawnAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RandomDigUpPointSpawnAreaBancParam"];
			}


			spl__RandomDigUpPointSpawnAreaBancParam.AddNode("MaxSpawnNum", this.MaxSpawnNum);

			spl__RandomDigUpPointSpawnAreaBancParam.AddNode("MinSpawnNum", this.MinSpawnNum);

			if (SerializedActor["spl__RandomDigUpPointSpawnAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RandomDigUpPointSpawnAreaBancParam);
			}
		}
	}
}
