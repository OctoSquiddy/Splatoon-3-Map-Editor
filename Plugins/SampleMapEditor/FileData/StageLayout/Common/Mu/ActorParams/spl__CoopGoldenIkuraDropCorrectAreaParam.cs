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
	public class Mu_spl__CoopGoldenIkuraDropCorrectAreaParam
	{
		public Mu_spl__CoopGoldenIkuraDropCorrectAreaParam()
		{
		}

		public Mu_spl__CoopGoldenIkuraDropCorrectAreaParam(Mu_spl__CoopGoldenIkuraDropCorrectAreaParam other)
		{
		}

		public Mu_spl__CoopGoldenIkuraDropCorrectAreaParam Clone()
		{
			return new Mu_spl__CoopGoldenIkuraDropCorrectAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopGoldenIkuraDropCorrectAreaParam = new BymlNode.DictionaryNode() { Name = "spl__CoopGoldenIkuraDropCorrectAreaParam" };

			if (SerializedActor["spl__CoopGoldenIkuraDropCorrectAreaParam"] != null)
			{
				spl__CoopGoldenIkuraDropCorrectAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopGoldenIkuraDropCorrectAreaParam"];
			}


			if (SerializedActor["spl__CoopGoldenIkuraDropCorrectAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopGoldenIkuraDropCorrectAreaParam);
			}
		}
	}
}
