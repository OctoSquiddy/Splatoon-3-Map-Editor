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
	public class Mu_spl__RestartPosUpdateAreaBancParam
	{
		public Mu_spl__RestartPosUpdateAreaBancParam()
		{
		}

		public Mu_spl__RestartPosUpdateAreaBancParam(Mu_spl__RestartPosUpdateAreaBancParam other)
		{
		}

		public Mu_spl__RestartPosUpdateAreaBancParam Clone()
		{
			return new Mu_spl__RestartPosUpdateAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RestartPosUpdateAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__RestartPosUpdateAreaBancParam" };

			if (SerializedActor["spl__RestartPosUpdateAreaBancParam"] != null)
			{
				spl__RestartPosUpdateAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RestartPosUpdateAreaBancParam"];
			}


			if (SerializedActor["spl__RestartPosUpdateAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RestartPosUpdateAreaBancParam);
			}
		}
	}
}
