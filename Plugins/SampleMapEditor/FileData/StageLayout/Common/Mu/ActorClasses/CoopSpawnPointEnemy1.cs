using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopSpawnPointEnemy1 : MuObj
	{
		[BindGUI("IsSpawnBoss", Category = "CoopSpawnPointEnemy1 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsSpawnBoss
		{
			get
			{
				return this.spl__CoopSpawnPointEnemyParam.IsSpawnBoss;
			}

			set
			{
				this.spl__CoopSpawnPointEnemyParam.IsSpawnBoss = value;
			}
		}

		[ByamlMember("spl__CoopSpawnPointEnemyParam")]
		public Mu_spl__CoopSpawnPointEnemyParam spl__CoopSpawnPointEnemyParam { get; set; }

		public CoopSpawnPointEnemy1() : base()
		{
			spl__CoopSpawnPointEnemyParam = new Mu_spl__CoopSpawnPointEnemyParam();

			Links = new List<Link>();
		}

		public CoopSpawnPointEnemy1(CoopSpawnPointEnemy1 other) : base(other)
		{
			spl__CoopSpawnPointEnemyParam = other.spl__CoopSpawnPointEnemyParam.Clone();
		}

		public override CoopSpawnPointEnemy1 Clone()
		{
			return new CoopSpawnPointEnemy1(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopSpawnPointEnemyParam.SaveParameterBank(SerializedActor);
		}
	}
}
