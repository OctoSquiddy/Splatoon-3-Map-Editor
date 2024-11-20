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
	public class Mu_spl__StartPosParam
	{
		[ByamlMember]
		public string Name { get; set; }

		[ByamlMember]
		public int PlayerIndex { get; set; }

		public Mu_spl__StartPosParam()
		{
			Name = "";
			PlayerIndex = 0;
		}

		public Mu_spl__StartPosParam(Mu_spl__StartPosParam other)
		{
			Name = other.Name;
			PlayerIndex = other.PlayerIndex;
		}

		public Mu_spl__StartPosParam Clone()
		{
			return new Mu_spl__StartPosParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__StartPosParam = new BymlNode.DictionaryNode() { Name = "spl__StartPosParam" };

			if (SerializedActor["spl__StartPosParam"] != null)
			{
				spl__StartPosParam = (BymlNode.DictionaryNode)SerializedActor["spl__StartPosParam"];
			}


			if (this.Name != "")
			{
				spl__StartPosParam.AddNode("Name", this.Name);
			}

			spl__StartPosParam.AddNode("PlayerIndex", this.PlayerIndex);

			if (SerializedActor["spl__StartPosParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__StartPosParam);
			}
		}
	}
}
