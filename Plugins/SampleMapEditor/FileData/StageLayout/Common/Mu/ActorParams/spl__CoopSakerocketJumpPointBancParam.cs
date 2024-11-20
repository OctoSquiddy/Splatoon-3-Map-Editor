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
	public class Mu_spl__CoopSakerocketJumpPointBancParam
	{
		public Mu_spl__CoopSakerocketJumpPointBancParam()
		{
		}

		public Mu_spl__CoopSakerocketJumpPointBancParam(Mu_spl__CoopSakerocketJumpPointBancParam other)
		{
		}

		public Mu_spl__CoopSakerocketJumpPointBancParam Clone()
		{
			return new Mu_spl__CoopSakerocketJumpPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopSakerocketJumpPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSakerocketJumpPointBancParam" };

			if (SerializedActor["spl__CoopSakerocketJumpPointBancParam"] != null)
			{
				spl__CoopSakerocketJumpPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSakerocketJumpPointBancParam"];
			}


			if (SerializedActor["spl__CoopSakerocketJumpPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopSakerocketJumpPointBancParam);
			}
		}
	}
}
