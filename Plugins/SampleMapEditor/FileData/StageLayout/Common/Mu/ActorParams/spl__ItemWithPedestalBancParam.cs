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
	public class Mu_spl__ItemWithPedestalBancParam
	{
		[ByamlMember]
		public int PlacementID { get; set; }

		public Mu_spl__ItemWithPedestalBancParam()
		{
			PlacementID = 0;
		}

		public Mu_spl__ItemWithPedestalBancParam(Mu_spl__ItemWithPedestalBancParam other)
		{
			PlacementID = other.PlacementID;
		}

		public Mu_spl__ItemWithPedestalBancParam Clone()
		{
			return new Mu_spl__ItemWithPedestalBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ItemWithPedestalBancParam = new BymlNode.DictionaryNode() { Name = "spl__ItemWithPedestalBancParam" };

			if (SerializedActor["spl__ItemWithPedestalBancParam"] != null)
			{
				spl__ItemWithPedestalBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ItemWithPedestalBancParam"];
			}


			spl__ItemWithPedestalBancParam.AddNode("PlacementID", this.PlacementID);

			if (SerializedActor["spl__ItemWithPedestalBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ItemWithPedestalBancParam);
			}
		}
	}
}
