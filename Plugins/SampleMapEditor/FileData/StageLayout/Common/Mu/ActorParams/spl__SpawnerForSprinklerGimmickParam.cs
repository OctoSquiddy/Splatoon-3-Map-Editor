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
	public class Mu_spl__SpawnerForSprinklerGimmickParam
	{
		[ByamlMember]
		public float ActivatePlayerDistance { get; set; }

		[ByamlMember]
		public int IkuraNum { get; set; }

		[ByamlMember]
		public string SpawnType { get; set; }

		[ByamlMember]
		public string SprinkleType { get; set; }

		public Mu_spl__SpawnerForSprinklerGimmickParam()
		{
			ActivatePlayerDistance = 0.0f;
			IkuraNum = 0;
			SpawnType = "";
			SprinkleType = "";
		}

		public Mu_spl__SpawnerForSprinklerGimmickParam(Mu_spl__SpawnerForSprinklerGimmickParam other)
		{
			ActivatePlayerDistance = other.ActivatePlayerDistance;
			IkuraNum = other.IkuraNum;
			SpawnType = other.SpawnType;
			SprinkleType = other.SprinkleType;
		}

		public Mu_spl__SpawnerForSprinklerGimmickParam Clone()
		{
			return new Mu_spl__SpawnerForSprinklerGimmickParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpawnerForSprinklerGimmickParam = new BymlNode.DictionaryNode() { Name = "spl__SpawnerForSprinklerGimmickParam" };

			if (SerializedActor["spl__SpawnerForSprinklerGimmickParam"] != null)
			{
				spl__SpawnerForSprinklerGimmickParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpawnerForSprinklerGimmickParam"];
			}


			spl__SpawnerForSprinklerGimmickParam.AddNode("ActivatePlayerDistance", this.ActivatePlayerDistance);

			spl__SpawnerForSprinklerGimmickParam.AddNode("IkuraNum", this.IkuraNum);

			if (this.SpawnType != "")
			{
				spl__SpawnerForSprinklerGimmickParam.AddNode("SpawnType", this.SpawnType);
			}

			if (this.SprinkleType != "")
			{
				spl__SpawnerForSprinklerGimmickParam.AddNode("SprinkleType", this.SprinkleType);
			}

			if (SerializedActor["spl__SpawnerForSprinklerGimmickParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpawnerForSprinklerGimmickParam);
			}
		}
	}
}
