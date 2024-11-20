using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SwitchStep : MuObj
	{
		[BindGUI("IsToggle", Category = "SwitchStep Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsToggle
		{
			get
			{
				return this.spl__SwitchStepBancParam.IsToggle;
			}

			set
			{
				this.spl__SwitchStepBancParam.IsToggle = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "SwitchStep Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "SwitchStep Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
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

		[ByamlMember("spl__SwitchStepBancParam")]
		public Mu_spl__SwitchStepBancParam spl__SwitchStepBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public SwitchStep() : base()
		{
			spl__SwitchStepBancParam = new Mu_spl__SwitchStepBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public SwitchStep(SwitchStep other) : base(other)
		{
			spl__SwitchStepBancParam = other.spl__SwitchStepBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override SwitchStep Clone()
		{
			return new SwitchStep(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SwitchStepBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
