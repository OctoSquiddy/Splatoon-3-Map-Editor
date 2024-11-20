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
	public class Mu_game__SoundShapeBancParam
	{
		[ByamlMember]
		public string Name { get; set; }

		public Mu_game__SoundShapeBancParam()
		{
			Name = "";
		}

		public Mu_game__SoundShapeBancParam(Mu_game__SoundShapeBancParam other)
		{
			Name = other.Name;
		}

		public Mu_game__SoundShapeBancParam Clone()
		{
			return new Mu_game__SoundShapeBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__SoundShapeBancParam = new BymlNode.DictionaryNode() { Name = "game__SoundShapeBancParam" };

			if (SerializedActor["game__SoundShapeBancParam"] != null)
			{
				game__SoundShapeBancParam = (BymlNode.DictionaryNode)SerializedActor["game__SoundShapeBancParam"];
			}


			if (this.Name != "")
			{
				game__SoundShapeBancParam.AddNode("Name", this.Name);
			}

			if (SerializedActor["game__SoundShapeBancParam"] == null)
			{
				SerializedActor.Nodes.Add(game__SoundShapeBancParam);
			}
		}
	}
}
