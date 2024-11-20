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
	public class Mu_spl__SpongeBancParam
	{
		[ByamlMember]
		public string Type { get; set; }

		public Mu_spl__SpongeBancParam()
		{
			Type = "";
		}

		public Mu_spl__SpongeBancParam(Mu_spl__SpongeBancParam other)
		{
			Type = other.Type;
		}

		public Mu_spl__SpongeBancParam Clone()
		{
			return new Mu_spl__SpongeBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpongeBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpongeBancParam" };

			if (SerializedActor["spl__SpongeBancParam"] != null)
			{
				spl__SpongeBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpongeBancParam"];
			}


			if (this.Type != "")
			{
				spl__SpongeBancParam.AddNode("Type", this.Type);
			}

			if (SerializedActor["spl__SpongeBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpongeBancParam);
			}
		}
	}
}
