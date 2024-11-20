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
	public class Mu_spl__GachihokoCheckPointBancParam
	{
		public Mu_spl__GachihokoCheckPointBancParam()
		{
		}

		public Mu_spl__GachihokoCheckPointBancParam(Mu_spl__GachihokoCheckPointBancParam other)
		{
		}

		public Mu_spl__GachihokoCheckPointBancParam Clone()
		{
			return new Mu_spl__GachihokoCheckPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GachihokoCheckPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__GachihokoCheckPointBancParam" };

			if (SerializedActor["spl__GachihokoCheckPointBancParam"] != null)
			{
				spl__GachihokoCheckPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GachihokoCheckPointBancParam"];
			}


			if (SerializedActor["spl__GachihokoCheckPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GachihokoCheckPointBancParam);
			}
		}
	}
}
