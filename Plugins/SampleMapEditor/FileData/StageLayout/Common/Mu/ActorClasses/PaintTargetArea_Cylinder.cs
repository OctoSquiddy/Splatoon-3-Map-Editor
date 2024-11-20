using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class PaintTargetArea_Cylinder : MuObj
	{
		[ByamlMember("spl__PaintTargetAreaBancParam")]
		public Mu_spl__PaintTargetAreaBancParam spl__PaintTargetAreaBancParam { get; set; }

		public PaintTargetArea_Cylinder() : base()
		{
			spl__PaintTargetAreaBancParam = new Mu_spl__PaintTargetAreaBancParam();

			Links = new List<Link>();
		}

		public PaintTargetArea_Cylinder(PaintTargetArea_Cylinder other) : base(other)
		{
			spl__PaintTargetAreaBancParam = other.spl__PaintTargetAreaBancParam.Clone();
		}

		public override PaintTargetArea_Cylinder Clone()
		{
			return new PaintTargetArea_Cylinder(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PaintTargetAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
