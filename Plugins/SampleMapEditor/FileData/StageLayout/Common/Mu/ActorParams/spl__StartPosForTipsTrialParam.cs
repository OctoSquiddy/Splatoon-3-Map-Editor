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
	public class Mu_spl__StartPosForTipsTrialParam
	{
		[ByamlMember]
		public string Name { get; set; }

		public Mu_spl__StartPosForTipsTrialParam()
		{
			Name = "";
		}

		public Mu_spl__StartPosForTipsTrialParam(Mu_spl__StartPosForTipsTrialParam other)
		{
			Name = other.Name;
		}

		public Mu_spl__StartPosForTipsTrialParam Clone()
		{
			return new Mu_spl__StartPosForTipsTrialParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__StartPosForTipsTrialParam = new BymlNode.DictionaryNode() { Name = "spl__StartPosForTipsTrialParam" };

			if (SerializedActor["spl__StartPosForTipsTrialParam"] != null)
			{
				spl__StartPosForTipsTrialParam = (BymlNode.DictionaryNode)SerializedActor["spl__StartPosForTipsTrialParam"];
			}


			if (this.Name != "")
			{
				spl__StartPosForTipsTrialParam.AddNode("Name", this.Name);
			}

			if (SerializedActor["spl__StartPosForTipsTrialParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__StartPosForTipsTrialParam);
			}
		}
	}
}
