using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class 雑魚大スポナー : MuObj
	{
		[BindGUI("ReserveNumTable", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("DelaySec", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IntervalSec", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MetaSpawnerId", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SelectPolicyType", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpawnNumPerTiming", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpawnOrderTable", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpawnThresholdTable", Category = "雑魚大スポナー Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public 雑魚大スポナー() : base()
		{
			spl__LinkTargetReservationBancParam = new Mu_spl__LinkTargetReservationBancParam();
			spl__MetaSpawnerSdodrBancParam = new Mu_spl__MetaSpawnerSdodrBancParam();

			Links = new List<Link>();
		}

		public 雑魚大スポナー(雑魚大スポナー other) : base(other)
		{
			spl__LinkTargetReservationBancParam = other.spl__LinkTargetReservationBancParam.Clone();
			spl__MetaSpawnerSdodrBancParam = other.spl__MetaSpawnerSdodrBancParam.Clone();
		}

		public override 雑魚大スポナー Clone()
		{
			return new 雑魚大スポナー(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LinkTargetReservationBancParam.SaveParameterBank(SerializedActor);
			this.spl__MetaSpawnerSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
