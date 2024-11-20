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
	public class Mu_spl__RivalAppearSequencerBancParamSdodr
	{
		[ByamlMember]
		public int TotalSpawnNumWave0 { get; set; }

		[ByamlMember]
		public int TotalSpawnNumWave1 { get; set; }

		[ByamlMember]
		public int TotalSpawnNumWave2 { get; set; }

		public Mu_spl__RivalAppearSequencerBancParamSdodr()
		{
			TotalSpawnNumWave0 = 0;
			TotalSpawnNumWave1 = 0;
			TotalSpawnNumWave2 = 0;
		}

		public Mu_spl__RivalAppearSequencerBancParamSdodr(Mu_spl__RivalAppearSequencerBancParamSdodr other)
		{
			TotalSpawnNumWave0 = other.TotalSpawnNumWave0;
			TotalSpawnNumWave1 = other.TotalSpawnNumWave1;
			TotalSpawnNumWave2 = other.TotalSpawnNumWave2;
		}

		public Mu_spl__RivalAppearSequencerBancParamSdodr Clone()
		{
			return new Mu_spl__RivalAppearSequencerBancParamSdodr(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RivalAppearSequencerBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__RivalAppearSequencerBancParamSdodr" };

			if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] != null)
			{
				spl__RivalAppearSequencerBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__RivalAppearSequencerBancParamSdodr"];
			}


			spl__RivalAppearSequencerBancParamSdodr.AddNode("TotalSpawnNumWave0", this.TotalSpawnNumWave0);

			spl__RivalAppearSequencerBancParamSdodr.AddNode("TotalSpawnNumWave1", this.TotalSpawnNumWave1);

			spl__RivalAppearSequencerBancParamSdodr.AddNode("TotalSpawnNumWave2", this.TotalSpawnNumWave2);

			if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] == null)
			{
				SerializedActor.Nodes.Add(spl__RivalAppearSequencerBancParamSdodr);
			}
		}
	}
}
