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
	public class Mu_game__SoundFxAreaBancParam
	{
		[ByamlMember]
		public string Bus { get; set; }

		[ByamlMember]
		public float Margin { get; set; }

		[ByamlMember]
		public float OcclusionMax { get; set; }

		public Mu_game__SoundFxAreaBancParam()
		{
			Bus = "";
			Margin = 0.0f;
			OcclusionMax = 0.0f;
		}

		public Mu_game__SoundFxAreaBancParam(Mu_game__SoundFxAreaBancParam other)
		{
			Bus = other.Bus;
			Margin = other.Margin;
			OcclusionMax = other.OcclusionMax;
		}

		public Mu_game__SoundFxAreaBancParam Clone()
		{
			return new Mu_game__SoundFxAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__SoundFxAreaBancParam = new BymlNode.DictionaryNode() { Name = "game__SoundFxAreaBancParam" };

			if (SerializedActor["game__SoundFxAreaBancParam"] != null)
			{
				game__SoundFxAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["game__SoundFxAreaBancParam"];
			}


			if (this.Bus != "")
			{
				game__SoundFxAreaBancParam.AddNode("Bus", this.Bus);
			}

			game__SoundFxAreaBancParam.AddNode("Margin", this.Margin);

			game__SoundFxAreaBancParam.AddNode("OcclusionMax", this.OcclusionMax);

			if (SerializedActor["game__SoundFxAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(game__SoundFxAreaBancParam);
			}
		}
	}
}
