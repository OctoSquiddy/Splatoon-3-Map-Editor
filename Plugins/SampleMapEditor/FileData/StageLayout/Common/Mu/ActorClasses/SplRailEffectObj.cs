using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplRailEffectObj : MuObj
	{
		[ByamlMember("spl__RailEffectObjBancParam")]
		public Mu_spl__RailEffectObjBancParam spl__RailEffectObjBancParam { get; set; }

		public SplRailEffectObj() : base()
		{
			spl__RailEffectObjBancParam = new Mu_spl__RailEffectObjBancParam();

			Links = new List<Link>();
		}

		public SplRailEffectObj(SplRailEffectObj other) : base(other)
		{
			spl__RailEffectObjBancParam = other.spl__RailEffectObjBancParam.Clone();
		}

		public override SplRailEffectObj Clone()
		{
			return new SplRailEffectObj(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RailEffectObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
