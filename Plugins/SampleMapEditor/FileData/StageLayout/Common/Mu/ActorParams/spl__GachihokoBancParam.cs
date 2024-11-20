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
	public class Mu_spl__GachihokoBancParam
	{
		[ByamlMember]
		public float HikikomoriDetectionBaseRate { get; set; }

		[ByamlMember]
		public float HikikomoriDetectionDist { get; set; }

		[ByamlMember]
		public float HikikomoriDetectionRate { get; set; }

		public Mu_spl__GachihokoBancParam()
		{
			HikikomoriDetectionBaseRate = 0.0f;
			HikikomoriDetectionDist = 0.0f;
			HikikomoriDetectionRate = 0.0f;
		}

		public Mu_spl__GachihokoBancParam(Mu_spl__GachihokoBancParam other)
		{
			HikikomoriDetectionBaseRate = other.HikikomoriDetectionBaseRate;
			HikikomoriDetectionDist = other.HikikomoriDetectionDist;
			HikikomoriDetectionRate = other.HikikomoriDetectionRate;
		}

		public Mu_spl__GachihokoBancParam Clone()
		{
			return new Mu_spl__GachihokoBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GachihokoBancParam = new BymlNode.DictionaryNode() { Name = "spl__GachihokoBancParam" };

			if (SerializedActor["spl__GachihokoBancParam"] != null)
			{
				spl__GachihokoBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GachihokoBancParam"];
			}


			spl__GachihokoBancParam.AddNode("HikikomoriDetectionBaseRate", this.HikikomoriDetectionBaseRate);

			spl__GachihokoBancParam.AddNode("HikikomoriDetectionDist", this.HikikomoriDetectionDist);

			spl__GachihokoBancParam.AddNode("HikikomoriDetectionRate", this.HikikomoriDetectionRate);

			if (SerializedActor["spl__GachihokoBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GachihokoBancParam);
			}
		}
	}
}
