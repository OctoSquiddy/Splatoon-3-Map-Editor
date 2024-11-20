using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpawnerForShieldGimmick_TipsTrial : MuObj
	{
		[ByamlMember("spl__SpawnerForShieldGimmickParam")]
		public Mu_spl__SpawnerForShieldGimmickParam spl__SpawnerForShieldGimmickParam { get; set; }

		public SpawnerForShieldGimmick_TipsTrial() : base()
		{
			spl__SpawnerForShieldGimmickParam = new Mu_spl__SpawnerForShieldGimmickParam();

			Links = new List<Link>();
		}

		public SpawnerForShieldGimmick_TipsTrial(SpawnerForShieldGimmick_TipsTrial other) : base(other)
		{
			spl__SpawnerForShieldGimmickParam = other.spl__SpawnerForShieldGimmickParam.Clone();
		}

		public override SpawnerForShieldGimmick_TipsTrial Clone()
		{
			return new SpawnerForShieldGimmick_TipsTrial(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SpawnerForShieldGimmickParam.SaveParameterBank(SerializedActor);
		}
	}
}
