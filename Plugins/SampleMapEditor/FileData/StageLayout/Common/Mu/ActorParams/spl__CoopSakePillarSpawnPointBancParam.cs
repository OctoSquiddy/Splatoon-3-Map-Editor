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
	public class Mu_spl__CoopSakePillarSpawnPointBancParam
	{
		public Mu_spl__CoopSakePillarSpawnPointBancParam()
		{
		}

		public Mu_spl__CoopSakePillarSpawnPointBancParam(Mu_spl__CoopSakePillarSpawnPointBancParam other)
		{
		}

		public Mu_spl__CoopSakePillarSpawnPointBancParam Clone()
		{
			return new Mu_spl__CoopSakePillarSpawnPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopSakePillarSpawnPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSakePillarSpawnPointBancParam" };

			if (SerializedActor["spl__CoopSakePillarSpawnPointBancParam"] != null)
			{
				spl__CoopSakePillarSpawnPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSakePillarSpawnPointBancParam"];
			}


			if (SerializedActor["spl__CoopSakePillarSpawnPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopSakePillarSpawnPointBancParam);
			}
		}
	}
}
