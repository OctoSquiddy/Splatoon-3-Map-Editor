using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class BGActorDObj_SdodrNapoleon_SwimRailB : MuObj
	{
		[BindGUI("AnimName", Category = "BGActorDObj_SdodrNapoleon_SwimRailB Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _AnimName
		{
			get
			{
				return this.spl__DesignerObjBancParam.AnimName;
			}

			set
			{
				this.spl__DesignerObjBancParam.AnimName = value;
			}
		}

		[ByamlMember("spl__DesignerObjBancParam")]
		public Mu_spl__DesignerObjBancParam spl__DesignerObjBancParam { get; set; }

		public BGActorDObj_SdodrNapoleon_SwimRailB() : base()
		{
			spl__DesignerObjBancParam = new Mu_spl__DesignerObjBancParam();

			Links = new List<Link>();
		}

		public BGActorDObj_SdodrNapoleon_SwimRailB(BGActorDObj_SdodrNapoleon_SwimRailB other) : base(other)
		{
			spl__DesignerObjBancParam = other.spl__DesignerObjBancParam.Clone();
		}

		public override BGActorDObj_SdodrNapoleon_SwimRailB Clone()
		{
			return new BGActorDObj_SdodrNapoleon_SwimRailB(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__DesignerObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
