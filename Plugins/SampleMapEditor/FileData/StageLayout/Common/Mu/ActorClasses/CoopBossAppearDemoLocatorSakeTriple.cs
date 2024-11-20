using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopBossAppearDemoLocatorSakeTriple : MuObj
	{
		[ByamlMember("spl__CoopBossAppearDemoLocatorParam")]
		public Mu_spl__CoopBossAppearDemoLocatorParam spl__CoopBossAppearDemoLocatorParam { get; set; }

		public CoopBossAppearDemoLocatorSakeTriple() : base()
		{
			spl__CoopBossAppearDemoLocatorParam = new Mu_spl__CoopBossAppearDemoLocatorParam();

			Links = new List<Link>();
		}

		public CoopBossAppearDemoLocatorSakeTriple(CoopBossAppearDemoLocatorSakeTriple other) : base(other)
		{
			spl__CoopBossAppearDemoLocatorParam = other.spl__CoopBossAppearDemoLocatorParam.Clone();
		}

		public override CoopBossAppearDemoLocatorSakeTriple Clone()
		{
			return new CoopBossAppearDemoLocatorSakeTriple(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopBossAppearDemoLocatorParam.SaveParameterBank(SerializedActor);
		}
	}
}
