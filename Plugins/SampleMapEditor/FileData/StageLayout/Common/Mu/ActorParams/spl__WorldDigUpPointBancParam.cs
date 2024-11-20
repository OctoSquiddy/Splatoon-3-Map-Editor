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
	public class Mu_spl__WorldDigUpPointBancParam
	{
		[ByamlMember]
		public string Type { get; set; }

		public Mu_spl__WorldDigUpPointBancParam()
		{
			Type = "";
		}

		public Mu_spl__WorldDigUpPointBancParam(Mu_spl__WorldDigUpPointBancParam other)
		{
			Type = other.Type;
		}

		public Mu_spl__WorldDigUpPointBancParam Clone()
		{
			return new Mu_spl__WorldDigUpPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__WorldDigUpPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__WorldDigUpPointBancParam" };

			if (SerializedActor["spl__WorldDigUpPointBancParam"] != null)
			{
				spl__WorldDigUpPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__WorldDigUpPointBancParam"];
			}


			if (this.Type != "")
			{
				spl__WorldDigUpPointBancParam.AddNode("Type", this.Type);
			}

			if (SerializedActor["spl__WorldDigUpPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__WorldDigUpPointBancParam);
			}
		}
	}
}
