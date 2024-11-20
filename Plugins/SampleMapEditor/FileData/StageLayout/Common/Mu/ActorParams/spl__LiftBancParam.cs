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
	public class Mu_spl__LiftBancParam
	{
		[ByamlMember]
		public bool IsNotReplaceMapPart { get; set; }

		public Mu_spl__LiftBancParam()
		{
			IsNotReplaceMapPart = false;
		}

		public Mu_spl__LiftBancParam(Mu_spl__LiftBancParam other)
		{
			IsNotReplaceMapPart = other.IsNotReplaceMapPart;
		}

		public Mu_spl__LiftBancParam Clone()
		{
			return new Mu_spl__LiftBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LiftBancParam = new BymlNode.DictionaryNode() { Name = "spl__LiftBancParam" };

			if (SerializedActor["spl__LiftBancParam"] != null)
			{
				spl__LiftBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LiftBancParam"];
			}


			if (this.IsNotReplaceMapPart)
			{
				spl__LiftBancParam.AddNode("IsNotReplaceMapPart", this.IsNotReplaceMapPart);
			}

			if (SerializedActor["spl__LiftBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LiftBancParam);
			}
		}
	}
}
