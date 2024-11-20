using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplSdodrRestingHimeLocator : MuObj
	{
		[BindGUI("AnimIndex", Category = "SplSdodrRestingHimeLocator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _AnimIndex
		{
			get
			{
				return this.spl__SdodrRestingHimeLocatorBancParam.AnimIndex;
			}

			set
			{
				this.spl__SdodrRestingHimeLocatorBancParam.AnimIndex = value;
			}
		}

		[ByamlMember("spl__SdodrRestingHimeLocatorBancParam")]
		public Mu_spl__SdodrRestingHimeLocatorBancParam spl__SdodrRestingHimeLocatorBancParam { get; set; }

		public SplSdodrRestingHimeLocator() : base()
		{
			spl__SdodrRestingHimeLocatorBancParam = new Mu_spl__SdodrRestingHimeLocatorBancParam();

			Links = new List<Link>();
		}

		public SplSdodrRestingHimeLocator(SplSdodrRestingHimeLocator other) : base(other)
		{
			spl__SdodrRestingHimeLocatorBancParam = other.spl__SdodrRestingHimeLocatorBancParam.Clone();
		}

		public override SplSdodrRestingHimeLocator Clone()
		{
			return new SplSdodrRestingHimeLocator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SdodrRestingHimeLocatorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
