using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class PaintTargetArea_Cube : MuObj
	{
		[ByamlMember("spl__PaintTargetAreaBancParam")]
		public Mu_spl__PaintTargetAreaBancParam spl__PaintTargetAreaBancParam { get; set; }

		public PaintTargetArea_Cube() : base()
		{
			spl__PaintTargetAreaBancParam = new Mu_spl__PaintTargetAreaBancParam();

			Links = new List<Link>();
		}

		public PaintTargetArea_Cube(PaintTargetArea_Cube other) : base(other)
		{
			spl__PaintTargetAreaBancParam = other.spl__PaintTargetAreaBancParam.Clone();
		}

		public override PaintTargetArea_Cube Clone()
		{
			return new PaintTargetArea_Cube(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PaintTargetAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
