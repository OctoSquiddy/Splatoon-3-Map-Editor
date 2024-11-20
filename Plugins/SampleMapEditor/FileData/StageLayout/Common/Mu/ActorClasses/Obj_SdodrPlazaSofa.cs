using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_SdodrPlazaSofa : MuObj
	{
		[ByamlMember("spl__ObjIdolSyncerBancParam")]
		public Mu_spl__ObjIdolSyncerBancParam spl__ObjIdolSyncerBancParam { get; set; }

		public Obj_SdodrPlazaSofa() : base()
		{
			spl__ObjIdolSyncerBancParam = new Mu_spl__ObjIdolSyncerBancParam();

			Links = new List<Link>();
		}

		public Obj_SdodrPlazaSofa(Obj_SdodrPlazaSofa other) : base(other)
		{
			spl__ObjIdolSyncerBancParam = other.spl__ObjIdolSyncerBancParam.Clone();
		}

		public override Obj_SdodrPlazaSofa Clone()
		{
			return new Obj_SdodrPlazaSofa(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ObjIdolSyncerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
