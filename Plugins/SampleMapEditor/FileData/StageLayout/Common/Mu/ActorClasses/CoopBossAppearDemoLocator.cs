using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopBossAppearDemoLocator : MuObj
	{
		[ByamlMember("spl__CoopBossAppearDemoLocatorParam")]
		public Mu_spl__CoopBossAppearDemoLocatorParam spl__CoopBossAppearDemoLocatorParam { get; set; }

		public CoopBossAppearDemoLocator() : base()
		{
			spl__CoopBossAppearDemoLocatorParam = new Mu_spl__CoopBossAppearDemoLocatorParam();

			Links = new List<Link>();
		}

		public CoopBossAppearDemoLocator(CoopBossAppearDemoLocator other) : base(other)
		{
			spl__CoopBossAppearDemoLocatorParam = other.spl__CoopBossAppearDemoLocatorParam.Clone();
		}

		public override CoopBossAppearDemoLocator Clone()
		{
			return new CoopBossAppearDemoLocator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopBossAppearDemoLocatorParam.SaveParameterBank(SerializedActor);
		}
	}
}
