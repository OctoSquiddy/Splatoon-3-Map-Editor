using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyBarrierKingSdodrStoryBoss : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "SplEnemyBarrierKingSdodrStoryBoss Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[BindGUI("MidiLinkNoteNumber", Category = "SplEnemyBarrierKingSdodrStoryBoss Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "SplEnemyBarrierKingSdodrStoryBoss Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam")]
		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public SplEnemyBarrierKingSdodrStoryBoss() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = new Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public SplEnemyBarrierKingSdodrStoryBoss(SplEnemyBarrierKingSdodrStoryBoss other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = other.spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override SplEnemyBarrierKingSdodrStoryBoss Clone()
		{
			return new SplEnemyBarrierKingSdodrStoryBoss(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
