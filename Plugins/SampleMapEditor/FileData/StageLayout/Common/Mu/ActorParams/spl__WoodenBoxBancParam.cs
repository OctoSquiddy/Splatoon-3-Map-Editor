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
	public class Mu_spl__WoodenBoxBancParam
	{
		[ByamlMember]
		public int IkuraNum { get; set; }

		[ByamlMember]
		public int IkuraValue { get; set; }

		[ByamlMember]
		public bool IsEnemyBreakMiss { get; set; }

		[ByamlMember]
		public bool IsKinematic { get; set; }

		[ByamlMember]
		public int TimeLimit { get; set; }

		public Mu_spl__WoodenBoxBancParam()
		{
			IkuraNum = 0;
			IkuraValue = 0;
			IsEnemyBreakMiss = false;
			IsKinematic = false;
			TimeLimit = 0;
		}

		public Mu_spl__WoodenBoxBancParam(Mu_spl__WoodenBoxBancParam other)
		{
			IkuraNum = other.IkuraNum;
			IkuraValue = other.IkuraValue;
			IsEnemyBreakMiss = other.IsEnemyBreakMiss;
			IsKinematic = other.IsKinematic;
			TimeLimit = other.TimeLimit;
		}

		public Mu_spl__WoodenBoxBancParam Clone()
		{
			return new Mu_spl__WoodenBoxBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__WoodenBoxBancParam = new BymlNode.DictionaryNode() { Name = "spl__WoodenBoxBancParam" };

			if (SerializedActor["spl__WoodenBoxBancParam"] != null)
			{
				spl__WoodenBoxBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__WoodenBoxBancParam"];
			}


			spl__WoodenBoxBancParam.AddNode("IkuraNum", this.IkuraNum);

			spl__WoodenBoxBancParam.AddNode("IkuraValue", this.IkuraValue);

			if (this.IsEnemyBreakMiss)
			{
				spl__WoodenBoxBancParam.AddNode("IsEnemyBreakMiss", this.IsEnemyBreakMiss);
			}

			if (this.IsKinematic)
			{
				spl__WoodenBoxBancParam.AddNode("IsKinematic", this.IsKinematic);
			}

			if (this.TimeLimit > 0)
				spl__WoodenBoxBancParam.AddNode("TimeLimit", this.TimeLimit);

			if (SerializedActor["spl__WoodenBoxBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__WoodenBoxBancParam);
			}
		}
	}
}
