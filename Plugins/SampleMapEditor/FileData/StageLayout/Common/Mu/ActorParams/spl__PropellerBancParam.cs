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
	public class Mu_spl__PropellerBancParam
	{
		[ByamlMember]
		public float ImpulseOnDamage { get; set; }

		[ByamlMember]
		public float MoveAcc { get; set; }

		[ByamlMember]
		public float MoveReturnAcc { get; set; }

		[ByamlMember]
		public float MoveReturnSpeedMax { get; set; }

		[ByamlMember]
		public float MoveSpeedMax { get; set; }

		public Mu_spl__PropellerBancParam()
		{
			ImpulseOnDamage = 0.0f;
			MoveAcc = 0.0f;
			MoveReturnAcc = 0.0f;
			MoveReturnSpeedMax = 0.0f;
			MoveSpeedMax = 0.0f;
		}

		public Mu_spl__PropellerBancParam(Mu_spl__PropellerBancParam other)
		{
			ImpulseOnDamage = other.ImpulseOnDamage;
			MoveAcc = other.MoveAcc;
			MoveReturnAcc = other.MoveReturnAcc;
			MoveReturnSpeedMax = other.MoveReturnSpeedMax;
			MoveSpeedMax = other.MoveSpeedMax;
		}

		public Mu_spl__PropellerBancParam Clone()
		{
			return new Mu_spl__PropellerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PropellerBancParam = new BymlNode.DictionaryNode() { Name = "spl__PropellerBancParam" };

			if (SerializedActor["spl__PropellerBancParam"] != null)
			{
				spl__PropellerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PropellerBancParam"];
			}

			spl__PropellerBancParam.AddNode("ImpulseOnDamage", this.ImpulseOnDamage);

			if (this.MoveAcc > 0.0f)
				spl__PropellerBancParam.AddNode("MoveAcc", this.MoveAcc);

            if (this.MoveReturnAcc > 0.0f)
                spl__PropellerBancParam.AddNode("MoveReturnAcc", this.MoveReturnAcc);

            if (this.MoveReturnSpeedMax > 0.0f)
                spl__PropellerBancParam.AddNode("MoveReturnSpeedMax", this.MoveReturnSpeedMax);

            if (this.MoveSpeedMax > 0.0f)
                spl__PropellerBancParam.AddNode("MoveSpeedMax", this.MoveSpeedMax);

			if (SerializedActor["spl__PropellerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PropellerBancParam);
			}
		}
	}
}
