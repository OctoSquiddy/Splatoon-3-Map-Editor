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
	public class Mu_spl__SwitchPaintBancParam
	{
		public Mu_spl__SwitchPaintBancParam()
		{
		}

		public Mu_spl__SwitchPaintBancParam(Mu_spl__SwitchPaintBancParam other)
		{
		}

		public Mu_spl__SwitchPaintBancParam Clone()
		{
			return new Mu_spl__SwitchPaintBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SwitchPaintBancParam = new BymlNode.DictionaryNode() { Name = "spl__SwitchPaintBancParam" };

			if (SerializedActor["spl__SwitchPaintBancParam"] != null)
			{
				spl__SwitchPaintBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SwitchPaintBancParam"];
			}


			if (SerializedActor["spl__SwitchPaintBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SwitchPaintBancParam);
			}
		}
	}
}
