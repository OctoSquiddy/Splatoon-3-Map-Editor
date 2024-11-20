using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class VehicleSpectacle : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "VehicleSpectacle Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__VehicleSpectacleBancParam")]
		public Mu_spl__VehicleSpectacleBancParam spl__VehicleSpectacleBancParam { get; set; }

		public VehicleSpectacle() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__VehicleSpectacleBancParam = new Mu_spl__VehicleSpectacleBancParam();

			Links = new List<Link>();
		}

		public VehicleSpectacle(VehicleSpectacle other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__VehicleSpectacleBancParam = other.spl__VehicleSpectacleBancParam.Clone();
		}

		public override VehicleSpectacle Clone()
		{
			return new VehicleSpectacle(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__VehicleSpectacleBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
