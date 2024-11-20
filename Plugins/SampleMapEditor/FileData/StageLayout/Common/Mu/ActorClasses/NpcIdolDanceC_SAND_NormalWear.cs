using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NpcIdolDanceC_SAND_NormalWear : MuObj
	{
		[BindGUI("StageType", Category = "NpcIdolDanceC_SAND_NormalWear Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public NpcIdolDanceC_SAND_NormalWear() : base()
		{
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public NpcIdolDanceC_SAND_NormalWear(NpcIdolDanceC_SAND_NormalWear other) : base(other)
		{
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override NpcIdolDanceC_SAND_NormalWear Clone()
		{
			return new NpcIdolDanceC_SAND_NormalWear(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
