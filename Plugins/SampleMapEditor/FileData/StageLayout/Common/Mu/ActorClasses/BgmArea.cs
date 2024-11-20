using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class BgmArea : MuObj
	{
		[BindGUI("Key", Category = "BgmArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Key
		{
			get
			{
				return this.spl__BgmAreaBancParam.Key;
			}

			set
			{
				this.spl__BgmAreaBancParam.Key = value;
			}
		}

		[BindGUI("Margin", Category = "BgmArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Margin
		{
			get
			{
				return this.spl__BgmAreaBancParam.Margin;
			}

			set
			{
				this.spl__BgmAreaBancParam.Margin = value;
			}
		}

		[ByamlMember("spl__BgmAreaBancParam")]
		public Mu_spl__BgmAreaBancParam spl__BgmAreaBancParam { get; set; }

		public BgmArea() : base()
		{
			spl__BgmAreaBancParam = new Mu_spl__BgmAreaBancParam();

			Links = new List<Link>();
		}

		public BgmArea(BgmArea other) : base(other)
		{
			spl__BgmAreaBancParam = other.spl__BgmAreaBancParam.Clone();
		}

		public override BgmArea Clone()
		{
			return new BgmArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__BgmAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
