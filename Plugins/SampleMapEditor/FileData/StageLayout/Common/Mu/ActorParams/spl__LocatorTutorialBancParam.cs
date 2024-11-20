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
	public class Mu_spl__LocatorTutorialBancParam
	{
		[ByamlMember]
		public int ProgressIndex { get; set; }

		public Mu_spl__LocatorTutorialBancParam()
		{
			ProgressIndex = 0;
		}

		public Mu_spl__LocatorTutorialBancParam(Mu_spl__LocatorTutorialBancParam other)
		{
			ProgressIndex = other.ProgressIndex;
		}

		public Mu_spl__LocatorTutorialBancParam Clone()
		{
			return new Mu_spl__LocatorTutorialBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorTutorialBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorTutorialBancParam" };

			if (SerializedActor["spl__LocatorTutorialBancParam"] != null)
			{
				spl__LocatorTutorialBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorTutorialBancParam"];
			}


			spl__LocatorTutorialBancParam.AddNode("ProgressIndex", this.ProgressIndex);

			if (SerializedActor["spl__LocatorTutorialBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorTutorialBancParam);
			}
		}
	}
}
