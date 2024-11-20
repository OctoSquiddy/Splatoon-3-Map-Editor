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
	public class Mu_spl__ActorMatrixBindableHelperBancParam
	{
		[ByamlMember]
		public bool IsFreeY { get; set; }

		public Mu_spl__ActorMatrixBindableHelperBancParam()
		{
			IsFreeY = false;
		}

		public Mu_spl__ActorMatrixBindableHelperBancParam(Mu_spl__ActorMatrixBindableHelperBancParam other)
		{
			IsFreeY = other.IsFreeY;
		}

		public Mu_spl__ActorMatrixBindableHelperBancParam Clone()
		{
			return new Mu_spl__ActorMatrixBindableHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ActorMatrixBindableHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__ActorMatrixBindableHelperBancParam" };

			if (SerializedActor["spl__ActorMatrixBindableHelperBancParam"] != null)
			{
				spl__ActorMatrixBindableHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ActorMatrixBindableHelperBancParam"];
			}


			if (this.IsFreeY)
			{
				spl__ActorMatrixBindableHelperBancParam.AddNode("IsFreeY", this.IsFreeY);
			}

			if (SerializedActor["spl__ActorMatrixBindableHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ActorMatrixBindableHelperBancParam);
			}
		}
	}
}
