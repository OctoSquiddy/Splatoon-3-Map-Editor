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
	public class Mu_spl__LocatorPlayerRejectAreaParam
	{
		public Mu_spl__LocatorPlayerRejectAreaParam()
		{
		}

		public Mu_spl__LocatorPlayerRejectAreaParam(Mu_spl__LocatorPlayerRejectAreaParam other)
		{
		}

		public Mu_spl__LocatorPlayerRejectAreaParam Clone()
		{
			return new Mu_spl__LocatorPlayerRejectAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorPlayerRejectAreaParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorPlayerRejectAreaParam" };

			if (SerializedActor["spl__LocatorPlayerRejectAreaParam"] != null)
			{
				spl__LocatorPlayerRejectAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorPlayerRejectAreaParam"];
			}


			if (SerializedActor["spl__LocatorPlayerRejectAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorPlayerRejectAreaParam);
			}
		}
	}
}
