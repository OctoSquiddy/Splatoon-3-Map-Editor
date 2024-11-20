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
	public class Mu_spl__LocatorNpcLobbyBancParam
	{
		[ByamlMember]
		public string ASCommand { get; set; }

		[ByamlMember]
		public int Pattern { get; set; }

		public Mu_spl__LocatorNpcLobbyBancParam()
		{
			ASCommand = "";
			Pattern = 0;
		}

		public Mu_spl__LocatorNpcLobbyBancParam(Mu_spl__LocatorNpcLobbyBancParam other)
		{
			ASCommand = other.ASCommand;
			Pattern = other.Pattern;
		}

		public Mu_spl__LocatorNpcLobbyBancParam Clone()
		{
			return new Mu_spl__LocatorNpcLobbyBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorNpcLobbyBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorNpcLobbyBancParam" };

			if (SerializedActor["spl__LocatorNpcLobbyBancParam"] != null)
			{
				spl__LocatorNpcLobbyBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorNpcLobbyBancParam"];
			}


			if (this.ASCommand != "")
			{
				spl__LocatorNpcLobbyBancParam.AddNode("ASCommand", this.ASCommand);
			}

			spl__LocatorNpcLobbyBancParam.AddNode("Pattern", this.Pattern);

			if (SerializedActor["spl__LocatorNpcLobbyBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorNpcLobbyBancParam);
			}
		}
	}
}
