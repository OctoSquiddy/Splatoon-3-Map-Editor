using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_SdodrMidiLinkPolyhedron : MuObj
	{
		[BindGUI("KeyName", Category = "Obj_SdodrMidiLinkPolyhedron Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _KeyName
		{
			get
			{
				return this.spl__MidiLinkPolyhedronSdodrBancParam.KeyName;
			}

			set
			{
				this.spl__MidiLinkPolyhedronSdodrBancParam.KeyName = value;
			}
		}

		[BindGUI("ManualDucking", Category = "Obj_SdodrMidiLinkPolyhedron Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ManualDucking
		{
			get
			{
				return this.spl__MidiLinkPolyhedronSdodrBancParam.ManualDucking;
			}

			set
			{
				this.spl__MidiLinkPolyhedronSdodrBancParam.ManualDucking = value;
			}
		}

		[BindGUI("NoteNumberArray", Category = "Obj_SdodrMidiLinkPolyhedron Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _NoteNumberArray
		{
			get
			{
				return this.spl__MidiLinkPolyhedronSdodrBancParam.NoteNumberArray;
			}

			set
			{
				this.spl__MidiLinkPolyhedronSdodrBancParam.NoteNumberArray = value;
			}
		}

		[BindGUI("TrackName", Category = "Obj_SdodrMidiLinkPolyhedron Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _TrackName
		{
			get
			{
				return this.spl__MidiLinkPolyhedronSdodrBancParam.TrackName;
			}

			set
			{
				this.spl__MidiLinkPolyhedronSdodrBancParam.TrackName = value;
			}
		}

		[ByamlMember("spl__MidiLinkPolyhedronSdodrBancParam")]
		public Mu_spl__MidiLinkPolyhedronSdodrBancParam spl__MidiLinkPolyhedronSdodrBancParam { get; set; }

		public Obj_SdodrMidiLinkPolyhedron() : base()
		{
			spl__MidiLinkPolyhedronSdodrBancParam = new Mu_spl__MidiLinkPolyhedronSdodrBancParam();

			Links = new List<Link>();
		}

		public Obj_SdodrMidiLinkPolyhedron(Obj_SdodrMidiLinkPolyhedron other) : base(other)
		{
			spl__MidiLinkPolyhedronSdodrBancParam = other.spl__MidiLinkPolyhedronSdodrBancParam.Clone();
		}

		public override Obj_SdodrMidiLinkPolyhedron Clone()
		{
			return new Obj_SdodrMidiLinkPolyhedron(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MidiLinkPolyhedronSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
