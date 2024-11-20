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
	public class Mu_spl__PlazaJerryFixedBancParam
	{
		[ByamlMember]
		public string ActorName { get; set; }

		[ByamlMember]
		public string ASCommand { get; set; }

		[ByamlMember]
		public int VariationId { get; set; }

		public Mu_spl__PlazaJerryFixedBancParam()
		{
			ActorName = "";
			ASCommand = "";
			VariationId = 0;
		}

		public Mu_spl__PlazaJerryFixedBancParam(Mu_spl__PlazaJerryFixedBancParam other)
		{
			ActorName = other.ActorName;
			ASCommand = other.ASCommand;
			VariationId = other.VariationId;
		}

		public Mu_spl__PlazaJerryFixedBancParam Clone()
		{
			return new Mu_spl__PlazaJerryFixedBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PlazaJerryFixedBancParam = new BymlNode.DictionaryNode() { Name = "spl__PlazaJerryFixedBancParam" };

			if (SerializedActor["spl__PlazaJerryFixedBancParam"] != null)
			{
				spl__PlazaJerryFixedBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PlazaJerryFixedBancParam"];
			}


			if (this.ActorName != "")
			{
				spl__PlazaJerryFixedBancParam.AddNode("ActorName", this.ActorName);
			}

			if (this.ASCommand != "")
			{
				spl__PlazaJerryFixedBancParam.AddNode("ASCommand", this.ASCommand);
			}

			spl__PlazaJerryFixedBancParam.AddNode("VariationId", this.VariationId);

			if (SerializedActor["spl__PlazaJerryFixedBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PlazaJerryFixedBancParam);
			}
		}
	}
}
