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
	public class Mu_spl__StartPosForLaunchPadWorldBancParam
	{
		[ByamlMember]
		public int Progress { get; set; }

		public Mu_spl__StartPosForLaunchPadWorldBancParam()
		{
			Progress = 0;
		}

		public Mu_spl__StartPosForLaunchPadWorldBancParam(Mu_spl__StartPosForLaunchPadWorldBancParam other)
		{
			Progress = other.Progress;
		}

		public Mu_spl__StartPosForLaunchPadWorldBancParam Clone()
		{
			return new Mu_spl__StartPosForLaunchPadWorldBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__StartPosForLaunchPadWorldBancParam = new BymlNode.DictionaryNode() { Name = "spl__StartPosForLaunchPadWorldBancParam" };

			if (SerializedActor["spl__StartPosForLaunchPadWorldBancParam"] != null)
			{
				spl__StartPosForLaunchPadWorldBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__StartPosForLaunchPadWorldBancParam"];
			}


			spl__StartPosForLaunchPadWorldBancParam.AddNode("Progress", this.Progress);

			if (SerializedActor["spl__StartPosForLaunchPadWorldBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__StartPosForLaunchPadWorldBancParam);
			}
		}
	}
}
