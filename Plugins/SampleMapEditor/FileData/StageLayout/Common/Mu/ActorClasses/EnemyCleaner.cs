using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyCleaner : MuObj
	{
		[ByamlMember("spl__EnemyCleanerBancParam")]
		public Mu_spl__EnemyCleanerBancParam spl__EnemyCleanerBancParam { get; set; }

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		public EnemyCleaner() : base()
		{
			spl__EnemyCleanerBancParam = new Mu_spl__EnemyCleanerBancParam();
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();

			Links = new List<Link>();
		}

		public EnemyCleaner(EnemyCleaner other) : base(other)
		{
			spl__EnemyCleanerBancParam = other.spl__EnemyCleanerBancParam.Clone();
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
		}

		public override EnemyCleaner Clone()
		{
			return new EnemyCleaner(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EnemyCleanerBancParam.SaveParameterBank(SerializedActor);
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
