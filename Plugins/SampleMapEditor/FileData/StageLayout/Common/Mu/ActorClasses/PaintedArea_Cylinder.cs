using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class PaintedArea_Cylinder : MuObj
	{
		[BindGUI("PaintFrame", Category = "PaintedArea_Cylinder Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _PaintFrame
		{
			get
			{
				return this.spl__PaintedAreaBancParam.PaintFrame;
			}

			set
			{
				this.spl__PaintedAreaBancParam.PaintFrame = value;
			}
		}

		[ByamlMember("spl__PaintedAreaBancParam")]
		public Mu_spl__PaintedAreaBancParam spl__PaintedAreaBancParam { get; set; }

		public PaintedArea_Cylinder() : base()
		{
			spl__PaintedAreaBancParam = new Mu_spl__PaintedAreaBancParam();

			Links = new List<Link>();
		}

		public PaintedArea_Cylinder(PaintedArea_Cylinder other) : base(other)
		{
			spl__PaintedAreaBancParam = other.spl__PaintedAreaBancParam.Clone();
		}

		public override PaintedArea_Cylinder Clone()
		{
			return new PaintedArea_Cylinder(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PaintedAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
