using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyBarrierKingSdodrOdakoTentacleDevice : MuObj
	{
		[BindGUI("IsSpawnDirSpecified", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ColorGroupType", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ColorGroupType
		{
			get
			{
				return this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.ColorGroupType;
			}

			set
			{
				this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.ColorGroupType = value;
			}
		}

		[BindGUI("OverwriteMaxHP", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _OverwriteMaxHP
		{
			get
			{
				return this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.OverwriteMaxHP;
			}

			set
			{
				this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.OverwriteMaxHP = value;
			}
		}

		[BindGUI("PlacementType", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _PlacementType
		{
			get
			{
				return this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.PlacementType;
			}

			set
			{
				this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.PlacementType = value;
			}
		}

		[BindGUI("ToRail", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRail
		{
			get
			{
				return this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.ToRail;
			}

			set
			{
				this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.ToRail = value;
			}
		}

		[BindGUI("Type", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Type
		{
			get
			{
				return this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.Type;
			}

			set
			{
				this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.Type = value;
			}
		}

		[BindGUI("ReserveNumTable", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkNoteNumber", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MidiLinkNoteNumber
		{
			get
			{
				return this.spl__MusicLinkBancParam.MidiLinkNoteNumber;
			}

			set
			{
				this.spl__MusicLinkBancParam.MidiLinkNoteNumber = value;
			}
		}

		[BindGUI("MidiLinkTrackName", Category = "SplEnemyBarrierKingSdodrOdakoTentacleDevice Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _MidiLinkTrackName
		{
			get
			{
				return this.spl__MusicLinkBancParam.MidiLinkTrackName;
			}

			set
			{
				this.spl__MusicLinkBancParam.MidiLinkTrackName = value;
			}
		}

		[ByamlMember("spl__ConcreteSpawnerSdodrHelperBancParam")]
		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam spl__ConcreteSpawnerSdodrHelperBancParam { get; set; }

		[ByamlMember("spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam")]
		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam { get; set; }

		[ByamlMember("spl__LinkTargetReservationBancParam")]
		public Mu_spl__LinkTargetReservationBancParam spl__LinkTargetReservationBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public SplEnemyBarrierKingSdodrOdakoTentacleDevice() : base()
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = new Mu_spl__ConcreteSpawnerSdodrHelperBancParam();
			spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam = new Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam();
			spl__LinkTargetReservationBancParam = new Mu_spl__LinkTargetReservationBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public SplEnemyBarrierKingSdodrOdakoTentacleDevice(SplEnemyBarrierKingSdodrOdakoTentacleDevice other) : base(other)
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = other.spl__ConcreteSpawnerSdodrHelperBancParam.Clone();
			spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam = other.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.Clone();
			spl__LinkTargetReservationBancParam = other.spl__LinkTargetReservationBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override SplEnemyBarrierKingSdodrOdakoTentacleDevice Clone()
		{
			return new SplEnemyBarrierKingSdodrOdakoTentacleDevice(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ConcreteSpawnerSdodrHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyBarrierKingSdodr__TentacleDeviceBancParam.SaveParameterBank(SerializedActor);
			this.spl__LinkTargetReservationBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
