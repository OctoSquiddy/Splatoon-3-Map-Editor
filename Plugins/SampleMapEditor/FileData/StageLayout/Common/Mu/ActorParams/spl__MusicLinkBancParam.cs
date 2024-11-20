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
    public class Mu_spl__MusicLinkBancParam
    {
        [ByamlMember]
        public int MidiLinkNoteNumber { get; set; }

        [ByamlMember]
        public string MidiLinkTrackName { get; set; }

        [ByamlMember]
        public string StageType { get; set; }

        public Mu_spl__MusicLinkBancParam()
        {
            MidiLinkNoteNumber = 0;
            MidiLinkTrackName = "";
            StageType = "";
        }

        public Mu_spl__MusicLinkBancParam(Mu_spl__MusicLinkBancParam other)
        {
            MidiLinkNoteNumber = other.MidiLinkNoteNumber;
            MidiLinkTrackName = other.MidiLinkTrackName;
            StageType = other.StageType;
        }

        public Mu_spl__MusicLinkBancParam Clone()
        {
            return new Mu_spl__MusicLinkBancParam(this);
        }

        public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
        {
            BymlNode.DictionaryNode spl__MusicLinkBancParam = new BymlNode.DictionaryNode() { Name = "spl__MusicLinkBancParam" };

            if (SerializedActor["spl__MusicLinkBancParam"] != null)
            {
                spl__MusicLinkBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MusicLinkBancParam"];
            }


            spl__MusicLinkBancParam.AddNode("MidiLinkNoteNumber", this.MidiLinkNoteNumber);

            if (this.MidiLinkTrackName != "")
            {
                spl__MusicLinkBancParam.AddNode("MidiLinkTrackName", this.MidiLinkTrackName);
            }

            if (this.StageType != "")
            {
                spl__MusicLinkBancParam.AddNode("StageType", this.StageType);
            }

            if (SerializedActor["spl__MusicLinkBancParam"] == null)
            {
                SerializedActor.Nodes.Add(spl__MusicLinkBancParam);
            }
        }
    }
}
