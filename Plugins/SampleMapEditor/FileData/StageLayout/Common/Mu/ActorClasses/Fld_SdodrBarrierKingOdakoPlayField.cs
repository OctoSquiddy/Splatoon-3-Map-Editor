using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Fld_SdodrBarrierKingOdakoPlayField : MuObj
	{
		[BindGUI("IsEventOnly", Category = "Fld_SdodrBarrierKingOdakoPlayField Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEventOnly
		{
			get
			{
				return this.spl__EventActorStateBancParam.IsEventOnly;
			}

			set
			{
				this.spl__EventActorStateBancParam.IsEventOnly = value;
			}
		}

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		public Fld_SdodrBarrierKingOdakoPlayField() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();

			Links = new List<Link>();
		}

		public Fld_SdodrBarrierKingOdakoPlayField(Fld_SdodrBarrierKingOdakoPlayField other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
		}

		public override Fld_SdodrBarrierKingOdakoPlayField Clone()
		{
			return new Fld_SdodrBarrierKingOdakoPlayField(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
