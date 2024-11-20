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
	public class Mu_spl__ExplodeSpawnerSdodrBancParam
	{
		[ByamlMember]
		public float SpawnOffsetY { get; set; }

		public Mu_spl__ExplodeSpawnerSdodrBancParam()
		{
			SpawnOffsetY = 0.0f;
		}

		public Mu_spl__ExplodeSpawnerSdodrBancParam(Mu_spl__ExplodeSpawnerSdodrBancParam other)
		{
			SpawnOffsetY = other.SpawnOffsetY;
		}

		public Mu_spl__ExplodeSpawnerSdodrBancParam Clone()
		{
			return new Mu_spl__ExplodeSpawnerSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ExplodeSpawnerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__ExplodeSpawnerSdodrBancParam" };

			if (SerializedActor["spl__ExplodeSpawnerSdodrBancParam"] != null)
			{
				spl__ExplodeSpawnerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ExplodeSpawnerSdodrBancParam"];
			}


			spl__ExplodeSpawnerSdodrBancParam.AddNode("SpawnOffsetY", this.SpawnOffsetY);

			if (SerializedActor["spl__ExplodeSpawnerSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ExplodeSpawnerSdodrBancParam);
			}
		}
	}
}
