using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplFieldEnvEffectArea : MuObj
	{
        [BindGUI("Key Name",
            new object[38] { "", "CameraDripSmall", "SeaPlantSplash", "Fish", "Fountain", "DrainageWater", "Moth",
            "GardenFountain", "FountainNose", "Butterfly", "Fly", "Firefly", "Spotlight", "DuctSmoke01", "SpotLightConeShape",
            "TorchFire", "FoodSmoke", "Bubble", "Smoke", "BubbleSmall", "Quicksand_L", "Quicksand_S", "Steam",
            "SandTornado", "SpoutedMuddyWater", "SpoutedMuddyWater_Little", "Quicksand_S_night", "Quicksand_L_night",
            "Light", "SpotLightCone", "WeedDark", "Weed", "WeedBright", "Drone_01", "Drone_02", "SpotLightBig", "AreaSnow",
            "AreaRain" },
            new string[38] { "None", "CameraDripSmall", "SeaPlantSplash", "Fish", "Fountain", "DrainageWater", "Moth",
            "GardenFountain", "FountainNose", "Butterfly", "Fly", "Firefly", "Spotlight", "DuctSmoke01", "SpotLightConeShape",
            "TorchFire", "FoodSmoke", "Bubble", "Smoke", "BubbleSmall", "Quicksand_L", "Quicksand_S", "Steam",
            "SandTornado", "SpoutedMuddyWater", "SpoutedMuddyWater_Little", "Quicksand_S_night", "Quicksand_L_night",
            "Light", "SpotLightCone", "WeedDark", "Weed", "WeedBright", "Drone_01", "Drone_02", "SpotLightBig", "AreaSnow",
            "AreaRain" },
            Category = "SplFieldEnvEffectLocator Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string _KeyName
		{
			get
			{
				return this.spl__FieldEnvEffectLocatorBancParam.KeyName;
			}

			set
			{
				this.spl__FieldEnvEffectLocatorBancParam.KeyName = value;
			}
		}

		[ByamlMember("spl__FieldEnvEffectLocatorBancParam")]
		public Mu_spl__FieldEnvEffectLocatorBancParam spl__FieldEnvEffectLocatorBancParam { get; set; }

		public SplFieldEnvEffectArea() : base()
		{
			spl__FieldEnvEffectLocatorBancParam = new Mu_spl__FieldEnvEffectLocatorBancParam();

			Links = new List<Link>();
		}

		public SplFieldEnvEffectArea(SplFieldEnvEffectArea other) : base(other)
		{
			spl__FieldEnvEffectLocatorBancParam = other.spl__FieldEnvEffectLocatorBancParam.Clone();
		}

		public override SplFieldEnvEffectArea Clone()
		{
			return new SplFieldEnvEffectArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__FieldEnvEffectLocatorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
