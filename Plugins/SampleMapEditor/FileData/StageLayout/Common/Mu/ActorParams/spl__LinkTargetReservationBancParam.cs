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
	public class Mu_spl__LinkTargetReservationBancParam
	{
		[ByamlMember]
		public string ReserveNumTable { get; set; }

		public Mu_spl__LinkTargetReservationBancParam()
		{
			ReserveNumTable = "";
		}

		public Mu_spl__LinkTargetReservationBancParam(Mu_spl__LinkTargetReservationBancParam other)
		{
			ReserveNumTable = other.ReserveNumTable;
		}

		public Mu_spl__LinkTargetReservationBancParam Clone()
		{
			return new Mu_spl__LinkTargetReservationBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LinkTargetReservationBancParam = new BymlNode.DictionaryNode() { Name = "spl__LinkTargetReservationBancParam" };

			if (SerializedActor["spl__LinkTargetReservationBancParam"] != null)
			{
				spl__LinkTargetReservationBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LinkTargetReservationBancParam"];
			}


			if (this.ReserveNumTable != "")
			{
				spl__LinkTargetReservationBancParam.AddNode("ReserveNumTable", this.ReserveNumTable);
			}

			if (SerializedActor["spl__LinkTargetReservationBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LinkTargetReservationBancParam);
			}
		}
	}
}
