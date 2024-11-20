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
	public class Mu_spl__HologramProjectorBancParam
	{
		public Mu_spl__HologramProjectorBancParam()
		{
		}

		public Mu_spl__HologramProjectorBancParam(Mu_spl__HologramProjectorBancParam other)
		{
		}

		public Mu_spl__HologramProjectorBancParam Clone()
		{
			return new Mu_spl__HologramProjectorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__HologramProjectorBancParam = new BymlNode.DictionaryNode() { Name = "spl__HologramProjectorBancParam" };

			if (SerializedActor["spl__HologramProjectorBancParam"] != null)
			{
				spl__HologramProjectorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__HologramProjectorBancParam"];
			}


			if (SerializedActor["spl__HologramProjectorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__HologramProjectorBancParam);
			}
		}
	}
}
