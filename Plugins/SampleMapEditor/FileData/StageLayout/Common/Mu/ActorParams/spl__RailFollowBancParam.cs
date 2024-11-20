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
	public class Mu_spl__RailFollowBancParam
	{
		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__RailFollowBancParam()
		{
			ToRailPoint = 0;
		}

		public Mu_spl__RailFollowBancParam(Mu_spl__RailFollowBancParam other)
		{
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__RailFollowBancParam Clone()
		{
			return new Mu_spl__RailFollowBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RailFollowBancParam = new BymlNode.DictionaryNode() { Name = "spl__RailFollowBancParam" };

			if (SerializedActor["spl__RailFollowBancParam"] != null)
			{
				spl__RailFollowBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RailFollowBancParam"];
			}


			spl__RailFollowBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__RailFollowBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RailFollowBancParam);
			}
		}
	}
}
