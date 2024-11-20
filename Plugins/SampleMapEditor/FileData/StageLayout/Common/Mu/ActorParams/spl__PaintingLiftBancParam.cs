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
	public class Mu_spl__PaintingLiftBancParam
	{
		[ByamlMember]
		public float PaintRepeatFrame { get; set; }

		public Mu_spl__PaintingLiftBancParam()
		{
			PaintRepeatFrame = 0.0f;
		}

		public Mu_spl__PaintingLiftBancParam(Mu_spl__PaintingLiftBancParam other)
		{
			PaintRepeatFrame = other.PaintRepeatFrame;
		}

		public Mu_spl__PaintingLiftBancParam Clone()
		{
			return new Mu_spl__PaintingLiftBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PaintingLiftBancParam = new BymlNode.DictionaryNode() { Name = "spl__PaintingLiftBancParam" };

			if (SerializedActor["spl__PaintingLiftBancParam"] != null)
			{
				spl__PaintingLiftBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PaintingLiftBancParam"];
			}


			spl__PaintingLiftBancParam.AddNode("PaintRepeatFrame", this.PaintRepeatFrame);

			if (SerializedActor["spl__PaintingLiftBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PaintingLiftBancParam);
			}
		}
	}
}
