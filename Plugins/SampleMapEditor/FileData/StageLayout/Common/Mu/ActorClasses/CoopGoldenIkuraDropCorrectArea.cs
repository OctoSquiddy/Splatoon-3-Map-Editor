using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopGoldenIkuraDropCorrectArea : MuObj
	{
		[ByamlMember("spl__CoopGoldenIkuraDropCorrectAreaParam")]
		public Mu_spl__CoopGoldenIkuraDropCorrectAreaParam spl__CoopGoldenIkuraDropCorrectAreaParam { get; set; }

		public CoopGoldenIkuraDropCorrectArea() : base()
		{
			spl__CoopGoldenIkuraDropCorrectAreaParam = new Mu_spl__CoopGoldenIkuraDropCorrectAreaParam();

			Links = new List<Link>();
		}

		public CoopGoldenIkuraDropCorrectArea(CoopGoldenIkuraDropCorrectArea other) : base(other)
		{
			spl__CoopGoldenIkuraDropCorrectAreaParam = other.spl__CoopGoldenIkuraDropCorrectAreaParam.Clone();
		}

		public override CoopGoldenIkuraDropCorrectArea Clone()
		{
			return new CoopGoldenIkuraDropCorrectArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopGoldenIkuraDropCorrectAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
