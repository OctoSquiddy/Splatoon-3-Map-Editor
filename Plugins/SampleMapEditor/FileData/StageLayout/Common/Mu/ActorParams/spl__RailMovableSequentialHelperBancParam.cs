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
	public class Mu_spl__RailMovableSequentialHelperBancParam
	{
		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__RailMovableSequentialHelperBancParam()
		{
			ToRailPoint = 0;
		}

		public Mu_spl__RailMovableSequentialHelperBancParam(Mu_spl__RailMovableSequentialHelperBancParam other)
		{
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__RailMovableSequentialHelperBancParam Clone()
		{
			return new Mu_spl__RailMovableSequentialHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RailMovableSequentialHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__RailMovableSequentialHelperBancParam" };

			if (SerializedActor["spl__RailMovableSequentialHelperBancParam"] != null)
			{
				spl__RailMovableSequentialHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RailMovableSequentialHelperBancParam"];
			}


			spl__RailMovableSequentialHelperBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__RailMovableSequentialHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RailMovableSequentialHelperBancParam);
			}
		}
	}
}
