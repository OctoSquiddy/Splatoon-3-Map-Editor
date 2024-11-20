using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DuckingAreaSAND : MuObj
	{
		[BindGUI("DuckingName", Category = "DuckingAreaSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _DuckingName
		{
			get
			{
				return this.spl__DuckingAreaSandBancParam.DuckingName;
			}

			set
			{
				this.spl__DuckingAreaSandBancParam.DuckingName = value;
			}
		}

		[BindGUI("Margin", Category = "DuckingAreaSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Margin
		{
			get
			{
				return this.spl__DuckingAreaSandBancParam.Margin;
			}

			set
			{
				this.spl__DuckingAreaSandBancParam.Margin = value;
			}
		}

		[BindGUI("StageType", Category = "DuckingAreaSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _StageType
		{
			get
			{
				return this.spl__DuckingAreaSandBancParam.StageType;
			}

			set
			{
				this.spl__DuckingAreaSandBancParam.StageType = value;
			}
		}

		[ByamlMember("spl__DuckingAreaSandBancParam")]
		public Mu_spl__DuckingAreaSandBancParam spl__DuckingAreaSandBancParam { get; set; }

		public DuckingAreaSAND() : base()
		{
			spl__DuckingAreaSandBancParam = new Mu_spl__DuckingAreaSandBancParam();

			Links = new List<Link>();
		}

		public DuckingAreaSAND(DuckingAreaSAND other) : base(other)
		{
			spl__DuckingAreaSandBancParam = other.spl__DuckingAreaSandBancParam.Clone();
		}

		public override DuckingAreaSAND Clone()
		{
			return new DuckingAreaSAND(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__DuckingAreaSandBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
