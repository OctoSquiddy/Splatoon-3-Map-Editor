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
	public class Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam
	{
		public Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam()
		{
		}

		public Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam(Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam other)
		{
		}

		public Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam Clone()
		{
			return new Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam = new BymlNode.DictionaryNode() { Name = "spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam" };

			if (SerializedActor["spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam"] != null)
			{
				spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam"];
			}


			if (SerializedActor["spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam);
			}
		}
	}
}
