using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class StartPosForCoopBossRound : MuObj
	{
		[BindGUI("Name", Category = "StartPosForCoopBossRound Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Name
		{
			get
			{
				return this.spl__StartPosParam.Name;
			}

			set
			{
				this.spl__StartPosParam.Name = value;
			}
		}

		[BindGUI("PlayerIndex", Category = "StartPosForCoopBossRound Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _PlayerIndex
		{
			get
			{
				return this.spl__StartPosParam.PlayerIndex;
			}

			set
			{
				this.spl__StartPosParam.PlayerIndex = value;
			}
		}

		[ByamlMember("spl__StartPosParam")]
		public Mu_spl__StartPosParam spl__StartPosParam { get; set; }

		public StartPosForCoopBossRound() : base()
		{
			spl__StartPosParam = new Mu_spl__StartPosParam();

			Links = new List<Link>();
		}

		public StartPosForCoopBossRound(StartPosForCoopBossRound other) : base(other)
		{
			spl__StartPosParam = other.spl__StartPosParam.Clone();
		}

		public override StartPosForCoopBossRound Clone()
		{
			return new StartPosForCoopBossRound(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__StartPosParam.SaveParameterBank(SerializedActor);
		}
	}
}
