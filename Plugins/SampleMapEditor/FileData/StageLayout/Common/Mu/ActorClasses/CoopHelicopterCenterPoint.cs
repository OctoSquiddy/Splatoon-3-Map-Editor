using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopHelicopterCenterPoint : MuObj
	{
		[BindGUI("DropDemoInitialAngleDeg", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DropDemoInitialAngleDeg
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.DropDemoInitialAngleDeg;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.DropDemoInitialAngleDeg = value;
			}
		}

		[BindGUI("HelicopterHeight", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HelicopterHeight
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HelicopterHeight;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HelicopterHeight = value;
			}
		}

		[BindGUI("HelicopterHeightStageView", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HelicopterHeightStageView
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HelicopterHeightStageView;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HelicopterHeightStageView = value;
			}
		}

		[BindGUI("HelicopterRadius", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HelicopterRadius
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HelicopterRadius;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HelicopterRadius = value;
			}
		}

		[BindGUI("HelicopterRadiusStageView", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HelicopterRadiusStageView
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HelicopterRadiusStageView;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HelicopterRadiusStageView = value;
			}
		}

		[BindGUI("HoveringDropDemoApproachDist", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HoveringDropDemoApproachDist
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HoveringDropDemoApproachDist;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HoveringDropDemoApproachDist = value;
			}
		}

		[BindGUI("HoveringDropDemoDescentHeight", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _HoveringDropDemoDescentHeight
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HoveringDropDemoDescentHeight;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HoveringDropDemoDescentHeight = value;
			}
		}

		[BindGUI("HoveringStageViewApproachDist", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _HoveringStageViewApproachDist
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HoveringStageViewApproachDist;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HoveringStageViewApproachDist = value;
			}
		}

		[BindGUI("HoveringStageViewDescentHeight", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _HoveringStageViewDescentHeight
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.HoveringStageViewDescentHeight;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.HoveringStageViewDescentHeight = value;
			}
		}

		[BindGUI("IsHovering", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsHovering
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.IsHovering;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.IsHovering = value;
			}
		}

		[BindGUI("StageViewDemoInitialAngleDeg", Category = "CoopHelicopterCenterPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StageViewDemoInitialAngleDeg
		{
			get
			{
				return this.spl__CoopHelicopterCenterPointBancParam.StageViewDemoInitialAngleDeg;
			}

			set
			{
				this.spl__CoopHelicopterCenterPointBancParam.StageViewDemoInitialAngleDeg = value;
			}
		}

		[ByamlMember("spl__CoopHelicopterCenterPointBancParam")]
		public Mu_spl__CoopHelicopterCenterPointBancParam spl__CoopHelicopterCenterPointBancParam { get; set; }

		public CoopHelicopterCenterPoint() : base()
		{
			spl__CoopHelicopterCenterPointBancParam = new Mu_spl__CoopHelicopterCenterPointBancParam();

			Links = new List<Link>();
		}

		public CoopHelicopterCenterPoint(CoopHelicopterCenterPoint other) : base(other)
		{
			spl__CoopHelicopterCenterPointBancParam = other.spl__CoopHelicopterCenterPointBancParam.Clone();
		}

		public override CoopHelicopterCenterPoint Clone()
		{
			return new CoopHelicopterCenterPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopHelicopterCenterPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
