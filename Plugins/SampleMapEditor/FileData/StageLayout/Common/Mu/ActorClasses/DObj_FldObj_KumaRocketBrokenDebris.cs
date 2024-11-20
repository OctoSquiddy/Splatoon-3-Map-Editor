using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DObj_FldObj_KumaRocketBrokenDebris : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "DObj_FldObj_KumaRocketBrokenDebris Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__SpectacleMtxSetterBancParam")]
		public Mu_spl__SpectacleMtxSetterBancParam spl__SpectacleMtxSetterBancParam { get; set; }

		public DObj_FldObj_KumaRocketBrokenDebris() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__SpectacleMtxSetterBancParam = new Mu_spl__SpectacleMtxSetterBancParam();

			Links = new List<Link>();
		}

		public DObj_FldObj_KumaRocketBrokenDebris(DObj_FldObj_KumaRocketBrokenDebris other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__SpectacleMtxSetterBancParam = other.spl__SpectacleMtxSetterBancParam.Clone();
		}

		public override DObj_FldObj_KumaRocketBrokenDebris Clone()
		{
			return new DObj_FldObj_KumaRocketBrokenDebris(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__SpectacleMtxSetterBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
