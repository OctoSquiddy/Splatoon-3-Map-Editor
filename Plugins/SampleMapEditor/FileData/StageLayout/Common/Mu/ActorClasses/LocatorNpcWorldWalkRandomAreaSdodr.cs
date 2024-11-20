using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorNpcWorldWalkRandomAreaSdodr : MuObj
	{
		[BindGUI("ReserveNumTable", Category = "LocatorNpcWorldWalkRandomAreaSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__LinkTargetReservationBancParam")]
		public Mu_spl__LinkTargetReservationBancParam spl__LinkTargetReservationBancParam { get; set; }

		public LocatorNpcWorldWalkRandomAreaSdodr() : base()
		{
			spl__LinkTargetReservationBancParam = new Mu_spl__LinkTargetReservationBancParam();

			Links = new List<Link>();
		}

		public LocatorNpcWorldWalkRandomAreaSdodr(LocatorNpcWorldWalkRandomAreaSdodr other) : base(other)
		{
			spl__LinkTargetReservationBancParam = other.spl__LinkTargetReservationBancParam.Clone();
		}

		public override LocatorNpcWorldWalkRandomAreaSdodr Clone()
		{
			return new LocatorNpcWorldWalkRandomAreaSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LinkTargetReservationBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
