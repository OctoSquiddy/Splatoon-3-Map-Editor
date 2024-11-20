using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplMissionSalmonBuddyLeadPlayerArea : MuObj
	{
		[ByamlMember("spl__MissionSalmonBuddyLeadPlayerAreaBancParam")]
		public Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam spl__MissionSalmonBuddyLeadPlayerAreaBancParam { get; set; }

		public SplMissionSalmonBuddyLeadPlayerArea() : base()
		{
			spl__MissionSalmonBuddyLeadPlayerAreaBancParam = new Mu_spl__MissionSalmonBuddyLeadPlayerAreaBancParam();

			Links = new List<Link>();
		}

		public SplMissionSalmonBuddyLeadPlayerArea(SplMissionSalmonBuddyLeadPlayerArea other) : base(other)
		{
			spl__MissionSalmonBuddyLeadPlayerAreaBancParam = other.spl__MissionSalmonBuddyLeadPlayerAreaBancParam.Clone();
		}

		public override SplMissionSalmonBuddyLeadPlayerArea Clone()
		{
			return new SplMissionSalmonBuddyLeadPlayerArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MissionSalmonBuddyLeadPlayerAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
