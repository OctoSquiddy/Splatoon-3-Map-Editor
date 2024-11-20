using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemySharkKing : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "EnemySharkKing Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EnemySharkKingBancParam")]
		public Mu_spl__EnemySharkKingBancParam spl__EnemySharkKingBancParam { get; set; }

		public EnemySharkKing() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__EnemySharkKingBancParam = new Mu_spl__EnemySharkKingBancParam();

			Links = new List<Link>();
		}

		public EnemySharkKing(EnemySharkKing other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__EnemySharkKingBancParam = other.spl__EnemySharkKingBancParam.Clone();
		}

		public override EnemySharkKing Clone()
		{
			return new EnemySharkKing(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemySharkKingBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
