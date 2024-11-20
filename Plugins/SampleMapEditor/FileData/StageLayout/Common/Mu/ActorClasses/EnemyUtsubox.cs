using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyUtsubox : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "EnemyUtsubox Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EnemyUtsuboxBancParam")]
		public Mu_spl__EnemyUtsuboxBancParam spl__EnemyUtsuboxBancParam { get; set; }

		public EnemyUtsubox() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__EnemyUtsuboxBancParam = new Mu_spl__EnemyUtsuboxBancParam();

			Links = new List<Link>();
		}

		public EnemyUtsubox(EnemyUtsubox other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__EnemyUtsuboxBancParam = other.spl__EnemyUtsuboxBancParam.Clone();
		}

		public override EnemyUtsubox Clone()
		{
			return new EnemyUtsubox(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyUtsuboxBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
