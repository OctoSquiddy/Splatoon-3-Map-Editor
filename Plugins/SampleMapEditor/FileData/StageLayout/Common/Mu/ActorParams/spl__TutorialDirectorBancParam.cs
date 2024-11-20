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
	public class Mu_spl__TutorialDirectorBancParam
	{
		[ByamlMember]
		public ulong ToBuddyRailAim { get; set; }

		[ByamlMember]
		public ulong ToBuddyRailChase { get; set; }

		[ByamlMember]
		public ulong ToBuddyRailTreasure { get; set; }

		public Mu_spl__TutorialDirectorBancParam()
		{
			ToBuddyRailAim = 0;
			ToBuddyRailChase = 0;
			ToBuddyRailTreasure = 0;
		}

		public Mu_spl__TutorialDirectorBancParam(Mu_spl__TutorialDirectorBancParam other)
		{
			ToBuddyRailAim = other.ToBuddyRailAim;
			ToBuddyRailChase = other.ToBuddyRailChase;
			ToBuddyRailTreasure = other.ToBuddyRailTreasure;
		}

		public Mu_spl__TutorialDirectorBancParam Clone()
		{
			return new Mu_spl__TutorialDirectorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__TutorialDirectorBancParam = new BymlNode.DictionaryNode() { Name = "spl__TutorialDirectorBancParam" };

			if (SerializedActor["spl__TutorialDirectorBancParam"] != null)
			{
				spl__TutorialDirectorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__TutorialDirectorBancParam"];
			}


			spl__TutorialDirectorBancParam.AddNode("ToBuddyRailAim", this.ToBuddyRailAim);

			spl__TutorialDirectorBancParam.AddNode("ToBuddyRailChase", this.ToBuddyRailChase);

			spl__TutorialDirectorBancParam.AddNode("ToBuddyRailTreasure", this.ToBuddyRailTreasure);

			if (SerializedActor["spl__TutorialDirectorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__TutorialDirectorBancParam);
			}
		}
	}
}
