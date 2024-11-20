using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__CoopBossAppearDemoLocatorParam
	{
		public Mu_spl__CoopBossAppearDemoLocatorParam()
		{
		}

		public Mu_spl__CoopBossAppearDemoLocatorParam(Mu_spl__CoopBossAppearDemoLocatorParam other)
		{
		}

		public Mu_spl__CoopBossAppearDemoLocatorParam Clone()
		{
			return new Mu_spl__CoopBossAppearDemoLocatorParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopBossAppearDemoLocatorParam = new BymlNode.DictionaryNode() { Name = "spl__CoopBossAppearDemoLocatorParam" };

			if (SerializedActor["spl__CoopBossAppearDemoLocatorParam"] != null)
			{
				spl__CoopBossAppearDemoLocatorParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopBossAppearDemoLocatorParam"];
			}


			if (SerializedActor["spl__CoopBossAppearDemoLocatorParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopBossAppearDemoLocatorParam);
			}
		}
	}
}
