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
	public class Mu_spl__CoopGoldenIkuraBoxDropPointBancParam
	{
		public Mu_spl__CoopGoldenIkuraBoxDropPointBancParam()
		{
		}

		public Mu_spl__CoopGoldenIkuraBoxDropPointBancParam(Mu_spl__CoopGoldenIkuraBoxDropPointBancParam other)
		{
		}

		public Mu_spl__CoopGoldenIkuraBoxDropPointBancParam Clone()
		{
			return new Mu_spl__CoopGoldenIkuraBoxDropPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopGoldenIkuraBoxDropPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopGoldenIkuraBoxDropPointBancParam" };

			if (SerializedActor["spl__CoopGoldenIkuraBoxDropPointBancParam"] != null)
			{
				spl__CoopGoldenIkuraBoxDropPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopGoldenIkuraBoxDropPointBancParam"];
			}


			if (SerializedActor["spl__CoopGoldenIkuraBoxDropPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopGoldenIkuraBoxDropPointBancParam);
			}
		}
	}
}
