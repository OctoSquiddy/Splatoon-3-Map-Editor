using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Fld_SdodrBarrierKing : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "Fld_SdodrBarrierKing Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkNoteNumber", Category = "Fld_SdodrBarrierKing Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "Fld_SdodrBarrierKing Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public Fld_SdodrBarrierKing() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public Fld_SdodrBarrierKing(Fld_SdodrBarrierKing other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override Fld_SdodrBarrierKing Clone()
		{
			return new Fld_SdodrBarrierKing(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
