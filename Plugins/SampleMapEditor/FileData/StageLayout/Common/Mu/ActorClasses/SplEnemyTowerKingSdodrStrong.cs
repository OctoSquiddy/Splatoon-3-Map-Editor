using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyTowerKingSdodrStrong : MuObj
	{
		[BindGUI("IsSpawnDirSpecified", Category = "SplEnemyTowerKingSdodrStrong Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsSpawnDirSpecified
		{
			get
			{
				return this.spl__ConcreteSpawnerSdodrHelperBancParam.IsSpawnDirSpecified;
			}

			set
			{
				this.spl__ConcreteSpawnerSdodrHelperBancParam.IsSpawnDirSpecified = value;
			}
		}

		[ByamlMember("spl__ConcreteSpawnerSdodrHelperBancParam")]
		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam spl__ConcreteSpawnerSdodrHelperBancParam { get; set; }

		[ByamlMember("spl__EnemyTowerKingBancParamSdodr")]
		public Mu_spl__EnemyTowerKingBancParamSdodr spl__EnemyTowerKingBancParamSdodr { get; set; }

		public SplEnemyTowerKingSdodrStrong() : base()
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = new Mu_spl__ConcreteSpawnerSdodrHelperBancParam();
			spl__EnemyTowerKingBancParamSdodr = new Mu_spl__EnemyTowerKingBancParamSdodr();

			Links = new List<Link>();
		}

		public SplEnemyTowerKingSdodrStrong(SplEnemyTowerKingSdodrStrong other) : base(other)
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = other.spl__ConcreteSpawnerSdodrHelperBancParam.Clone();
			spl__EnemyTowerKingBancParamSdodr = other.spl__EnemyTowerKingBancParamSdodr.Clone();
		}

		public override SplEnemyTowerKingSdodrStrong Clone()
		{
			return new SplEnemyTowerKingSdodrStrong(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ConcreteSpawnerSdodrHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyTowerKingBancParamSdodr.SaveParameterBank(SerializedActor);
		}
	}
}