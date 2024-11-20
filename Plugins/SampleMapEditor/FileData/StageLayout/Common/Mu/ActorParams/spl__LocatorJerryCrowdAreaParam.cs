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
	public class Mu_spl__LocatorJerryCrowdAreaParam
	{
		[ByamlMember]
		public float PlayerHumanMoveRt { get; set; }

		public Mu_spl__LocatorJerryCrowdAreaParam()
		{
			PlayerHumanMoveRt = 0.0f;
		}

		public Mu_spl__LocatorJerryCrowdAreaParam(Mu_spl__LocatorJerryCrowdAreaParam other)
		{
			PlayerHumanMoveRt = other.PlayerHumanMoveRt;
		}

		public Mu_spl__LocatorJerryCrowdAreaParam Clone()
		{
			return new Mu_spl__LocatorJerryCrowdAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorJerryCrowdAreaParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorJerryCrowdAreaParam" };

			if (SerializedActor["spl__LocatorJerryCrowdAreaParam"] != null)
			{
				spl__LocatorJerryCrowdAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorJerryCrowdAreaParam"];
			}


			spl__LocatorJerryCrowdAreaParam.AddNode("PlayerHumanMoveRt", this.PlayerHumanMoveRt);

			if (SerializedActor["spl__LocatorJerryCrowdAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorJerryCrowdAreaParam);
			}
		}
	}
}
