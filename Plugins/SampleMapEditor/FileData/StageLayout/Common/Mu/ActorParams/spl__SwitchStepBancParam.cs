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
	public class Mu_spl__SwitchStepBancParam
	{
		[ByamlMember]
		public bool IsToggle { get; set; }

		public Mu_spl__SwitchStepBancParam()
		{
			IsToggle = false;
		}

		public Mu_spl__SwitchStepBancParam(Mu_spl__SwitchStepBancParam other)
		{
			IsToggle = other.IsToggle;
		}

		public Mu_spl__SwitchStepBancParam Clone()
		{
			return new Mu_spl__SwitchStepBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SwitchStepBancParam = new BymlNode.DictionaryNode() { Name = "spl__SwitchStepBancParam" };

			if (SerializedActor["spl__SwitchStepBancParam"] != null)
			{
				spl__SwitchStepBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SwitchStepBancParam"];
			}


			if (this.IsToggle)
			{
				spl__SwitchStepBancParam.AddNode("IsToggle", this.IsToggle);
			}

			if (SerializedActor["spl__SwitchStepBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SwitchStepBancParam);
			}
		}
	}
}
