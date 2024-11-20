using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplPlazaSalmonBuddyLocator : MuObj
	{
		[BindGUI("LocatorName", Category = "SplPlazaSalmonBuddyLocator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _LocatorName
		{
			get
			{
				return this.spl__PlazaSalmonBuddyLocatorBancParam.LocatorName;
			}

			set
			{
				this.spl__PlazaSalmonBuddyLocatorBancParam.LocatorName = value;
			}
		}

		[ByamlMember("spl__PlazaSalmonBuddyLocatorBancParam")]
		public Mu_spl__PlazaSalmonBuddyLocatorBancParam spl__PlazaSalmonBuddyLocatorBancParam { get; set; }

		public SplPlazaSalmonBuddyLocator() : base()
		{
			spl__PlazaSalmonBuddyLocatorBancParam = new Mu_spl__PlazaSalmonBuddyLocatorBancParam();

			Links = new List<Link>();
		}

		public SplPlazaSalmonBuddyLocator(SplPlazaSalmonBuddyLocator other) : base(other)
		{
			spl__PlazaSalmonBuddyLocatorBancParam = other.spl__PlazaSalmonBuddyLocatorBancParam.Clone();
		}

		public override SplPlazaSalmonBuddyLocator Clone()
		{
			return new SplPlazaSalmonBuddyLocator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PlazaSalmonBuddyLocatorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
