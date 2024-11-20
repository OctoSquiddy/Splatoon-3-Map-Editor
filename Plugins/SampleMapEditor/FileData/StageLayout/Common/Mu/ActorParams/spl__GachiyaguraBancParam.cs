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
	public class Mu_spl__GachiyaguraBancParam
	{
		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__GachiyaguraBancParam()
		{
			ToRailPoint = 0;
		}

		public Mu_spl__GachiyaguraBancParam(Mu_spl__GachiyaguraBancParam other)
		{
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__GachiyaguraBancParam Clone()
		{
			return new Mu_spl__GachiyaguraBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GachiyaguraBancParam = new BymlNode.DictionaryNode() { Name = "spl__GachiyaguraBancParam" };

			if (SerializedActor["spl__GachiyaguraBancParam"] != null)
			{
				spl__GachiyaguraBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GachiyaguraBancParam"];
			}


			spl__GachiyaguraBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__GachiyaguraBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GachiyaguraBancParam);
			}
		}
	}
}
