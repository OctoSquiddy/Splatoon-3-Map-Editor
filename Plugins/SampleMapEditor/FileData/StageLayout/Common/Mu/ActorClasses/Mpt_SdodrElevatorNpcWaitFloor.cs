using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Mpt_SdodrElevatorNpcWaitFloor : MuObj
	{
		[BindGUI("CreateBlister", Category = "Mpt_SdodrElevatorNpcWaitFloor Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _CreateBlister
		{
			get
			{
				return this.game__gfx__SdodrDesignTemplateBancParam.CreateBlister;
			}

			set
			{
				this.game__gfx__SdodrDesignTemplateBancParam.CreateBlister = value;
			}
		}

		[BindGUI("PaintColAttr", Category = "Mpt_SdodrElevatorNpcWaitFloor Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _PaintColAttr
		{
			get
			{
				return this.game__gfx__SdodrDesignTemplateBancParam.PaintColAttr;
			}

			set
			{
				this.game__gfx__SdodrDesignTemplateBancParam.PaintColAttr = value;
			}
		}

		[BindGUI("IsEventOnly", Category = "Mpt_SdodrElevatorNpcWaitFloor Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "Mpt_SdodrElevatorNpcWaitFloor Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("game__gfx__SdodrDesignTemplateBancParam")]
		public Mu_game__gfx__SdodrDesignTemplateBancParam game__gfx__SdodrDesignTemplateBancParam { get; set; }

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		public Mpt_SdodrElevatorNpcWaitFloor() : base()
		{
			game__gfx__SdodrDesignTemplateBancParam = new Mu_game__gfx__SdodrDesignTemplateBancParam();
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();

			Links = new List<Link>();
		}

		public Mpt_SdodrElevatorNpcWaitFloor(Mpt_SdodrElevatorNpcWaitFloor other) : base(other)
		{
			game__gfx__SdodrDesignTemplateBancParam = other.game__gfx__SdodrDesignTemplateBancParam.Clone();
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
		}

		public override Mpt_SdodrElevatorNpcWaitFloor Clone()
		{
			return new Mpt_SdodrElevatorNpcWaitFloor(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__gfx__SdodrDesignTemplateBancParam.SaveParameterBank(SerializedActor);
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
