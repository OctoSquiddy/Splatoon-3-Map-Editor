using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class ActionAreaSwitchForPlayer : MuObj
	{
		[BindGUI("EnableMsnArmorDieToZero", Category = "ActionAreaSwitchForPlayer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _EnableMsnArmorDieToZero
		{
			get
			{
				return this.spl__ActionAreaSwitchForPlayerBancParam.EnableMsnArmorDieToZero;
			}

			set
			{
				this.spl__ActionAreaSwitchForPlayerBancParam.EnableMsnArmorDieToZero = value;
			}
		}

		[BindGUI("EnableMsnArmorGetFromZero", Category = "ActionAreaSwitchForPlayer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _EnableMsnArmorGetFromZero
		{
			get
			{
				return this.spl__ActionAreaSwitchForPlayerBancParam.EnableMsnArmorGetFromZero;
			}

			set
			{
				this.spl__ActionAreaSwitchForPlayerBancParam.EnableMsnArmorGetFromZero = value;
			}
		}

		[BindGUI("EnablePlayerSpecialStart", Category = "ActionAreaSwitchForPlayer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _EnablePlayerSpecialStart
		{
			get
			{
				return this.spl__ActionAreaSwitchForPlayerBancParam.EnablePlayerSpecialStart;
			}

			set
			{
				this.spl__ActionAreaSwitchForPlayerBancParam.EnablePlayerSpecialStart = value;
			}
		}

		[BindGUI("EnableSpecialFullSuperLanding", Category = "ActionAreaSwitchForPlayer Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _EnableSpecialFullSuperLanding
		{
			get
			{
				return this.spl__ActionAreaSwitchForPlayerBancParam.EnableSpecialFullSuperLanding;
			}

			set
			{
				this.spl__ActionAreaSwitchForPlayerBancParam.EnableSpecialFullSuperLanding = value;
			}
		}

		[ByamlMember("spl__ActionAreaSwitchForPlayerBancParam")]
		public Mu_spl__ActionAreaSwitchForPlayerBancParam spl__ActionAreaSwitchForPlayerBancParam { get; set; }

		public ActionAreaSwitchForPlayer() : base()
		{
			spl__ActionAreaSwitchForPlayerBancParam = new Mu_spl__ActionAreaSwitchForPlayerBancParam();

			Links = new List<Link>();
		}

		public ActionAreaSwitchForPlayer(ActionAreaSwitchForPlayer other) : base(other)
		{
			spl__ActionAreaSwitchForPlayerBancParam = other.spl__ActionAreaSwitchForPlayerBancParam.Clone();
		}

		public override ActionAreaSwitchForPlayer Clone()
		{
			return new ActionAreaSwitchForPlayer(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActionAreaSwitchForPlayerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
