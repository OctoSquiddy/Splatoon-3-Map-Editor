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
	public class Mu_spl__WallaObjBancParam
	{
		public Mu_spl__WallaObjBancParam()
		{
		}

		public Mu_spl__WallaObjBancParam(Mu_spl__WallaObjBancParam other)
		{
		}

		public Mu_spl__WallaObjBancParam Clone()
		{
			return new Mu_spl__WallaObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__WallaObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__WallaObjBancParam" };

			if (SerializedActor["spl__WallaObjBancParam"] != null)
			{
				spl__WallaObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__WallaObjBancParam"];
			}


			if (SerializedActor["spl__WallaObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__WallaObjBancParam);
			}
		}
	}
}
