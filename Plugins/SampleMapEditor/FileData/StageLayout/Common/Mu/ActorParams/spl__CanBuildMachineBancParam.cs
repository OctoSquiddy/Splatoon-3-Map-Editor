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
	public class Mu_spl__CanBuildMachineBancParam
	{
		public Mu_spl__CanBuildMachineBancParam()
		{
		}

		public Mu_spl__CanBuildMachineBancParam(Mu_spl__CanBuildMachineBancParam other)
		{
		}

		public Mu_spl__CanBuildMachineBancParam Clone()
		{
			return new Mu_spl__CanBuildMachineBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CanBuildMachineBancParam = new BymlNode.DictionaryNode() { Name = "spl__CanBuildMachineBancParam" };

			if (SerializedActor["spl__CanBuildMachineBancParam"] != null)
			{
				spl__CanBuildMachineBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CanBuildMachineBancParam"];
			}


			if (SerializedActor["spl__CanBuildMachineBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CanBuildMachineBancParam);
			}
		}
	}
}
