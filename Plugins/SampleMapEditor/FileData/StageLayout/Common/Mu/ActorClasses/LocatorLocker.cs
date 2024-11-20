using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorLocker : MuObj
	{
		[BindGUI("Index", Category = "LocatorLocker Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Index
		{
			get
			{
				return this.spl__LocatorLockerBancParam.Index;
			}

			set
			{
				this.spl__LocatorLockerBancParam.Index = value;
			}
		}

		[ByamlMember("spl__LocatorLockerBancParam")]
		public Mu_spl__LocatorLockerBancParam spl__LocatorLockerBancParam { get; set; }

		public LocatorLocker() : base()
		{
			spl__LocatorLockerBancParam = new Mu_spl__LocatorLockerBancParam();

			Links = new List<Link>();
		}

		public LocatorLocker(LocatorLocker other) : base(other)
		{
			spl__LocatorLockerBancParam = other.spl__LocatorLockerBancParam.Clone();
		}

		public override LocatorLocker Clone()
		{
			return new LocatorLocker(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorLockerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
