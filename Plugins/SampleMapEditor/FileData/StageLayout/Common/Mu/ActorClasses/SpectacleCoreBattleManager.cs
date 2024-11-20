using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpectacleCoreBattleManager : MuObj
	{
		[BindGUI("CoreBattlePhase", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CoreBattlePhase
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.CoreBattlePhase;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.CoreBattlePhase = value;
			}
		}

		[BindGUI("CoreName0", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _CoreName0
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.CoreName0;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.CoreName0 = value;
			}
		}

		[BindGUI("EnemyGenerateIntervalSec", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _EnemyGenerateIntervalSec
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateIntervalSec;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateIntervalSec = value;
			}
		}

		[BindGUI("EnemyGenerateSecPhase0", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _EnemyGenerateSecPhase0
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateSecPhase0;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateSecPhase0 = value;
			}
		}

		[BindGUI("EnemyGenerateSecPhase1", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _EnemyGenerateSecPhase1
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateSecPhase1;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateSecPhase1 = value;
			}
		}

		[BindGUI("EnemyGenerateSecPhase2", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _EnemyGenerateSecPhase2
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateSecPhase2;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.EnemyGenerateSecPhase2 = value;
			}
		}

		[BindGUI("LinkToRailNode", Category = "SpectacleCoreBattleManager Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRailNode
		{
			get
			{
				return this.spl__SpectacleCoreBattleManagerBancParam.LinkToRailNode;
			}

			set
			{
				this.spl__SpectacleCoreBattleManagerBancParam.LinkToRailNode = value;
			}
		}

		[ByamlMember("spl__SpectacleCoreBattleManagerBancParam")]
		public Mu_spl__SpectacleCoreBattleManagerBancParam spl__SpectacleCoreBattleManagerBancParam { get; set; }

		public SpectacleCoreBattleManager() : base()
		{
			spl__SpectacleCoreBattleManagerBancParam = new Mu_spl__SpectacleCoreBattleManagerBancParam();

			Links = new List<Link>();
		}

		public SpectacleCoreBattleManager(SpectacleCoreBattleManager other) : base(other)
		{
			spl__SpectacleCoreBattleManagerBancParam = other.spl__SpectacleCoreBattleManagerBancParam.Clone();
		}

		public override SpectacleCoreBattleManager Clone()
		{
			return new SpectacleCoreBattleManager(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SpectacleCoreBattleManagerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
