using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyZakoStandardSdodr : MuObj
	{
		[BindGUI("MidiLinkNoteNumber", Category = "SplEnemyZakoStandardSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MidiLinkNoteNumber
		{
			get
			{
				return this.spl__MusicLinkBancParam.MidiLinkNoteNumber;
			}

			set
			{
				this.spl__MusicLinkBancParam.MidiLinkNoteNumber = value;
			}
		}

		[BindGUI("MidiLinkTrackName", Category = "SplEnemyZakoStandardSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _MidiLinkTrackName
		{
			get
			{
				return this.spl__MusicLinkBancParam.MidiLinkTrackName;
			}

			set
			{
				this.spl__MusicLinkBancParam.MidiLinkTrackName = value;
			}
		}

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public SplEnemyZakoStandardSdodr() : base()
		{
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public SplEnemyZakoStandardSdodr(SplEnemyZakoStandardSdodr other) : base(other)
		{
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override SplEnemyZakoStandardSdodr Clone()
		{
			return new SplEnemyZakoStandardSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
