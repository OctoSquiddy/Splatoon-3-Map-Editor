using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorKumasanRocketChangeStep : MuObj
	{
		[BindGUI("Level", Category = "LocatorKumasanRocketChangeStep Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Level
		{
			get
			{
				return this.spl__LocatorKumasanRocketChangeStepBancParam.Level;
			}

			set
			{
				this.spl__LocatorKumasanRocketChangeStepBancParam.Level = value;
			}
		}

		[BindGUI("Step", Category = "LocatorKumasanRocketChangeStep Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Step
		{
			get
			{
				return this.spl__LocatorKumasanRocketChangeStepBancParam.Step;
			}

			set
			{
				this.spl__LocatorKumasanRocketChangeStepBancParam.Step = value;
			}
		}

		[ByamlMember("spl__LocatorKumasanRocketChangeStepBancParam")]
		public Mu_spl__LocatorKumasanRocketChangeStepBancParam spl__LocatorKumasanRocketChangeStepBancParam { get; set; }

		public LocatorKumasanRocketChangeStep() : base()
		{
			spl__LocatorKumasanRocketChangeStepBancParam = new Mu_spl__LocatorKumasanRocketChangeStepBancParam();

			Links = new List<Link>();
		}

		public LocatorKumasanRocketChangeStep(LocatorKumasanRocketChangeStep other) : base(other)
		{
			spl__LocatorKumasanRocketChangeStepBancParam = other.spl__LocatorKumasanRocketChangeStepBancParam.Clone();
		}

		public override LocatorKumasanRocketChangeStep Clone()
		{
			return new LocatorKumasanRocketChangeStep(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorKumasanRocketChangeStepBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
