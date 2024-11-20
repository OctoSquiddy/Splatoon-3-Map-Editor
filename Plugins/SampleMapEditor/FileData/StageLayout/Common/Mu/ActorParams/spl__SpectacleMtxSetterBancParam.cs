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
	public class Mu_spl__SpectacleMtxSetterBancParam
	{
		public Mu_spl__SpectacleMtxSetterBancParam()
		{
		}

		public Mu_spl__SpectacleMtxSetterBancParam(Mu_spl__SpectacleMtxSetterBancParam other)
		{
		}

		public Mu_spl__SpectacleMtxSetterBancParam Clone()
		{
			return new Mu_spl__SpectacleMtxSetterBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpectacleMtxSetterBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleMtxSetterBancParam" };

			if (SerializedActor["spl__SpectacleMtxSetterBancParam"] != null)
			{
				spl__SpectacleMtxSetterBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleMtxSetterBancParam"];
			}


			if (SerializedActor["spl__SpectacleMtxSetterBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpectacleMtxSetterBancParam);
			}
		}
	}
}
