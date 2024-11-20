using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class StartPosForLaunchPadWorld : MuObj
	{
		[BindGUI("Progress", Category = "StartPosForLaunchPadWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Progress
		{
			get
			{
				return this.spl__StartPosForLaunchPadWorldBancParam.Progress;
			}

			set
			{
				this.spl__StartPosForLaunchPadWorldBancParam.Progress = value;
			}
		}

		[ByamlMember("spl__StartPosForLaunchPadWorldBancParam")]
		public Mu_spl__StartPosForLaunchPadWorldBancParam spl__StartPosForLaunchPadWorldBancParam { get; set; }

		public StartPosForLaunchPadWorld() : base()
		{
			spl__StartPosForLaunchPadWorldBancParam = new Mu_spl__StartPosForLaunchPadWorldBancParam();

			Links = new List<Link>();
		}

		public StartPosForLaunchPadWorld(StartPosForLaunchPadWorld other) : base(other)
		{
			spl__StartPosForLaunchPadWorldBancParam = other.spl__StartPosForLaunchPadWorldBancParam.Clone();
		}

		public override StartPosForLaunchPadWorld Clone()
		{
			return new StartPosForLaunchPadWorld(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__StartPosForLaunchPadWorldBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
