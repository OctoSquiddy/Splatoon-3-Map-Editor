using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplMissionSalmonBuddy : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "SplMissionSalmonBuddy Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ConstraintType", Category = "SplMissionSalmonBuddy Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ConstraintType
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.ConstraintType;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.ConstraintType = value;
			}
		}

		[BindGUI("FreeAxis", Category = "SplMissionSalmonBuddy Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _FreeAxis
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.FreeAxis;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.FreeAxis = value;
			}
		}

		[BindGUI("IsBindToWorld", Category = "SplMissionSalmonBuddy Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsBindToWorld
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.IsBindToWorld;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.IsBindToWorld = value;
			}
		}

		[BindGUI("IsNoHitBetweenBodies", Category = "SplMissionSalmonBuddy Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsNoHitBetweenBodies
		{
			get
			{
				return this.spl__ConstraintBindableHelperBancParam.IsNoHitBetweenBodies;
			}

			set
			{
				this.spl__ConstraintBindableHelperBancParam.IsNoHitBetweenBodies = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "SplMissionSalmonBuddy Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__ConstraintBindableHelperBancParam")]
		public Mu_spl__ConstraintBindableHelperBancParam spl__ConstraintBindableHelperBancParam { get; set; }

		[ByamlMember("spl__RailFollowBancParam")]
		public Mu_spl__RailFollowBancParam spl__RailFollowBancParam { get; set; }

		public SplMissionSalmonBuddy() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__ConstraintBindableHelperBancParam = new Mu_spl__ConstraintBindableHelperBancParam();
			spl__RailFollowBancParam = new Mu_spl__RailFollowBancParam();

			Links = new List<Link>();
		}

		public SplMissionSalmonBuddy(SplMissionSalmonBuddy other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__ConstraintBindableHelperBancParam = other.spl__ConstraintBindableHelperBancParam.Clone();
			spl__RailFollowBancParam = other.spl__RailFollowBancParam.Clone();
		}

		public override SplMissionSalmonBuddy Clone()
		{
			return new SplMissionSalmonBuddy(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__ConstraintBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__RailFollowBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
