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
	public class Mu_spl__RailPaintObjBancParam
	{
		[ByamlMember]
		public ulong PaintFrame { get; set; }

		[ByamlMember]
		public float Width { get; set; }

		public Mu_spl__RailPaintObjBancParam()
		{
			PaintFrame = 0;
			Width = 0.0f;
		}

		public Mu_spl__RailPaintObjBancParam(Mu_spl__RailPaintObjBancParam other)
		{
			PaintFrame = other.PaintFrame;
			Width = other.Width;
		}

		public Mu_spl__RailPaintObjBancParam Clone()
		{
			return new Mu_spl__RailPaintObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RailPaintObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__RailPaintObjBancParam" };

			if (SerializedActor["spl__RailPaintObjBancParam"] != null)
			{
				spl__RailPaintObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RailPaintObjBancParam"];
			}


			spl__RailPaintObjBancParam.AddNode("PaintFrame", this.PaintFrame);

			spl__RailPaintObjBancParam.AddNode("Width", this.Width);

			if (SerializedActor["spl__RailPaintObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RailPaintObjBancParam);
			}
		}
	}
}
