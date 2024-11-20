using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Npc_KnickKnacksShop_Sdodr : MuObj
	{
		[ByamlMember("spl__ShopBindableBancParam")]
		public Mu_spl__ShopBindableBancParam spl__ShopBindableBancParam { get; set; }

		public Npc_KnickKnacksShop_Sdodr() : base()
		{
			spl__ShopBindableBancParam = new Mu_spl__ShopBindableBancParam();

			Links = new List<Link>();
		}

		public Npc_KnickKnacksShop_Sdodr(Npc_KnickKnacksShop_Sdodr other) : base(other)
		{
			spl__ShopBindableBancParam = other.spl__ShopBindableBancParam.Clone();
		}

		public override Npc_KnickKnacksShop_Sdodr Clone()
		{
			return new Npc_KnickKnacksShop_Sdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ShopBindableBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
