using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorTutorial : MuObj
	{
		[BindGUI("ProgressIndex", Category = "LocatorTutorial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ProgressIndex
		{
			get
			{
				return this.spl__LocatorTutorialBancParam.ProgressIndex;
			}

			set
			{
				this.spl__LocatorTutorialBancParam.ProgressIndex = value;
			}
		}

		[ByamlMember("spl__LocatorTutorialBancParam")]
		public Mu_spl__LocatorTutorialBancParam spl__LocatorTutorialBancParam { get; set; }

		public LocatorTutorial() : base()
		{
			spl__LocatorTutorialBancParam = new Mu_spl__LocatorTutorialBancParam();

			Links = new List<Link>();
		}

		public LocatorTutorial(LocatorTutorial other) : base(other)
		{
			spl__LocatorTutorialBancParam = other.spl__LocatorTutorialBancParam.Clone();
		}

		public override LocatorTutorial Clone()
		{
			return new LocatorTutorial(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorTutorialBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
