using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class RivalAppearPoint : MuObj
	{
        [BindGUI("Escape Rest Hp Border", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public int _EscapeRestHpBorder
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.EscapeRestHpBorder;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.EscapeRestHpBorder = value;
            }
        }

        [BindGUI("Is Search Ground On Super Jump", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public bool _IsSearchGroundOnSuperJump
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.IsSearchGroundOnSuperJump;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.IsSearchGroundOnSuperJump = value;
            }
        }

        [BindGUI("Is Motionless Until Shot", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public bool _IsShotSwitch
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.IsShotSwitch;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.IsShotSwitch = value;
            }
        }

        [BindGUI("Is Elite Octoling", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public bool _IsStrongLook
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.IsStrongLook;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.IsStrongLook = value;
            }
        }

        [BindGUI("Main Weapon",
            new object[7] { "", "Roller", "Brush", "Maneuver", "Shelter", "Slosher", "Blaster" },
            new string[7] { "Octo Shot", "Splat Roller", "Octobrush", "Splat Dualies", "Splat Brella", "Slosher", "Blaster" },
            Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string _MainWeapon
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.MainWeapon;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.MainWeapon = value;
            }
        }

        [BindGUI("Sub Weapon",
            new object[5] { "", "SplashBomb", "QuickBomb", "CurlingBomb", "RoboBomb" },
            new string[5] { "None", "Splat Bomb", "Burst Bomb", "Curling Bomb", "Autobomb" },
            Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string _SubWeapon
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.SubWeapon;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.SubWeapon = value;
            }
        }

        [BindGUI("Special Weapon",
            new object[7] { "", "MultiMissile", "InkStorm", "JetPack", "SuperLanding", "ShockSonar", "GreatBarrier" },
            new string[7] { "None", "Tenta Missiles", "Ink Storm", "Inkjet", "Splashdown", "Wave Breaker", "Big Bubbler" },
            Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string _SpecialWeapon
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.SpecialWeapon;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.SpecialWeapon = value;
            }
        }

        [BindGUI("Special Charge Time (in seconds)", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public float _SpecialChargeSec
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.SpecialChargeSec;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.SpecialChargeSec = value;
            }
        }

        [BindGUI("Special Recharge Time (in seconds)", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public float _SpecialReChargeSec
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.SpecialReChargeSec;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.SpecialReChargeSec = value;
            }
        }

        [BindGUI("Spawn Type", Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.Default)]
        public string _SpawnType
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.SpawnType;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.SpawnType = value;
            }
        }

        [BindGUI("Switch Type",
            new object[3] { "Standby", "Search", "Offense" },
            new string[3] { "Stand By", "Search", "Offense" },
            Category = "Octoling Properties", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string _SwitchType
        {
            get
            {
                return this.spl__RivalAppearPointBancParam.SwitchType;
            }

            set
            {
                this.spl__RivalAppearPointBancParam.SwitchType = value;
            }
        }

        [ByamlMember("spl__RivalAppearPointBancParam")]
		public Mu_spl__RivalAppearPointBancParam spl__RivalAppearPointBancParam { get; set; }

		public RivalAppearPoint() : base()
		{
			spl__RivalAppearPointBancParam = new Mu_spl__RivalAppearPointBancParam();

			Links = new List<Link>();
		}

		public RivalAppearPoint(RivalAppearPoint other) : base(other)
		{
			spl__RivalAppearPointBancParam = other.spl__RivalAppearPointBancParam.Clone();
		}

		public override RivalAppearPoint Clone()
		{
			return new RivalAppearPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RivalAppearPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
