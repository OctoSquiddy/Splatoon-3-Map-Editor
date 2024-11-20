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
	public class Mu_spl__PlazaConditionalActorBancParam
	{
		[ByamlMember]
		public int VariationId { get; set; }

		public Mu_spl__PlazaConditionalActorBancParam()
		{
			VariationId = 0;
		}

		public Mu_spl__PlazaConditionalActorBancParam(Mu_spl__PlazaConditionalActorBancParam other)
		{
			VariationId = other.VariationId;
		}

		public Mu_spl__PlazaConditionalActorBancParam Clone()
		{
			return new Mu_spl__PlazaConditionalActorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PlazaConditionalActorBancParam = new BymlNode.DictionaryNode() { Name = "spl__PlazaConditionalActorBancParam" };

			if (SerializedActor["spl__PlazaConditionalActorBancParam"] != null)
			{
				spl__PlazaConditionalActorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PlazaConditionalActorBancParam"];
			}


			spl__PlazaConditionalActorBancParam.AddNode("VariationId", this.VariationId);

			if (SerializedActor["spl__PlazaConditionalActorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PlazaConditionalActorBancParam);
			}
		}
	}
}
