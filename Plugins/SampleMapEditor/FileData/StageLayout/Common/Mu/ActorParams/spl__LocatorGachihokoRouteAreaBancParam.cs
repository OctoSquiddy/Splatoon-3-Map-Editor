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
	public class Mu_spl__LocatorGachihokoRouteAreaBancParam
	{
		public Mu_spl__LocatorGachihokoRouteAreaBancParam()
		{
		}

		public Mu_spl__LocatorGachihokoRouteAreaBancParam(Mu_spl__LocatorGachihokoRouteAreaBancParam other)
		{
		}

		public Mu_spl__LocatorGachihokoRouteAreaBancParam Clone()
		{
			return new Mu_spl__LocatorGachihokoRouteAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorGachihokoRouteAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorGachihokoRouteAreaBancParam" };

			if (SerializedActor["spl__LocatorGachihokoRouteAreaBancParam"] != null)
			{
				spl__LocatorGachihokoRouteAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorGachihokoRouteAreaBancParam"];
			}


			if (SerializedActor["spl__LocatorGachihokoRouteAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorGachihokoRouteAreaBancParam);
			}
		}
	}
}
