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
	public class Mu_spl__SpectacleManagerBancParam
	{
		public Mu_spl__SpectacleManagerBancParam()
		{
		}

		public Mu_spl__SpectacleManagerBancParam(Mu_spl__SpectacleManagerBancParam other)
		{
		}

		public Mu_spl__SpectacleManagerBancParam Clone()
		{
			return new Mu_spl__SpectacleManagerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SpectacleManagerBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleManagerBancParam" };

			if (SerializedActor["spl__SpectacleManagerBancParam"] != null)
			{
				spl__SpectacleManagerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleManagerBancParam"];
			}


			if (SerializedActor["spl__SpectacleManagerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SpectacleManagerBancParam);
			}
		}
	}
}
