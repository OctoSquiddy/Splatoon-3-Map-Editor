using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpawnerForSprinklerGimmick_TipsTrial : MuObj
	{
		[BindGUI("ActivatePlayerDistance", Category = "SpawnerForSprinklerGimmick_TipsTrial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _ActivatePlayerDistance
		{
			get
			{
				return this.spl__SpawnerForSprinklerGimmickParam.ActivatePlayerDistance;
			}

			set
			{
				this.spl__SpawnerForSprinklerGimmickParam.ActivatePlayerDistance = value;
			}
		}

		[BindGUI("IkuraNum", Category = "SpawnerForSprinklerGimmick_TipsTrial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IkuraNum
		{
			get
			{
				return this.spl__SpawnerForSprinklerGimmickParam.IkuraNum;
			}

			set
			{
				this.spl__SpawnerForSprinklerGimmickParam.IkuraNum = value;
			}
		}

		[BindGUI("SpawnType", Category = "SpawnerForSprinklerGimmick_TipsTrial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SpawnType
		{
			get
			{
				return this.spl__SpawnerForSprinklerGimmickParam.SpawnType;
			}

			set
			{
				this.spl__SpawnerForSprinklerGimmickParam.SpawnType = value;
			}
		}

		[BindGUI("SprinkleType", Category = "SpawnerForSprinklerGimmick_TipsTrial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SprinkleType
		{
			get
			{
				return this.spl__SpawnerForSprinklerGimmickParam.SprinkleType;
			}

			set
			{
				this.spl__SpawnerForSprinklerGimmickParam.SprinkleType = value;
			}
		}

		[ByamlMember("spl__SpawnerForSprinklerGimmickParam")]
		public Mu_spl__SpawnerForSprinklerGimmickParam spl__SpawnerForSprinklerGimmickParam { get; set; }

		public SpawnerForSprinklerGimmick_TipsTrial() : base()
		{
			spl__SpawnerForSprinklerGimmickParam = new Mu_spl__SpawnerForSprinklerGimmickParam();

			Links = new List<Link>();
		}

		public SpawnerForSprinklerGimmick_TipsTrial(SpawnerForSprinklerGimmick_TipsTrial other) : base(other)
		{
			spl__SpawnerForSprinklerGimmickParam = other.spl__SpawnerForSprinklerGimmickParam.Clone();
		}

		public override SpawnerForSprinklerGimmick_TipsTrial Clone()
		{
			return new SpawnerForSprinklerGimmick_TipsTrial(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SpawnerForSprinklerGimmickParam.SaveParameterBank(SerializedActor);
		}
	}
}
