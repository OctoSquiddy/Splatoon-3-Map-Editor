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
	public class Mu_spl__VictoryLiftBancParamSdodr
	{
		[ByamlMember]
		public int CheckPointIndex0 { get; set; }

		[ByamlMember]
		public int CheckPointIndex1 { get; set; }

		[ByamlMember]
		public int CheckPointIndex2 { get; set; }

		public Mu_spl__VictoryLiftBancParamSdodr()
		{
			CheckPointIndex0 = 0;
			CheckPointIndex1 = 0;
			CheckPointIndex2 = 0;
		}

		public Mu_spl__VictoryLiftBancParamSdodr(Mu_spl__VictoryLiftBancParamSdodr other)
		{
			CheckPointIndex0 = other.CheckPointIndex0;
			CheckPointIndex1 = other.CheckPointIndex1;
			CheckPointIndex2 = other.CheckPointIndex2;
		}

		public Mu_spl__VictoryLiftBancParamSdodr Clone()
		{
			return new Mu_spl__VictoryLiftBancParamSdodr(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__VictoryLiftBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__VictoryLiftBancParamSdodr" };

			if (SerializedActor["spl__VictoryLiftBancParamSdodr"] != null)
			{
				spl__VictoryLiftBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__VictoryLiftBancParamSdodr"];
			}


			spl__VictoryLiftBancParamSdodr.AddNode("CheckPointIndex0", this.CheckPointIndex0);

			spl__VictoryLiftBancParamSdodr.AddNode("CheckPointIndex1", this.CheckPointIndex1);

			spl__VictoryLiftBancParamSdodr.AddNode("CheckPointIndex2", this.CheckPointIndex2);

			if (SerializedActor["spl__VictoryLiftBancParamSdodr"] == null)
			{
				SerializedActor.Nodes.Add(spl__VictoryLiftBancParamSdodr);
			}
		}
	}
}
