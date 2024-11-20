using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyTreeSdodr : MuObj
	{
		[BindGUI("MidiLinkNoteNumber", Category = "SplEnemyTreeSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "SplEnemyTreeSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EnemyTreeSdodrBancParam")]
		public Mu_spl__EnemyTreeSdodrBancParam spl__EnemyTreeSdodrBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public SplEnemyTreeSdodr() : base()
		{
			spl__EnemyTreeSdodrBancParam = new Mu_spl__EnemyTreeSdodrBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public SplEnemyTreeSdodr(SplEnemyTreeSdodr other) : base(other)
		{
			spl__EnemyTreeSdodrBancParam = other.spl__EnemyTreeSdodrBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override SplEnemyTreeSdodr Clone()
		{
			return new SplEnemyTreeSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EnemyTreeSdodrBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
