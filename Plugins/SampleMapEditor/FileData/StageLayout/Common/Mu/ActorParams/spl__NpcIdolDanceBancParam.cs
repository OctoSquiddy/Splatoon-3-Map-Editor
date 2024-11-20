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
	public class Mu_spl__NpcIdolDanceBancParam
	{
		[ByamlMember]
		public string BoneName { get; set; }

		public Mu_spl__NpcIdolDanceBancParam()
		{
			BoneName = "";
		}

		public Mu_spl__NpcIdolDanceBancParam(Mu_spl__NpcIdolDanceBancParam other)
		{
			BoneName = other.BoneName;
		}

		public Mu_spl__NpcIdolDanceBancParam Clone()
		{
			return new Mu_spl__NpcIdolDanceBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__NpcIdolDanceBancParam = new BymlNode.DictionaryNode() { Name = "spl__NpcIdolDanceBancParam" };

			if (SerializedActor["spl__NpcIdolDanceBancParam"] != null)
			{
				spl__NpcIdolDanceBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__NpcIdolDanceBancParam"];
			}


			if (this.BoneName != "")
			{
				spl__NpcIdolDanceBancParam.AddNode("BoneName", this.BoneName);
			}

			if (SerializedActor["spl__NpcIdolDanceBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__NpcIdolDanceBancParam);
			}
		}
	}
}
