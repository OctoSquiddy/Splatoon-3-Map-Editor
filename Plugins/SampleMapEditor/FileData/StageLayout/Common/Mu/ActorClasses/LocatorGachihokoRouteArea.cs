using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorGachihokoRouteArea : MuObj
	{
		[ByamlMember("spl__LocatorGachihokoRouteAreaBancParam")]
		public Mu_spl__LocatorGachihokoRouteAreaBancParam spl__LocatorGachihokoRouteAreaBancParam { get; set; }

		public LocatorGachihokoRouteArea() : base()
		{
			spl__LocatorGachihokoRouteAreaBancParam = new Mu_spl__LocatorGachihokoRouteAreaBancParam();

			Links = new List<Link>();
		}

		public LocatorGachihokoRouteArea(LocatorGachihokoRouteArea other) : base(other)
		{
			spl__LocatorGachihokoRouteAreaBancParam = other.spl__LocatorGachihokoRouteAreaBancParam.Clone();
		}

		public override LocatorGachihokoRouteArea Clone()
		{
			return new LocatorGachihokoRouteArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorGachihokoRouteAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
