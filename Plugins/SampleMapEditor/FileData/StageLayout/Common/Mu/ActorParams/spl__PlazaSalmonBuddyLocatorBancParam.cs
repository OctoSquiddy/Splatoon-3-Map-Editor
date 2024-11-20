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
	public class Mu_spl__PlazaSalmonBuddyLocatorBancParam
	{
		[ByamlMember]
		public string LocatorName { get; set; }

		public Mu_spl__PlazaSalmonBuddyLocatorBancParam()
		{
			LocatorName = "";
		}

		public Mu_spl__PlazaSalmonBuddyLocatorBancParam(Mu_spl__PlazaSalmonBuddyLocatorBancParam other)
		{
			LocatorName = other.LocatorName;
		}

		public Mu_spl__PlazaSalmonBuddyLocatorBancParam Clone()
		{
			return new Mu_spl__PlazaSalmonBuddyLocatorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PlazaSalmonBuddyLocatorBancParam = new BymlNode.DictionaryNode() { Name = "spl__PlazaSalmonBuddyLocatorBancParam" };

			if (SerializedActor["spl__PlazaSalmonBuddyLocatorBancParam"] != null)
			{
				spl__PlazaSalmonBuddyLocatorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PlazaSalmonBuddyLocatorBancParam"];
			}


			if (this.LocatorName != "")
			{
				spl__PlazaSalmonBuddyLocatorBancParam.AddNode("LocatorName", this.LocatorName);
			}

			if (SerializedActor["spl__PlazaSalmonBuddyLocatorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PlazaSalmonBuddyLocatorBancParam);
			}
		}
	}
}
