using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NpcIdolDanceB_Sdodr_SAND_VS : MuObj
	{
		[BindGUI("IsFreeY", Category = "NpcIdolDanceB_Sdodr_SAND_VS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsFreeY
		{
			get
			{
				return this.spl__ActorMatrixBindableHelperBancParam.IsFreeY;
			}

			set
			{
				this.spl__ActorMatrixBindableHelperBancParam.IsFreeY = value;
			}
		}

		[BindGUI("StageType", Category = "NpcIdolDanceB_Sdodr_SAND_VS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public NpcIdolDanceB_Sdodr_SAND_VS() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public NpcIdolDanceB_Sdodr_SAND_VS(NpcIdolDanceB_Sdodr_SAND_VS other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override NpcIdolDanceB_Sdodr_SAND_VS Clone()
		{
			return new NpcIdolDanceB_Sdodr_SAND_VS(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
