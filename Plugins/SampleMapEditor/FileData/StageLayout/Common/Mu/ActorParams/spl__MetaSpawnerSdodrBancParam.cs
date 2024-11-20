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
	public class Mu_spl__MetaSpawnerSdodrBancParam
	{
		[ByamlMember]
		public float DelaySec { get; set; }

		[ByamlMember]
		public float IntervalSec { get; set; }

		[ByamlMember]
		public int MetaSpawnerId { get; set; }

		[ByamlMember]
		public string SelectPolicyType { get; set; }

		[ByamlMember]
		public int SpawnNumPerTiming { get; set; }

		[ByamlMember]
		public string SpawnOrderTable { get; set; }

		[ByamlMember]
		public string SpawnThresholdTable { get; set; }

		public Mu_spl__MetaSpawnerSdodrBancParam()
		{
			DelaySec = 0.0f;
			IntervalSec = 0.0f;
			MetaSpawnerId = 0;
			SelectPolicyType = "";
			SpawnNumPerTiming = 0;
			SpawnOrderTable = "";
			SpawnThresholdTable = "";
		}

		public Mu_spl__MetaSpawnerSdodrBancParam(Mu_spl__MetaSpawnerSdodrBancParam other)
		{
			DelaySec = other.DelaySec;
			IntervalSec = other.IntervalSec;
			MetaSpawnerId = other.MetaSpawnerId;
			SelectPolicyType = other.SelectPolicyType;
			SpawnNumPerTiming = other.SpawnNumPerTiming;
			SpawnOrderTable = other.SpawnOrderTable;
			SpawnThresholdTable = other.SpawnThresholdTable;
		}

		public Mu_spl__MetaSpawnerSdodrBancParam Clone()
		{
			return new Mu_spl__MetaSpawnerSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MetaSpawnerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__MetaSpawnerSdodrBancParam" };

			if (SerializedActor["spl__MetaSpawnerSdodrBancParam"] != null)
			{
				spl__MetaSpawnerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MetaSpawnerSdodrBancParam"];
			}


			spl__MetaSpawnerSdodrBancParam.AddNode("DelaySec", this.DelaySec);

			spl__MetaSpawnerSdodrBancParam.AddNode("IntervalSec", this.IntervalSec);

			spl__MetaSpawnerSdodrBancParam.AddNode("MetaSpawnerId", this.MetaSpawnerId);

			if (this.SelectPolicyType != "")
			{
				spl__MetaSpawnerSdodrBancParam.AddNode("SelectPolicyType", this.SelectPolicyType);
			}

			spl__MetaSpawnerSdodrBancParam.AddNode("SpawnNumPerTiming", this.SpawnNumPerTiming);

			if (this.SpawnOrderTable != "")
			{
				spl__MetaSpawnerSdodrBancParam.AddNode("SpawnOrderTable", this.SpawnOrderTable);
			}

			if (this.SpawnThresholdTable != "")
			{
				spl__MetaSpawnerSdodrBancParam.AddNode("SpawnThresholdTable", this.SpawnThresholdTable);
			}

			if (SerializedActor["spl__MetaSpawnerSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MetaSpawnerSdodrBancParam);
			}
		}
	}
}
