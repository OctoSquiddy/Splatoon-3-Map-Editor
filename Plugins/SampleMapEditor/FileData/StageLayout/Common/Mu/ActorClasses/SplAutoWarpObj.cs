using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplAutoWarpObj : MuObj
	{
		[ByamlMember("spl__AutoWarpObjBancParam")]
		public Mu_spl__AutoWarpObjBancParam spl__AutoWarpObjBancParam { get; set; }

		public SplAutoWarpObj() : base()
		{
			spl__AutoWarpObjBancParam = new Mu_spl__AutoWarpObjBancParam();

			Links = new List<Link>();
		}

		public SplAutoWarpObj(SplAutoWarpObj other) : base(other)
		{
			spl__AutoWarpObjBancParam = other.spl__AutoWarpObjBancParam.Clone();
		}

		public override SplAutoWarpObj Clone()
		{
			return new SplAutoWarpObj(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AutoWarpObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
