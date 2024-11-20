using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Mpt_PlayerDead : MuObj
	{
		[BindGUI("IsEventOnly", Category = "Mpt_PlayerDead Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "Mpt_PlayerDead Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsIncludeVArea
		{
			get
			{
				return this.spl__PaintBancParam.IsIncludeVArea;
			}

			set
			{
				this.spl__PaintBancParam.IsIncludeVArea = value;
			}
		}

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		public Mpt_PlayerDead() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();

			Links = new List<Link>();
		}

		public Mpt_PlayerDead(Mpt_PlayerDead other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
		}

		public override Mpt_PlayerDead Clone()
		{
			return new Mpt_PlayerDead(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
