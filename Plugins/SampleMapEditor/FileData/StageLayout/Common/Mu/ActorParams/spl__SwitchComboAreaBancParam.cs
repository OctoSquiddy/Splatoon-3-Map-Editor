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
	public class Mu_spl__SwitchComboAreaBancParam
	{
		[ByamlMember]
		public string CheckActorName0 { get; set; }

		[ByamlMember]
		public string CheckActorName1 { get; set; }

		[ByamlMember]
		public string CheckActorName2 { get; set; }

		[ByamlMember]
		public int ComboNum { get; set; }

		[ByamlMember]
		public float ComboTime { get; set; }

		public Mu_spl__SwitchComboAreaBancParam()
		{
			CheckActorName0 = "";
			CheckActorName1 = "";
			CheckActorName2 = "";
			ComboNum = 0;
			ComboTime = 0.0f;
		}

		public Mu_spl__SwitchComboAreaBancParam(Mu_spl__SwitchComboAreaBancParam other)
		{
			CheckActorName0 = other.CheckActorName0;
			CheckActorName1 = other.CheckActorName1;
			CheckActorName2 = other.CheckActorName2;
			ComboNum = other.ComboNum;
			ComboTime = other.ComboTime;
		}

		public Mu_spl__SwitchComboAreaBancParam Clone()
		{
			return new Mu_spl__SwitchComboAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SwitchComboAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__SwitchComboAreaBancParam" };

			if (SerializedActor["spl__SwitchComboAreaBancParam"] != null)
			{
				spl__SwitchComboAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SwitchComboAreaBancParam"];
			}


			if (this.CheckActorName0 != "")
			{
				spl__SwitchComboAreaBancParam.AddNode("CheckActorName0", this.CheckActorName0);
			}

			if (this.CheckActorName1 != "")
			{
				spl__SwitchComboAreaBancParam.AddNode("CheckActorName1", this.CheckActorName1);
			}

			if (this.CheckActorName2 != "")
			{
				spl__SwitchComboAreaBancParam.AddNode("CheckActorName2", this.CheckActorName2);
			}

			spl__SwitchComboAreaBancParam.AddNode("ComboNum", this.ComboNum);

			spl__SwitchComboAreaBancParam.AddNode("ComboTime", this.ComboTime);

			if (SerializedActor["spl__SwitchComboAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SwitchComboAreaBancParam);
			}
		}
	}
}
