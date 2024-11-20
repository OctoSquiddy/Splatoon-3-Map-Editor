using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DObj_Fld_SdodrBarrierKingOdako1stSignage : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "DObj_Fld_SdodrBarrierKingOdako1stSignage Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		public DObj_Fld_SdodrBarrierKingOdako1stSignage() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();

			Links = new List<Link>();
		}

		public DObj_Fld_SdodrBarrierKingOdako1stSignage(DObj_Fld_SdodrBarrierKingOdako1stSignage other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
		}

		public override DObj_Fld_SdodrBarrierKingOdako1stSignage Clone()
		{
			return new DObj_Fld_SdodrBarrierKingOdako1stSignage(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
