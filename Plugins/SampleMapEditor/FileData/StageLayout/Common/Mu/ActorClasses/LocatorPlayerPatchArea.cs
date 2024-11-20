using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorPlayerPatchArea : MuObj
	{
		[BindGUI("MoveResistXZ_Th", Category = "LocatorPlayerPatchArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveResistXZ_Th
		{
			get
			{
				return this.spl__LocatorPlayerPatchAreaParam.MoveResistXZ_Th;
			}

			set
			{
				this.spl__LocatorPlayerPatchAreaParam.MoveResistXZ_Th = value;
			}
		}

		[BindGUI("MoveResistXZ_Val", Category = "LocatorPlayerPatchArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveResistXZ_Val
		{
			get
			{
				return this.spl__LocatorPlayerPatchAreaParam.MoveResistXZ_Val;
			}

			set
			{
				this.spl__LocatorPlayerPatchAreaParam.MoveResistXZ_Val = value;
			}
		}

		[BindGUI("NiceBallHoverAltitudeInflate", Category = "LocatorPlayerPatchArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _NiceBallHoverAltitudeInflate
		{
			get
			{
				return this.spl__LocatorPlayerPatchAreaParam.NiceBallHoverAltitudeInflate;
			}

			set
			{
				this.spl__LocatorPlayerPatchAreaParam.NiceBallHoverAltitudeInflate = value;
			}
		}

		[BindGUI("NoAirFall", Category = "LocatorPlayerPatchArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _NoAirFall
		{
			get
			{
				return this.spl__LocatorPlayerPatchAreaParam.NoAirFall;
			}

			set
			{
				this.spl__LocatorPlayerPatchAreaParam.NoAirFall = value;
			}
		}

		[ByamlMember("spl__LocatorPlayerPatchAreaParam")]
		public Mu_spl__LocatorPlayerPatchAreaParam spl__LocatorPlayerPatchAreaParam { get; set; }

		public LocatorPlayerPatchArea() : base()
		{
			spl__LocatorPlayerPatchAreaParam = new Mu_spl__LocatorPlayerPatchAreaParam();

			Links = new List<Link>();
		}

		public LocatorPlayerPatchArea(LocatorPlayerPatchArea other) : base(other)
		{
			spl__LocatorPlayerPatchAreaParam = other.spl__LocatorPlayerPatchAreaParam.Clone();
		}

		public override LocatorPlayerPatchArea Clone()
		{
			return new LocatorPlayerPatchArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorPlayerPatchAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
