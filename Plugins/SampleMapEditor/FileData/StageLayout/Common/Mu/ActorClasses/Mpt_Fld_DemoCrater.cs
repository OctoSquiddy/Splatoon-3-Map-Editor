using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Mpt_Fld_DemoCrater : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "Mpt_Fld_DemoCrater Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[BindGUI("IsEventOnly", Category = "Mpt_Fld_DemoCrater Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "Mpt_Fld_DemoCrater Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		public Mpt_Fld_DemoCrater() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();

			Links = new List<Link>();
		}

		public Mpt_Fld_DemoCrater(Mpt_Fld_DemoCrater other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
		}

		public override Mpt_Fld_DemoCrater Clone()
		{
			return new Mpt_Fld_DemoCrater(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
