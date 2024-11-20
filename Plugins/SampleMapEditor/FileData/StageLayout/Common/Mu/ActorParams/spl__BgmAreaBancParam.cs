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
	public class Mu_spl__BgmAreaBancParam
	{
		[ByamlMember]
		public string Key { get; set; }

		[ByamlMember]
		public float Margin { get; set; }

		public Mu_spl__BgmAreaBancParam()
		{
			Key = "";
			Margin = 0.0f;
		}

		public Mu_spl__BgmAreaBancParam(Mu_spl__BgmAreaBancParam other)
		{
			Key = other.Key;
			Margin = other.Margin;
		}

		public Mu_spl__BgmAreaBancParam Clone()
		{
			return new Mu_spl__BgmAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__BgmAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__BgmAreaBancParam" };

			if (SerializedActor["spl__BgmAreaBancParam"] != null)
			{
				spl__BgmAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__BgmAreaBancParam"];
			}


			if (this.Key != "")
			{
				spl__BgmAreaBancParam.AddNode("Key", this.Key);
			}

			spl__BgmAreaBancParam.AddNode("Margin", this.Margin);

			if (SerializedActor["spl__BgmAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__BgmAreaBancParam);
			}
		}
	}
}
