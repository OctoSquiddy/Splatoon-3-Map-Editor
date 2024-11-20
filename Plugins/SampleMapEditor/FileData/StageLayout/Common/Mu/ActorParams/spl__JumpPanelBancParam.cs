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
	public class Mu_spl__JumpPanelBancParam
	{
		public Mu_spl__JumpPanelBancParam()
		{
		}

		public Mu_spl__JumpPanelBancParam(Mu_spl__JumpPanelBancParam other)
		{
		}

		public Mu_spl__JumpPanelBancParam Clone()
		{
			return new Mu_spl__JumpPanelBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__JumpPanelBancParam = new BymlNode.DictionaryNode() { Name = "spl__JumpPanelBancParam" };

			if (SerializedActor["spl__JumpPanelBancParam"] != null)
			{
				spl__JumpPanelBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__JumpPanelBancParam"];
			}


			if (SerializedActor["spl__JumpPanelBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__JumpPanelBancParam);
			}
		}
	}
}
