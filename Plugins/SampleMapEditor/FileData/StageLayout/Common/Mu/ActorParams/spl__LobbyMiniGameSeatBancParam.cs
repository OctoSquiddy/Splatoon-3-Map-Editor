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
	public class Mu_spl__LobbyMiniGameSeatBancParam
	{
		public Mu_spl__LobbyMiniGameSeatBancParam()
		{
		}

		public Mu_spl__LobbyMiniGameSeatBancParam(Mu_spl__LobbyMiniGameSeatBancParam other)
		{
		}

		public Mu_spl__LobbyMiniGameSeatBancParam Clone()
		{
			return new Mu_spl__LobbyMiniGameSeatBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LobbyMiniGameSeatBancParam = new BymlNode.DictionaryNode() { Name = "spl__LobbyMiniGameSeatBancParam" };

			if (SerializedActor["spl__LobbyMiniGameSeatBancParam"] != null)
			{
				spl__LobbyMiniGameSeatBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LobbyMiniGameSeatBancParam"];
			}


			if (SerializedActor["spl__LobbyMiniGameSeatBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LobbyMiniGameSeatBancParam);
			}
		}
	}
}
