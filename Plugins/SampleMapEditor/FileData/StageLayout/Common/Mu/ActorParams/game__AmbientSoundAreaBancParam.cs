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
	public class Mu_game__AmbientSoundAreaBancParam
	{
		[ByamlMember]
		public bool IsSetPos { get; set; }

		[ByamlMember]
		public string Key { get; set; }

		[ByamlMember]
		public float Margin { get; set; }

		public Mu_game__AmbientSoundAreaBancParam()
		{
			IsSetPos = false;
			Key = "";
			Margin = 0.0f;
		}

		public Mu_game__AmbientSoundAreaBancParam(Mu_game__AmbientSoundAreaBancParam other)
		{
			IsSetPos = other.IsSetPos;
			Key = other.Key;
			Margin = other.Margin;
		}

		public Mu_game__AmbientSoundAreaBancParam Clone()
		{
			return new Mu_game__AmbientSoundAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__AmbientSoundAreaBancParam = new BymlNode.DictionaryNode() { Name = "game__AmbientSoundAreaBancParam" };

			if (SerializedActor["game__AmbientSoundAreaBancParam"] != null)
			{
				game__AmbientSoundAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["game__AmbientSoundAreaBancParam"];
			}


			if (this.IsSetPos)
			{
				game__AmbientSoundAreaBancParam.AddNode("IsSetPos", this.IsSetPos);
			}

			if (this.Key != "")
			{
				game__AmbientSoundAreaBancParam.AddNode("Key", this.Key);
			}

			game__AmbientSoundAreaBancParam.AddNode("Margin", this.Margin);

			if (SerializedActor["game__AmbientSoundAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(game__AmbientSoundAreaBancParam);
			}
		}
	}
}
