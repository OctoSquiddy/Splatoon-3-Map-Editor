using GLFrameworkEngine;
using GLFrameworkEngine.UI;
using ImGuiNET;
using IONET.Collada.Core.Scene;
using IONET.Collada.Core.Transform;
using IONET.Collada.FX.Rendering;
using MapStudio.UI;
using Newtonsoft.Json.Linq;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Toolbox.Core.ViewModels;
using static SampleMapEditor.MuMapInfo;
using static Toolbox.Core.Runtime;

namespace SampleMapEditor.LayoutEditor
{
    public class MapInfoEditor : ILayoutEditor
    {
        public MuMapInfo MapInfo { get; set; }

        public string Name => "MapInfo Editor";

        public StageLayoutPlugin MapEditor { get; set; }

        public IToolWindowDrawer ToolWindowDrawer { get; }

        public List<IDrawable> Renderers { get; set; }

        public NodeBase Root { get; set; }

        public List<MenuItemModel> MenuItems { get; set; }

        public List<NodeBase> GetSelected()
        {
            return null;
        }

        public bool IsActive { get; set; }

        public MapInfoEditor(StageLayoutPlugin editor, MuMapInfo mMapInfo)
        {
            MapEditor = editor;
            MapInfo = mMapInfo;

            Root = new NodeBase(Name);
            Root.Icon = $"{IconManager.SETTINGS_ICON}";
            Root.IconColor = new System.Numerics.Vector4(0.859f, 0.031f, 0.969f, 1.0f);

            Root.TagUI.UIDrawer += delegate
            {
                ImGui.Text("Type of MapInfo");

                ImGui.SameLine();

                ImGui.PushItemWidth(ImGui.GetWindowSize().X - ImGui.GetCursorPosX());

                if (ImGui.BeginCombo($"###MapInfoTypeCbBox", MapInfoTypeDisplay[(int)MapInfo.mapInfoType], ImGuiComboFlags.HeightLarge))
                {
                    for (int i = 0; i < MapInfoTypeDisplay.Count; i++)
                    {
                        bool isSelected = (int)MapInfo.mapInfoType == i;

                        if (ImGui.Selectable(MapInfoTypeDisplay[i], isSelected))
                        {
                            MapInfo.mapInfoType = (MapInfoType)i;
                        }

                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopItemWidth();

                switch (mMapInfo.mapInfoType)
                {
                    case MapInfoType.Versus:
                        UIVersus();
                        break;

                    case MapInfoType.SalmonRun:
                        UICoop();
                        break;

                    case MapInfoType.StoryMode:
                        UIMission();
                        break;
                }               
            };

            Renderers = new List<IDrawable>();
        }

        private void UIVersus()
        {
            int numColumns = 2;            
            object refValue = 0;

            ImGui.BeginGroup();
            ImGui.BeginColumns("##" + "Points" + numColumns.ToString(), numColumns);

            refValue = MapInfo.DisplayOrder;
            if (SetUIProperty("Display Order", ref refValue))
            {
                MapInfo.DisplayOrder = (int)refValue;
            }

            refValue = MapInfo.Id;
            if (SetUIProperty("Id", ref refValue))
            {
                MapInfo.Id = (int)refValue;
            }

            ComboBoxData = Seasons;
            refValue = MapInfo.Season;
            if (SetUIProperty("Season", ref refValue, true))
            {
                MapInfo.Season = (int)refValue;
            }

            ImGui.EndColumns();
            ImGui.EndGroup();
        }
    
        private void UICoop()
        {
            int numColumns = 2;
            object refValue = 0;

            ImGui.BeginGroup();
            ImGui.BeginColumns("##" + "Points" + numColumns.ToString(), numColumns);

            refValue = MapInfo.DisplayOrder;
            if (SetUIProperty("Display Order", ref refValue))
            {
                MapInfo.DisplayOrder = (int)refValue;
            }

            refValue = MapInfo.Id;
            if (SetUIProperty("Id", ref refValue))
            {
                MapInfo.Id = (int)refValue;
            }

            refValue = MapInfo.IsBadgeInfo;
            if (SetUIProperty("Has badge info", ref refValue))
            {
                MapInfo.IsBadgeInfo = (bool)refValue;
            }

            refValue = MapInfo.IsBigRun;
            if (SetUIProperty("Is Big Run", ref refValue))
            {
                MapInfo.IsBigRun = (bool)refValue;
            }

            ComboBoxData = Seasons;
            refValue = MapInfo.Season;
            if (SetUIProperty("Season", ref refValue, true))
            {
                MapInfo.Season = (int)refValue;
            }

            ImGui.EndColumns();
            ImGui.EndGroup();
        }

        private void UIMission()
        {
            ImGui.Text("Type of Stage");

            ImGui.SameLine();

            ImGui.PushItemWidth(ImGui.GetWindowSize().X - ImGui.GetCursorPosX());

            string cbCurrentlabel = MapInfo.MapType != null ? MapTypeDisplay.ContainsKey(MapInfo.MapType) 
                ? MapTypeDisplay[MapInfo.MapType] : "" : "";

            if (ImGui.BeginCombo($"###MapTypeCbBox", cbCurrentlabel, ImGuiComboFlags.HeightLarge))
            {
                foreach (KeyValuePair<string, string> kvp in MapTypeDisplay)
                {
                    bool isSelected = MapInfo.MapType == kvp.Key;

                    if (ImGui.Selectable(kvp.Value, isSelected))
                    {
                        MapInfo.MapType = kvp.Key;
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            switch (MapInfo.MapType)
            {
                case "ChallengeStage":
                    break;
            }

            UIColorData();
            UIStageMission();
            UIChallenge();
            UIOctaWeaponSupply();
            UIMessage();
        }

        private void UIColorData()
        {
            if (ImGui.CollapsingHeader("Color Data", ImGuiTreeNodeFlags.DefaultOpen))
            {
                int numColumns = 2;
                object refValue = 0;

                ImGui.BeginGroup();
                ImGui.BeginColumns("##" + "ColorData" + numColumns.ToString(), numColumns);

                refValue = MapInfo.TeamColorData.__RowId;
                if (SetUIProperty("Color Data Name", ref refValue))
                {
                    MapInfo.TeamColorData.__RowId = (string)refValue;
                }

                refValue = MapInfo.TeamColorData.AlphaTeamColor;
                if (SetUIProperty("Alpha", ref refValue))
                {
                    MapInfo.TeamColorData.AlphaTeamColor = (System.Numerics.Vector4)refValue;
                }

                refValue = MapInfo.TeamColorData.BravoTeamColor;
                if (SetUIProperty("Bravo", ref refValue))
                {
                    MapInfo.TeamColorData.BravoTeamColor = (System.Numerics.Vector4)refValue;
                }

                refValue = MapInfo.TeamColorData.NeutralColor;
                if (SetUIProperty("Neutral", ref refValue))
                {
                    MapInfo.TeamColorData.NeutralColor = (System.Numerics.Vector4)refValue;
                }

                ImGui.EndColumns();
                ImGui.EndGroup();
            }
        }

        private void UIChallenge()
        {
            if (ImGui.CollapsingHeader("Challenge", ImGuiTreeNodeFlags.DefaultOpen))
            {
                int numColumns = 2;
                bool isWritten = false;
                object refValue = 0;

                if (ImGui.Button($"   {IconManager.ADD_ICON}   ###AddChallengeParam"))
                {
                    MapInfo.ChallengeParamArray.Add(new MuMapInfo.MuChallengeParam());
                }

                ImGui.SameLine();

                if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###DeleteChallengeParam"))
                {
                    if (MapInfo.ChallengeParamArray.Count > 0)
                        MapInfo.ChallengeParamArray.RemoveAt(MapInfo.ChallengeParamArray.Count - 1);
                }

                ImGui.SameLine();

                isWritten = MapInfo.isChallengeParamArrayWritten;
                if (ImGui.Checkbox("Write challenge parameters", ref isWritten))
                {
                    MapInfo.isChallengeParamArrayWritten = isWritten;
                }

                if (ImGui.BeginTabBar("UIChallengeTab"))
                {
                    for (int i = 0; i < MapInfo.ChallengeParamArray.Count; i++)
                    {
                        MuMapInfo.MuChallengeParam CurrentChallengeParam = MapInfo.ChallengeParamArray[i];

                        if (ImGui.BeginTabItem($"{ChallengeParamType[CurrentChallengeParam.Type]}###UIChallengeTab{i}"))
                        {
                            ImGui.BeginGroup();
                            ImGui.BeginColumns($"###UIChallengeTabItemCol{i}", numColumns);

                            ComboBoxData = ChallengeParamType;
                            refValue = CurrentChallengeParam.Type;
                            if (SetUIProperty("Challenge type", ref refValue, true))
                            {
                                CurrentChallengeParam.Type = (string)refValue;
                            }

                            ImGui.EndColumns();

                            ImGui.Separator();
                            ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.533f, 0.0f, 1.0f), "BreakCounterParam");

                            if (ImGui.Button($"   {IconManager.ADD_ICON}   ###AddActorToList"))
                            {
                                CurrentChallengeParam.TargetActorNameList.Add("");
                            }

                            ImGui.SameLine();

                            bool boolRef = CurrentChallengeParam.isTargetActorNameListWritten;
                            if (ImGui.Checkbox("Is target list written", ref boolRef))
                            {
                                CurrentChallengeParam.isTargetActorNameListWritten = boolRef;
                            }

                            ImGui.BeginGroup();
                            for (int j = 0; j < CurrentChallengeParam.TargetActorNameList.Count; j++)
                            {
                                if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###TargetActorDel.{i}"))
                                {
                                    if (CurrentChallengeParam.TargetActorNameList.Count > 0)
                                        CurrentChallengeParam.TargetActorNameList.RemoveAt(j);
                                }

                                ImGui.SameLine();

                                ImGui.PushItemWidth(ImGui.GetWindowSize().X - ImGui.GetCursorPosX());

                                string refStr = CurrentChallengeParam.TargetActorNameList[j];
                                if (ImGui.InputText($"###TargetActor.{i}", ref refStr, 0x1000))
                                {
                                    CurrentChallengeParam.TargetActorNameList[j] = refStr;
                                }

                                ImGui.PopItemWidth();
                            }
                            ImGui.EndGroup();

                            ImGui.BeginColumns($"###UIChallengeTabItemCol{i}", numColumns);

                            isWritten = CurrentChallengeParam.isCountNumWritten;
                            refValue = CurrentChallengeParam.CountNum;
                            if (SetUIOptionalProperty("Number of actors to destroy", ref isWritten, ref refValue))
                            {
                                CurrentChallengeParam.isCountNumWritten = isWritten;
                                CurrentChallengeParam.CountNum = (int)refValue;
                            }

                            isWritten = CurrentChallengeParam.isIsOnlyPlayerBreakWritten;
                            refValue = CurrentChallengeParam.IsOnlyPlayerBreak;
                            if (SetUIOptionalProperty("Only the player can break the actors", ref isWritten, ref refValue))
                            {
                                CurrentChallengeParam.isIsOnlyPlayerBreakWritten = isWritten;
                                CurrentChallengeParam.IsOnlyPlayerBreak = (bool)refValue;
                            }

                            isWritten = CurrentChallengeParam.isViewUIRemainNumWritten;
                            refValue = CurrentChallengeParam.ViewUIRemainNum;
                            if (SetUIOptionalProperty("View UI Remain Number", ref isWritten, ref refValue))
                            {
                                CurrentChallengeParam.isViewUIRemainNumWritten = isWritten;
                                CurrentChallengeParam.ViewUIRemainNum = (int)refValue;
                            }

                            ImGui.EndColumns();

                            ImGui.Separator();
                            ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.533f, 0.0f, 1.0f), "InkLimitParam");

                            ImGui.BeginColumns($"###UIChallengeTabItemCol{i}", numColumns);

                            isWritten = CurrentChallengeParam.isInkLimitWritten;
                            refValue = CurrentChallengeParam.InkLimit * 100.0f;
                            if (SetUIOptionalProperty("Ink tank (in %)", ref isWritten, ref refValue))
                            {
                                CurrentChallengeParam.isInkLimitWritten = isWritten;
                                CurrentChallengeParam.InkLimit = (float)refValue / 100.0f;
                            }

                            ImGui.EndColumns();

                            ImGui.Separator();
                            ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.533f, 0.0f, 1.0f), "OneShotMissDieParam");

                            ImGui.BeginColumns($"###UIChallengeTabItemCol{i}", numColumns);

                            isWritten = CurrentChallengeParam.isClearWaitTimeWritten;
                            refValue = CurrentChallengeParam.ClearWaitTime;
                            if (SetUIOptionalProperty("Clear time (in seconds)", ref isWritten, ref refValue))
                            {
                                CurrentChallengeParam.isClearWaitTimeWritten = isWritten;
                                CurrentChallengeParam.ClearWaitTime = (float)refValue;
                            }

                            ImGui.EndColumns();

                            ImGui.Separator();
                            ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.533f, 0.0f, 1.0f), "TimeLimitParam");

                            ImGui.BeginColumns($"###UIChallengeTabItemCol{i}", numColumns);

                            isWritten = CurrentChallengeParam.isTimeLimitWritten;
                            refValue = CurrentChallengeParam.TimeLimit;
                            if (SetUIOptionalProperty("Time limit (in seconds)", ref isWritten, ref refValue))
                            {
                                CurrentChallengeParam.isTimeLimitWritten = isWritten;
                                CurrentChallengeParam.TimeLimit = (float)refValue;
                            }

                            ImGui.EndColumns();
                            ImGui.EndGroup();

                            ImGui.EndTabItem();
                        }
                    }

                    ImGui.EndTabBar();
                }
            }
        }

        private void UIStageMission()
        {
            if (ImGui.CollapsingHeader("Stage", ImGuiTreeNodeFlags.DefaultOpen))
            {
                int numColumns = 2;
                object refValue = 0;
                bool isWritten = false;

                ImGui.BeginGroup();
                ImGui.BeginColumns("##" + "StageMission" + numColumns.ToString(), numColumns);

                refValue = MapInfo.MapNameLabel;
                if (SetUIProperty("Map Name Label", ref refValue))
                {
                    MapInfo.MapNameLabel = (string)refValue;
                }

                isWritten = MapInfo.isAdmissionWritten;
                refValue = MapInfo.Admission;
                if (SetUIOptionalProperty("Number of power eggs required", ref isWritten, ref refValue))
                {
                    MapInfo.isAdmissionWritten = isWritten;
                    MapInfo.Admission = (int)refValue;
                }

                isWritten = MapInfo.isFirstRewardWritten;
                refValue = MapInfo.FirstReward;
                if (SetUIOptionalProperty("Reward for the first time", ref isWritten, ref refValue))
                {
                    MapInfo.isFirstRewardWritten = isWritten;
                    MapInfo.FirstReward = (int)refValue;
                }

                isWritten = MapInfo.isSecondRewardWritten;
                refValue = MapInfo.SecondReward;
                if (SetUIOptionalProperty("Reward after the first time", ref isWritten, ref refValue))
                {
                    MapInfo.isSecondRewardWritten = isWritten;
                    MapInfo.SecondReward = (int)refValue;
                }

                isWritten = MapInfo.isRetryNumWritten;
                refValue = MapInfo.RetryNum;
                if (SetUIOptionalProperty("Number of lives", ref isWritten, ref refValue))
                {
                    MapInfo.isRetryNumWritten = isWritten;
                    MapInfo.RetryNum = (int)refValue;
                }

                ImGui.EndColumns();
                ImGui.EndGroup();
            }
        }

        private void UIMessage()
        {
            if (ImGui.CollapsingHeader("Message Data", ImGuiTreeNodeFlags.DefaultOpen))
            {
                int numColumns = 2;
                bool isWritten = false;
                object refValue = 0;

                ImGui.BeginGroup();
                ImGui.BeginColumns("##" + "UIMessageDD" + numColumns.ToString(), numColumns);

                refValue = MapInfo.MapNameLabel;
                if (SetUIProperty("Label of the level's name", ref refValue))
                {
                    MapInfo.MapNameLabel = (string)refValue;
                }

                ImGui.EndColumns();
                ImGui.EndGroup();

                ImGui.Separator();

                if (ImGui.Button($"   {IconManager.ADD_ICON}   ###AddStageMsg"))
                {
                    MapInfo.StageDolphinMessage.Add(new MuMapInfo.MuDolphinMessage());
                }

                ImGui.SameLine();

                if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###DeleteStageMsg"))
                {
                    if (MapInfo.StageDolphinMessage.Count > 0)
                        MapInfo.StageDolphinMessage.RemoveAt(MapInfo.StageDolphinMessage.Count - 1);
                }

                ImGui.SameLine();

                isWritten = MapInfo.isStageDolphinMessageWritten;
                if (ImGui.Checkbox("Write stage messages", ref isWritten))
                {
                    MapInfo.isStageDolphinMessageWritten = isWritten;
                }

                if (ImGui.BeginTabBar("UIMessageTab"))
                {
                    for (int i = 0; i < MapInfo.StageDolphinMessage.Count; i++)
                    {
                        if (ImGui.BeginTabItem($"{MapInfo.StageDolphinMessage[i].Label}###UIMessageTab{i}"))
                        {
                            ImGui.BeginGroup();
                            ImGui.BeginColumns($"###UIMessageTabItemCol{i}", numColumns);

                            refValue = MapInfo.StageDolphinMessage[i].Devtext;
                            if (SetUIProperty("Developper Text", ref refValue))
                            {
                                MapInfo.StageDolphinMessage[i].Devtext = (string)refValue;
                            }

                            refValue = MapInfo.StageDolphinMessage[i].Label;
                            if (SetUIProperty("Label of the message", ref refValue))
                            {
                                MapInfo.StageDolphinMessage[i].Label = (string)refValue;
                            }

                            ImGui.EndColumns();
                            ImGui.EndGroup();

                            ImGui.EndTabItem();
                        }
                    }

                    ImGui.EndTabBar();
                }
            }
        }

        private void UIOctaWeaponSupply()
        {
            if (ImGui.CollapsingHeader("Weapon Supply Data", ImGuiTreeNodeFlags.DefaultOpen))
            {
                int numColumns = 2;
                bool isWritten = false;
                object refValue = 0;

                if (ImGui.Button($"   {IconManager.ADD_ICON}   ###AddWeaponSupply"))
                {
                    MapInfo.OctaSupplyWeaponInfoArray.Add(new MuMapInfo.MuOctaWeaponSupply());
                }

                ImGui.SameLine();

                if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###DeleteWeaponSupply"))
                {
                    if (MapInfo.OctaSupplyWeaponInfoArray.Count > 0)
                        MapInfo.OctaSupplyWeaponInfoArray.RemoveAt(MapInfo.OctaSupplyWeaponInfoArray.Count - 1);
                }

                ImGui.SameLine();

                isWritten = MapInfo.isOctaSupplyWeaponInfoArrayWritten;
                if (ImGui.Checkbox("Write Weapon Supply", ref isWritten))
                {
                    MapInfo.isOctaSupplyWeaponInfoArrayWritten = isWritten;
                }

                if (ImGui.BeginTabBar("UIWeaponTab"))
                {
                    for (int i = 0; i < MapInfo.OctaSupplyWeaponInfoArray.Count; i++)
                    {
                        MuMapInfo.MuOctaWeaponSupply CurrentWeapon = MapInfo.OctaSupplyWeaponInfoArray[i];

                        if (ImGui.BeginTabItem($"{MainWeapons[CurrentWeapon.WeaponMain]}###UIWeaponTab{i}"))
                        {
                            ImGui.BeginGroup();
                            ImGui.BeginColumns($"###UIWeaponTabItemCol{i}", numColumns);

                            ComboBoxData = SupplyWeaponTypes;
                            isWritten = CurrentWeapon.isSupplyWeaponTypeWritten;
                            refValue = CurrentWeapon.SupplyWeaponType;
                            if (SetUIOptionalProperty("Supply weapon type", ref isWritten, ref refValue, true))
                            {
                                CurrentWeapon.isSupplyWeaponTypeWritten = isWritten;
                                CurrentWeapon.SupplyWeaponType = (string)refValue;

                                switch (CurrentWeapon.SupplyWeaponType)
                                {
                                    case "Normal":
                                        CurrentWeapon.isWeaponMainWritten = true;
                                        CurrentWeapon.isSubWeaponWritten = true;
                                        CurrentWeapon.isSpecialWeaponWritten = false;
                                        break;

                                    case "Hero":
                                        CurrentWeapon.isWeaponMainWritten = true;
                                        CurrentWeapon.isSubWeaponWritten = true;
                                        CurrentWeapon.isSpecialWeaponWritten = false;
                                        break;

                                    case "Special":
                                        CurrentWeapon.isWeaponMainWritten = false;
                                        CurrentWeapon.isSubWeaponWritten = false;
                                        CurrentWeapon.isSpecialWeaponWritten = true;
                                        break;

                                    case "MainAndSpecial":
                                        CurrentWeapon.isWeaponMainWritten = true;
                                        CurrentWeapon.isSubWeaponWritten = true;
                                        CurrentWeapon.isSpecialWeaponWritten = true;
                                        break;
                                }
                            }

                            ComboBoxData = MainWeapons;
                            isWritten = CurrentWeapon.isWeaponMainWritten;
                            refValue = CurrentWeapon.WeaponMain;
                            if (SetUIOptionalProperty("Main Weapon", ref isWritten, ref refValue, true))
                            {
                                CurrentWeapon.isWeaponMainWritten = isWritten;
                                CurrentWeapon.WeaponMain = (string)refValue;
                            }

                            ComboBoxData = SubWeapons;
                            isWritten = CurrentWeapon.isSubWeaponWritten;
                            refValue = CurrentWeapon.SubWeapon;
                            if (SetUIOptionalProperty("Sub Weapon", ref isWritten, ref refValue, true))
                            {
                                CurrentWeapon.isSubWeaponWritten = isWritten;
                                CurrentWeapon.SubWeapon = (string)refValue;
                            }

                            ComboBoxData = SpecialWeapons;
                            isWritten = CurrentWeapon.isSpecialWeaponWritten;
                            refValue = CurrentWeapon.SpecialWeapon;
                            if (SetUIOptionalProperty("Special Weapon", ref isWritten, ref refValue, true))
                            {
                                CurrentWeapon.isSpecialWeaponWritten = isWritten;
                                CurrentWeapon.SpecialWeapon = (string)refValue;
                            }

                            ImGui.EndColumns();
                            ImGui.Separator();
                            ImGui.BeginColumns($"###UIWeaponTabItemCol{i}", numColumns);

                            isWritten = CurrentWeapon.isIsRecommendedWritten;
                            refValue = CurrentWeapon.IsRecommended;
                            if (SetUIOptionalProperty("Is recommended", ref isWritten, ref refValue))
                            {
                                CurrentWeapon.isIsRecommendedWritten = isWritten;
                                CurrentWeapon.IsRecommended = (bool)refValue;
                            }

                            isWritten = CurrentWeapon.isFirstRewardWritten;
                            refValue = CurrentWeapon.FirstReward;
                            if (SetUIOptionalProperty("Reward for the first time ", ref isWritten, ref refValue))
                            {
                                CurrentWeapon.isFirstRewardWritten = isWritten;
                                CurrentWeapon.FirstReward = (int)refValue;
                            }

                            isWritten = CurrentWeapon.isSecondRewardWritten;
                            refValue = CurrentWeapon.SecondReward;
                            if (SetUIOptionalProperty("Reward after the first time ", ref isWritten, ref refValue))
                            {
                                CurrentWeapon.isSecondRewardWritten = isWritten;
                                CurrentWeapon.SecondReward = (int)refValue;
                            }

                            ImGui.EndColumns();
                            ImGui.Separator();
                            ImGui.BeginColumns($"###UIWeaponTabItemCol{i}", numColumns);

                            isWritten = CurrentWeapon.isDolphinMessageWritten;
                            refValue = CurrentWeapon.DolphinMessage.Devtext;
                            if (SetUIOptionalProperty("Weapon DevText ", ref isWritten, ref refValue))
                            {
                                CurrentWeapon.isDolphinMessageWritten = isWritten;
                                CurrentWeapon.DolphinMessage.Devtext = (string)refValue;
                            }

                            isWritten = CurrentWeapon.isDolphinMessageWritten;
                            refValue = CurrentWeapon.DolphinMessage.Label;
                            if (SetUIOptionalProperty("Weapon Label ", ref isWritten, ref refValue))
                            {
                                CurrentWeapon.isDolphinMessageWritten = isWritten;
                                CurrentWeapon.DolphinMessage.Label = (string)refValue;
                            }

                            ImGui.EndColumns();
                            ImGui.EndGroup();

                            ImGui.EndTabItem();
                        }
                    }

                    ImGui.EndTabBar();
                }
            }
        }

        private bool SetUIOptionalProperty(string label, ref bool isWritten, ref object Value, bool isComboBox = false)
        {
            bool edit = false;
            int refIntValue = 0;
            float refFloatValue = 0;
            string refStringValue = "";
            bool refBoolValue = false;
            System.Numerics.Vector4 refVector4Value = new System.Numerics.Vector4();

            // Get type of the value sent in the function
            Type type = Value.GetType();
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                type = nullableType;
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            ImGui.AlignTextToFramePadding();

            refBoolValue = isWritten;
            edit = ImGui.Checkbox(label, ref refBoolValue);

            if (edit)
            {
                isWritten = refBoolValue;
                return edit;
            }

            ImGui.NextColumn();

            float colwidth = ImGui.GetColumnWidth();
            ImGui.PushItemWidth(colwidth - 6);

            string mLabel = $"###{label}";

            if (isComboBox)
            {
                string cbCurrentlabel = Value != null ? ComboBoxData.ContainsKey(Value) ? ComboBoxData[Value] : "" : "";

                if (ImGui.BeginCombo(mLabel, cbCurrentlabel, ImGuiComboFlags.HeightLarge))
                {
                    foreach (KeyValuePair<object, string> kvp in ComboBoxData)
                    {
                        bool isSelected = Value == kvp.Key;

                        if (ImGui.Selectable(kvp.Value, isSelected))
                        {
                            Value = kvp.Key;
                            edit = true;
                        }
                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopItemWidth();
                ImGui.NextColumn();

                return edit;
            }

            if (type == typeof(bool))
            {
                refBoolValue = (bool)Value;
                edit = ImGui.Checkbox(mLabel, ref refBoolValue);

                if (edit)
                {
                    Value = (bool)refBoolValue;
                }
            }
            else if (type == typeof(string))
            {
                refStringValue = (string)Value;
                edit = ImGui.InputText(mLabel, ref refStringValue, 0x1000);

                if (edit)
                {
                    Value = (string)refStringValue;
                }
            }
            else if (type == typeof(int))
            {
                refIntValue = (int)Value;
                edit = ImGui.InputInt(mLabel, ref refIntValue, 0, 0, ImGuiInputTextFlags.None);

                if (edit)
                {
                    Value = refIntValue;
                }
            }
            else if (type == typeof(float))
            {
                refFloatValue = (float)Value;
                edit = ImGui.InputFloat(mLabel, ref refFloatValue, 0, 0);

                if (edit)
                {
                    Value = refFloatValue;
                }
            }
            else if (type == typeof(System.Numerics.Vector4))
            {
                refVector4Value = (System.Numerics.Vector4)Value;
                edit = ImGui.ColorEdit4(mLabel, ref refVector4Value, ImGuiColorEditFlags.NoInputs);

                if (edit)
                {
                    Value = refVector4Value;
                }
            }

            ImGui.PopItemWidth();
            ImGui.NextColumn();

            return edit;
        }

        private Dictionary<object, string> ComboBoxData;

        private bool SetUIProperty(string label, ref object Value, bool isComboBox = false)
        {
            bool edit = false;
            string refStringValue = "";
            int refIntValue = 0;
            float refFloatValue = 0.0f;
            bool refBoolValue = false;
            System.Numerics.Vector4 refVector4Value = new System.Numerics.Vector4();

            // Get type of the value sent in the function
            Type type = Value.GetType();
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                type = nullableType;
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            ImGui.AlignTextToFramePadding();
            ImGui.Text(label);
            ImGui.NextColumn();

            float colwidth = ImGui.GetColumnWidth();
            ImGui.PushItemWidth(colwidth - 6);

            string mLabel = $"###{label}";

            if (isComboBox)
            {
                string cbCurrentlabel = Value != null ? ComboBoxData.ContainsKey(Value) ? ComboBoxData[Value] : "" : "";

                if (ImGui.BeginCombo(mLabel, cbCurrentlabel, ImGuiComboFlags.HeightLarge))
                {
                    foreach (KeyValuePair<object, string> kvp in ComboBoxData)
                    {
                        bool isSelected = Value == kvp.Key;

                        if (ImGui.Selectable(kvp.Value, isSelected))
                        {
                            Value = kvp.Key;
                            edit = true;
                        }
                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopItemWidth();
                ImGui.NextColumn();

                return edit;
            }

            if (type == typeof(bool))
            {
                refBoolValue = (bool)Value;
                edit = ImGui.Checkbox(mLabel, ref refBoolValue);

                if (edit)
                {
                    Value = (bool)refBoolValue;
                }
            }
            else if (type == typeof(string))
            {
                refStringValue = (string)Value;
                edit = ImGui.InputText(mLabel, ref refStringValue, 0x1000);

                if (edit)
                {
                    Value = (string)refStringValue;
                }
            }
            else if (type == typeof(int))
            {
                refIntValue = (int)Value;
                edit = ImGui.InputInt(mLabel, ref refIntValue, 0, 0, ImGuiInputTextFlags.None);

                if (edit)
                {
                    Value = refIntValue;
                }
            }
            else if (type == typeof(float))
            {
                refFloatValue = (float)Value;
                edit = ImGui.InputFloat(mLabel, ref refFloatValue, 0, 0);

                if (edit)
                {
                    Value = refFloatValue;
                }
            }
            else if (type == typeof(System.Numerics.Vector4))
            {
                refVector4Value = (System.Numerics.Vector4)Value;
                edit = ImGui.ColorEdit4(mLabel, ref refVector4Value, ImGuiColorEditFlags.NoInputs);

                if (edit)
                {
                    Value = refVector4Value;
                }
            }

            ImGui.PopItemWidth();
            ImGui.NextColumn();

            return edit;
        }






























        public void OnSave(StageDefinition stage)
        {

        }

        public void ReloadEditor()
        {

        }

        public void DrawHelpWindow()
        {

        }

        public void DrawEditMenuBar()
        {

        }

        public void RemoveSelected()
        {

        }

        public void OnMouseMove(MouseEventInfo mouseInfo)
        { 

        }

        public void OnMouseDown(MouseEventInfo mouseInfo)
        {

        }

        public void OnMouseUp(MouseEventInfo mouseInfo)
        {

        }

        public void OnKeyDown(KeyEventInfo keyInfo)
        {

        }

        List<string> MapInfoTypeDisplay = new List<string>()
        {
            "Versus",
            "Salmon Run",
            "Mission"
        };

        Dictionary<string, string> MapTypeDisplay = new Dictionary<string, string>()
        {
            { "NormalStage", "Normal Stage" },
            { "SmallWorldStage", "Octo Canyon Stage" },
            { "ChallengeStage", "Challenge Stage" },
            { "SmallWorldBossStage", "Octo Canyon Boss Stage" },
            { "BigWorldBossStage", "Alterna Stage" },
            { "LaunchPadStage", "Launch Pad Stage" },
            { "LastBoss", "Last Boss Stage" },
            { "ExStage", "After Alterna Stage" },
            { "SmallWorld", "Octo Canyon Overworld" },
            { "BigWorld", "Alterna Overworld" },
            { "LaunchPadWorld", "Launch Pad World" },
        };

        Dictionary<object, string> Seasons = new Dictionary<object, string>()
        {
            { 0, "Pre-Game" },
            { 1, "Drizzle Season 2022" },
            { 2, "Chill Season 2022" },
            { 3, "Fresh Season 2023" },
            { 4, "Sizzle Season 2023" },
            { 5, "Drizzle Season 2023" },
            { 6, "Chill Season 2023" },
        };

        Dictionary<object, string> SupplyWeaponTypes = new Dictionary<object, string>()
        {
            {"Normal", "Main and sub weapon" },
            {"Hero", "Hero weapon" },
            {"Special", "Special only" },
            {"MainAndSpecial", "Main sub and special weapon" },
        };

        Dictionary<object, string> ChallengeParamType = new Dictionary<object, string>()
        {
            {"", "None" },
            {"Normal", "Normal" },
            {"TimeLimit", "Time limit until death" },
            {"TimeLimitClear", "Time limit until the level is cleared" },
            {"OneShotMissDie", "One shot level" },
            {"InkLimit", "Ink limit" },
            {"DamageSuddenDeath", "Instant death" },
            {"BreakCounter", "Break a number of actors" },
            {"BreakCounterMiss", "Break counter miss (unused)" },
        };

        Dictionary<object, string> MainWeapons = new Dictionary<object, string>()
        {
            {"", "None" },
            {"Blaster_Bear_Coop", "Grizzco Blaster (SR)" },
            {"Blaster_Light_00", "Rapid Blaster" },
            {"Blaster_Light_01", "Rapid Blaster Deco" },
            {"Blaster_Light_Coop", "Rapid Blaster (SR)" },
            {"Blaster_Light_Msn", "Rapid Blaster (Mission)" },
            {"Blaster_LightLong_00", "Rapid Blaster Pro" },
            {"Blaster_LightLong_01", "Rapid Blaster Pro Deco" },
            {"Blaster_LightLong_Coop", "Rapid Blaster Pro (SR)" },
            {"Blaster_LightLong_Msn", "Rapid Blaster Pro (Mission)" },
            {"Blaster_LightShort_00", "Clash Blaster" },
            {"Blaster_LightShort_01", "Clash Blaster Neo" },
            {"Blaster_LightShort_Coop", "Clash Blaster (SR)" },
            {"Blaster_LightShort_Msn", "Clash Blaster (Mission)" },
            {"Blaster_Long_00", "Range Blaster" },
            {"Blaster_Long_Coop", "Range Blaster (SR)" },
            {"Blaster_Long_Msn", "Range Blaster (Mission)" },
            {"Blaster_Middle_00", "Blaster" },
            {"Blaster_Middle_Coop", "Blaster (SR)" },
            {"Blaster_Middle_Msn", "Blaster (Mission)" },
            {"Blaster_Precision_00", "S-BLAST '92" },
            {"Blaster_Precision_Coop", "S-BLAST '92 (SR)" },
            {"Blaster_RivalLv1_00", "Octoling Blaster Lvl 1" },
            {"Blaster_RivalLv2_00", "Octoling Blaster Lvl 2" },
            {"Blaster_Short_00", "Luna Blaster" },
            {"Blaster_Short_01", "Luna Blaster Neo" },
            {"Blaster_Short_Coop", "Luna Blaster (SR)" },
            {"Blaster_Short_Msn", "Luna Blaster (Mission)" },
            {"Brush_Heavy_00", "Painbrush" },
            {"Brush_Heavy_Coop", "Painbrush (SR)" },
            {"Brush_Mini_00", "Inkbrush" },
            {"Brush_Mini_01", "Inkbrush Nouveau" },
            {"Brush_Mini_Coop", "Inkbrush (SR)" },
            {"Brush_Mini_Msn", "Inkbrush (Mission)" },
            {"Brush_Normal_00", "Octobrush" },
            {"Brush_Normal_Coop", "Octobrush (SR)" },
            {"Brush_Normal_Msn", "Octobrush (Mission)" },
            {"Brush_RivalLv1_00", "Octoling Octobrush Lvl 1" },
            {"Brush_RivalLv2_00", "Octoling Octobrush Lvl 2" },
            {"Charger_Bear_Coop", "Grizzco Charger (SR)" },
            {"Charger_Keeper_00", "Goo Tuber" },
            {"Charger_Keeper_Coop", "Goo Tuber (SR)" },
            {"Charger_Keeper_Msn", "Goo Tuber (Mission)" },
            {"Charger_Light_00", "Bamboozler 14 Mk I" },
            {"Charger_Light_Coop", "Bamboozler 14 Mk I (SR)" },
            {"Charger_Light_Msn", "Bamboozler 14 Mk I (Mission)" },
            {"Charger_Long_00", "E-liter 4K" },
            {"Charger_Long_Coop", "E-liter 4K (SR)" },
            {"Charger_Long_Msn", "E-liter 4K (Mission)" },
            {"Charger_LongScope_00", "E-liter 4K Scope" },
            {"Charger_LongScope_Msn", "E-liter 4K Scope (Mission)" },
            {"Charger_Normal_00", "Splat Charger" },
            {"Charger_Normal_01", "Z+F Splat Charger" },
            {"Charger_Normal_Coop", "Splat Charger (SR)" },
            {"Charger_Normal_Msn", "Splat Charger (Mission)" },
            {"Charger_NormalScope_00", "Splatterscope" },
            {"Charger_NormalScope_01", "Z+F Splatterscope" },
            {"Charger_NormalScope_Msn", "Splatterscope (Mission)" },
            {"Charger_Pencil_00", "Snipewriter 5H" },
            {"Charger_Pencil_Coop", "Snipewriter 5H (SR)" },
            {"Charger_Quick_00", "Classic Squiffer" },
            {"Charger_Quick_Coop", "Classic Squiffer (SR)" },
            {"Charger_Quick_Msn", "Classic Squiffer (Mission)" },
            {"Maneuver_Dual_00", "Dualie Squelchers" },
            {"Maneuver_Dual_01", "Custom Dualie Squelchers" },
            {"Maneuver_Dual_Coop", "Dualie Squelchers (SR)" },
            {"Maneuver_Dual_Msn", "Dualie Squelchers (Mission)" },
            {"Maneuver_Gallon_00", "Glooga Dualies" },
            {"Maneuver_Gallon_Coop", "Glooga Dualies (SR)" },
            {"Maneuver_Gallon_Msn", "Glooga Dualies (Mission)" },
            {"Maneuver_Normal_00", "Splat Dualies" },
            {"Maneuver_Normal_Coop", "Splat Dualies (SR)" },
            {"Maneuver_Normal_Msn", "Splat Dualies (Mission)" },
            {"Maneuver_RivalLv1_00", "Octoling Splat Dualies Lvl 1" },
            {"Maneuver_RivalLv2_00", "Octoling Splat Dualies Lvl 2" },
            {"Maneuver_Short_00", "Dapple Dualies" },
            {"Maneuver_Short_01", "Dapple Dualies Nouveau" },
            {"Maneuver_Short_Coop", "Dapple Dualies (SR)" },
            {"Maneuver_Short_Msn", "Dapple Dualies (Mission)" },
            {"Maneuver_Stepper_00", "Dark Tetra Dualies" },
            {"Maneuver_Stepper_01", "Light Tetra Dualies" },
            {"Maneuver_Stepper_Coop", "Dark Tetra Dualies (SR)" },
            {"Maneuver_Stepper_Msn", "Dark Tetra Dualies (Mission)" },
            {"Roller_Compact_00", "Carbon Roller" },
            {"Roller_Compact_01", "Carbon Roller Deco" },
            {"Roller_Compact_Coop", "Carbon Roller (SR)" },
            {"Roller_Compact_Msn", "Carbon Roller (Mission)" },
            {"Roller_Heavy_00", "Dynamo Roller" },
            {"Roller_Heavy_Coop", "Dynamo Roller (SR)" },
            {"Roller_Heavy_Msn", "Dynamo Roller (Mission)" },
            {"Roller_Hunter_00", "Flingza Roller" },
            {"Roller_Hunter_Coop", "Flingza Roller (SR)" },
            {"Roller_Hunter_Msn", "Flingza Roller (Mission)" },
            {"Roller_Normal_00", "Splat Roller" },
            {"Roller_Normal_01", "Krak-On Splat Roller" },
            {"Roller_Normal_Coop", "Splat Roller (SR)" },
            {"Roller_Normal_Msn", "Splat Roller (Mission)" },
            {"Roller_RivalLv1_00", "Octoling Splat Roller Lvl 1" },
            {"Roller_RivalLv2_00", "Octoling Splat Roller Lvl 2" },
            {"Roller_Wide_00", "Big Swig Roller" },
            {"Roller_Wide_01", "Big Swig Roller Express" },
            {"Roller_Wide_Coop", "Big Swig Roller (SR)" },
            {"Saber_Bear_Coop", "Grizzco Splatana (SR)" },
            {"Saber_Lite_00", "Splatana Wiper" },
            {"Saber_Lite_01", "Splatana Wiper Deco" },
            {"Saber_Lite_Coop", "Splatana Wiper (SR)" },
            {"Saber_Normal_00", "Splatana Stamper" },
            {"Saber_Normal_Coop", "Splatana Stamper (SR)" },
            {"Saber_Normal_Msn", "Splatana Stamper (Mission)" },
            {"Shelter_Bear_Coop", "Grizzco Brella (SR)" },
            {"Shelter_Compact_00", "Undercover Brella" },
            {"Shelter_Compact_Coop", "Undercover Brella (SR)" },
            {"Shelter_Compact_Msn", "Undercover Brella (Mission)" },
            {"Shelter_Normal_00", "Splat Brella" },
            {"Shelter_Normal_Coop", "Splat Brella (SR)" },
            {"Shelter_Normal_Msn", "Splat Brella (Mission)" },
            {"Shelter_RivalLv1_00", "Octoling Splat Brella Lvl 1" },
            {"Shelter_RivalLv2_00", "Octoling Splat Brella Lvl 2" },
            {"Shelter_Wide_00", "Tenta Brella" },
            {"Shelter_Wide_01", "Tenta Sorella Brella" },
            {"Shelter_Wide_Coop", "Tenta Brella (SR)" },
            {"Shelter_Wide_Msn", "Tenta Brella (Mission)" },
            {"Shooter_Blaze_00", "Aerospray MG" },
            {"Shooter_Blaze_01", "Aerospray RG" },
            {"Shooter_Blaze_Coop", "Aerospray MG (SR)" },
            {"Shooter_Blaze_Msn", "Aerospray MG (Mission)" },
            {"Shooter_Expert_00", "Splattershot Pro" },
            {"Shooter_Expert_01", "Forge Splattershot Pro" },
            {"Shooter_Expert_Coop", "Splattershot Pro (SR)" },
            {"Shooter_Expert_Msn", "Splattershot Pro (Mission)" },
            {"Shooter_First_00", "Splattershot Jr." },
            {"Shooter_First_01", "Custom Splattershot Jr." },
            {"Shooter_First_Coop", "Splattershot Jr. (SR)" },
            {"Shooter_First_Msn", "Splattershot Jr. (Mission)" },
            {"Shooter_Flash_00", "Squeezer" },
            {"Shooter_Flash_Coop", "Squeezer (SR)" },
            {"Shooter_Flash_Msn", "Squeezer (Mission)" },
            {"Shooter_Gravity_00", ".52 Gal" },
            {"Shooter_Gravity_Coop", ".52 Gal (SR)" },
            {"Shooter_Gravity_Msn", ".52 Gal (Mission)" },
            {"Shooter_Heavy_00", ".96 Gal" },
            {"Shooter_Heavy_01", ".96 Gal Deco" },
            {"Shooter_Heavy_Coop", ".96 Gal (SR)" },
            {"Shooter_Heavy_Msn", ".96 Gal (Mission)" },
            {"Shooter_Long_00", "Jet Squelcher" },
            {"Shooter_Long_01", "Custom Jet Squelcher" },
            {"Shooter_Long_Coop", "Jet Squelcher (SR)" },
            {"Shooter_Long_Msn", "Jet Squelcher (Mission)" },
            {"Shooter_MissionLv1_00", "Octoshot Replica Lvl 1" },
            {"Shooter_MissionLv2_00", "Octoshot Replica Lvl 2" },
            {"Shooter_MissionLv3_00", "Octoshot Replica Lvl 3" },
            {"Shooter_Normal_00", "Splattershot" },
            {"Shooter_Normal_01", "Tentatek Splattershot" },
            {"Shooter_Normal_Coop", "Splattershot (SR)" },
            {"Shooter_Normal_H", "Hero Shot Replica" },
            {"Shooter_Normal_Msn", "Splattershot (Mission)" },
            {"Shooter_Precision_00", "Splash-o-matic" },
            {"Shooter_Precision_01", "Neo Splash-o-matic" },
            {"Shooter_Precision_Coop", "Splash-o-matic (SR)" },
            {"Shooter_Precision_Msn", "Splash-o-matic (Mission)" },
            {"Shooter_QuickLong_00", "Splattershot Nova" },
            {"Shooter_QuickLong_01", "Annaki Splattershot Nova" },
            {"Shooter_QuickLong_Coop", "Splattershot Nova (SR)" },
            {"Shooter_QuickMiddle_00", "N-ZAP '85" },
            {"Shooter_QuickMiddle_01", "N-ZAP '89" },
            {"Shooter_QuickMiddle_Coop", "N-ZAP '85 (SR)" },
            {"Shooter_QuickMiddle_Msn", "N-ZAP '85 (Mission)" },
            {"Shooter_RivalLv1_00", "Octo Shot Lvl 1" },
            {"Shooter_RivalLv2_00", "Octo Shot Lvl 2" },
            {"Shooter_Short_00", "Sploosh-o-matic" },
            {"Shooter_Short_01", "Neo Sploosh-o-matic" },
            {"Shooter_Short_Coop", "Sploosh-o-matic (SR)" },
            {"Shooter_Short_Msn", "Sploosh-o-matic (Mission)" },
            {"Shooter_TripleMiddle_00", "H-3 Nozzlenose" },
            {"Shooter_TripleMiddle_01", "H-3 Nozzlenose D" },
            {"Shooter_TripleMiddle_Coop", "H-3 Nozzlenose (SR)" },
            {"Shooter_TripleMiddle_Msn", "H-3 Nozzlenose (Mission)" },
            {"Shooter_TripleQuick_00", "L-3 Nozzlenose" },
            {"Shooter_TripleQuick_01", "L-3 Nozzlenose D" },
            {"Shooter_TripleQuick_Coop", "L-3 Nozzlenose (SR)" },
            {"Shooter_TripleQuick_Msn", "L-3 Nozzlenose (Mission)" },
            {"Slosher_Bathtub_00", "Bloblobber" },
            {"Slosher_Bathtub_Coop", "Bloblobber (SR)" },
            {"Slosher_Bathtub_Msn", "Bloblobber (Mission)" },
            {"Slosher_Bear_Coop", "Grizzco Slosher (SR)" },
            {"Slosher_Diffusion_00", "Tri-Slosher" },
            {"Slosher_Diffusion_01", "Tri-Slosher Nouveau" },
            {"Slosher_Diffusion_Coop", "Tri-Slosher (SR)" },
            {"Slosher_Diffusion_Msn", "Tri-Slosher (Mission)" },
            {"Slosher_Launcher_00", "Sloshing Machine" },
            {"Slosher_Launcher_Coop", "Sloshing Machine (SR)" },
            {"Slosher_Launcher_Msn", "Sloshing Machine (Mission)" },
            {"Slosher_RivalLv1_00", "Octoling Slosher Lvl 1" },
            {"Slosher_RivalLv2_00", "Octoling Slosher Lvl 2" },
            {"Slosher_Strong_00", "Slosher" },
            {"Slosher_Strong_01", "Slosher Deco" },
            {"Slosher_Strong_Coop", "Slosher (SR)" },
            {"Slosher_Strong_Msn", "Slosher (Mission)" },
            {"Slosher_Washtub_00", "Explosher" },
            {"Slosher_Washtub_Coop", "Explosher (SR)" },
            {"Slosher_Washtub_Msn", "Explosher (Mission)" },
            {"Spinner_Downpour_00", "Ballpoint Splatling" },
            {"Spinner_Downpour_Coop", "Ballpoint Splatling (SR)" },
            {"Spinner_Downpour_Msn", "Ballpoint Splatling (Mission)" },
            {"Spinner_Hyper_00", "Hydra Splatling" },
            {"Spinner_Hyper_Coop", "Hydra Splatling (SR)" },
            {"Spinner_Hyper_Msn", "Hydra Splatling (Mission)" },
            {"Spinner_Quick_00", "Mini Splatling" },
            {"Spinner_Quick_01", "Zink Mini Splatling" },
            {"Spinner_Quick_Coop", "Mini Splatling (SR)" },
            {"Spinner_Quick_Msn", "Mini Splatling (Mission)" },
            {"Spinner_Serein_00", "Nautilus 47" },
            {"Spinner_Serein_Coop", "Nautilus 47 (SR)" },
            {"Spinner_Serein_Msn", "Nautilus 47 (Mission)" },
            {"Spinner_Standard_00", "Heavy Splatling" },
            {"Spinner_Standard_01", "Heavy Splatling Deco" },
            {"Spinner_Standard_Coop", "Heavy Splatling (SR)" },
            {"Spinner_Standard_Msn", "Heavy Splatling (Mission)" },
            {"Stringer_Bear_Coop", "Grizzco Stringer (SR)" },
            {"Stringer_Normal_00", "Tri-Stringer" },
            {"Stringer_Normal_Coop", "Tri-Stringer (SR)" },
            {"Stringer_Normal_Msn", "Tri-Stringer (Mission)" },
            {"Stringer_Short_00", "REEF-LUX 450" },
            {"Stringer_Short_Coop", "REEF-LUX 450 (SR)" },
        };

        Dictionary<object, string> SubWeapons = new Dictionary<object, string>()
        {
            {"", "None" },
            {"Beacon", "Squid Beakon" },
            {"Bomb_Curling", "Curling Bomb" },
            {"Bomb_Curling_Hero", "Hero Curling Bomb" },
            {"Bomb_Curling_Mission", "Curling Bomb (Mission)" },
            {"Bomb_Curling_Rival", "Octoling Curling Bomb" },
            {"Bomb_Fizzy", "Fizzy Bomb" },
            {"Bomb_Quick", "Burst Bomb" },
            {"Bomb_Quick_Hero", "Hero Burst Bomb" },
            {"Bomb_Quick_Mission", "Burst Bomb (Mission)" },
            {"Bomb_Quick_Rival", "Octoling Burst Bomb" },
            {"Bomb_Robot", "Autobomb" },
            {"Bomb_Robot_Mission", "Autobomb (Mission)" },
            {"Bomb_Robot_Rival", "Octoling Autobomb" },
            {"Bomb_Splash", "Splat Bomb" },
            {"Bomb_Splash_Big_Coop", "Big Splat Bomb (SR)" },
            {"Bomb_Splash_Coop", "Splat Bomb (SR)" },
            {"Bomb_Splash_Hero", "Hero Splat Bomb" },
            {"Bomb_Splash_Mission", "Splat Bomb (Mission)" },
            {"Bomb_Splash_Rival", "Octoling Splat Bomb" },
            {"Bomb_Suction", "Suction Bomb" },
            {"Bomb_Suction_Mission", "Suction Bomb (Mission)" },
            {"Bomb_Torpedo", "Torpedo" },
            {"LineMarker", "Angle Shooter" },
            {"LineMarker_Mission", "Angle Shooter (Mission)" },
            {"PointSensor", "Point Sensor" },
            {"PoisonMist", "Toxic Mist" },
            {"SalmonBuddy", "Smallfry" },
            {"Shield", "Splash Wall" },
            {"Sprinkler", "Sprinkler" },
            {"Trap", "Ink Mine" },
            {"Trap_Mission", "Ink Mine (Mission)" },
        };

        Dictionary<object, string> SpecialWeapons = new Dictionary<object, string>()
        {
            {"", "None" },
            {"SpBlower", "Ink Vac" },
            {"SpBlower_Mission", "Ink Vac (Mission)" },
            {"SpCastle", "Kraken Royale" },
            {"SpChariot", "Crab Tank" },
            {"SpChariot_Coop", "Crab Tank (SR)" },
            {"SpChariot_Mission", "Crab Tank (Mission)" },
            {"SpEnergyStand", "Tacticooler" },
            {"SpFirework", "Super Chump" },
            {"SpGachihoko", "Rainmaker" },
            {"SpGreatBarrier", "Big Bubbler" },
            {"SpGreatBarrier_Rival", "-" },
            {"SpIkuraShoot", "-" },
            {"SpInkStorm", "Ink Storm" },
            {"SpInkStorm_Mission", "Ink Storm (Mission)" },
            {"SpInkStorm_Rival", "-" },
            {"SpJetpack", "Inkjet" },
            {"SpJetpack_Coop", "Inkjet (SR)" },
            {"SpJetpack_Mission", "Inkjet (Mission)" },
            {"SpJetpack_Rival", "-" },
            {"SpMicroLaser", "Killer Wail 5.1" },
            {"SpMicroLaser_Coop", "Killer Wail 5.1 (SR)" },
            {"SpMicroLaser_Mission", "Killer Wail 5.1 (Mission)" },
            {"SpMultiMissile", "Tenta Missiles" },
            {"SpMultiMissile_Mission", "Tenta Missiles (Mission)" },
            {"SpMultiMissile_Rival", "-" },
            {"SpNiceBall", "Booyah Bomb" },
            {"SpNiceBall_Coop", "Booyah Bomb (SR)" },
            {"SpShockSonar", "Wave Breaker" },
            {"SpShockSonar_Coop", "Wave Breaker (SR)" },
            {"SpShockSonar_Mission", "Wave Breaker (Mission)" },
            {"SpShockSonar_Rival", "-" },
            {"SpSkewer", "Reefslider" },
            {"SpSkewer_Coop", "Reefslider (SR)" },
            {"SpSkewer_Mission", "Reefslider (Mission)" },
            {"SpSuperHook", "Zipcaster" },
            {"SpSuperHook_Mission", "Zipcaster (Mission)" },
            {"SpSuperLanding", "Splashdown" },
            {"SpSuperLanding_Coop", "Splashdown (SR)" },
            {"SpSuperLanding_Rival", "-" },
            {"SpTripleTornado", "Triple Inkstrike" },
            {"SpTripleTornado_Coop", "Triple Inkstrike (SR)" },
            {"SpTripleTornado_Mission", "Triple Inkstrike (Mission)" },
            {"SpUltraShot", "Trizooka" },
            {"SpUltraShot_Coop", "Trizooka (SR)" },
            {"SpUltraShot_Mission", "Trizooka (Mission)" },
            {"SpUltraStamp", "Ultra Stamp" },
            {"SpUltraStamp_Mission", "Ultra Stamp (Mission)" },
        };
    }
}
