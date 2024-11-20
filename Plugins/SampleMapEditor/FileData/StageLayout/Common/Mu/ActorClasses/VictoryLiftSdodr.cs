using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class VictoryLiftSdodr : MuObj
	{
		[BindGUI("CreateBlister", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("PaintColAttr", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsIncludeVArea", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__RailFollowBancParam.ToRailPoint;
			}

			set
			{
				this.spl__RailFollowBancParam.ToRailPoint = value;
			}
		}

		[BindGUI("CheckPointIndex0", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CheckPointIndex0
		{
			get
			{
				return this.spl__VictoryLiftBancParamSdodr.CheckPointIndex0;
			}

			set
			{
				this.spl__VictoryLiftBancParamSdodr.CheckPointIndex0 = value;
			}
		}

		[BindGUI("CheckPointIndex1", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CheckPointIndex1
		{
			get
			{
				return this.spl__VictoryLiftBancParamSdodr.CheckPointIndex1;
			}

			set
			{
				this.spl__VictoryLiftBancParamSdodr.CheckPointIndex1 = value;
			}
		}

		[BindGUI("CheckPointIndex2", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _CheckPointIndex2
		{
			get
			{
				return this.spl__VictoryLiftBancParamSdodr.CheckPointIndex2;
			}

			set
			{
				this.spl__VictoryLiftBancParamSdodr.CheckPointIndex2 = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnableMoveChatteringPrevent
		{
			get
			{
				return this.spl__ailift__AILiftBancParam.IsEnableMoveChatteringPrevent;
			}

			set
			{
				this.spl__ailift__AILiftBancParam.IsEnableMoveChatteringPrevent = value;
			}
		}

		[BindGUI("ToRailPoint1", Category = "VictoryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint1
		{
			get
			{
				return this.spl__ailift__AILiftBancParam.ToRailPoint;
			}

			set
			{
				this.spl__ailift__AILiftBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("game__gfx__SdodrDesignTemplateBancParam")]
		public Mu_game__gfx__SdodrDesignTemplateBancParam game__gfx__SdodrDesignTemplateBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		[ByamlMember("spl__VictoryLiftBancParamSdodr")]
		public Mu_spl__VictoryLiftBancParamSdodr spl__VictoryLiftBancParamSdodr { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public VictoryLiftSdodr() : base()
		{
			game__gfx__SdodrDesignTemplateBancParam = new Mu_game__gfx__SdodrDesignTemplateBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();
			spl__VictoryLiftBancParamSdodr = new Mu_spl__VictoryLiftBancParamSdodr();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public VictoryLiftSdodr(VictoryLiftSdodr other) : base(other)
		{
			game__gfx__SdodrDesignTemplateBancParam = other.game__gfx__SdodrDesignTemplateBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
			spl__VictoryLiftBancParamSdodr = other.spl__VictoryLiftBancParamSdodr.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override VictoryLiftSdodr Clone()
		{
			return new VictoryLiftSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__gfx__SdodrDesignTemplateBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
			this.spl__VictoryLiftBancParamSdodr.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
