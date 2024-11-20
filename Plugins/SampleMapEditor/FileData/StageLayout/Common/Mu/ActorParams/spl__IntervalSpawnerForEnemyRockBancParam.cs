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
	public class Mu_spl__IntervalSpawnerForEnemyRockBancParam
	{
		[ByamlMember]
		public float CycleSec { get; set; }

		[ByamlMember]
		public bool IsTotem { get; set; }

		[ByamlMember]
		public ulong LinkToRail { get; set; }

		[ByamlMember]
		public string NormalTimingTable0 { get; set; }

		[ByamlMember]
		public string NormalTimingTable1 { get; set; }

		[ByamlMember]
		public string NormalTimingTable2 { get; set; }

		[ByamlMember]
		public int PauseSecAfterKilled { get; set; }

		[ByamlMember]
		public int ReserveNum { get; set; }

		[ByamlMember]
		public float SpeedRate { get; set; }

		public Mu_spl__IntervalSpawnerForEnemyRockBancParam()
		{
			CycleSec = 0.0f;
			IsTotem = false;
			LinkToRail = 0;
			NormalTimingTable0 = "";
			NormalTimingTable1 = "";
			NormalTimingTable2 = "";
			PauseSecAfterKilled = 0;
			ReserveNum = 0;
			SpeedRate = 0.0f;
		}

		public Mu_spl__IntervalSpawnerForEnemyRockBancParam(Mu_spl__IntervalSpawnerForEnemyRockBancParam other)
		{
			CycleSec = other.CycleSec;
			IsTotem = other.IsTotem;
			LinkToRail = other.LinkToRail;
			NormalTimingTable0 = other.NormalTimingTable0;
			NormalTimingTable1 = other.NormalTimingTable1;
			NormalTimingTable2 = other.NormalTimingTable2;
			PauseSecAfterKilled = other.PauseSecAfterKilled;
			ReserveNum = other.ReserveNum;
			SpeedRate = other.SpeedRate;
		}

		public Mu_spl__IntervalSpawnerForEnemyRockBancParam Clone()
		{
			return new Mu_spl__IntervalSpawnerForEnemyRockBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__IntervalSpawnerForEnemyRockBancParam = new BymlNode.DictionaryNode() { Name = "spl__IntervalSpawnerForEnemyRockBancParam" };

			if (SerializedActor["spl__IntervalSpawnerForEnemyRockBancParam"] != null)
			{
				spl__IntervalSpawnerForEnemyRockBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__IntervalSpawnerForEnemyRockBancParam"];
			}


			spl__IntervalSpawnerForEnemyRockBancParam.AddNode("CycleSec", this.CycleSec);

			if (this.IsTotem)
			{
				spl__IntervalSpawnerForEnemyRockBancParam.AddNode("IsTotem", this.IsTotem);
			}

			spl__IntervalSpawnerForEnemyRockBancParam.AddNode("LinkToRail", this.LinkToRail);

			if (this.NormalTimingTable0 != "")
			{
				spl__IntervalSpawnerForEnemyRockBancParam.AddNode("NormalTimingTable0", this.NormalTimingTable0);
			}

			if (this.NormalTimingTable1 != "")
			{
				spl__IntervalSpawnerForEnemyRockBancParam.AddNode("NormalTimingTable1", this.NormalTimingTable1);
			}

			if (this.NormalTimingTable2 != "")
			{
				spl__IntervalSpawnerForEnemyRockBancParam.AddNode("NormalTimingTable2", this.NormalTimingTable2);
			}

			spl__IntervalSpawnerForEnemyRockBancParam.AddNode("PauseSecAfterKilled", this.PauseSecAfterKilled);

			spl__IntervalSpawnerForEnemyRockBancParam.AddNode("ReserveNum", this.ReserveNum);

			spl__IntervalSpawnerForEnemyRockBancParam.AddNode("SpeedRate", this.SpeedRate);

			if (SerializedActor["spl__IntervalSpawnerForEnemyRockBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__IntervalSpawnerForEnemyRockBancParam);
			}
		}
	}
}
