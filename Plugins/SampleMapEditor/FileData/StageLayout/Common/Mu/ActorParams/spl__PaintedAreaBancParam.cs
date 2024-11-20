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
	public class Mu_spl__PaintedAreaBancParam
	{
		[ByamlMember]
		public ulong PaintFrame { get; set; }

		public Mu_spl__PaintedAreaBancParam()
		{
			PaintFrame = 0;
		}

		public Mu_spl__PaintedAreaBancParam(Mu_spl__PaintedAreaBancParam other)
		{
			PaintFrame = other.PaintFrame;
		}

		public Mu_spl__PaintedAreaBancParam Clone()
		{
			return new Mu_spl__PaintedAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PaintedAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__PaintedAreaBancParam" };

			if (SerializedActor["spl__PaintedAreaBancParam"] != null)
			{
				spl__PaintedAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PaintedAreaBancParam"];
			}


			spl__PaintedAreaBancParam.AddNode("PaintFrame", this.PaintFrame);

			if (SerializedActor["spl__PaintedAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PaintedAreaBancParam);
			}
		}
	}
}
