using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DObj_PlazaJerry_Zakka00_Fsodr : MuObj
	{
		[ByamlMember("spl__ShopBindableBancParam")]
		public Mu_spl__ShopBindableBancParam spl__ShopBindableBancParam { get; set; }

		public DObj_PlazaJerry_Zakka00_Fsodr() : base()
		{
			spl__ShopBindableBancParam = new Mu_spl__ShopBindableBancParam();

			Links = new List<Link>();
		}

		public DObj_PlazaJerry_Zakka00_Fsodr(DObj_PlazaJerry_Zakka00_Fsodr other) : base(other)
		{
			spl__ShopBindableBancParam = other.spl__ShopBindableBancParam.Clone();
		}

		public override DObj_PlazaJerry_Zakka00_Fsodr Clone()
		{
			return new DObj_PlazaJerry_Zakka00_Fsodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ShopBindableBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
