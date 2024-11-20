using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplRivalMetaSpawnerSdodr : MuObj
	{
		[BindGUI("ReserveNumTable", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ReserveNumTable
		{
			get
			{
				return this.spl__LinkTargetReservationBancParam.ReserveNumTable;
			}

			set
			{
				this.spl__LinkTargetReservationBancParam.ReserveNumTable = value;
			}
		}

		[BindGUI("DelaySec", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DelaySec
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.DelaySec;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.DelaySec = value;
			}
		}

		[BindGUI("IntervalSec", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _IntervalSec
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.IntervalSec;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.IntervalSec = value;
			}
		}

		[BindGUI("MetaSpawnerId", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MetaSpawnerId
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.MetaSpawnerId;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.MetaSpawnerId = value;
			}
		}

		[BindGUI("SelectPolicyType", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SelectPolicyType
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.SelectPolicyType;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.SelectPolicyType = value;
			}
		}

		[BindGUI("SpawnNumPerTiming", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _SpawnNumPerTiming
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.SpawnNumPerTiming;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.SpawnNumPerTiming = value;
			}
		}

		[BindGUI("SpawnOrderTable", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SpawnOrderTable
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.SpawnOrderTable;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.SpawnOrderTable = value;
			}
		}

		[BindGUI("SpawnThresholdTable", Category = "SplRivalMetaSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SpawnThresholdTable
		{
			get
			{
				return this.spl__MetaSpawnerSdodrBancParam.SpawnThresholdTable;
			}

			set
			{
				this.spl__MetaSpawnerSdodrBancParam.SpawnThresholdTable = value;
			}
		}

		[ByamlMember("spl__LinkTargetReservationBancParam")]
		public Mu_spl__LinkTargetReservationBancParam spl__LinkTargetReservationBancParam { get; set; }

		[ByamlMember("spl__MetaSpawnerSdodrBancParam")]
		public Mu_spl__MetaSpawnerSdodrBancParam spl__MetaSpawnerSdodrBancParam { get; set; }

		public SplRivalMetaSpawnerSdodr() : base()
		{
			spl__LinkTargetReservationBancParam = new Mu_spl__LinkTargetReservationBancParam();
			spl__MetaSpawnerSdodrBancParam = new Mu_spl__MetaSpawnerSdodrBancParam();

			Links = new List<Link>();
		}

		public SplRivalMetaSpawnerSdodr(SplRivalMetaSpawnerSdodr other) : base(other)
		{
			spl__LinkTargetReservationBancParam = other.spl__LinkTargetReservationBancParam.Clone();
			spl__MetaSpawnerSdodrBancParam = other.spl__MetaSpawnerSdodrBancParam.Clone();
		}

		public override SplRivalMetaSpawnerSdodr Clone()
		{
			return new SplRivalMetaSpawnerSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LinkTargetReservationBancParam.SaveParameterBank(SerializedActor);
			this.spl__MetaSpawnerSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}