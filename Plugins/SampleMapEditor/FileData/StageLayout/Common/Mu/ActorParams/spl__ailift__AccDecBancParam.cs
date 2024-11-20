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
	public class Mu_spl__ailift__AccDecBancParam
	{
		[ByamlMember]
		public float Accel { get; set; }

		[ByamlMember]
		public float Decel { get; set; }

		[ByamlMember]
		public float MaxSpeed { get; set; }

		public Mu_spl__ailift__AccDecBancParam()
		{
			Accel = 0.0f;
			Decel = 0.0f;
			MaxSpeed = 0.0f;
		}

		public Mu_spl__ailift__AccDecBancParam(Mu_spl__ailift__AccDecBancParam other)
		{
			Accel = other.Accel;
			Decel = other.Decel;
			MaxSpeed = other.MaxSpeed;
		}

		public Mu_spl__ailift__AccDecBancParam Clone()
		{
			return new Mu_spl__ailift__AccDecBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ailift__AccDecBancParam = new BymlNode.DictionaryNode() { Name = "spl__ailift__AccDecBancParam" };

			if (SerializedActor["spl__ailift__AccDecBancParam"] != null)
			{
				spl__ailift__AccDecBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ailift__AccDecBancParam"];
			}

            if (this.Accel > 0.0f)
                spl__ailift__AccDecBancParam.AddNode("Accel", this.Accel);

            if (this.Decel > 0.0f)
                spl__ailift__AccDecBancParam.AddNode("Decel", this.Decel);

            if (this.MaxSpeed > 0.0f)
                spl__ailift__AccDecBancParam.AddNode("MaxSpeed", this.MaxSpeed);

			if (SerializedActor["spl__ailift__AccDecBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ailift__AccDecBancParam);
			}
		}
	}
}
