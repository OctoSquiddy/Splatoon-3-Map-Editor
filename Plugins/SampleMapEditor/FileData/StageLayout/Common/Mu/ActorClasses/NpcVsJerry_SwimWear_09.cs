using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NpcVsJerry_SwimWear_09 : MuObj
	{
		[BindGUI("IsOceanBind", Category = "NpcVsJerry_SwimWear_09 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsOceanBind
		{
			get
			{
				return this.spl__OceanBindableHelperBancParam.IsOceanBind;
			}

			set
			{
				this.spl__OceanBindableHelperBancParam.IsOceanBind = value;
			}
		}

		[BindGUI("Ratio", Category = "NpcVsJerry_SwimWear_09 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Ratio
		{
			get
			{
				return this.spl__OceanBindableHelperBancParam.Ratio;
			}

			set
			{
				this.spl__OceanBindableHelperBancParam.Ratio = value;
			}
		}

		[ByamlMember("spl__OceanBindableHelperBancParam")]
		public Mu_spl__OceanBindableHelperBancParam spl__OceanBindableHelperBancParam { get; set; }

		public NpcVsJerry_SwimWear_09() : base()
		{
			spl__OceanBindableHelperBancParam = new Mu_spl__OceanBindableHelperBancParam();

			Links = new List<Link>();
		}

		public NpcVsJerry_SwimWear_09(NpcVsJerry_SwimWear_09 other) : base(other)
		{
			spl__OceanBindableHelperBancParam = other.spl__OceanBindableHelperBancParam.Clone();
		}

		public override NpcVsJerry_SwimWear_09 Clone()
		{
			return new NpcVsJerry_SwimWear_09(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__OceanBindableHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
