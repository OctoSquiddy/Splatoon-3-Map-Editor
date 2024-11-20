using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class MissionBossGateway : MuObj
	{
		[BindGUI("ChangeSceneName", Category = "MissionBossGateway Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ChangeSceneName
		{
			get
			{
				return this.spl__MissionGatewayBancParam.ChangeSceneName;
			}

			set
			{
				this.spl__MissionGatewayBancParam.ChangeSceneName = value;
			}
		}

		[BindGUI("DevText", Category = "MissionBossGateway Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _DevText
		{
			get
			{
				return this.spl__MissionGatewayBancParam.DevText;
			}

			set
			{
				this.spl__MissionGatewayBancParam.DevText = value;
			}
		}

		[BindGUI("SendSignalOnOpen", Category = "MissionBossGateway Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _SendSignalOnOpen
		{
			get
			{
				return this.spl__MissionGatewayBancParam.SendSignalOnOpen;
			}

			set
			{
				this.spl__MissionGatewayBancParam.SendSignalOnOpen = value;
			}
		}

		[ByamlMember("spl__MissionGatewayBancParam")]
		public Mu_spl__MissionGatewayBancParam spl__MissionGatewayBancParam { get; set; }

		public MissionBossGateway() : base()
		{
			spl__MissionGatewayBancParam = new Mu_spl__MissionGatewayBancParam();

			Links = new List<Link>();
		}

		public MissionBossGateway(MissionBossGateway other) : base(other)
		{
			spl__MissionGatewayBancParam = other.spl__MissionGatewayBancParam.Clone();
		}

		public override MissionBossGateway Clone()
		{
			return new MissionBossGateway(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MissionGatewayBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
