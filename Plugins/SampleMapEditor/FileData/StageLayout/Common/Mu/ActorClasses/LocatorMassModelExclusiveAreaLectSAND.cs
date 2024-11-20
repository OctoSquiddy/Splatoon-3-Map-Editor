using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorMassModelExclusiveAreaLectSAND : MuObj
	{
		[ByamlMember("spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam")]
		public Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam { get; set; }

		public LocatorMassModelExclusiveAreaLectSAND() : base()
		{
			spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam = new Mu_spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam();

			Links = new List<Link>();
		}

		public LocatorMassModelExclusiveAreaLectSAND(LocatorMassModelExclusiveAreaLectSAND other) : base(other)
		{
			spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam = other.spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam.Clone();
		}

		public override LocatorMassModelExclusiveAreaLectSAND Clone()
		{
			return new LocatorMassModelExclusiveAreaLectSAND(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorMassModelExclusiveAreaSettingSANDBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
