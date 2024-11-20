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
	public class Mu_spl__SdodrRestingHimeLocatorBancParam
	{
		[ByamlMember]
		public int AnimIndex { get; set; }

		public Mu_spl__SdodrRestingHimeLocatorBancParam()
		{
			AnimIndex = 0;
		}

		public Mu_spl__SdodrRestingHimeLocatorBancParam(Mu_spl__SdodrRestingHimeLocatorBancParam other)
		{
			AnimIndex = other.AnimIndex;
		}

		public Mu_spl__SdodrRestingHimeLocatorBancParam Clone()
		{
			return new Mu_spl__SdodrRestingHimeLocatorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SdodrRestingHimeLocatorBancParam = new BymlNode.DictionaryNode() { Name = "spl__SdodrRestingHimeLocatorBancParam" };

			if (SerializedActor["spl__SdodrRestingHimeLocatorBancParam"] != null)
			{
				spl__SdodrRestingHimeLocatorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SdodrRestingHimeLocatorBancParam"];
			}


			spl__SdodrRestingHimeLocatorBancParam.AddNode("AnimIndex", this.AnimIndex);

			if (SerializedActor["spl__SdodrRestingHimeLocatorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SdodrRestingHimeLocatorBancParam);
			}
		}
	}
}
