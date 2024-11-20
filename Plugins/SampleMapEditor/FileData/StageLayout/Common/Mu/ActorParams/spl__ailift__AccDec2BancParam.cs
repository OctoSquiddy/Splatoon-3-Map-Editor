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
	public class Mu_spl__ailift__AccDec2BancParam
	{
		[ByamlMember]
		public float Accel { get; set; }

		[ByamlMember]
		public float Decel { get; set; }

		[ByamlMember]
		public float MaxSpeed { get; set; }

		[ByamlMember]
		public float ReverseMaxSpeed { get; set; }

		public Mu_spl__ailift__AccDec2BancParam()
		{
			Accel = 0.0f;
			Decel = 0.0f;
			MaxSpeed = 0.0f;
			ReverseMaxSpeed = 0.0f;
		}

		public Mu_spl__ailift__AccDec2BancParam(Mu_spl__ailift__AccDec2BancParam other)
		{
			Accel = other.Accel;
			Decel = other.Decel;
			MaxSpeed = other.MaxSpeed;
			ReverseMaxSpeed = other.ReverseMaxSpeed;
		}

		public Mu_spl__ailift__AccDec2BancParam Clone()
		{
			return new Mu_spl__ailift__AccDec2BancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ailift__AccDec2BancParam = new BymlNode.DictionaryNode() { Name = "spl__ailift__AccDec2BancParam" };

			if (SerializedActor["spl__ailift__AccDec2BancParam"] != null)
			{
				spl__ailift__AccDec2BancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ailift__AccDec2BancParam"];
			}

			if (this.Accel > 0.0f)
				spl__ailift__AccDec2BancParam.AddNode("Accel", this.Accel);

            if (this.Decel > 0.0f)
                spl__ailift__AccDec2BancParam.AddNode("Decel", this.Decel);

            if (this.MaxSpeed > 0.0f)
                spl__ailift__AccDec2BancParam.AddNode("MaxSpeed", this.MaxSpeed);

            if (this.ReverseMaxSpeed > 0.0f)
                spl__ailift__AccDec2BancParam.AddNode("ReverseMaxSpeed", this.ReverseMaxSpeed);

			if (SerializedActor["spl__ailift__AccDec2BancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ailift__AccDec2BancParam);
			}
		}
	}
}
