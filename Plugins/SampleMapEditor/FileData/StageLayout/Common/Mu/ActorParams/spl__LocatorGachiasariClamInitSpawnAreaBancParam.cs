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
	public class Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam
	{
		[ByamlMember]
		public int Weight { get; set; }

		public Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam()
		{
			Weight = 0;
		}

		public Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam(Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam other)
		{
			Weight = other.Weight;
		}

		public Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam Clone()
		{
			return new Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorGachiasariClamInitSpawnAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorGachiasariClamInitSpawnAreaBancParam" };

			if (SerializedActor["spl__LocatorGachiasariClamInitSpawnAreaBancParam"] != null)
			{
				spl__LocatorGachiasariClamInitSpawnAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorGachiasariClamInitSpawnAreaBancParam"];
			}


			spl__LocatorGachiasariClamInitSpawnAreaBancParam.AddNode("Weight", this.Weight);

			if (SerializedActor["spl__LocatorGachiasariClamInitSpawnAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorGachiasariClamInitSpawnAreaBancParam);
			}
		}
	}
}
