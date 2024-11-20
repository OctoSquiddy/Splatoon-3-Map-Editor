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
	public class Mu_spl__PeriscopeBancParam
	{
		[ByamlMember]
		public ulong LinkToRail { get; set; }

		[ByamlMember]
		public float PitchAngleDegMax { get; set; }

		[ByamlMember]
		public float PitchAngleDegMin { get; set; }

		[ByamlMember]
		public float YawAngleDegMax { get; set; }

		[ByamlMember]
		public float YawAngleDegMin { get; set; }

		public Mu_spl__PeriscopeBancParam()
		{
			LinkToRail = 0;
			PitchAngleDegMax = 0.0f;
			PitchAngleDegMin = 0.0f;
			YawAngleDegMax = 0.0f;
			YawAngleDegMin = 0.0f;
		}

		public Mu_spl__PeriscopeBancParam(Mu_spl__PeriscopeBancParam other)
		{
			LinkToRail = other.LinkToRail;
			PitchAngleDegMax = other.PitchAngleDegMax;
			PitchAngleDegMin = other.PitchAngleDegMin;
			YawAngleDegMax = other.YawAngleDegMax;
			YawAngleDegMin = other.YawAngleDegMin;
		}

		public Mu_spl__PeriscopeBancParam Clone()
		{
			return new Mu_spl__PeriscopeBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PeriscopeBancParam = new BymlNode.DictionaryNode() { Name = "spl__PeriscopeBancParam" };

			if (SerializedActor["spl__PeriscopeBancParam"] != null)
			{
				spl__PeriscopeBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PeriscopeBancParam"];
			}


			spl__PeriscopeBancParam.AddNode("LinkToRail", this.LinkToRail);

			spl__PeriscopeBancParam.AddNode("PitchAngleDegMax", this.PitchAngleDegMax);

			spl__PeriscopeBancParam.AddNode("PitchAngleDegMin", this.PitchAngleDegMin);

			spl__PeriscopeBancParam.AddNode("YawAngleDegMax", this.YawAngleDegMax);

			spl__PeriscopeBancParam.AddNode("YawAngleDegMin", this.YawAngleDegMin);

			if (SerializedActor["spl__PeriscopeBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PeriscopeBancParam);
			}
		}
	}
}
