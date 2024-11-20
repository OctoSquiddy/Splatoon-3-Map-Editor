using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_Ruins03PatrolLamp : MuObj
	{
		[ByamlMember("spl__LiftDecorationBancParam")]
		public Mu_spl__LiftDecorationBancParam spl__LiftDecorationBancParam { get; set; }

		public Obj_Ruins03PatrolLamp() : base()
		{
			spl__LiftDecorationBancParam = new Mu_spl__LiftDecorationBancParam();

			Links = new List<Link>();
		}

		public Obj_Ruins03PatrolLamp(Obj_Ruins03PatrolLamp other) : base(other)
		{
			spl__LiftDecorationBancParam = other.spl__LiftDecorationBancParam.Clone();
		}

		public override Obj_Ruins03PatrolLamp Clone()
		{
			return new Obj_Ruins03PatrolLamp(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LiftDecorationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
