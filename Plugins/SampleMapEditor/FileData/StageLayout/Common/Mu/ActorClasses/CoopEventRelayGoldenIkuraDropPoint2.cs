using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopEventRelayGoldenIkuraDropPoint2 : MuObj
	{
		[ByamlMember("spl__CoopGoldenIkuraBoxDropPointBancParam")]
		public Mu_spl__CoopGoldenIkuraBoxDropPointBancParam spl__CoopGoldenIkuraBoxDropPointBancParam { get; set; }

		public CoopEventRelayGoldenIkuraDropPoint2() : base()
		{
			spl__CoopGoldenIkuraBoxDropPointBancParam = new Mu_spl__CoopGoldenIkuraBoxDropPointBancParam();

			Links = new List<Link>();
		}

		public CoopEventRelayGoldenIkuraDropPoint2(CoopEventRelayGoldenIkuraDropPoint2 other) : base(other)
		{
			spl__CoopGoldenIkuraBoxDropPointBancParam = other.spl__CoopGoldenIkuraBoxDropPointBancParam.Clone();
		}

		public override CoopEventRelayGoldenIkuraDropPoint2 Clone()
		{
			return new CoopEventRelayGoldenIkuraDropPoint2(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopGoldenIkuraBoxDropPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
