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
	public class Mu_spl__ItemIkuraBottleBancParam
	{
		[ByamlMember]
		public int DropIkuraValue { get; set; }

		[ByamlMember]
		public int DropNum { get; set; }

		public Mu_spl__ItemIkuraBottleBancParam()
		{
			DropIkuraValue = 5;
			DropNum = 10;
		}

		public Mu_spl__ItemIkuraBottleBancParam(Mu_spl__ItemIkuraBottleBancParam other)
		{
			DropIkuraValue = other.DropIkuraValue;
			DropNum = other.DropNum;
		}

		public Mu_spl__ItemIkuraBottleBancParam Clone()
		{
			return new Mu_spl__ItemIkuraBottleBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ItemIkuraBottleBancParam = new BymlNode.DictionaryNode() { Name = "spl__ItemIkuraBottleBancParam" };

			if (SerializedActor["spl__ItemIkuraBottleBancParam"] != null)
			{
				spl__ItemIkuraBottleBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ItemIkuraBottleBancParam"];
			}


			spl__ItemIkuraBottleBancParam.AddNode("DropIkuraValue", this.DropIkuraValue);

			spl__ItemIkuraBottleBancParam.AddNode("DropNum", this.DropNum);

			if (SerializedActor["spl__ItemIkuraBottleBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ItemIkuraBottleBancParam);
			}
		}
	}
}
