using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpongeSmall_3p0_VS : MuObj
	{
		[BindGUI("IsFreeY", Category = "SpongeSmall_3p0_VS Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Type", Category = "SpongeSmall_3p0_VS Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Type
		{
			get
			{
				return this.spl__SpongeBancParam.Type;
			}

			set
			{
				this.spl__SpongeBancParam.Type = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__SpongeBancParam")]
		public Mu_spl__SpongeBancParam spl__SpongeBancParam { get; set; }

		public SpongeSmall_3p0_VS() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__SpongeBancParam = new Mu_spl__SpongeBancParam();

			Links = new List<Link>();
		}

		public SpongeSmall_3p0_VS(SpongeSmall_3p0_VS other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__SpongeBancParam = other.spl__SpongeBancParam.Clone();
		}

		public override SpongeSmall_3p0_VS Clone()
		{
			return new SpongeSmall_3p0_VS(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__SpongeBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
