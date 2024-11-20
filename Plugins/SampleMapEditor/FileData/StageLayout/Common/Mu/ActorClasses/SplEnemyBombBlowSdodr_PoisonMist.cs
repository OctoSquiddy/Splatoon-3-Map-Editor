using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyBombBlowSdodr_PoisonMist : MuObj
	{
		[BindGUI("MidiLinkNoteNumber", Category = "SplEnemyBombBlowSdodr_PoisonMist Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "SplEnemyBombBlowSdodr_PoisonMist Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EnemyBombBlowSdodrBancParam")]
		public Mu_spl__EnemyBombBlowSdodrBancParam spl__EnemyBombBlowSdodrBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public SplEnemyBombBlowSdodr_PoisonMist() : base()
		{
			spl__EnemyBombBlowSdodrBancParam = new Mu_spl__EnemyBombBlowSdodrBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public SplEnemyBombBlowSdodr_PoisonMist(SplEnemyBombBlowSdodr_PoisonMist other) : base(other)
		{
			spl__EnemyBombBlowSdodrBancParam = other.spl__EnemyBombBlowSdodrBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override SplEnemyBombBlowSdodr_PoisonMist Clone()
		{
			return new SplEnemyBombBlowSdodr_PoisonMist(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EnemyBombBlowSdodrBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
