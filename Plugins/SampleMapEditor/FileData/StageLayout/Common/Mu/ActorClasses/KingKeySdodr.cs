using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class KingKeySdodr : MuObj
	{
		[BindGUI("BossType", Category = "KingKeySdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _BossType
		{
			get
			{
				return this.spl__KingKeyBancParamSdodr.BossType;
			}

			set
			{
				this.spl__KingKeyBancParamSdodr.BossType = value;
			}
		}

		[ByamlMember("spl__KingKeyBancParamSdodr")]
		public Mu_spl__KingKeyBancParamSdodr spl__KingKeyBancParamSdodr { get; set; }

		public KingKeySdodr() : base()
		{
			spl__KingKeyBancParamSdodr = new Mu_spl__KingKeyBancParamSdodr();

			Links = new List<Link>();
		}

		public KingKeySdodr(KingKeySdodr other) : base(other)
		{
			spl__KingKeyBancParamSdodr = other.spl__KingKeyBancParamSdodr.Clone();
		}

		public override KingKeySdodr Clone()
		{
			return new KingKeySdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__KingKeyBancParamSdodr.SaveParameterBank(SerializedActor);
		}
	}
}
