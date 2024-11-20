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
	public class Mu_spl__AerialRingBancParam
	{
		[ByamlMember]
		public int IkuraNum { get; set; }

		public Mu_spl__AerialRingBancParam()
		{
			IkuraNum = 5;
		}

		public Mu_spl__AerialRingBancParam(Mu_spl__AerialRingBancParam other)
		{
			IkuraNum = other.IkuraNum;
		}

		public Mu_spl__AerialRingBancParam Clone()
		{
			return new Mu_spl__AerialRingBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__AerialRingBancParam = new BymlNode.DictionaryNode() { Name = "spl__AerialRingBancParam" };

			if (SerializedActor["spl__AerialRingBancParam"] != null)
			{
				spl__AerialRingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AerialRingBancParam"];
			}


			spl__AerialRingBancParam.AddNode("IkuraNum", this.IkuraNum);

			if (SerializedActor["spl__AerialRingBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__AerialRingBancParam);
			}
		}
	}
}
