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
	public class Mu_spl__LocatorLockerBancParam
	{
		[ByamlMember]
		public int Index { get; set; }

		public Mu_spl__LocatorLockerBancParam()
		{
			Index = 0;
		}

		public Mu_spl__LocatorLockerBancParam(Mu_spl__LocatorLockerBancParam other)
		{
			Index = other.Index;
		}

		public Mu_spl__LocatorLockerBancParam Clone()
		{
			return new Mu_spl__LocatorLockerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorLockerBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorLockerBancParam" };

			if (SerializedActor["spl__LocatorLockerBancParam"] != null)
			{
				spl__LocatorLockerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorLockerBancParam"];
			}


			spl__LocatorLockerBancParam.AddNode("Index", this.Index);

			if (SerializedActor["spl__LocatorLockerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorLockerBancParam);
			}
		}
	}
}
