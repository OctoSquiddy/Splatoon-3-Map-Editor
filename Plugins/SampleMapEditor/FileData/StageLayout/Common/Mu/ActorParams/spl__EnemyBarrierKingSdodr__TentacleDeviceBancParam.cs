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
	public class Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam
	{
		[ByamlMember]
		public string ColorGroupType { get; set; }

		[ByamlMember]
		public int OverwriteMaxHP { get; set; }

		[ByamlMember]
		public string PlacementType { get; set; }

		[ByamlMember]
		public ulong ToRail { get; set; }

		[ByamlMember]
		public string Type { get; set; }

		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam()
		{
			ColorGroupType = "";
			OverwriteMaxHP = 0;
			PlacementType = "";
			ToRail = 0;
			Type = "";
		}

		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam(Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam other)
		{
			ColorGroupType = other.ColorGroupType;
			OverwriteMaxHP = other.OverwriteMaxHP;
			PlacementType = other.PlacementType;
			ToRail = other.ToRail;
			Type = other.Type;
		}

		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam Clone()
		{
			return new Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam" };

			if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam"] != null)
			{
				spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam"];
			}


			if (this.ColorGroupType != "")
			{
				spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.AddNode("ColorGroupType", this.ColorGroupType);
			}

			spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.AddNode("OverwriteMaxHP", this.OverwriteMaxHP);

			if (this.PlacementType != "")
			{
				spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.AddNode("PlacementType", this.PlacementType);
			}

			spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.AddNode("ToRail", this.ToRail);

			if (this.Type != "")
			{
				spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.AddNode("Type", this.Type);
			}

			if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam);
			}
		}
	}
}
