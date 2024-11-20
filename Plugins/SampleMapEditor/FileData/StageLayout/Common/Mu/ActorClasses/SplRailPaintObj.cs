using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplRailPaintObj : MuObj
	{
		[BindGUI("PaintFrame", Category = "SplRailPaintObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _PaintFrame
		{
			get
			{
				return this.spl__RailPaintObjBancParam.PaintFrame;
			}

			set
			{
				this.spl__RailPaintObjBancParam.PaintFrame = value;
			}
		}

		[BindGUI("Width", Category = "SplRailPaintObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Width
		{
			get
			{
				return this.spl__RailPaintObjBancParam.Width;
			}

			set
			{
				this.spl__RailPaintObjBancParam.Width = value;
			}
		}

		[ByamlMember("spl__RailPaintObjBancParam")]
		public Mu_spl__RailPaintObjBancParam spl__RailPaintObjBancParam { get; set; }

		public SplRailPaintObj() : base()
		{
			spl__RailPaintObjBancParam = new Mu_spl__RailPaintObjBancParam();

			Links = new List<Link>();
		}

		public SplRailPaintObj(SplRailPaintObj other) : base(other)
		{
			spl__RailPaintObjBancParam = other.spl__RailPaintObjBancParam.Clone();
		}

		public override SplRailPaintObj Clone()
		{
			return new SplRailPaintObj(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RailPaintObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
