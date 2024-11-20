using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class RocketSpectacle : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "RocketSpectacle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public RocketSpectacle() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__SpectacleMtxSetterBancParam = new Mu_spl__SpectacleMtxSetterBancParam();

			Links = new List<Link>();
		}

		public RocketSpectacle(RocketSpectacle other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__SpectacleMtxSetterBancParam = other.spl__SpectacleMtxSetterBancParam.Clone();
		}

		public override RocketSpectacle Clone()
		{
			return new RocketSpectacle(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__SpectacleMtxSetterBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
