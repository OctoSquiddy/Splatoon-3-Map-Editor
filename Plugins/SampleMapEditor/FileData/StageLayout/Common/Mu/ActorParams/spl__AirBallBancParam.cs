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
	public class Mu_spl__AirBallBancParam
	{
		[ByamlMember]
		public int IkuraNum { get; set; }

		[ByamlMember]
		public int IkuraValue { get; set; }

		[ByamlMember]
		public int TimeLimit { get; set; }

		public Mu_spl__AirBallBancParam()
		{
			IkuraNum = 0;
			IkuraValue = 0;
			TimeLimit = 0;
		}

		public Mu_spl__AirBallBancParam(Mu_spl__AirBallBancParam other)
		{
			IkuraNum = other.IkuraNum;
			IkuraValue = other.IkuraValue;
			TimeLimit = other.TimeLimit;
		}

		public Mu_spl__AirBallBancParam Clone()
		{
			return new Mu_spl__AirBallBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__AirBallBancParam = new BymlNode.DictionaryNode() { Name = "spl__AirBallBancParam" };

			if (SerializedActor["spl__AirBallBancParam"] != null)
			{
				spl__AirBallBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AirBallBancParam"];
			}


			spl__AirBallBancParam.AddNode("IkuraNum", this.IkuraNum);

			spl__AirBallBancParam.AddNode("IkuraValue", this.IkuraValue);

			spl__AirBallBancParam.AddNode("TimeLimit", this.TimeLimit);

			if (SerializedActor["spl__AirBallBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__AirBallBancParam);
			}
		}
	}
}
