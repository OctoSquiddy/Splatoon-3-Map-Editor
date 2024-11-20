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
	public class Mu_spl__MissionGatewayBancParam
	{
		[ByamlMember]
		public string ChangeSceneName { get; set; }

		[ByamlMember]
		public string DevText { get; set; }

		[ByamlMember]
		public bool SendSignalOnOpen { get; set; }

		public Mu_spl__MissionGatewayBancParam()
		{
			ChangeSceneName = "";
			DevText = "";
			SendSignalOnOpen = false;
		}

		public Mu_spl__MissionGatewayBancParam(Mu_spl__MissionGatewayBancParam other)
		{
			ChangeSceneName = other.ChangeSceneName;
			DevText = other.DevText;
			SendSignalOnOpen = other.SendSignalOnOpen;
		}

		public Mu_spl__MissionGatewayBancParam Clone()
		{
			return new Mu_spl__MissionGatewayBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MissionGatewayBancParam = new BymlNode.DictionaryNode() { Name = "spl__MissionGatewayBancParam" };

			if (SerializedActor["spl__MissionGatewayBancParam"] != null)
			{
				spl__MissionGatewayBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MissionGatewayBancParam"];
			}


			if (this.ChangeSceneName != "")
			{
				spl__MissionGatewayBancParam.AddNode("ChangeSceneName", this.ChangeSceneName);
			}

			if (this.DevText != "")
			{
				spl__MissionGatewayBancParam.AddNode("DevText", this.DevText);
			}

			if (this.SendSignalOnOpen)
			{
				spl__MissionGatewayBancParam.AddNode("SendSignalOnOpen", this.SendSignalOnOpen);
			}

			if (SerializedActor["spl__MissionGatewayBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MissionGatewayBancParam);
			}
		}
	}
}
