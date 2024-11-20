using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorPlazaJerryFixed : MuObj
	{
		[BindGUI("ActorName", Category = "LocatorPlazaJerryFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ActorName
		{
			get
			{
				return this.spl__PlazaJerryFixedBancParam.ActorName;
			}

			set
			{
				this.spl__PlazaJerryFixedBancParam.ActorName = value;
			}
		}

		[BindGUI("ASCommand", Category = "LocatorPlazaJerryFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ASCommand
		{
			get
			{
				return this.spl__PlazaJerryFixedBancParam.ASCommand;
			}

			set
			{
				this.spl__PlazaJerryFixedBancParam.ASCommand = value;
			}
		}

		[BindGUI("VariationId", Category = "LocatorPlazaJerryFixed Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _VariationId
		{
			get
			{
				return this.spl__PlazaJerryFixedBancParam.VariationId;
			}

			set
			{
				this.spl__PlazaJerryFixedBancParam.VariationId = value;
			}
		}

		[ByamlMember("spl__PlazaJerryFixedBancParam")]
		public Mu_spl__PlazaJerryFixedBancParam spl__PlazaJerryFixedBancParam { get; set; }

		public LocatorPlazaJerryFixed() : base()
		{
			spl__PlazaJerryFixedBancParam = new Mu_spl__PlazaJerryFixedBancParam();

			Links = new List<Link>();
		}

		public LocatorPlazaJerryFixed(LocatorPlazaJerryFixed other) : base(other)
		{
			spl__PlazaJerryFixedBancParam = other.spl__PlazaJerryFixedBancParam.Clone();
		}

		public override LocatorPlazaJerryFixed Clone()
		{
			return new LocatorPlazaJerryFixed(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PlazaJerryFixedBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
