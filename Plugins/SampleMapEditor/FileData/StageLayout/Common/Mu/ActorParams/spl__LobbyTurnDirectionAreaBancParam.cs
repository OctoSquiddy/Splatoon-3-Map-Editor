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
	public class Mu_spl__LobbyTurnDirectionAreaBancParam
	{
		[ByamlMember]
		public ByamlVector3F SafeDir { get; set; }

		public Mu_spl__LobbyTurnDirectionAreaBancParam()
		{
			SafeDir = new ByamlVector3F();
		}

		public Mu_spl__LobbyTurnDirectionAreaBancParam(Mu_spl__LobbyTurnDirectionAreaBancParam other)
		{
			SafeDir = new ByamlVector3F(other.SafeDir.X, other.SafeDir.Y, other.SafeDir.Z);
		}

		public Mu_spl__LobbyTurnDirectionAreaBancParam Clone()
		{
			return new Mu_spl__LobbyTurnDirectionAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LobbyTurnDirectionAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__LobbyTurnDirectionAreaBancParam" };

			if (SerializedActor["spl__LobbyTurnDirectionAreaBancParam"] != null)
			{
				spl__LobbyTurnDirectionAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LobbyTurnDirectionAreaBancParam"];
			}

			BymlNode.DictionaryNode SafeDir = new BymlNode.DictionaryNode() { Name = "SafeDir" };

			if (this.SafeDir.Z != 0.0f || this.SafeDir.Y != 0.0f || this.SafeDir.X != 0.0f)
			{
				SafeDir.AddNode("X", this.SafeDir.X);
				SafeDir.AddNode("Y", this.SafeDir.Y);
				SafeDir.AddNode("Z", this.SafeDir.Z);
				spl__LobbyTurnDirectionAreaBancParam.Nodes.Add(SafeDir);
			}

			if (SerializedActor["spl__LobbyTurnDirectionAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LobbyTurnDirectionAreaBancParam);
			}
		}
	}
}
