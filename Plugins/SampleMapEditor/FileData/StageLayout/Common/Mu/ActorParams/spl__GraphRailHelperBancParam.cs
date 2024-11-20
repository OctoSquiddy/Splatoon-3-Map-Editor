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
	public class Mu_spl__GraphRailHelperBancParam
	{
		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__GraphRailHelperBancParam()
		{
			ToRailPoint = 0;
		}

		public Mu_spl__GraphRailHelperBancParam(Mu_spl__GraphRailHelperBancParam other)
		{
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__GraphRailHelperBancParam Clone()
		{
			return new Mu_spl__GraphRailHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GraphRailHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__GraphRailHelperBancParam" };

			if (SerializedActor["spl__GraphRailHelperBancParam"] != null)
			{
				spl__GraphRailHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GraphRailHelperBancParam"];
			}


			spl__GraphRailHelperBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__GraphRailHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GraphRailHelperBancParam);
			}
		}
	}
}
