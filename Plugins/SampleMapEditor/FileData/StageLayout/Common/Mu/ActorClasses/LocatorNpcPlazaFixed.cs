using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorNpcPlazaFixed : MuObj
	{
		[BindGUI("ASCommand", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ASCommand
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.ASCommand;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.ASCommand = value;
			}
		}

		[BindGUI("BindActor", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _BindActor
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.BindActor;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.BindActor = value;
			}
		}

		[BindGUI("BindBone", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _BindBone
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.BindBone;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.BindBone = value;
			}
		}

		[BindGUI("ForMiniGame", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _ForMiniGame
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.ForMiniGame;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.ForMiniGame = value;
			}
		}

		[BindGUI("LookPlayer", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _LookPlayer
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.LookPlayer;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.LookPlayer = value;
			}
		}

		[BindGUI("SourceType", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _SourceType
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.SourceType;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.SourceType = value;
			}
		}

		[BindGUI("VariationId", Category = "LocatorNpcPlazaFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _VariationId
		{
			get
			{
				return this.spl__LocatorNpcPlazaFixedBancParam.VariationId;
			}

			set
			{
				this.spl__LocatorNpcPlazaFixedBancParam.VariationId = value;
			}
		}

		[ByamlMember("spl__LocatorNpcPlazaFixedBancParam")]
		public Mu_spl__LocatorNpcPlazaFixedBancParam spl__LocatorNpcPlazaFixedBancParam { get; set; }

		public LocatorNpcPlazaFixed() : base()
		{
			spl__LocatorNpcPlazaFixedBancParam = new Mu_spl__LocatorNpcPlazaFixedBancParam();

			Links = new List<Link>();
		}

		public LocatorNpcPlazaFixed(LocatorNpcPlazaFixed other) : base(other)
		{
			spl__LocatorNpcPlazaFixedBancParam = other.spl__LocatorNpcPlazaFixedBancParam.Clone();
		}

		public override LocatorNpcPlazaFixed Clone()
		{
			return new LocatorNpcPlazaFixed(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorNpcPlazaFixedBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
