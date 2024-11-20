using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SandPlazaStageLocator : MuObj
	{
		[BindGUI("StageType", Category = "SandPlazaStageLocator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _StageType
		{
			get
			{
				return this.spl__PlazaSandStageLocatorParam.StageType;
			}

			set
			{
				this.spl__PlazaSandStageLocatorParam.StageType = value;
			}
		}

		[ByamlMember("spl__PlazaSandStageLocatorParam")]
		public Mu_spl__PlazaSandStageLocatorParam spl__PlazaSandStageLocatorParam { get; set; }

		public SandPlazaStageLocator() : base()
		{
			spl__PlazaSandStageLocatorParam = new Mu_spl__PlazaSandStageLocatorParam();

			Links = new List<Link>();
		}

		public SandPlazaStageLocator(SandPlazaStageLocator other) : base(other)
		{
			spl__PlazaSandStageLocatorParam = other.spl__PlazaSandStageLocatorParam.Clone();
		}

		public override SandPlazaStageLocator Clone()
		{
			return new SandPlazaStageLocator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PlazaSandStageLocatorParam.SaveParameterBank(SerializedActor);
		}
	}
}
