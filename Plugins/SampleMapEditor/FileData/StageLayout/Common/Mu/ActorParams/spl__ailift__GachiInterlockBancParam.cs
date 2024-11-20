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
	public class Mu_spl__ailift__GachiInterlockBancParam
	{
		[ByamlMember]
		public float Accel { get; set; }

		[ByamlMember]
		public float MaxSpeed { get; set; }

		[ByamlMember]
		public int StopFrameNum { get; set; }

		public Mu_spl__ailift__GachiInterlockBancParam()
		{
			Accel = 0.0f;
			MaxSpeed = 0.0f;
			StopFrameNum = 0;
		}

		public Mu_spl__ailift__GachiInterlockBancParam(Mu_spl__ailift__GachiInterlockBancParam other)
		{
			Accel = other.Accel;
			MaxSpeed = other.MaxSpeed;
			StopFrameNum = other.StopFrameNum;
		}

		public Mu_spl__ailift__GachiInterlockBancParam Clone()
		{
			return new Mu_spl__ailift__GachiInterlockBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ailift__GachiInterlockBancParam = new BymlNode.DictionaryNode() { Name = "spl__ailift__GachiInterlockBancParam" };

			if (SerializedActor["spl__ailift__GachiInterlockBancParam"] != null)
			{
				spl__ailift__GachiInterlockBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ailift__GachiInterlockBancParam"];
			}

            if (this.Accel > 0.0f)
                spl__ailift__GachiInterlockBancParam.AddNode("Accel", this.Accel);

            if (this.MaxSpeed > 0.0f)
                spl__ailift__GachiInterlockBancParam.AddNode("MaxSpeed", this.MaxSpeed);

            if (this.StopFrameNum > 0)
                spl__ailift__GachiInterlockBancParam.AddNode("StopFrameNum", this.StopFrameNum);

			if (SerializedActor["spl__ailift__GachiInterlockBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ailift__GachiInterlockBancParam);
			}
		}
	}
}
