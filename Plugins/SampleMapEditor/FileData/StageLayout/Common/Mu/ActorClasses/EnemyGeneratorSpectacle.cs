using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyGeneratorSpectacle : MuObj
	{
		[BindGUI("EnemyNamePhase0", Category = "EnemyGeneratorSpectacle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _EnemyNamePhase0
		{
			get
			{
				return this.spl__EnemyGeneratorSpectacleBancParam.EnemyNamePhase0;
			}

			set
			{
				this.spl__EnemyGeneratorSpectacleBancParam.EnemyNamePhase0 = value;
			}
		}

		[BindGUI("EnemyNamePhase1", Category = "EnemyGeneratorSpectacle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _EnemyNamePhase1
		{
			get
			{
				return this.spl__EnemyGeneratorSpectacleBancParam.EnemyNamePhase1;
			}

			set
			{
				this.spl__EnemyGeneratorSpectacleBancParam.EnemyNamePhase1 = value;
			}
		}

		[BindGUI("EnemyNamePhase2", Category = "EnemyGeneratorSpectacle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _EnemyNamePhase2
		{
			get
			{
				return this.spl__EnemyGeneratorSpectacleBancParam.EnemyNamePhase2;
			}

			set
			{
				this.spl__EnemyGeneratorSpectacleBancParam.EnemyNamePhase2 = value;
			}
		}

		[ByamlMember("spl__EnemyGeneratorSpectacleBancParam")]
		public Mu_spl__EnemyGeneratorSpectacleBancParam spl__EnemyGeneratorSpectacleBancParam { get; set; }

		public EnemyGeneratorSpectacle() : base()
		{
			spl__EnemyGeneratorSpectacleBancParam = new Mu_spl__EnemyGeneratorSpectacleBancParam();

			Links = new List<Link>();
		}

		public EnemyGeneratorSpectacle(EnemyGeneratorSpectacle other) : base(other)
		{
			spl__EnemyGeneratorSpectacleBancParam = other.spl__EnemyGeneratorSpectacleBancParam.Clone();
		}

		public override EnemyGeneratorSpectacle Clone()
		{
			return new EnemyGeneratorSpectacle(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EnemyGeneratorSpectacleBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
