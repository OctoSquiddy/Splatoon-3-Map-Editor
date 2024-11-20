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
	public class Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam
	{
		public Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam()
		{
		}

		public Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam(Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam other)
		{
		}

		public Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam Clone()
		{
			return new Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopSakeFlyBagManArrivalPointForLiftParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSakeFlyBagManArrivalPointForLiftParam" };

			if (SerializedActor["spl__CoopSakeFlyBagManArrivalPointForLiftParam"] != null)
			{
				spl__CoopSakeFlyBagManArrivalPointForLiftParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSakeFlyBagManArrivalPointForLiftParam"];
			}


			if (SerializedActor["spl__CoopSakeFlyBagManArrivalPointForLiftParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopSakeFlyBagManArrivalPointForLiftParam);
			}
		}
	}
}
