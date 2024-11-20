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
	public class Mu_spl__SnakeBlockBancParam
	{
		[ByamlMember]
		public float DamagePerExtension { get; set; }

		[ByamlMember]
		public int ShrinkNoDamageFrame { get; set; }

		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__SnakeBlockBancParam()
		{
			DamagePerExtension = 0.0f;
			ShrinkNoDamageFrame = 0;
			ToRailPoint = 0;
		}

		public Mu_spl__SnakeBlockBancParam(Mu_spl__SnakeBlockBancParam other)
		{
			DamagePerExtension = other.DamagePerExtension;
			ShrinkNoDamageFrame = other.ShrinkNoDamageFrame;
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__SnakeBlockBancParam Clone()
		{
			return new Mu_spl__SnakeBlockBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SnakeBlockBancParam = new BymlNode.DictionaryNode() { Name = "spl__SnakeBlockBancParam" };

			if (SerializedActor["spl__SnakeBlockBancParam"] != null)
			{
				spl__SnakeBlockBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SnakeBlockBancParam"];
			}


			spl__SnakeBlockBancParam.AddNode("DamagePerExtension", this.DamagePerExtension);

			spl__SnakeBlockBancParam.AddNode("ShrinkNoDamageFrame", this.ShrinkNoDamageFrame);

			spl__SnakeBlockBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__SnakeBlockBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SnakeBlockBancParam);
			}
		}
	}
}
