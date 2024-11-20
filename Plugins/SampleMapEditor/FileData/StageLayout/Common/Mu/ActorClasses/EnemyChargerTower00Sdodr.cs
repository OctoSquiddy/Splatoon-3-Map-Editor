using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyChargerTower00Sdodr : MuObj
	{
		[BindGUI("MidiLinkNoteNumber", Category = "EnemyChargerTower00Sdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "EnemyChargerTower00Sdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EnemyChargerTowerSdodrBancParam")]
		public Mu_spl__EnemyChargerTowerSdodrBancParam spl__EnemyChargerTowerSdodrBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public EnemyChargerTower00Sdodr() : base()
		{
			spl__EnemyChargerTowerSdodrBancParam = new Mu_spl__EnemyChargerTowerSdodrBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public EnemyChargerTower00Sdodr(EnemyChargerTower00Sdodr other) : base(other)
		{
			spl__EnemyChargerTowerSdodrBancParam = other.spl__EnemyChargerTowerSdodrBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override EnemyChargerTower00Sdodr Clone()
		{
			return new EnemyChargerTower00Sdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EnemyChargerTowerSdodrBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
