using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Fld_SdodrPlaza : MuObj
	{
		[BindGUI("IsEventOnly", Category = "Fld_SdodrPlaza Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEventOnly
		{
			get
			{
				return this.spl__EventActorStateBancParam.IsEventOnly;
			}

			set
			{
				this.spl__EventActorStateBancParam.IsEventOnly = value;
			}
		}

		[BindGUI("MidiLinkNoteNumber", Category = "Fld_SdodrPlaza Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "Fld_SdodrPlaza Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public Fld_SdodrPlaza() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public Fld_SdodrPlaza(Fld_SdodrPlaza other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override Fld_SdodrPlaza Clone()
		{
			return new Fld_SdodrPlaza(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
