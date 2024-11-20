using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyKumasanRocket : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "EnemyKumasanRocket Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public EnemyKumasanRocket() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();

			Links = new List<Link>();
		}

		public EnemyKumasanRocket(EnemyKumasanRocket other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
		}

		public override EnemyKumasanRocket Clone()
		{
			return new EnemyKumasanRocket(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}