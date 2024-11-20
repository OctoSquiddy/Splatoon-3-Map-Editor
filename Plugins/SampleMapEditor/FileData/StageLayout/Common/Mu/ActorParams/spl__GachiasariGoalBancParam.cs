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
	public class Mu_spl__GachiasariGoalBancParam
	{
		[ByamlMember]
		public float DegAxisY_SpawnFromGoal { get; set; }

		[ByamlMember]
		public float Height { get; set; }

		public Mu_spl__GachiasariGoalBancParam()
		{
			DegAxisY_SpawnFromGoal = 0.0f;
			Height = 0.0f;
		}

		public Mu_spl__GachiasariGoalBancParam(Mu_spl__GachiasariGoalBancParam other)
		{
			DegAxisY_SpawnFromGoal = other.DegAxisY_SpawnFromGoal;
			Height = other.Height;
		}

		public Mu_spl__GachiasariGoalBancParam Clone()
		{
			return new Mu_spl__GachiasariGoalBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GachiasariGoalBancParam = new BymlNode.DictionaryNode() { Name = "spl__GachiasariGoalBancParam" };

			if (SerializedActor["spl__GachiasariGoalBancParam"] != null)
			{
				spl__GachiasariGoalBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GachiasariGoalBancParam"];
			}


			spl__GachiasariGoalBancParam.AddNode("DegAxisY_SpawnFromGoal", this.DegAxisY_SpawnFromGoal);

			spl__GachiasariGoalBancParam.AddNode("Height", this.Height);

			if (SerializedActor["spl__GachiasariGoalBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GachiasariGoalBancParam);
			}
		}
	}
}
