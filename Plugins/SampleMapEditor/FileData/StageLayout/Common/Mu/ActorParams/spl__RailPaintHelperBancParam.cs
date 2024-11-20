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
	public class Mu_spl__RailPaintHelperBancParam
	{
		[ByamlMember]
		public bool IsCancel { get; set; }

		[ByamlMember]
		public ulong PaintFrame { get; set; }

		[ByamlMember]
		public float Width { get; set; }

		public Mu_spl__RailPaintHelperBancParam()
		{
			IsCancel = false;
			PaintFrame = 0;
			Width = 0.0f;
		}

		public Mu_spl__RailPaintHelperBancParam(Mu_spl__RailPaintHelperBancParam other)
		{
			IsCancel = other.IsCancel;
			PaintFrame = other.PaintFrame;
			Width = other.Width;
		}

		public Mu_spl__RailPaintHelperBancParam Clone()
		{
			return new Mu_spl__RailPaintHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RailPaintHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__RailPaintHelperBancParam" };

			if (SerializedActor["spl__RailPaintHelperBancParam"] != null)
			{
				spl__RailPaintHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RailPaintHelperBancParam"];
			}


			if (this.IsCancel)
			{
				spl__RailPaintHelperBancParam.AddNode("IsCancel", this.IsCancel);
			}

			spl__RailPaintHelperBancParam.AddNode("PaintFrame", this.PaintFrame);

			spl__RailPaintHelperBancParam.AddNode("Width", this.Width);

			if (SerializedActor["spl__RailPaintHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RailPaintHelperBancParam);
			}
		}
	}
}
