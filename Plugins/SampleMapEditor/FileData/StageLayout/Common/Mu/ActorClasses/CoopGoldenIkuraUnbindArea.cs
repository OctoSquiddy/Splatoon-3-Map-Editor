using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopGoldenIkuraUnbindArea : MuObj
	{
		[ByamlMember("spl__CoopGoldenIkuraDropCorrectAreaParam")]
		public Mu_spl__CoopGoldenIkuraDropCorrectAreaParam spl__CoopGoldenIkuraDropCorrectAreaParam { get; set; }

		public CoopGoldenIkuraUnbindArea() : base()
		{
			spl__CoopGoldenIkuraDropCorrectAreaParam = new Mu_spl__CoopGoldenIkuraDropCorrectAreaParam();

			Links = new List<Link>();
		}

		public CoopGoldenIkuraUnbindArea(CoopGoldenIkuraUnbindArea other) : base(other)
		{
			spl__CoopGoldenIkuraDropCorrectAreaParam = other.spl__CoopGoldenIkuraDropCorrectAreaParam.Clone();
		}

		public override CoopGoldenIkuraUnbindArea Clone()
		{
			return new CoopGoldenIkuraUnbindArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopGoldenIkuraDropCorrectAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
