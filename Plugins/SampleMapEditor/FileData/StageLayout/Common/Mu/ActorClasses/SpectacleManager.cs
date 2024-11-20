using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpectacleManager : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "SpectacleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__SpectacleManagerBancParam")]
		public Mu_spl__SpectacleManagerBancParam spl__SpectacleManagerBancParam { get; set; }

		public SpectacleManager() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__SpectacleManagerBancParam = new Mu_spl__SpectacleManagerBancParam();

			Links = new List<Link>();
		}

		public SpectacleManager(SpectacleManager other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__SpectacleManagerBancParam = other.spl__SpectacleManagerBancParam.Clone();
		}

		public override SpectacleManager Clone()
		{
			return new SpectacleManager(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__SpectacleManagerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
