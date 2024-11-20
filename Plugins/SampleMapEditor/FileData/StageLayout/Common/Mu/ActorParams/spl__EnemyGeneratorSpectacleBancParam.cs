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
	public class Mu_spl__EnemyGeneratorSpectacleBancParam
	{
		[ByamlMember]
		public string EnemyNamePhase0 { get; set; }

		[ByamlMember]
		public string EnemyNamePhase1 { get; set; }

		[ByamlMember]
		public string EnemyNamePhase2 { get; set; }

		public Mu_spl__EnemyGeneratorSpectacleBancParam()
		{
			EnemyNamePhase0 = "";
			EnemyNamePhase1 = "";
			EnemyNamePhase2 = "";
		}

		public Mu_spl__EnemyGeneratorSpectacleBancParam(Mu_spl__EnemyGeneratorSpectacleBancParam other)
		{
			EnemyNamePhase0 = other.EnemyNamePhase0;
			EnemyNamePhase1 = other.EnemyNamePhase1;
			EnemyNamePhase2 = other.EnemyNamePhase2;
		}

		public Mu_spl__EnemyGeneratorSpectacleBancParam Clone()
		{
			return new Mu_spl__EnemyGeneratorSpectacleBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyGeneratorSpectacleBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyGeneratorSpectacleBancParam" };

			if (SerializedActor["spl__EnemyGeneratorSpectacleBancParam"] != null)
			{
				spl__EnemyGeneratorSpectacleBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyGeneratorSpectacleBancParam"];
			}


			if (this.EnemyNamePhase0 != "")
			{
				spl__EnemyGeneratorSpectacleBancParam.AddNode("EnemyNamePhase0", this.EnemyNamePhase0);
			}

			if (this.EnemyNamePhase1 != "")
			{
				spl__EnemyGeneratorSpectacleBancParam.AddNode("EnemyNamePhase1", this.EnemyNamePhase1);
			}

			if (this.EnemyNamePhase2 != "")
			{
				spl__EnemyGeneratorSpectacleBancParam.AddNode("EnemyNamePhase2", this.EnemyNamePhase2);
			}

			if (SerializedActor["spl__EnemyGeneratorSpectacleBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyGeneratorSpectacleBancParam);
			}
		}
	}
}
