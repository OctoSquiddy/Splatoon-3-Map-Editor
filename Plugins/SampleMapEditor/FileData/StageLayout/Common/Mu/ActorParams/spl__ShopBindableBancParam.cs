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
	public class Mu_spl__ShopBindableBancParam
	{
		public Mu_spl__ShopBindableBancParam()
		{
		}

		public Mu_spl__ShopBindableBancParam(Mu_spl__ShopBindableBancParam other)
		{
		}

		public Mu_spl__ShopBindableBancParam Clone()
		{
			return new Mu_spl__ShopBindableBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ShopBindableBancParam = new BymlNode.DictionaryNode() { Name = "spl__ShopBindableBancParam" };

			if (SerializedActor["spl__ShopBindableBancParam"] != null)
			{
				spl__ShopBindableBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ShopBindableBancParam"];
			}


			if (SerializedActor["spl__ShopBindableBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ShopBindableBancParam);
			}
		}
	}
}
