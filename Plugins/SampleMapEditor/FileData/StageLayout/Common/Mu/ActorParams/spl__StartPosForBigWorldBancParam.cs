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
	public class Mu_spl__StartPosForBigWorldBancParam
	{
		[ByamlMember]
		public string StartPosType { get; set; }

		public Mu_spl__StartPosForBigWorldBancParam()
		{
			StartPosType = "";
		}

		public Mu_spl__StartPosForBigWorldBancParam(Mu_spl__StartPosForBigWorldBancParam other)
		{
			StartPosType = other.StartPosType;
		}

		public Mu_spl__StartPosForBigWorldBancParam Clone()
		{
			return new Mu_spl__StartPosForBigWorldBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__StartPosForBigWorldBancParam = new BymlNode.DictionaryNode() { Name = "spl__StartPosForBigWorldBancParam" };

			if (SerializedActor["spl__StartPosForBigWorldBancParam"] != null)
			{
				spl__StartPosForBigWorldBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__StartPosForBigWorldBancParam"];
			}


			if (this.StartPosType != "")
			{
				spl__StartPosForBigWorldBancParam.AddNode("StartPosType", this.StartPosType);
			}

			if (SerializedActor["spl__StartPosForBigWorldBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__StartPosForBigWorldBancParam);
			}
		}
	}
}
