using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopGoldenIkuraReserveDropPos : MuObj
	{
		[ByamlMember("spl__CoopGoldenIkuraDropCorrectAreaParam")]
		public Mu_spl__CoopGoldenIkuraDropCorrectAreaParam spl__CoopGoldenIkuraDropCorrectAreaParam { get; set; }

		public CoopGoldenIkuraReserveDropPos() : base()
		{
			spl__CoopGoldenIkuraDropCorrectAreaParam = new Mu_spl__CoopGoldenIkuraDropCorrectAreaParam();

			Links = new List<Link>();
		}

		public CoopGoldenIkuraReserveDropPos(CoopGoldenIkuraReserveDropPos other) : base(other)
		{
			spl__CoopGoldenIkuraDropCorrectAreaParam = other.spl__CoopGoldenIkuraDropCorrectAreaParam.Clone();
		}

		public override CoopGoldenIkuraReserveDropPos Clone()
		{
			return new CoopGoldenIkuraReserveDropPos(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopGoldenIkuraDropCorrectAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
