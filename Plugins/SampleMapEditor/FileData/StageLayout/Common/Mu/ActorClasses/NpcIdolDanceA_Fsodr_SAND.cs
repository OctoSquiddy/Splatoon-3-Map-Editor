using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NpcIdolDanceA_Fsodr_SAND : MuObj
	{
		[BindGUI("StageType", Category = "NpcIdolDanceA_Fsodr_SAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _StageType
		{
			get
			{
				return this.spl__MusicLinkBancParam.StageType;
			}

			set
			{
				this.spl__MusicLinkBancParam.StageType = value;
			}
		}

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public NpcIdolDanceA_Fsodr_SAND() : base()
		{
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public NpcIdolDanceA_Fsodr_SAND(NpcIdolDanceA_Fsodr_SAND other) : base(other)
		{
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override NpcIdolDanceA_Fsodr_SAND Clone()
		{
			return new NpcIdolDanceA_Fsodr_SAND(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
