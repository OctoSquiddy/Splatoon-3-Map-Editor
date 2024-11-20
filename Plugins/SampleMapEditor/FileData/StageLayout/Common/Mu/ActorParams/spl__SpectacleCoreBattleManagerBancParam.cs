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
	public class Mu_spl__SpectacleCoreBattleManagerBancParam
	{
		[ByamlMember]
		public int CoreBattlePhase { get; set; }

		[ByamlMember]
		public string CoreName0 { get; set; }

		[ByamlMember]
		public float EnemyGenerateIntervalSec { get; set; }

		[ByamlMember]
		public float EnemyGenerateSecPhase0 { get; set; }

		[ByamlMember]
		public float EnemyGenerateSecPhase1 { get; set; }

		[ByamlMember]
		public float EnemyGenerateSecPhase2 { get; set; }

		[ByamlMember]
		public ulong LinkToRailNode { get; set; }

		public Mu_spl__SpectacleCoreBattleManagerBancParam()
		{
			CoreBattlePhase = 0;
			CoreName0 = "";
			EnemyGenerateIntervalSec = 0.0f;
			EnemyGenerateSecPhase0 = 0.0f;
			EnemyGenerateSecPhase1 = 0.0f;
			EnemyGenerateSecPhase2 = 0.0f;
			LinkToRailNode = 0;
		}

		public Mu_spl__SpectacleCoreBattleManagerBancParam(Mu_spl__SpectacleCoreBattleManagerBancParam other)
		{
			CoreBattlePhase = other.CoreBattlePhase;
			CoreName0 = other.CoreName0;
			EnemyGenerateIntervalSec = other.EnemyGenerateIntervalSec;
			EnemyGenerateSecPhase0 = other.EnemyGenerateSecPhase0;
			EnemyGenerateSecPhase1 = other.EnemyGenerateSecPhase1;
			EnemyGenerateSecPhase2 = other.EnemyGenerateSecPhase2;
			LinkToRailNode = other.LinkToRailNode;
		}

		public Mu_spl__SpectacleCoreBattleManagerBancParam Clone()
		{
			return new Mu_spl__SpectacleCoreBattleManagerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpectacleCoreBattleManagerBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleCoreBattleManagerBancParam" };

			if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] != null)
			{
				spl__SpectacleCoreBattleManagerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleCoreBattleManagerBancParam"];
			}


			spl__SpectacleCoreBattleManagerBancParam.AddNode("CoreBattlePhase", this.CoreBattlePhase);

			if (this.CoreName0 != "")
			{
				spl__SpectacleCoreBattleManagerBancParam.AddNode("CoreName0", this.CoreName0);
			}

			spl__SpectacleCoreBattleManagerBancParam.AddNode("EnemyGenerateIntervalSec", this.EnemyGenerateIntervalSec);

			spl__SpectacleCoreBattleManagerBancParam.AddNode("EnemyGenerateSecPhase0", this.EnemyGenerateSecPhase0);

			spl__SpectacleCoreBattleManagerBancParam.AddNode("EnemyGenerateSecPhase1", this.EnemyGenerateSecPhase1);

			spl__SpectacleCoreBattleManagerBancParam.AddNode("EnemyGenerateSecPhase2", this.EnemyGenerateSecPhase2);

			spl__SpectacleCoreBattleManagerBancParam.AddNode("LinkToRailNode", this.LinkToRailNode);

			if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpectacleCoreBattleManagerBancParam);
			}
		}
	}
}
