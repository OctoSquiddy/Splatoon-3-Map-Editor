using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class PaintTargetAreaManagerSdodr : MuObj
	{
		[BindGUI("CountCheckPoint0", Category = "PaintTargetAreaManagerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CountCheckPoint0
		{
			get
			{
				return this.spl__PaintTargetAreaManagerBancParamSdodr.CountCheckPoint0;
			}

			set
			{
				this.spl__PaintTargetAreaManagerBancParamSdodr.CountCheckPoint0 = value;
			}
		}

		[BindGUI("CountCheckPoint1", Category = "PaintTargetAreaManagerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CountCheckPoint1
		{
			get
			{
				return this.spl__PaintTargetAreaManagerBancParamSdodr.CountCheckPoint1;
			}

			set
			{
				this.spl__PaintTargetAreaManagerBancParamSdodr.CountCheckPoint1 = value;
			}
		}

		[BindGUI("CountFirst", Category = "PaintTargetAreaManagerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CountFirst
		{
			get
			{
				return this.spl__PaintTargetAreaManagerBancParamSdodr.CountFirst;
			}

			set
			{
				this.spl__PaintTargetAreaManagerBancParamSdodr.CountFirst = value;
			}
		}

		[ByamlMember("spl__PaintTargetAreaManagerBancParamSdodr")]
		public Mu_spl__PaintTargetAreaManagerBancParamSdodr spl__PaintTargetAreaManagerBancParamSdodr { get; set; }

		public PaintTargetAreaManagerSdodr() : base()
		{
			spl__PaintTargetAreaManagerBancParamSdodr = new Mu_spl__PaintTargetAreaManagerBancParamSdodr();

			Links = new List<Link>();
		}

		public PaintTargetAreaManagerSdodr(PaintTargetAreaManagerSdodr other) : base(other)
		{
			spl__PaintTargetAreaManagerBancParamSdodr = other.spl__PaintTargetAreaManagerBancParamSdodr.Clone();
		}

		public override PaintTargetAreaManagerSdodr Clone()
		{
			return new PaintTargetAreaManagerSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PaintTargetAreaManagerBancParamSdodr.SaveParameterBank(SerializedActor);
		}
	}
}
