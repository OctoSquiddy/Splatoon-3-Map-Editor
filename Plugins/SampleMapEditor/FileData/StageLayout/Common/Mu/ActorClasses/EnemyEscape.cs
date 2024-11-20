using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyEscape : MuObj
	{
		[BindGUI("MaxOffsetToNode", Category = "EnemyEscape Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxOffsetToNode
		{
			get
			{
				return this.spl__EnemyEscapeBancParam.MaxOffsetToNode;
			}

			set
			{
				this.spl__EnemyEscapeBancParam.MaxOffsetToNode = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "EnemyEscape Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__GraphRailHelperBancParam.ToRailPoint;
			}

			set
			{
				this.spl__GraphRailHelperBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__AttentionTargetingBancParam")]
		public Mu_spl__AttentionTargetingBancParam spl__AttentionTargetingBancParam { get; set; }

		[ByamlMember("spl__EnemyEscapeBancParam")]
		public Mu_spl__EnemyEscapeBancParam spl__EnemyEscapeBancParam { get; set; }

		[ByamlMember("spl__GraphRailHelperBancParam")]
		public Mu_spl__GraphRailHelperBancParam spl__GraphRailHelperBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		public EnemyEscape() : base()
		{
			spl__AttentionTargetingBancParam = new Mu_spl__AttentionTargetingBancParam();
			spl__EnemyEscapeBancParam = new Mu_spl__EnemyEscapeBancParam();
			spl__GraphRailHelperBancParam = new Mu_spl__GraphRailHelperBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();

			Links = new List<Link>();
		}

		public EnemyEscape(EnemyEscape other) : base(other)
		{
			spl__AttentionTargetingBancParam = other.spl__AttentionTargetingBancParam.Clone();
			spl__EnemyEscapeBancParam = other.spl__EnemyEscapeBancParam.Clone();
			spl__GraphRailHelperBancParam = other.spl__GraphRailHelperBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
		}

		public override EnemyEscape Clone()
		{
			return new EnemyEscape(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AttentionTargetingBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyEscapeBancParam.SaveParameterBank(SerializedActor);
			this.spl__GraphRailHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
