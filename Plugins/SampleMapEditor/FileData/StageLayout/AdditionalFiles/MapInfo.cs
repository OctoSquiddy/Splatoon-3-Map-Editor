using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMapEditor
{
    public class MuMapInfo
    {
        public class MuChallengeParam
        {
            public string Type { get; set; } = "";

            // BreakCounterParam
            public bool isCountNumWritten { get; set; } = false;
            public int CountNum { get; set; } = 0;

            public bool isIsOnlyPlayerBreakWritten { get; set; } = false;
            public bool IsOnlyPlayerBreak { get; set; } = false;

            public bool isTargetActorNameListWritten { get; set; } = false;
            public List<string> TargetActorNameList { get; set; } = new List<string>();

            public bool isViewUIRemainNumWritten { get; set; } = false;
            public int ViewUIRemainNum { get; set; } = 0;

            // OneShotMissDieParam
            public bool isClearWaitTimeWritten { get; set; } = false;
            public float ClearWaitTime { get; set; } = 60.0f;

            // InkLimitParam
            public bool isInkLimitWritten { get; set; } = false;
            public float InkLimit { get; set; } = 1.0f;

            // TimeLimitParam
            public bool isTimeLimitWritten { get; set; } = false;
            public float TimeLimit { get; set; } = 60.0f;
        }

        public class MuDolphinMessage
        {
            public string Devtext { get; set; } = "";

            public string Label { get; set; } = "";
        }

        public class MuOctaWeaponSupply
        {
            // Text associated to the weapon
            public bool isDolphinMessageWritten { get; set; } = false;
            public MuDolphinMessage DolphinMessage { get; set; } = new MuDolphinMessage();

            // Reward after beating the level for the first time
            public bool isFirstRewardWritten { get; set; } = false;
            public int FirstReward { get; set; } = 0;

            // Is the weapon recommended
            public bool isIsRecommendedWritten { get; set; } = false;
            public bool IsRecommended { get; set; } = false;

            // Reward after beating the level after the first time
            public bool isSecondRewardWritten { get; set; } = false;
            public int SecondReward { get; set; } = 0;

            // Special Weapon
            public bool isSpecialWeaponWritten { get; set; } = false;
            public string SpecialWeapon { get; set; } = "";

            // Sub Weapon
            public bool isSubWeaponWritten { get; set; } = false;
            public string SubWeapon { get; set; } = "";

            // Supply Weapon Type
            public bool isSupplyWeaponTypeWritten { get; set; } = false;
            public string SupplyWeaponType { get; set; } = "Normal";

            // Main Weapon
            public bool isWeaponMainWritten { get; set; } = false;
            public string WeaponMain { get; set; } = "";
        }

        public MapInfoType mapInfoType { get; set; } = MapInfoType.Versus;

        public string Name { get; set; } = "";

        public bool HasInfoMap { get; set; } = false;

        // Versus and Salmon run data
        public int DisplayOrder { get; set; } = 0;

        // Versus and Salmon run data
        public int Id { get; set; } = 0;

        // Versus and Salmon run data
        public bool IsBadgeInfo { get; set; } = false;

        // Versus and Salmon run data
        public bool IsBigRun { get; set; } = false;

        // Versus and Salmon run data
        public int Season { get; set; } = 0;

        /*-----------------------------------------------*/
        /*              Mission Exclusive                */
        /*-----------------------------------------------*/

        // Number of eggs required to enter the level
        public bool isAdmissionWritten { get; set; } = false;
        public int Admission { get; set; } = 0;

        // Reward after beating the level for the first time
        public bool isFirstRewardWritten { get; set; } = false;
        public int FirstReward { get; set; } = 0;

        // Reward after beating the level after the first time
        public bool isSecondRewardWritten { get; set; } = false;
        public int SecondReward { get; set; } = 0;

        // Label of the message associated to the level
        public string MapNameLabel { get; set; } = "";

        // This will be very useful to know which variable is useful for loading and saving
        public string MapType { get; set; } = "NormalStage";

        // Octo Weapon delivery
        public bool isStageDolphinMessageWritten { get; set; } = false;
        public List<MuDolphinMessage> StageDolphinMessage { get; set; } = new List<MuDolphinMessage>();

        // Number of retries before game over
        public bool isRetryNumWritten { get; set; } = false;
        public int RetryNum { get; set; } = 3;

        // Challenge Array
        public bool isChallengeParamArrayWritten { get; set; } = false;
        public List<MuChallengeParam> ChallengeParamArray { get; set; } = new List<MuChallengeParam>();

        // Stage Messages
        public bool isOctaSupplyWeaponInfoArrayWritten { get; set; } = false;
        public List<MuOctaWeaponSupply> OctaSupplyWeaponInfoArray { get; set; } = new List<MuOctaWeaponSupply>();

        // Color Data
        public MuTeamColorDataSet.MuTeamColorData TeamColorData = new MuTeamColorDataSet.MuTeamColorData();

        /*-----------------------------------------------*/
        /*                 Constructors                  */
        /*-----------------------------------------------*/

        public MuMapInfo() { }
    }

    public enum MapInfoType : int
    {
        Versus,
        SalmonRun,
        StoryMode,      
    }
}
