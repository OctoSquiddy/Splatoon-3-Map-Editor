using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorPlayerRejectArea : MuObj
	{
		[ByamlMember("spl__LocatorPlayerRejectAreaParam")]
		public Mu_spl__LocatorPlayerRejectAreaParam spl__LocatorPlayerRejectAreaParam { get; set; }

		public LocatorPlayerRejectArea() : base()
		{
			spl__LocatorPlayerRejectAreaParam = new Mu_spl__LocatorPlayerRejectAreaParam();

			Links = new List<Link>();
		}

		public LocatorPlayerRejectArea(LocatorPlayerRejectArea other) : base(other)
		{
			spl__LocatorPlayerRejectAreaParam = other.spl__LocatorPlayerRejectAreaParam.Clone();
		}

		public override LocatorPlayerRejectArea Clone()
		{
			return new LocatorPlayerRejectArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorPlayerRejectAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
