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
	public class Mu_spl__DynamicLoadingAreaBancParam
	{
		[ByamlMember]
		public int Id { get; set; }

		[ByamlMember]
		public string Layer { get; set; }

		public Mu_spl__DynamicLoadingAreaBancParam()
		{
			Id = 0;
			Layer = "";
		}

		public Mu_spl__DynamicLoadingAreaBancParam(Mu_spl__DynamicLoadingAreaBancParam other)
		{
			Id = other.Id;
			Layer = other.Layer;
		}

		public Mu_spl__DynamicLoadingAreaBancParam Clone()
		{
			return new Mu_spl__DynamicLoadingAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__DynamicLoadingAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__DynamicLoadingAreaBancParam" };

			if (SerializedActor["spl__DynamicLoadingAreaBancParam"] != null)
			{
				spl__DynamicLoadingAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__DynamicLoadingAreaBancParam"];
			}


			spl__DynamicLoadingAreaBancParam.AddNode("Id", this.Id);

			if (this.Layer != "")
			{
				spl__DynamicLoadingAreaBancParam.AddNode("Layer", this.Layer);
			}

			if (SerializedActor["spl__DynamicLoadingAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__DynamicLoadingAreaBancParam);
			}
		}
	}
}
