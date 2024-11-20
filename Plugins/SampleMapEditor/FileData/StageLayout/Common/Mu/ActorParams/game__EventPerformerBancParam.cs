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
	public class Mu_game__EventPerformerBancParam
	{
		[ByamlMember]
		public bool IsActivateOnlyInBeingPerformer { get; set; }

		public Mu_game__EventPerformerBancParam()
		{
			IsActivateOnlyInBeingPerformer = false;
		}

		public Mu_game__EventPerformerBancParam(Mu_game__EventPerformerBancParam other)
		{
			IsActivateOnlyInBeingPerformer = other.IsActivateOnlyInBeingPerformer;
		}

		public Mu_game__EventPerformerBancParam Clone()
		{
			return new Mu_game__EventPerformerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__EventPerformerBancParam = new BymlNode.DictionaryNode() { Name = "game__EventPerformerBancParam" };

			if (SerializedActor["game__EventPerformerBancParam"] != null)
			{
				game__EventPerformerBancParam = (BymlNode.DictionaryNode)SerializedActor["game__EventPerformerBancParam"];
			}


			if (this.IsActivateOnlyInBeingPerformer)
			{
				game__EventPerformerBancParam.AddNode("IsActivateOnlyInBeingPerformer", this.IsActivateOnlyInBeingPerformer);
			}

			if (SerializedActor["game__EventPerformerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(game__EventPerformerBancParam);
			}
		}
	}
}
