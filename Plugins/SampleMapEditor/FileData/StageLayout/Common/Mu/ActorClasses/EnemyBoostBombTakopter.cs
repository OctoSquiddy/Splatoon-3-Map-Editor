using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyBoostBombTakopter : MuObj
	{
		[BindGUI("IsFixedMode", Category = "EnemyBoostBombTakopter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsFixedMode
		{
			get
			{
				return this.spl__EnemyTakopterBancParam.IsFixedMode;
			}

			set
			{
				this.spl__EnemyTakopterBancParam.IsFixedMode = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "EnemyBoostBombTakopter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__RailFollowBancParam.ToRailPoint;
			}

			set
			{
				this.spl__RailFollowBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__AttentionTargetingBancParam")]
		public Mu_spl__AttentionTargetingBancParam spl__AttentionTargetingBancParam { get; set; }

		[ByamlMember("spl__EnemyTakopterBancParam")]
		public Mu_spl__EnemyTakopterBancParam spl__EnemyTakopterBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		public EnemyBoostBombTakopter() : base()
		{
			spl__AttentionTargetingBancParam = new Mu_spl__AttentionTargetingBancParam();
			spl__EnemyTakopterBancParam = new Mu_spl__EnemyTakopterBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();

			Links = new List<Link>();
		}

		public EnemyBoostBombTakopter(EnemyBoostBombTakopter other) : base(other)
		{
			spl__AttentionTargetingBancParam = other.spl__AttentionTargetingBancParam.Clone();
			spl__EnemyTakopterBancParam = other.spl__EnemyTakopterBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
		}

		public override EnemyBoostBombTakopter Clone()
		{
			return new EnemyBoostBombTakopter(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AttentionTargetingBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyTakopterBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
