using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpawnerForSprinklerGimmick : MuObj
	{
		[BindGUI("ActivatePlayerDistance", Category = "SpawnerForSprinklerGimmick Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IkuraNum", Category = "SpawnerForSprinklerGimmick Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpawnType", Category = "SpawnerForSprinklerGimmick Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SprinkleType", Category = "SpawnerForSprinklerGimmick Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public SpawnerForSprinklerGimmick() : base()
		{
			spl__SpawnerForSprinklerGimmickParam = new Mu_spl__SpawnerForSprinklerGimmickParam();

			Links = new List<Link>();
		}

		public SpawnerForSprinklerGimmick(SpawnerForSprinklerGimmick other) : base(other)
		{
			spl__SpawnerForSprinklerGimmickParam = other.spl__SpawnerForSprinklerGimmickParam.Clone();
		}

		public override SpawnerForSprinklerGimmick Clone()
		{
			return new SpawnerForSprinklerGimmick(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SpawnerForSprinklerGimmickParam.SaveParameterBank(SerializedActor);
		}
	}
}
