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
	public class Mu_spl__CoopParamHolderEventGeyserParam
	{
		[ByamlMember]
		public float ThDistanceWaterHigh { get; set; }

		[ByamlMember]
		public float ThDistanceWaterMid { get; set; }

		public Mu_spl__CoopParamHolderEventGeyserParam()
		{
			ThDistanceWaterHigh = 0.0f;
			ThDistanceWaterMid = 0.0f;
		}

		public Mu_spl__CoopParamHolderEventGeyserParam(Mu_spl__CoopParamHolderEventGeyserParam other)
		{
			ThDistanceWaterHigh = other.ThDistanceWaterHigh;
			ThDistanceWaterMid = other.ThDistanceWaterMid;
		}

		public Mu_spl__CoopParamHolderEventGeyserParam Clone()
		{
			return new Mu_spl__CoopParamHolderEventGeyserParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopParamHolderEventGeyserParam = new BymlNode.DictionaryNode() { Name = "spl__CoopParamHolderEventGeyserParam" };

			if (SerializedActor["spl__CoopParamHolderEventGeyserParam"] != null)
			{
				spl__CoopParamHolderEventGeyserParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopParamHolderEventGeyserParam"];
			}


			spl__CoopParamHolderEventGeyserParam.AddNode("ThDistanceWaterHigh", this.ThDistanceWaterHigh);

			spl__CoopParamHolderEventGeyserParam.AddNode("ThDistanceWaterMid", this.ThDistanceWaterMid);

			if (SerializedActor["spl__CoopParamHolderEventGeyserParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopParamHolderEventGeyserParam);
			}
		}
	}
}
