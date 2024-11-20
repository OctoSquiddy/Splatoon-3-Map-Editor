using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__MidiLinkPolyhedronSdodrBancParam
	{
		[ByamlMember]
		public string KeyName { get; set; }

		[ByamlMember]
		public string ManualDucking { get; set; }

		[ByamlMember]
		public string NoteNumberArray { get; set; }

		[ByamlMember]
		public string TrackName { get; set; }

		public Mu_spl__MidiLinkPolyhedronSdodrBancParam()
		{
			KeyName = "";
			ManualDucking = "";
			NoteNumberArray = "";
			TrackName = "";
		}

		public Mu_spl__MidiLinkPolyhedronSdodrBancParam(Mu_spl__MidiLinkPolyhedronSdodrBancParam other)
		{
			KeyName = other.KeyName;
			ManualDucking = other.ManualDucking;
			NoteNumberArray = other.NoteNumberArray;
			TrackName = other.TrackName;
		}

		public Mu_spl__MidiLinkPolyhedronSdodrBancParam Clone()
		{
			return new Mu_spl__MidiLinkPolyhedronSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MidiLinkPolyhedronSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__MidiLinkPolyhedronSdodrBancParam" };

			if (SerializedActor["spl__MidiLinkPolyhedronSdodrBancParam"] != null)
			{
				spl__MidiLinkPolyhedronSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MidiLinkPolyhedronSdodrBancParam"];
			}


			if (this.KeyName != "")
			{
				spl__MidiLinkPolyhedronSdodrBancParam.AddNode("KeyName", this.KeyName);
			}

			if (this.ManualDucking != "")
			{
				spl__MidiLinkPolyhedronSdodrBancParam.AddNode("ManualDucking", this.ManualDucking);
			}

			if (this.NoteNumberArray != "")
			{
				spl__MidiLinkPolyhedronSdodrBancParam.AddNode("NoteNumberArray", this.NoteNumberArray);
			}

			if (this.TrackName != "")
			{
				spl__MidiLinkPolyhedronSdodrBancParam.AddNode("TrackName", this.TrackName);
			}

			if (SerializedActor["spl__MidiLinkPolyhedronSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MidiLinkPolyhedronSdodrBancParam);
			}
		}
	}
}
