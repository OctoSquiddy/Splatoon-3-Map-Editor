using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class OneShotMissDieTag : MuObj
	{
		[ByamlMember("spl__OneShotMissDieTagBancParam")]
		public Mu_spl__OneShotMissDieTagBancParam spl__OneShotMissDieTagBancParam { get; set; }

		public OneShotMissDieTag() : base()
		{
			spl__OneShotMissDieTagBancParam = new Mu_spl__OneShotMissDieTagBancParam();

			Links = new List<Link>();
		}

		public OneShotMissDieTag(OneShotMissDieTag other) : base(other)
		{
			spl__OneShotMissDieTagBancParam = other.spl__OneShotMissDieTagBancParam.Clone();
		}

		public override OneShotMissDieTag Clone()
		{
			return new OneShotMissDieTag(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__OneShotMissDieTagBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
