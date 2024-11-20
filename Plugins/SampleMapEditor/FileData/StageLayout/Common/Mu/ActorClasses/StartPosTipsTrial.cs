using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class StartPosTipsTrial : MuObj
	{
		[BindGUI("Name", Category = "StartPosTipsTrial Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Name
		{
			get
			{
				return this.spl__StartPosForTipsTrialParam.Name;
			}

			set
			{
				this.spl__StartPosForTipsTrialParam.Name = value;
			}
		}

		[ByamlMember("spl__StartPosForTipsTrialParam")]
		public Mu_spl__StartPosForTipsTrialParam spl__StartPosForTipsTrialParam { get; set; }

		public StartPosTipsTrial() : base()
		{
			spl__StartPosForTipsTrialParam = new Mu_spl__StartPosForTipsTrialParam();

			Links = new List<Link>();
		}

		public StartPosTipsTrial(StartPosTipsTrial other) : base(other)
		{
			spl__StartPosForTipsTrialParam = other.spl__StartPosForTipsTrialParam.Clone();
		}

		public override StartPosTipsTrial Clone()
		{
			return new StartPosTipsTrial(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__StartPosForTipsTrialParam.SaveParameterBank(SerializedActor);
		}
	}
}
