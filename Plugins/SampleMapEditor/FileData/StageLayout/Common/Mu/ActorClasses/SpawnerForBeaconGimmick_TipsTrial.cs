using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpawnerForBeaconGimmick_TipsTrial : MuObj
	{
		[ByamlMember("spl__SpawnerForBeaconGimmickParam")]
		public Mu_spl__SpawnerForBeaconGimmickParam spl__SpawnerForBeaconGimmickParam { get; set; }

		public SpawnerForBeaconGimmick_TipsTrial() : base()
		{
			spl__SpawnerForBeaconGimmickParam = new Mu_spl__SpawnerForBeaconGimmickParam();

			Links = new List<Link>();
		}

		public SpawnerForBeaconGimmick_TipsTrial(SpawnerForBeaconGimmick_TipsTrial other) : base(other)
		{
			spl__SpawnerForBeaconGimmickParam = other.spl__SpawnerForBeaconGimmickParam.Clone();
		}

		public override SpawnerForBeaconGimmick_TipsTrial Clone()
		{
			return new SpawnerForBeaconGimmick_TipsTrial(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SpawnerForBeaconGimmickParam.SaveParameterBank(SerializedActor);
		}
	}
}
