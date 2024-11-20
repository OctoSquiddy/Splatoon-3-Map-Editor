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
	public class Mu_spl__DummyTextAreaBancParam
	{
		[ByamlMember]
		public string DummyText { get; set; }

		public Mu_spl__DummyTextAreaBancParam()
		{
			DummyText = "";
		}

		public Mu_spl__DummyTextAreaBancParam(Mu_spl__DummyTextAreaBancParam other)
		{
			DummyText = other.DummyText;
		}

		public Mu_spl__DummyTextAreaBancParam Clone()
		{
			return new Mu_spl__DummyTextAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__DummyTextAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__DummyTextAreaBancParam" };

			if (SerializedActor["spl__DummyTextAreaBancParam"] != null)
			{
				spl__DummyTextAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__DummyTextAreaBancParam"];
			}


			if (this.DummyText != "")
			{
				spl__DummyTextAreaBancParam.AddNode("DummyText", this.DummyText);
			}

			if (SerializedActor["spl__DummyTextAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__DummyTextAreaBancParam);
			}
		}
	}
}
