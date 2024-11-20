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
	public class Mu_spl__KingKeyBancParamSdodr
	{
		[ByamlMember]
		public string BossType { get; set; }

		public Mu_spl__KingKeyBancParamSdodr()
		{
			BossType = "";
		}

		public Mu_spl__KingKeyBancParamSdodr(Mu_spl__KingKeyBancParamSdodr other)
		{
			BossType = other.BossType;
		}

		public Mu_spl__KingKeyBancParamSdodr Clone()
		{
			return new Mu_spl__KingKeyBancParamSdodr(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__KingKeyBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__KingKeyBancParamSdodr" };

			if (SerializedActor["spl__KingKeyBancParamSdodr"] != null)
			{
				spl__KingKeyBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__KingKeyBancParamSdodr"];
			}


			if (this.BossType != "")
			{
				spl__KingKeyBancParamSdodr.AddNode("BossType", this.BossType);
			}

			if (SerializedActor["spl__KingKeyBancParamSdodr"] == null)
			{
				SerializedActor.Nodes.Add(spl__KingKeyBancParamSdodr);
			}
		}
	}
}
