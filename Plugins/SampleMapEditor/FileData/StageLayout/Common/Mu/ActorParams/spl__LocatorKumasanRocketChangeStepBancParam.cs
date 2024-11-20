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
	public class Mu_spl__LocatorKumasanRocketChangeStepBancParam
	{
		[ByamlMember]
		public string Level { get; set; }

		[ByamlMember]
		public string Step { get; set; }

		public Mu_spl__LocatorKumasanRocketChangeStepBancParam()
		{
			Level = "";
			Step = "";
		}

		public Mu_spl__LocatorKumasanRocketChangeStepBancParam(Mu_spl__LocatorKumasanRocketChangeStepBancParam other)
		{
			Level = other.Level;
			Step = other.Step;
		}

		public Mu_spl__LocatorKumasanRocketChangeStepBancParam Clone()
		{
			return new Mu_spl__LocatorKumasanRocketChangeStepBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorKumasanRocketChangeStepBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorKumasanRocketChangeStepBancParam" };

			if (SerializedActor["spl__LocatorKumasanRocketChangeStepBancParam"] != null)
			{
				spl__LocatorKumasanRocketChangeStepBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorKumasanRocketChangeStepBancParam"];
			}


			if (this.Level != "")
			{
				spl__LocatorKumasanRocketChangeStepBancParam.AddNode("Level", this.Level);
			}

			if (this.Step != "")
			{
				spl__LocatorKumasanRocketChangeStepBancParam.AddNode("Step", this.Step);
			}

			if (SerializedActor["spl__LocatorKumasanRocketChangeStepBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorKumasanRocketChangeStepBancParam);
			}
		}
	}
}
