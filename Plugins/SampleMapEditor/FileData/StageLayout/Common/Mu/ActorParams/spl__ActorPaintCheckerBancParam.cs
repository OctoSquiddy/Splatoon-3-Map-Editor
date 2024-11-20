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
	public class Mu_spl__ActorPaintCheckerBancParam
	{
		[ByamlMember]
		public float OffRate { get; set; }

		[ByamlMember]
		public float OnRate { get; set; }

		public Mu_spl__ActorPaintCheckerBancParam()
		{
			OffRate = 0.0f;
			OnRate = 0.0f;
		}

		public Mu_spl__ActorPaintCheckerBancParam(Mu_spl__ActorPaintCheckerBancParam other)
		{
			OffRate = other.OffRate;
			OnRate = other.OnRate;
		}

		public Mu_spl__ActorPaintCheckerBancParam Clone()
		{
			return new Mu_spl__ActorPaintCheckerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ActorPaintCheckerBancParam = new BymlNode.DictionaryNode() { Name = "spl__ActorPaintCheckerBancParam" };

			if (SerializedActor["spl__ActorPaintCheckerBancParam"] != null)
			{
				spl__ActorPaintCheckerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ActorPaintCheckerBancParam"];
			}


			spl__ActorPaintCheckerBancParam.AddNode("OffRate", this.OffRate);

			spl__ActorPaintCheckerBancParam.AddNode("OnRate", this.OnRate);

			if (SerializedActor["spl__ActorPaintCheckerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ActorPaintCheckerBancParam);
			}
		}
	}
}
