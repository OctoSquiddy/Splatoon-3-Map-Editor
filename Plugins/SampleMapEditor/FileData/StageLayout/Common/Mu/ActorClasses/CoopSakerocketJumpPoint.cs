using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopSakerocketJumpPoint : MuObj
	{
		[ByamlMember("spl__CoopSakerocketJumpPointBancParam")]
		public Mu_spl__CoopSakerocketJumpPointBancParam spl__CoopSakerocketJumpPointBancParam { get; set; }

		public CoopSakerocketJumpPoint() : base()
		{
			spl__CoopSakerocketJumpPointBancParam = new Mu_spl__CoopSakerocketJumpPointBancParam();

			Links = new List<Link>();
		}

		public CoopSakerocketJumpPoint(CoopSakerocketJumpPoint other) : base(other)
		{
			spl__CoopSakerocketJumpPointBancParam = other.spl__CoopSakerocketJumpPointBancParam.Clone();
		}

		public override CoopSakerocketJumpPoint Clone()
		{
			return new CoopSakerocketJumpPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopSakerocketJumpPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
