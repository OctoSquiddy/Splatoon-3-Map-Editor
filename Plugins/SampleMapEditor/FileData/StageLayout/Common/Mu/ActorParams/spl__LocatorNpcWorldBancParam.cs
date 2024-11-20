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
	public class Mu_spl__LocatorNpcWorldBancParam
	{
		[ByamlMember]
		public string ConversationKey { get; set; }

		[ByamlMember]
		public bool IsActivateOnlyInBeingPerformer { get; set; }

		public Mu_spl__LocatorNpcWorldBancParam()
		{
			ConversationKey = "";
			IsActivateOnlyInBeingPerformer = false;
		}

		public Mu_spl__LocatorNpcWorldBancParam(Mu_spl__LocatorNpcWorldBancParam other)
		{
			ConversationKey = other.ConversationKey;
			IsActivateOnlyInBeingPerformer = other.IsActivateOnlyInBeingPerformer;
		}

		public Mu_spl__LocatorNpcWorldBancParam Clone()
		{
			return new Mu_spl__LocatorNpcWorldBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorNpcWorldBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorNpcWorldBancParam" };

			if (SerializedActor["spl__LocatorNpcWorldBancParam"] != null)
			{
				spl__LocatorNpcWorldBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorNpcWorldBancParam"];
			}


			if (this.ConversationKey != "")
			{
				spl__LocatorNpcWorldBancParam.AddNode("ConversationKey", this.ConversationKey);
			}

			if (this.IsActivateOnlyInBeingPerformer)
			{
				spl__LocatorNpcWorldBancParam.AddNode("IsActivateOnlyInBeingPerformer", this.IsActivateOnlyInBeingPerformer);
			}

			if (SerializedActor["spl__LocatorNpcWorldBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorNpcWorldBancParam);
			}
		}
	}
}
