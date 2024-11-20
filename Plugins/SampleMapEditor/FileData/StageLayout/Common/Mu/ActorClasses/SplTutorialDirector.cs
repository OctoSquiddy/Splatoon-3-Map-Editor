using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplTutorialDirector : MuObj
	{
		[BindGUI("ToBuddyRailAim", Category = "SplTutorialDirector Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToBuddyRailAim
		{
			get
			{
				return this.spl__TutorialDirectorBancParam.ToBuddyRailAim;
			}

			set
			{
				this.spl__TutorialDirectorBancParam.ToBuddyRailAim = value;
			}
		}

		[BindGUI("ToBuddyRailChase", Category = "SplTutorialDirector Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToBuddyRailChase
		{
			get
			{
				return this.spl__TutorialDirectorBancParam.ToBuddyRailChase;
			}

			set
			{
				this.spl__TutorialDirectorBancParam.ToBuddyRailChase = value;
			}
		}

		[BindGUI("ToBuddyRailTreasure", Category = "SplTutorialDirector Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToBuddyRailTreasure
		{
			get
			{
				return this.spl__TutorialDirectorBancParam.ToBuddyRailTreasure;
			}

			set
			{
				this.spl__TutorialDirectorBancParam.ToBuddyRailTreasure = value;
			}
		}

		[ByamlMember("spl__TutorialDirectorBancParam")]
		public Mu_spl__TutorialDirectorBancParam spl__TutorialDirectorBancParam { get; set; }

		public SplTutorialDirector() : base()
		{
			spl__TutorialDirectorBancParam = new Mu_spl__TutorialDirectorBancParam();

			Links = new List<Link>();
		}

		public SplTutorialDirector(SplTutorialDirector other) : base(other)
		{
			spl__TutorialDirectorBancParam = other.spl__TutorialDirectorBancParam.Clone();
		}

		public override SplTutorialDirector Clone()
		{
			return new SplTutorialDirector(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__TutorialDirectorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
