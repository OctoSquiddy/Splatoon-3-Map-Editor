using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplExplodeSpawnerSdodr : MuObj
	{
		[BindGUI("IsSpawnDirSpecified", Category = "SplExplodeSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("SpawnOffsetY", Category = "SplExplodeSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _SpawnOffsetY
		{
			get
			{
				return this.spl__ExplodeSpawnerSdodrBancParam.SpawnOffsetY;
			}

			set
			{
				this.spl__ExplodeSpawnerSdodrBancParam.SpawnOffsetY = value;
			}
		}

		[BindGUI("ReserveNumTable", Category = "SplExplodeSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__ConcreteSpawnerSdodrHelperBancParam")]
		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam spl__ConcreteSpawnerSdodrHelperBancParam { get; set; }

		[ByamlMember("spl__ExplodeSpawnerSdodrBancParam")]
		public Mu_spl__ExplodeSpawnerSdodrBancParam spl__ExplodeSpawnerSdodrBancParam { get; set; }

		[ByamlMember("spl__LinkTargetReservationBancParam")]
		public Mu_spl__LinkTargetReservationBancParam spl__LinkTargetReservationBancParam { get; set; }

		public SplExplodeSpawnerSdodr() : base()
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = new Mu_spl__ConcreteSpawnerSdodrHelperBancParam();
			spl__ExplodeSpawnerSdodrBancParam = new Mu_spl__ExplodeSpawnerSdodrBancParam();
			spl__LinkTargetReservationBancParam = new Mu_spl__LinkTargetReservationBancParam();

			Links = new List<Link>();
		}

		public SplExplodeSpawnerSdodr(SplExplodeSpawnerSdodr other) : base(other)
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = other.spl__ConcreteSpawnerSdodrHelperBancParam.Clone();
			spl__ExplodeSpawnerSdodrBancParam = other.spl__ExplodeSpawnerSdodrBancParam.Clone();
			spl__LinkTargetReservationBancParam = other.spl__LinkTargetReservationBancParam.Clone();
		}

		public override SplExplodeSpawnerSdodr Clone()
		{
			return new SplExplodeSpawnerSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ConcreteSpawnerSdodrHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__ExplodeSpawnerSdodrBancParam.SaveParameterBank(SerializedActor);
			this.spl__LinkTargetReservationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
