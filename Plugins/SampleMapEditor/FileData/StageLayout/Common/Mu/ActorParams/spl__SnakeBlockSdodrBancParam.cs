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
	public class Mu_spl__SnakeBlockSdodrBancParam
	{
		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__SnakeBlockSdodrBancParam()
		{
			ToRailPoint = 0;
		}

		public Mu_spl__SnakeBlockSdodrBancParam(Mu_spl__SnakeBlockSdodrBancParam other)
		{
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__SnakeBlockSdodrBancParam Clone()
		{
			return new Mu_spl__SnakeBlockSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SnakeBlockSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__SnakeBlockSdodrBancParam" };

			if (SerializedActor["spl__SnakeBlockSdodrBancParam"] != null)
			{
				spl__SnakeBlockSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SnakeBlockSdodrBancParam"];
			}


			spl__SnakeBlockSdodrBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__SnakeBlockSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SnakeBlockSdodrBancParam);
			}
		}
	}
}
