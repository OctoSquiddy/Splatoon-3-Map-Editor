using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SandPlazaBandLocator : MuObj
	{
		[BindGUI("Part", Category = "SandPlazaBandLocator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Part
		{
			get
			{
				return this.spl__PlazaSandBandLocatorParam.Part;
			}

			set
			{
				this.spl__PlazaSandBandLocatorParam.Part = value;
			}
		}

		[BindGUI("StageType", Category = "SandPlazaBandLocator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _StageType
		{
			get
			{
				return this.spl__PlazaSandBandLocatorParam.StageType;
			}

			set
			{
				this.spl__PlazaSandBandLocatorParam.StageType = value;
			}
		}

		[ByamlMember("spl__PlazaSandBandLocatorParam")]
		public Mu_spl__PlazaSandBandLocatorParam spl__PlazaSandBandLocatorParam { get; set; }

		public SandPlazaBandLocator() : base()
		{
			spl__PlazaSandBandLocatorParam = new Mu_spl__PlazaSandBandLocatorParam();

			Links = new List<Link>();
		}

		public SandPlazaBandLocator(SandPlazaBandLocator other) : base(other)
		{
			spl__PlazaSandBandLocatorParam = other.spl__PlazaSandBandLocatorParam.Clone();
		}

		public override SandPlazaBandLocator Clone()
		{
			return new SandPlazaBandLocator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PlazaSandBandLocatorParam.SaveParameterBank(SerializedActor);
		}
	}
}
