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
	public class Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam
	{
		public Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam()
		{
		}

		public Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam(Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam other)
		{
		}

		public Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam Clone()
		{
			return new Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MissionSalmonBuddyLeadPlayerAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__MissionSalmonBuddyLeadPlayerAreaBancParam" };

			if (SerializedActor["spl__MissionSalmonBuddyLeadPlayerAreaBancParam"] != null)
			{
				spl__MissionSalmonBuddyLeadPlayerAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MissionSalmonBuddyLeadPlayerAreaBancParam"];
			}


			if (SerializedActor["spl__MissionSalmonBuddyLeadPlayerAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MissionSalmonBuddyLeadPlayerAreaBancParam);
			}
		}
	}
}
