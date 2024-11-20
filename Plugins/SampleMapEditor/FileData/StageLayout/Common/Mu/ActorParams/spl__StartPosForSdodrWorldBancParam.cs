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
	public class Mu_spl__StartPosForSdodrWorldBancParam
	{
		[ByamlMember]
		public bool IsCameraReverse { get; set; }

		[ByamlMember]
		public string StartPosType { get; set; }

		public Mu_spl__StartPosForSdodrWorldBancParam()
		{
			IsCameraReverse = false;
			StartPosType = "";
		}

		public Mu_spl__StartPosForSdodrWorldBancParam(Mu_spl__StartPosForSdodrWorldBancParam other)
		{
			IsCameraReverse = other.IsCameraReverse;
			StartPosType = other.StartPosType;
		}

		public Mu_spl__StartPosForSdodrWorldBancParam Clone()
		{
			return new Mu_spl__StartPosForSdodrWorldBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__StartPosForSdodrWorldBancParam = new BymlNode.DictionaryNode() { Name = "spl__StartPosForSdodrWorldBancParam" };

			if (SerializedActor["spl__StartPosForSdodrWorldBancParam"] != null)
			{
				spl__StartPosForSdodrWorldBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__StartPosForSdodrWorldBancParam"];
			}


			if (this.IsCameraReverse)
			{
				spl__StartPosForSdodrWorldBancParam.AddNode("IsCameraReverse", this.IsCameraReverse);
			}

			if (this.StartPosType != "")
			{
				spl__StartPosForSdodrWorldBancParam.AddNode("StartPosType", this.StartPosType);
			}

			if (SerializedActor["spl__StartPosForSdodrWorldBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__StartPosForSdodrWorldBancParam);
			}
		}
	}
}
