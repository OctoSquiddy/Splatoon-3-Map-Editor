using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NpcIdolDanceA : MuObj
	{
		[BindGUI("BoneName", Category = "NpcIdolDanceA Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _BoneName
		{
			get
			{
				return this.spl__NpcIdolDanceBancParam.BoneName;
			}

			set
			{
				this.spl__NpcIdolDanceBancParam.BoneName = value;
			}
		}

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		[ByamlMember("spl__NpcIdolDanceBancParam")]
		public Mu_spl__NpcIdolDanceBancParam spl__NpcIdolDanceBancParam { get; set; }

		public NpcIdolDanceA() : base()
		{
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();
			spl__NpcIdolDanceBancParam = new Mu_spl__NpcIdolDanceBancParam();

			Links = new List<Link>();
		}

		public NpcIdolDanceA(NpcIdolDanceA other) : base(other)
		{
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
			spl__NpcIdolDanceBancParam = other.spl__NpcIdolDanceBancParam.Clone();
		}

		public override NpcIdolDanceA Clone()
		{
			return new NpcIdolDanceA(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
			this.spl__NpcIdolDanceBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
