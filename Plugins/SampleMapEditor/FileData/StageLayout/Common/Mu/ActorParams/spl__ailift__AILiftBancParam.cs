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
	public class Mu_spl__ailift__AILiftBancParam
	{
		[ByamlMember]
		public bool IsEnableMoveChatteringPrevent { get; set; }

		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__ailift__AILiftBancParam()
		{
			IsEnableMoveChatteringPrevent = false;
			ToRailPoint = 0;
		}

		public Mu_spl__ailift__AILiftBancParam(Mu_spl__ailift__AILiftBancParam other)
		{
			IsEnableMoveChatteringPrevent = other.IsEnableMoveChatteringPrevent;
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__ailift__AILiftBancParam Clone()
		{
			return new Mu_spl__ailift__AILiftBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ailift__AILiftBancParam = new BymlNode.DictionaryNode() { Name = "spl__ailift__AILiftBancParam" };

			if (SerializedActor["spl__ailift__AILiftBancParam"] != null)
			{
				spl__ailift__AILiftBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ailift__AILiftBancParam"];
			}

			if (this.IsEnableMoveChatteringPrevent)
			{
				spl__ailift__AILiftBancParam.AddNode("IsEnableMoveChatteringPrevent", this.IsEnableMoveChatteringPrevent);
			}

            spl__ailift__AILiftBancParam.AddNode("ToRailPoint", this.ToRailPoint);

            if (SerializedActor["spl__ailift__AILiftBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ailift__AILiftBancParam);
			}
		}
	}
}
