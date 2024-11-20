using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopSpawnGeyser : MuObj
	{
		[BindGUI("FlgFarBank", Category = "CoopSpawnGeyser Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _FlgFarBank
		{
			get
			{
				return this.spl__CoopSpawnGeyserBancParam.FlgFarBank;
			}

			set
			{
				this.spl__CoopSpawnGeyserBancParam.FlgFarBank = value;
			}
		}

		[ByamlMember("spl__CoopSpawnGeyserBancParam")]
		public Mu_spl__CoopSpawnGeyserBancParam spl__CoopSpawnGeyserBancParam { get; set; }

		public CoopSpawnGeyser() : base()
		{
			spl__CoopSpawnGeyserBancParam = new Mu_spl__CoopSpawnGeyserBancParam();

			Links = new List<Link>();
		}

		public CoopSpawnGeyser(CoopSpawnGeyser other) : base(other)
		{
			spl__CoopSpawnGeyserBancParam = other.spl__CoopSpawnGeyserBancParam.Clone();
		}

		public override CoopSpawnGeyser Clone()
		{
			return new CoopSpawnGeyser(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopSpawnGeyserBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
