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
	public class Mu_spl__EventActorStateBancParam
	{
		[ByamlMember]
		public bool IsEventOnly { get; set; }

		public Mu_spl__EventActorStateBancParam()
		{
			IsEventOnly = false;
		}

		public Mu_spl__EventActorStateBancParam(Mu_spl__EventActorStateBancParam other)
		{
			IsEventOnly = other.IsEventOnly;
		}

		public Mu_spl__EventActorStateBancParam Clone()
		{
			return new Mu_spl__EventActorStateBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EventActorStateBancParam = new BymlNode.DictionaryNode() { Name = "spl__EventActorStateBancParam" };

			if (SerializedActor["spl__EventActorStateBancParam"] != null)
			{
				spl__EventActorStateBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EventActorStateBancParam"];
			}


			if (this.IsEventOnly)
			{
				spl__EventActorStateBancParam.AddNode("IsEventOnly", this.IsEventOnly);
			}

			if (SerializedActor["spl__EventActorStateBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EventActorStateBancParam);
			}
		}
	}
}
