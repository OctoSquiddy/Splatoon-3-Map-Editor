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
	public class Mu_game__RailMovableSequentialParam
	{
		[ByamlMember]
		public string AttCalcType { get; set; }

		[ByamlMember]
		public string InterpolationType { get; set; }

		[ByamlMember]
		public int MoveSpeed { get; set; }

		[ByamlMember]
		public float MoveTime { get; set; }

		[ByamlMember]
		public string PatrolType { get; set; }

		[ByamlMember]
		public string SpeedCalcType { get; set; }

		[ByamlMember]
		public float WaitTime { get; set; }

		public Mu_game__RailMovableSequentialParam()
		{
			AttCalcType = "";
			InterpolationType = "";
			MoveSpeed = 0;
			MoveTime = 0.0f;
			PatrolType = "";
			SpeedCalcType = "";
			WaitTime = 0.0f;
		}

		public Mu_game__RailMovableSequentialParam(Mu_game__RailMovableSequentialParam other)
		{
			AttCalcType = other.AttCalcType;
			InterpolationType = other.InterpolationType;
			MoveSpeed = other.MoveSpeed;
			MoveTime = other.MoveTime;
			PatrolType = other.PatrolType;
			SpeedCalcType = other.SpeedCalcType;
			WaitTime = other.WaitTime;
		}

		public Mu_game__RailMovableSequentialParam Clone()
		{
			return new Mu_game__RailMovableSequentialParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__RailMovableSequentialParam = new BymlNode.DictionaryNode() { Name = "game__RailMovableSequentialParam" };

			if (SerializedActor["game__RailMovableSequentialParam"] != null)
			{
				game__RailMovableSequentialParam = (BymlNode.DictionaryNode)SerializedActor["game__RailMovableSequentialParam"];
			}

			if (this.AttCalcType != "")
			{
				game__RailMovableSequentialParam.AddNode("AttCalcType", this.AttCalcType);
			}

			if (this.InterpolationType != "")
			{
				game__RailMovableSequentialParam.AddNode("InterpolationType", this.InterpolationType);
			}

			if (this.MoveSpeed > 0.0f)
                game__RailMovableSequentialParam.AddNode("MoveSpeed", this.MoveSpeed);

            if (this.MoveTime > 0.0f)
                game__RailMovableSequentialParam.AddNode("MoveTime", this.MoveTime);

            if (this.PatrolType != "")
			{
				game__RailMovableSequentialParam.AddNode("PatrolType", this.PatrolType);
			}

			if (this.SpeedCalcType != "")
			{
				game__RailMovableSequentialParam.AddNode("SpeedCalcType", this.SpeedCalcType);
			}

			if (this.WaitTime > 0.0f)
			{
                game__RailMovableSequentialParam.AddNode("WaitTime", this.WaitTime);
            }		

			if (SerializedActor["game__RailMovableSequentialParam"] == null)
			{
				SerializedActor.Nodes.Add(game__RailMovableSequentialParam);
			}
		}
	}
}
