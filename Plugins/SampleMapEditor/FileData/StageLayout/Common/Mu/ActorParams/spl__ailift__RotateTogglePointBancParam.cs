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
	public class Mu_spl__ailift__RotateTogglePointBancParam
	{
		[ByamlMember]
		public float AccelDecrease { get; set; }

		[ByamlMember]
		public float AccelIncrease { get; set; }

		[ByamlMember]
		public int DegStopLcm { get; set; }

		[ByamlMember]
		public float RotSpeedMax { get; set; }

		public Mu_spl__ailift__RotateTogglePointBancParam()
		{
			AccelDecrease = 0.0f;
			AccelIncrease = 0.0f;
			DegStopLcm = 0;
			RotSpeedMax = 0.0f;
		}

		public Mu_spl__ailift__RotateTogglePointBancParam(Mu_spl__ailift__RotateTogglePointBancParam other)
		{
			AccelDecrease = other.AccelDecrease;
			AccelIncrease = other.AccelIncrease;
			DegStopLcm = other.DegStopLcm;
			RotSpeedMax = other.RotSpeedMax;
		}

		public Mu_spl__ailift__RotateTogglePointBancParam Clone()
		{
			return new Mu_spl__ailift__RotateTogglePointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ailift__RotateTogglePointBancParam = new BymlNode.DictionaryNode() { Name = "spl__ailift__RotateTogglePointBancParam" };

			if (SerializedActor["spl__ailift__RotateTogglePointBancParam"] != null)
			{
				spl__ailift__RotateTogglePointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ailift__RotateTogglePointBancParam"];
			}

            if (this.AccelDecrease > 0.0f)
                spl__ailift__RotateTogglePointBancParam.AddNode("AccelDecrease", this.AccelDecrease);

            if (this.AccelIncrease > 0.0f)
                spl__ailift__RotateTogglePointBancParam.AddNode("AccelIncrease", this.AccelIncrease);

            if (this.DegStopLcm > 0)
                spl__ailift__RotateTogglePointBancParam.AddNode("DegStopLcm", this.DegStopLcm);

            if (this.RotSpeedMax > 0.0f)
                spl__ailift__RotateTogglePointBancParam.AddNode("RotSpeedMax", this.RotSpeedMax);

			if (SerializedActor["spl__ailift__RotateTogglePointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ailift__RotateTogglePointBancParam);
			}
		}
	}
}
