using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__CoopHelicopterCenterPointBancParam
	{
		[ByamlMember]
		public float DropDemoInitialAngleDeg { get; set; }

		[ByamlMember]
		public float HelicopterHeight { get; set; }

		[ByamlMember]
		public float HelicopterHeightStageView { get; set; }

		[ByamlMember]
		public float HelicopterRadius { get; set; }

		[ByamlMember]
		public float HelicopterRadiusStageView { get; set; }

		[ByamlMember]
		public float HoveringDropDemoApproachDist { get; set; }

		[ByamlMember]
		public float HoveringDropDemoDescentHeight { get; set; }

		[ByamlMember]
		public int HoveringStageViewApproachDist { get; set; }

		[ByamlMember]
		public int HoveringStageViewDescentHeight { get; set; }

		[ByamlMember]
		public bool IsHovering { get; set; }

		[ByamlMember]
		public float StageViewDemoInitialAngleDeg { get; set; }

		public Mu_spl__CoopHelicopterCenterPointBancParam()
		{
			DropDemoInitialAngleDeg = 0.0f;
			HelicopterHeight = 0.0f;
			HelicopterHeightStageView = 0.0f;
			HelicopterRadius = 0.0f;
			HelicopterRadiusStageView = 0.0f;
			HoveringDropDemoApproachDist = 0.0f;
			HoveringDropDemoDescentHeight = 0.0f;
			HoveringStageViewApproachDist = 0;
			HoveringStageViewDescentHeight = 0;
			IsHovering = false;
			StageViewDemoInitialAngleDeg = 0.0f;
		}

		public Mu_spl__CoopHelicopterCenterPointBancParam(Mu_spl__CoopHelicopterCenterPointBancParam other)
		{
			DropDemoInitialAngleDeg = other.DropDemoInitialAngleDeg;
			HelicopterHeight = other.HelicopterHeight;
			HelicopterHeightStageView = other.HelicopterHeightStageView;
			HelicopterRadius = other.HelicopterRadius;
			HelicopterRadiusStageView = other.HelicopterRadiusStageView;
			HoveringDropDemoApproachDist = other.HoveringDropDemoApproachDist;
			HoveringDropDemoDescentHeight = other.HoveringDropDemoDescentHeight;
			HoveringStageViewApproachDist = other.HoveringStageViewApproachDist;
			HoveringStageViewDescentHeight = other.HoveringStageViewDescentHeight;
			IsHovering = other.IsHovering;
			StageViewDemoInitialAngleDeg = other.StageViewDemoInitialAngleDeg;
		}

		public Mu_spl__CoopHelicopterCenterPointBancParam Clone()
		{
			return new Mu_spl__CoopHelicopterCenterPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopHelicopterCenterPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopHelicopterCenterPointBancParam" };

			if (SerializedActor["spl__CoopHelicopterCenterPointBancParam"] != null)
			{
				spl__CoopHelicopterCenterPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopHelicopterCenterPointBancParam"];
			}


			spl__CoopHelicopterCenterPointBancParam.AddNode("DropDemoInitialAngleDeg", this.DropDemoInitialAngleDeg);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HelicopterHeight", this.HelicopterHeight);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HelicopterHeightStageView", this.HelicopterHeightStageView);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HelicopterRadius", this.HelicopterRadius);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HelicopterRadiusStageView", this.HelicopterRadiusStageView);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HoveringDropDemoApproachDist", this.HoveringDropDemoApproachDist);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HoveringDropDemoDescentHeight", this.HoveringDropDemoDescentHeight);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HoveringStageViewApproachDist", this.HoveringStageViewApproachDist);

			spl__CoopHelicopterCenterPointBancParam.AddNode("HoveringStageViewDescentHeight", this.HoveringStageViewDescentHeight);

			if (this.IsHovering)
			{
				spl__CoopHelicopterCenterPointBancParam.AddNode("IsHovering", this.IsHovering);
			}

			spl__CoopHelicopterCenterPointBancParam.AddNode("StageViewDemoInitialAngleDeg", this.StageViewDemoInitialAngleDeg);

			if (SerializedActor["spl__CoopHelicopterCenterPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopHelicopterCenterPointBancParam);
			}
		}
	}
}
