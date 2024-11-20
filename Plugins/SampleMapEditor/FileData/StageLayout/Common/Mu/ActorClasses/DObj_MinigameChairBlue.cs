using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DObj_MinigameChairBlue : MuObj
	{
		[ByamlMember("spl__LobbyMiniGameSeatBancParam")]
		public Mu_spl__LobbyMiniGameSeatBancParam spl__LobbyMiniGameSeatBancParam { get; set; }

		public DObj_MinigameChairBlue() : base()
		{
			spl__LobbyMiniGameSeatBancParam = new Mu_spl__LobbyMiniGameSeatBancParam();

			Links = new List<Link>();
		}

		public DObj_MinigameChairBlue(DObj_MinigameChairBlue other) : base(other)
		{
			spl__LobbyMiniGameSeatBancParam = other.spl__LobbyMiniGameSeatBancParam.Clone();
		}

		public override DObj_MinigameChairBlue Clone()
		{
			return new DObj_MinigameChairBlue(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LobbyMiniGameSeatBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
