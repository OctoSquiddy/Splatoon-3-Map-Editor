using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorNpcLobby : MuObj
	{
		[BindGUI("ASCommand", Category = "LocatorNpcLobby Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ASCommand
		{
			get
			{
				return this.spl__LocatorNpcLobbyBancParam.ASCommand;
			}

			set
			{
				this.spl__LocatorNpcLobbyBancParam.ASCommand = value;
			}
		}

		[BindGUI("Pattern", Category = "LocatorNpcLobby Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Pattern
		{
			get
			{
				return this.spl__LocatorNpcLobbyBancParam.Pattern;
			}

			set
			{
				this.spl__LocatorNpcLobbyBancParam.Pattern = value;
			}
		}

		[ByamlMember("spl__LocatorNpcLobbyBancParam")]
		public Mu_spl__LocatorNpcLobbyBancParam spl__LocatorNpcLobbyBancParam { get; set; }

		public LocatorNpcLobby() : base()
		{
			spl__LocatorNpcLobbyBancParam = new Mu_spl__LocatorNpcLobbyBancParam();

			Links = new List<Link>();
		}

		public LocatorNpcLobby(LocatorNpcLobby other) : base(other)
		{
			spl__LocatorNpcLobbyBancParam = other.spl__LocatorNpcLobbyBancParam.Clone();
		}

		public override LocatorNpcLobby Clone()
		{
			return new LocatorNpcLobby(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorNpcLobbyBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
