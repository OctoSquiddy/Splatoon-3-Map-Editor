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
	public class Mu_spl__RandomDeactivatorBancParam
	{
		[ByamlMember]
		public string ActorTag { get; set; }

		[ByamlMember]
		public int MaxActiveNum { get; set; }

		public Mu_spl__RandomDeactivatorBancParam()
		{
			ActorTag = "";
			MaxActiveNum = 0;
		}

		public Mu_spl__RandomDeactivatorBancParam(Mu_spl__RandomDeactivatorBancParam other)
		{
			ActorTag = other.ActorTag;
			MaxActiveNum = other.MaxActiveNum;
		}

		public Mu_spl__RandomDeactivatorBancParam Clone()
		{
			return new Mu_spl__RandomDeactivatorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RandomDeactivatorBancParam = new BymlNode.DictionaryNode() { Name = "spl__RandomDeactivatorBancParam" };

			if (SerializedActor["spl__RandomDeactivatorBancParam"] != null)
			{
				spl__RandomDeactivatorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RandomDeactivatorBancParam"];
			}


			if (this.ActorTag != "")
			{
				spl__RandomDeactivatorBancParam.AddNode("ActorTag", this.ActorTag);
			}

			spl__RandomDeactivatorBancParam.AddNode("MaxActiveNum", this.MaxActiveNum);

			if (SerializedActor["spl__RandomDeactivatorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RandomDeactivatorBancParam);
			}
		}
	}
}
