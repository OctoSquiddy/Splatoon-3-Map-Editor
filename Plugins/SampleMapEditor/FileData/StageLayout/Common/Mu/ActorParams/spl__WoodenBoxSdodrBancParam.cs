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
	public class Mu_spl__WoodenBoxSdodrBancParam
	{
		public Mu_spl__WoodenBoxSdodrBancParam()
		{
		}

		public Mu_spl__WoodenBoxSdodrBancParam(Mu_spl__WoodenBoxSdodrBancParam other)
		{
		}

		public Mu_spl__WoodenBoxSdodrBancParam Clone()
		{
			return new Mu_spl__WoodenBoxSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__WoodenBoxSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__WoodenBoxSdodrBancParam" };

			if (SerializedActor["spl__WoodenBoxSdodrBancParam"] != null)
			{
				spl__WoodenBoxSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__WoodenBoxSdodrBancParam"];
			}


			if (SerializedActor["spl__WoodenBoxSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__WoodenBoxSdodrBancParam);
			}
		}
	}
}
