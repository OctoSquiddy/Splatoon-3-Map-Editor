using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByamlExt.Byaml;
using CafeLibrary;
using Syroot.BinaryData;
using Toolbox.Core;
using Wheatley.io.BYML;
using SampleMapEditor.Ainb;
using Toolbox.Core.IO;
using Octokit;
using Wheatley.io;
using static SampleMapEditor.EditorLoader;
using static System.Net.WebRequestMethods;
using static SampleMapEditor.MuMapInfo;
using static Toolbox.Core.GUI.Controls;

namespace SampleMapEditor
{
    public class StageDefinition : IByamlSerializable
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        //For saving back in the same place
        private string originalPath;

        // In Splatoon 3, the byaml file is contained with a bunch of other files that can't be
        // recreated on the fly. So we need to store the original arc file.
        private bool isZSTDCompressed = false;
        private SARC arc = null;

        // TEST func
        //private FileInfo originalFileInfo => new FileInfo(originalPath);
        public bool isSDORMapFile = false;

        private string stageName = "Fld_Custom01_Vss";
        private string bymlFileName => originalPath != String.Empty ? new FileInfo(originalPath).Name + ".byaml" : "Fld_Custom01_Vss.byaml";

        private BymlFileData BymlData;
        private AINB _ainbFile;

        /// <summary>
        /// The AINB (AI Node Binary) logic data for this stage.
        /// </summary>
        public AINB AINBFile
        {
            get => _ainbFile;
            set => _ainbFile = value;
        }

        /// <summary>
        /// Indicates if the AINB file has been modified and needs saving.
        /// </summary>
        public bool AINBModified { get; set; } = false;

        public StageDefinition()
        {
            BymlData = new BymlFileData()
            {
                byteOrder = Syroot.BinaryData.ByteOrder.LittleEndian,
                SupportPaths = false,
                Version = 3
            };
        }

        public StageDefinition(string fileName)
        {
            Console.WriteLine($"StageDefinition Ctor : Filename = {fileName}");
            originalPath = fileName;
            //stageName = new FileInfo(originalPath).Name; // Test line
            stageName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(originalPath));

            Console.WriteLine("StageDefinition Ctor called ->   StageDefinition(string)");
            Load(System.IO.File.OpenRead(fileName));
        }

        public StageDefinition(System.IO.Stream stream)
        {
            Console.WriteLine("StageDefinition Ctor called ->   StageDefinition(stream)");
            Load(stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StageDefinition"/> class from the given stream.
        /// </summary>
        /// <param name="stream">The stream from which the instance will be loaded.</param>
        private void Load(System.IO.Stream stream)
        {
            // Try to load ColorDataSet file (auto-detect version)
            string path = GlobalSettings.FindVersionedFile("RSDB", "TeamColorDataSet.Product.*.rstbl.byml.zs");
            if (!string.IsNullOrEmpty(path))
                TryLoadColorDataSetFile(path);

            // Account for the ZStandard compression
            BinaryDataReader br = new BinaryDataReader(stream, Encoding.UTF8, false);
            uint ZSTDMagic = br.ReadUInt32();
            br.Position = 0;

            if (ZSTDMagic == 0xFD2FB528)
            {
                isZSTDCompressed = true;

                ZstdNet.Decompressor Dec = new ZstdNet.Decompressor();
                byte[] res = Dec.Unwrap(br.ReadBytes((int)br.Length));

                MemoryStream stream1 = new MemoryStream();
                stream1.Write(res, 0, res.Length);
                stream1.Position = 0;
                Load(stream1);
                return;
            }

            arc = new SARC();
            arc.Load(stream);

            // Determine if file is sdor file
            int bancFileCount = 0;
            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Banc")
                {
                    bancFileCount++;
                }
            }
            isSDORMapFile = bancFileCount > 1;

            LoadLogicFile();

            if (isSDORMapFile)
            {
                LoadBancFiles();
            }
            else
            {
                LoadMapInfo();
                MapInfo.HasInfoMap = false; // Temporary, until MapInfo saving works

                LoadBancFile();
            }         
        }

        public void TryLoadColorDataSetFile(string FilePath)
        {
            if (!System.IO.File.Exists(FilePath))
            {
                Console.WriteLine($"File \"{FilePath}\" could not be found!");

                return;
            }

            NitroFile mFile = new NitroFile(FilePath);
            uint ZSTDMagic = mFile.ReadUInt32();
            mFile.Position = 0;

            TeamColorDataSet.IsZSTDCompressed = ZSTDMagic == 0x28B52FFD;
            if (TeamColorDataSet.IsZSTDCompressed)
            {
                ZstdNet.Decompressor Dec = new ZstdNet.Decompressor();
                byte[] res = Dec.Unwrap(mFile.ReadBytes((int)mFile.mFile.Count));

                mFile.Position = 0;
                mFile.mFile = res.ToList();
            }

            string Header = mFile.ReadUTF8String(2);
            mFile.Position = 0;

            if (Header != "BY" && Header != "YB")
            {
                Console.WriteLine($"File \"{FilePath}\" is not a valid BYML File!");

                return;
            }

            BYML FileData = new BYML(mFile);

            // Test color file validity
            if (FileData.Root.Type != BymlNodeType.ArrayNode || FileData.Root.Nodes.Count == 0
                || FileData.Root.Nodes[0]["AlphaHueOffset"] == null)
            {
                Console.WriteLine($"File \"{FilePath}\" is not valid!");

                return;
            }

            TeamColorDataSet.ColorDataArray.Clear();
            foreach (BymlNode.DictionaryNode ColorEntry in FileData.Root.Nodes)
            {
                TeamColorDataSet.ColorDataArray.Add(DeserializeColorData(ColorEntry));
            }

            TeamColorDataSet.FileName = mFile.FileName;
            TeamColorDataSet.IsFileFound = true;
        }

        private MuTeamColorDataSet.MuTeamColorData DeserializeColorData(BymlNode.DictionaryNode SerializedEntry)
        {
            MuTeamColorDataSet.MuTeamColorData Entry = new MuTeamColorDataSet.MuTeamColorData();

            Entry.AlphaHueOffset = ((BymlNode.Single)SerializedEntry["AlphaHueOffset"]).Value;
            Entry.AlphaHueOffsetDetailBright = ((BymlNode.Single)SerializedEntry["AlphaHueOffsetDetailBright"]).Value;
            Entry.AlphaHueOffsetDetailDark = ((BymlNode.Single)SerializedEntry["AlphaHueOffsetDetailDark"]).Value;
            Entry.AlphaHueOffsetDetailEnable = ((BymlNode.Boolean)SerializedEntry["AlphaHueOffsetDetailEnable"]).Value;
            Entry.AlphaInkRimIntensity = ((BymlNode.Single)SerializedEntry["AlphaInkRimIntensity"]).Value;
            Entry.AlphaInkRimIntensityNight = ((BymlNode.Single)SerializedEntry["AlphaInkRimIntensityNight"]).Value;
            Entry.AlphaMapInkBrightnessOffset = ((BymlNode.Single)SerializedEntry["AlphaMapInkBrightnessOffset"]).Value;
            Entry.AlphaNightTeamColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["AlphaNightTeamColor"]);
            Entry.AlphaTeamColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["AlphaTeamColor"]);
            Entry.AlphaUIColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["AlphaUIColor"]);
            Entry.AlphaUIEffectColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["AlphaUIEffectColor"]);

            Entry.BravoHueOffset = ((BymlNode.Single)SerializedEntry["BravoHueOffset"]).Value;
            Entry.BravoHueOffsetDetailBright = ((BymlNode.Single)SerializedEntry["BravoHueOffsetDetailBright"]).Value;
            Entry.BravoHueOffsetDetailDark = ((BymlNode.Single)SerializedEntry["BravoHueOffsetDetailDark"]).Value;
            Entry.BravoHueOffsetDetailEnable = ((BymlNode.Boolean)SerializedEntry["BravoHueOffsetDetailEnable"]).Value;
            Entry.BravoInkRimIntensity = ((BymlNode.Single)SerializedEntry["BravoInkRimIntensity"]).Value;
            Entry.BravoInkRimIntensityNight = ((BymlNode.Single)SerializedEntry["BravoInkRimIntensityNight"]).Value;
            Entry.BravoMapInkBrightnessOffset = ((BymlNode.Single)SerializedEntry["BravoMapInkBrightnessOffset"]).Value;
            Entry.BravoNightTeamColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["BravoNightTeamColor"]);
            Entry.BravoTeamColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["BravoTeamColor"]);
            Entry.BravoUIColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["BravoUIColor"]);
            Entry.BravoUIEffectColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["BravoUIEffectColor"]);

            Entry.CharlieHueOffset = ((BymlNode.Single)SerializedEntry["CharlieHueOffset"]).Value;
            Entry.CharlieHueOffsetDetailBright = ((BymlNode.Single)SerializedEntry["CharlieHueOffsetDetailBright"]).Value;
            Entry.CharlieHueOffsetDetailDark = ((BymlNode.Single)SerializedEntry["CharlieHueOffsetDetailDark"]).Value;
            Entry.CharlieHueOffsetDetailEnable = ((BymlNode.Boolean)SerializedEntry["CharlieHueOffsetDetailEnable"]).Value;
            Entry.CharlieInkRimIntensity = ((BymlNode.Single)SerializedEntry["CharlieInkRimIntensity"]).Value;
            Entry.CharlieInkRimIntensityNight = ((BymlNode.Single)SerializedEntry["CharlieInkRimIntensityNight"]).Value;
            Entry.CharlieMapInkBrightnessOffset = ((BymlNode.Single)SerializedEntry["CharlieMapInkBrightnessOffset"]).Value;
            Entry.CharlieNightTeamColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["CharlieNightTeamColor"]);
            Entry.CharlieTeamColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["CharlieTeamColor"]);
            Entry.CharlieUIColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["CharlieUIColor"]);
            Entry.CharlieUIEffectColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["CharlieUIEffectColor"]);

            Entry.EnableInkRimIntensity = ((BymlNode.Boolean)SerializedEntry["EnableInkRimIntensity"]).Value;
            Entry.EnableMapInkBrightnessOffset = ((BymlNode.Boolean)SerializedEntry["EnableMapInkBrightnessOffset"]).Value;
            Entry.HueOffsetEnable = ((BymlNode.Boolean)SerializedEntry["HueOffsetEnable"]).Value;
            Entry.IsSetNightTeamColor = ((BymlNode.Boolean)SerializedEntry["IsSetNightTeamColor"]).Value;
            Entry.IsSetUIColor = ((BymlNode.Boolean)SerializedEntry["IsSetUIColor"]).Value;
            Entry.IsSetUIEffectColor = ((BymlNode.Boolean)SerializedEntry["IsSetUIEffectColor"]).Value;

            Entry.NeutralColor = DeserializeColor((BymlNode.DictionaryNode)SerializedEntry["NeutralColor"]);
            Entry.NeutralHueOffset = ((BymlNode.Single)SerializedEntry["NeutralHueOffset"]).Value;
            Entry.NeutralHueOffsetDetailBright = ((BymlNode.Single)SerializedEntry["NeutralHueOffsetDetailBright"]).Value;
            Entry.NeutralHueOffsetDetailDark = ((BymlNode.Single)SerializedEntry["NeutralHueOffsetDetailDark"]).Value;
            Entry.NeutralHueOffsetDetailEnable = ((BymlNode.Boolean)SerializedEntry["NeutralHueOffsetDetailEnable"]).Value;

            Entry.Tag = ((BymlNode.String)SerializedEntry["Tag"]).Text;
            Entry.__RowId = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(((BymlNode.String)SerializedEntry["__RowId"]).Text));

            return Entry;
        }

        private System.Numerics.Vector4 DeserializeColor(BymlNode.DictionaryNode SerializedColor)
        {
            return new System.Numerics.Vector4(
                ((BymlNode.Single)SerializedColor["R"]).Value,
                ((BymlNode.Single)SerializedColor["G"]).Value,
                ((BymlNode.Single)SerializedColor["B"]).Value,
                ((BymlNode.Single)SerializedColor["A"]).Value
                );
        }

        private void LoadMapInfo()
        {
            // Trying to find the map info type
            bool hasFoundMapInfo = false;
            for (int i = 0; i < arc.files.Count && !hasFoundMapInfo; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "SceneComponent")
                {
                    switch (words[1])
                    {
                        case "VersusMapInfo":
                            hasFoundMapInfo = true;
                            LoadVersusInfoMap(arc.files[i]);
                            break;

                        case "CoopMapInfo":
                            LoadCoopInfoMap(arc.files[i]);
                            hasFoundMapInfo = true;
                            break;

                        case "MissionMapInfo":
                            LoadMissionInfoMap(arc.files[i]);
                            hasFoundMapInfo = true;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void LoadVersusInfoMap(ArchiveFileInfo VersusMapInfoArchive)
        {
            MapInfo.Name = VersusMapInfoArchive.FileName;
            MapInfo.HasInfoMap = true;
            MapInfo.mapInfoType = MapInfoType.Versus;

            NitroFile mFile = new NitroFile()
            {
                FileName = VersusMapInfoArchive.FileName,
                mFile = VersusMapInfoArchive.FileData.ToArray().ToList()
            };
            BYML FileData = new BYML(mFile);

            if (FileData.Root["DisplayOrder"] != null)
            {
                MapInfo.DisplayOrder = ((BymlNode.Int32)FileData.Root["DisplayOrder"]).Value;
            }

            if (FileData.Root["Id"] != null)
            {
                MapInfo.Id = ((BymlNode.Int32)FileData.Root["Id"]).Value;
            }

            if (FileData.Root["Season"] != null)
            {
                MapInfo.Season = ((BymlNode.Int32)FileData.Root["Season"]).Value;
            }
        }

        private void LoadCoopInfoMap(ArchiveFileInfo MissionMapInfoArchive)
        {
            MapInfo.Name = MissionMapInfoArchive.FileName;
            MapInfo.HasInfoMap = true;
            MapInfo.mapInfoType = MapInfoType.SalmonRun;

            NitroFile mFile = new NitroFile()
            {
                FileName = MissionMapInfoArchive.FileName,
                mFile = MissionMapInfoArchive.FileData.ToArray().ToList()
            };
            BYML FileData = new BYML(mFile);

            if (FileData.Root["DisplayOrder"] != null)
            {
                MapInfo.DisplayOrder = ((BymlNode.Int32)FileData.Root["DisplayOrder"]).Value;
            }

            if (FileData.Root["Id"] != null)
            {
                MapInfo.Id = ((BymlNode.Int32)FileData.Root["Id"]).Value;
            }

            if (FileData.Root["IsBigRun"] != null)
            {
                MapInfo.IsBigRun = ((BymlNode.Boolean)FileData.Root["IsBigRun"]).Value;
            }

            if (FileData.Root["IsBadgeInfo"] != null)
            {
                MapInfo.IsBadgeInfo = ((BymlNode.Boolean)FileData.Root["IsBadgeInfo"]).Value;
            }

            if (FileData.Root["Season"] != null)
            {
                MapInfo.Season = ((BymlNode.Int32)FileData.Root["Season"]).Value;
            }
        }

        private void LoadMissionInfoMap(ArchiveFileInfo MissionMapInfoArchive)
        {
            MapInfo.Name = MissionMapInfoArchive.FileName;
            MapInfo.HasInfoMap = true;
            MapInfo.mapInfoType = MapInfoType.StoryMode;

            NitroFile mFile = new NitroFile()
            {
                FileName = MissionMapInfoArchive.FileName,
                mFile = MissionMapInfoArchive.FileData.ToArray().ToList()
            };
            BYML FileData = new BYML(mFile);

            ReadStageVariables(FileData);

            MapInfo.isChallengeParamArrayWritten = FileData.Root["ChallengeParamArray"] != null;
            if (MapInfo.isChallengeParamArrayWritten)
            {
                ReadChallengeParamArray(FileData);
            }

            MapInfo.isStageDolphinMessageWritten = FileData.Root["StageDolphinMessage"] != null;
            if (MapInfo.isStageDolphinMessageWritten)
            {
                ReadStageDolphinMessageArray(FileData);
            }

            // Get Color data
            MapInfo.isOctaSupplyWeaponInfoArrayWritten = FileData.Root["OctaSupplyWeaponInfoArray"] != null;
            if (MapInfo.isOctaSupplyWeaponInfoArrayWritten)
            {
                ReadOctoWeaponSupplyArray(FileData);
            }

            ReadMapInfoColorData(FileData);
        }

        private void ReadStageVariables(BYML FileData)
        {
            // Get MapType
            if (FileData.Root["MapType"] != null)
            {
                MapInfo.MapType = ((BymlNode.String)FileData.Root["MapType"]).Text;
            }

            MapInfo.isAdmissionWritten = FileData.Root["Admission"] != null;
            if (MapInfo.isAdmissionWritten)
            {
                MapInfo.Admission = ((BymlNode.Int32)FileData.Root["Admission"]).Value;
            }

            MapInfo.isFirstRewardWritten = FileData.Root["FirstReward"] != null;
            if (MapInfo.isFirstRewardWritten)
            {
                MapInfo.FirstReward = ((BymlNode.Int32)FileData.Root["FirstReward"]).Value;
            }

            MapInfo.isRetryNumWritten = FileData.Root["RetryNum"] != null;
            if (MapInfo.isRetryNumWritten)
            {
                MapInfo.RetryNum = ((BymlNode.Int32)FileData.Root["RetryNum"]).Value;
            }

            MapInfo.isSecondRewardWritten = FileData.Root["SecondReward"] != null;
            if (MapInfo.isSecondRewardWritten)
            {
                MapInfo.SecondReward = ((BymlNode.Int32)FileData.Root["SecondReward"]).Value;
            }

            if (FileData.Root["MapNameLabel"] != null)
            {
                MapInfo.MapNameLabel = ((BymlNode.String)FileData.Root["MapNameLabel"]).Text;
            }
        }

        private void ReadChallengeParamArray(BYML FileData)
        {
            foreach (BymlNode.DictionaryNode ChallengeParamInfo in FileData.Root["ChallengeParamArray"].Nodes)
            {
                MuMapInfo.MuChallengeParam ChallengeParam = new MuMapInfo.MuChallengeParam();

                if (ChallengeParamInfo["Type"] != null)
                {
                    ChallengeParam.Type = ((BymlNode.String)ChallengeParamInfo["Type"]).Text;
                }

                if (ChallengeParamInfo["BreakCounterParam"] != null)
                {
                    BymlNode.DictionaryNode BreakCounterParam = (BymlNode.DictionaryNode)ChallengeParamInfo["BreakCounterParam"];

                    ChallengeParam.isCountNumWritten = BreakCounterParam["CountNum"] != null;
                    if (ChallengeParam.isCountNumWritten)
                    {
                        ChallengeParam.CountNum = ((BymlNode.Int32)BreakCounterParam["CountNum"]).Value;
                    }

                    ChallengeParam.isIsOnlyPlayerBreakWritten = BreakCounterParam["IsOnlyPlayerBreak"] != null;
                    if (ChallengeParam.isIsOnlyPlayerBreakWritten)
                    {
                        ChallengeParam.IsOnlyPlayerBreak = ((BymlNode.Boolean)BreakCounterParam["IsOnlyPlayerBreak"]).Value;
                    }

                    ChallengeParam.isTargetActorNameListWritten = BreakCounterParam["TargetActorNameList"] != null;
                    if (ChallengeParam.isTargetActorNameListWritten)
                    {
                        foreach (BymlNode.String ActorListEntry in BreakCounterParam["TargetActorNameList"].Nodes)
                        {
                            ChallengeParam.TargetActorNameList.Add(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(ActorListEntry.Text)));
                        }
                    }

                    ChallengeParam.isViewUIRemainNumWritten = BreakCounterParam["ViewUIRemainNum"] != null;
                    if (ChallengeParam.isViewUIRemainNumWritten)
                    {
                        ChallengeParam.ViewUIRemainNum = ((BymlNode.Int32)BreakCounterParam["ViewUIRemainNum"]).Value;
                    }
                }

                if (ChallengeParamInfo["InkLimitParam"] != null)
                {
                    BymlNode.DictionaryNode InkLimitParam = (BymlNode.DictionaryNode)ChallengeParamInfo["InkLimitParam"];

                    ChallengeParam.isInkLimitWritten = InkLimitParam["InkLimit"] != null;
                    if (ChallengeParam.isInkLimitWritten)
                    {
                        ChallengeParam.InkLimit = ((BymlNode.Single)InkLimitParam["InkLimit"]).Value;
                    }
                }

                if (ChallengeParamInfo["OneShotMissDieParam"] != null)
                {
                    BymlNode.DictionaryNode OneShotMissDieParam = (BymlNode.DictionaryNode)ChallengeParamInfo["OneShotMissDieParam"];

                    ChallengeParam.isClearWaitTimeWritten = OneShotMissDieParam["ClearWaitTime"] != null;
                    if (ChallengeParam.isClearWaitTimeWritten)
                    {
                        ChallengeParam.ClearWaitTime = ((BymlNode.Single)OneShotMissDieParam["ClearWaitTime"]).Value;
                    }
                }

                if (ChallengeParamInfo["TimeLimitParam"] != null)
                {
                    BymlNode.DictionaryNode TimeLimitParam = (BymlNode.DictionaryNode)ChallengeParamInfo["TimeLimitParam"];

                    ChallengeParam.isTimeLimitWritten = TimeLimitParam["TimeLimit"] != null;
                    if (ChallengeParam.isTimeLimitWritten)
                    {
                        ChallengeParam.TimeLimit = ((BymlNode.Single)TimeLimitParam["TimeLimit"]).Value;
                    }
                }

                MapInfo.ChallengeParamArray.Add(ChallengeParam);
            }
        }

        private void ReadStageDolphinMessageArray(BYML FileData)
        {
            foreach (BymlNode.DictionaryNode StageDolphinMessage in FileData.Root["StageDolphinMessage"].Nodes)
            {
                string DevText = "";
                string Label = "";

                if (StageDolphinMessage["DevText"] != null)
                {
                    DevText = ((BymlNode.String)StageDolphinMessage["DevText"]).Text;
                }

                if (StageDolphinMessage["Label"] != null)
                {
                    Label = ((BymlNode.String)StageDolphinMessage["Label"]).Text;
                }

                MapInfo.StageDolphinMessage.Add(new MuMapInfo.MuDolphinMessage()
                {
                    Devtext = DevText,
                    Label = Label
                });
            }
        }

        private void ReadOctoWeaponSupplyArray(BYML FileData)
        {
            foreach (BymlNode.DictionaryNode OctaSupplyWeaponInfo in FileData.Root["OctaSupplyWeaponInfoArray"].Nodes)
            {
                MuMapInfo.MuOctaWeaponSupply OctoWeapon = new MuMapInfo.MuOctaWeaponSupply();

                OctoWeapon.isDolphinMessageWritten = OctaSupplyWeaponInfo["DolphinMessage"] != null;
                if (OctoWeapon.isDolphinMessageWritten)
                {
                    if (((BymlNode.DictionaryNode)OctaSupplyWeaponInfo["DolphinMessage"])["DevText"] != null)
                    {
                        OctoWeapon.DolphinMessage.Devtext = ((BymlNode.String)((BymlNode.DictionaryNode)OctaSupplyWeaponInfo["DolphinMessage"])["DevText"]).Text;
                    }

                    if (((BymlNode.DictionaryNode)OctaSupplyWeaponInfo["DolphinMessage"])["Label"] != null)
                    {
                        OctoWeapon.DolphinMessage.Label = ((BymlNode.String)((BymlNode.DictionaryNode)OctaSupplyWeaponInfo["DolphinMessage"])["Label"]).Text;
                    }
                }

                OctoWeapon.isFirstRewardWritten = OctaSupplyWeaponInfo["FirstReward"] != null;
                if (OctoWeapon.isFirstRewardWritten)
                {
                    OctoWeapon.FirstReward = ((BymlNode.Int32)OctaSupplyWeaponInfo["FirstReward"]).Value;
                }

                OctoWeapon.isIsRecommendedWritten = OctaSupplyWeaponInfo["IsRecommended"] != null;
                if (OctoWeapon.isIsRecommendedWritten)
                {
                    OctoWeapon.IsRecommended = ((BymlNode.Boolean)OctaSupplyWeaponInfo["IsRecommended"]).Value;
                }

                OctoWeapon.isSecondRewardWritten = OctaSupplyWeaponInfo["SecondReward"] != null;
                if (OctoWeapon.isSecondRewardWritten)
                {
                    OctoWeapon.SecondReward = ((BymlNode.Int32)OctaSupplyWeaponInfo["SecondReward"]).Value;
                }

                OctoWeapon.isSpecialWeaponWritten = OctaSupplyWeaponInfo["SpecialWeapon"] != null;
                if (OctoWeapon.isSpecialWeaponWritten)
                {
                    OctoWeapon.SpecialWeapon = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(((BymlNode.String)OctaSupplyWeaponInfo["SpecialWeapon"]).Text));
                }

                OctoWeapon.isSubWeaponWritten = OctaSupplyWeaponInfo["SubWeapon"] != null;
                if (OctoWeapon.isSubWeaponWritten)
                {
                    OctoWeapon.SubWeapon = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(((BymlNode.String)OctaSupplyWeaponInfo["SubWeapon"]).Text));
                }

                OctoWeapon.isSupplyWeaponTypeWritten = OctaSupplyWeaponInfo["SupplyWeaponType"] != null;
                if (OctoWeapon.isSupplyWeaponTypeWritten)
                {
                    OctoWeapon.SupplyWeaponType = ((BymlNode.String)OctaSupplyWeaponInfo["SupplyWeaponType"]).Text;
                }

                OctoWeapon.isWeaponMainWritten = OctaSupplyWeaponInfo["WeaponMain"] != null;
                if (OctoWeapon.isWeaponMainWritten)
                {
                    OctoWeapon.WeaponMain = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(((BymlNode.String)OctaSupplyWeaponInfo["WeaponMain"]).Text));
                }

                MapInfo.OctaSupplyWeaponInfoArray.Add(OctoWeapon);
            }
        }

        private void ReadMapInfoColorData(BYML FileData)
        {
            // Get File Name
            if (FileData.Root["TeamColor"] != null)
            {
                string __RowID = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(((BymlNode.String)FileData.Root["TeamColor"]).Text));

                if (TeamColorDataSet.ColorDataArray.Any(x => x.__RowId == __RowID))
                {
                    MapInfo.TeamColorData = TeamColorDataSet.ColorDataArray.First(x => x.__RowId == __RowID).Clone();
                }
            }
        }

        private void LoadBancFile()
        {
            // Determine the file located in the banc folder
            ArchiveFileInfo BymlFile = null;

            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Banc")
                {
                    BymlFile = arc.files[i];
                    break;
                }
            }

            BymlData = ByamlFile.LoadN(BymlFile.FileData, false);

            GlobalSettings.LoadDataBase();

            ByamlSerialize.SpecialDeserialize(this, BymlData.RootNode);
            Console.WriteLine("Special Deserialized byaml!");

            Rails?.ForEach(x => x.DeserializeReferences(this));
            Actors?.ForEach(x => x.DeserializeReferences(this));

            Actors?.ForEach(x => Console.WriteLine(x.Name));
            Rails?.ForEach(r => Console.WriteLine(r.Name));

            // Get AiGroups (thx squiddy)
            NitroFile mFile = new NitroFile()
            {
                FileName = BymlFile.FileName,
                mFile = BymlFile.FileData.ToArray().ToList()
            };
            BYML FileData = new BYML(mFile);

            BymlNode AiGroups = FileData.Root["AiGroups"];

            if (AiGroups != null)
            {
                // Assign every AiGroupID to the concerned actors
                BymlNode.ArrayNode References = (BymlNode.ArrayNode)AiGroups.Nodes[0]["References"];

                foreach (BymlNode Reference in References.Nodes)
                {
                    string AiGroupID = ((BymlNode.String)Reference["Id"]).Text;
                    string[] SplitGroup = AiGroupID.Split('_');

                    // Get actor
                    MuObj Actor = GetActorByReference(((BymlNode.UInt64)Reference["Reference"]).Value);

                    // If the actor couldn't get found
                    if (Actor == null) continue;

                    // Assign the value
                    Actor.AIGroupID = SplitGroup[SplitGroup.Length - 1];
                }
            }

            // Check each rail point for the presence of control points
            BymlNode RailData = FileData.Root["Rails"];

            for (int i = 0; RailData != null && i < RailData.Nodes.Count; i++)
            {
                BymlNode Rail = RailData.Nodes[i];
                MuRail CorrespondingActor = Rails[i];

                BymlNode Points = Rail["Points"];

                for (int j = 0; j < Points.Nodes.Count; j++)
                {
                    if (Points.Nodes[j]["Control0"] != null)
                    {
                        CorrespondingActor.Points[j].hasControlPoints = true;
                    }
                }
            }
        }

        private void LoadBancFiles()
        {
            // Determine the file located in the banc folder
            ArchiveFileInfo BymlFile0 = null;
            ArchiveFileInfo BymlFile1 = null;

            int currentFileFound = 0;
            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Banc")
                {
                    if (currentFileFound == 0)
                    {
                        BymlFile0 = arc.files[i];
                        currentFileFound++;
                    }
                    else
                    {
                        BymlFile1 = arc.files[i];
                        break;
                    }
                }
            }

            BymlData = ByamlFile.LoadN(BymlFile0.FileData, false);

            GlobalSettings.LoadDataBase();

            ByamlSerialize.SpecialDeserialize(this, BymlData.RootNode);
            Console.WriteLine("Special Deserialized byaml!");

            Rails?.ForEach(x => x.DeserializeReferences(this));
            Actors?.ForEach(x => x.DeserializeReferences(this));

            Actors?.ForEach(x => Console.WriteLine(x.Name));
            Rails?.ForEach(r => Console.WriteLine(r.Name));

            // Get AiGroups (thx squiddy)
            NitroFile mFile = new NitroFile()
            {
                FileName = BymlFile0.FileName,
                mFile = BymlFile0.FileData.ToArray().ToList()
            };
            BYML FileData = new BYML(mFile);

            BymlNode AiGroups = FileData.Root["AiGroups"];

            if (AiGroups != null)
            {
                // Assign every AiGroupID to the concerned actors
                BymlNode.ArrayNode References = (BymlNode.ArrayNode)AiGroups.Nodes[0]["References"];

                foreach (BymlNode Reference in References.Nodes)
                {
                    string AiGroupID = ((BymlNode.String)Reference["Id"]).Text;
                    string[] SplitGroup = AiGroupID.Split('_');

                    // Get actor
                    MuObj Actor = GetActorByReference(((BymlNode.UInt64)Reference["Reference"]).Value);

                    // If the actor couldn't get found
                    if (Actor == null) continue;

                    // Assign the value
                    Actor.AIGroupID = SplitGroup[SplitGroup.Length - 1];
                }
            }

            // Check each rail point for the presence of control points
            BymlNode RailData = FileData.Root["Rails"];

            for (int i = 0; RailData != null && i < RailData.Nodes.Count; i++)
            {
                BymlNode Rail = RailData.Nodes[i];
                MuRail CorrespondingActor = Rails[i];

                BymlNode Points = Rail["Points"];

                for (int j = 0; j < Points.Nodes.Count; j++)
                {
                    if (Points.Nodes[j]["Control0"] != null)
                    {
                        CorrespondingActor.Points[j].hasControlPoints = true;
                    }
                }
            }

            ActorsFile0 = new List<MuObj>();
            ActorsFile0.AddRange(Actors != null ? Actors : new List<MuObj>());

            RailsFile0 = new List<MuRail>();
            RailsFile0.AddRange(Rails != null ? Rails : new List<MuRail>());

            Actors?.Clear();
            Rails?.Clear();

            BymlData = ByamlFile.LoadN(BymlFile1.FileData, false);

            GlobalSettings.LoadDataBase();

            ByamlSerialize.SpecialDeserialize(this, BymlData.RootNode);
            Console.WriteLine("Special Deserialized byaml!");

            Rails?.ForEach(x => x.DeserializeReferences(this));
            Actors?.ForEach(x => x.DeserializeReferences(this));

            Actors?.ForEach(x => Console.WriteLine(x.Name));
            Rails?.ForEach(r => Console.WriteLine(r.Name));

            // Get AiGroups (thx squiddy)
            mFile = new NitroFile()
            {
                FileName = BymlFile1.FileName,
                mFile = BymlFile1.FileData.ToArray().ToList()
            };
            FileData = new BYML(mFile);

            AiGroups = FileData.Root["AiGroups"];

            if (AiGroups != null)
            {
                // Assign every AiGroupID to the concerned actors
                BymlNode.ArrayNode References = (BymlNode.ArrayNode)AiGroups.Nodes[0]["References"];

                foreach (BymlNode Reference in References.Nodes)
                {
                    string AiGroupID = ((BymlNode.String)Reference["Id"]).Text;
                    string[] SplitGroup = AiGroupID.Split('_');

                    // Get actor
                    MuObj Actor = GetActorByReference(((BymlNode.UInt64)Reference["Reference"]).Value);

                    // If the actor couldn't get found
                    if (Actor == null) continue;

                    // Assign the value
                    Actor.AIGroupID = SplitGroup[SplitGroup.Length - 1];
                }
            }

            // Check each rail point for the presence of control points
            RailData = FileData.Root["Rails"];

            for (int i = 0; RailData != null && i < RailData.Nodes.Count; i++)
            {
                BymlNode Rail = RailData.Nodes[i];
                MuRail CorrespondingActor = Rails[i];

                BymlNode Points = Rail["Points"];

                for (int j = 0; j < Points.Nodes.Count; j++)
                {
                    if (Points.Nodes[j]["Control0"] != null)
                    {
                        CorrespondingActor.Points[j].hasControlPoints = true;
                    }
                }
            }

            ActorsFile1 = new List<MuObj>();
            ActorsFile1.AddRange(Actors != null ? Actors : new List<MuObj>());

            RailsFile1 = new List<MuRail>();
            RailsFile1.AddRange(Rails != null ? Rails : new List<MuRail>());

            Actors?.Clear();
            Rails?.Clear();
        }

        private void LoadLogicFile()
        {
            ArchiveFileInfo LogicFile = null;

            Console.WriteLine($"[AINB] Searching for Logic file in archive with {arc.files.Count} files...");

            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');
                Console.WriteLine($"[AINB]   File {i}: {arc.files[i].FileName} (first part: {words[0]})");

                if (words[0] == "Logic")
                {
                    LogicFile = arc.files[i];
                    Console.WriteLine($"[AINB] Found Logic file: {arc.files[i].FileName}");
                }
            }

            if (LogicFile != null)
            {
                Console.WriteLine($"[AINB] Loading AINB from Logic file ({LogicFile.FileData.ToArray().Length} bytes)...");
                try
                {
                    AINBFile = AINB.LoadFromAINBData(LogicFile.FileData.ToArray().ToList());
                    Console.WriteLine($"[AINB] AINB loaded successfully: {AINBFile?.Info?.Filename ?? "Unknown"}, {AINBFile?.Nodes?.Count ?? 0} nodes");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AINB] Error loading AINB: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("[AINB] No Logic file found in archive - AINB Editor will not be available");
            }
        }

        public void Save() { this.Save(originalPath); }

        public void Save(string fileName)
        {
            using (var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                Save(stream);
        }

        public void Save(System.IO.Stream stream)
        {
            Console.WriteLine("~ Called StageDefinition.Save(); ~");

            if (TeamColorDataSet.IsFileFound && MapInfo.HasInfoMap)
            {
                SaveColorDataSetFile();
            }

            SaveLogicFileToArchive();
            SaveMapInfoFileToArchive();

            if (isSDORMapFile)
            {
                SaveBANCFilesToArchive();
            }
            else
            {
                SaveBANCFileToArchive();
            }

            byte[] acrFileBin = arc.Save(stream);

            // If is ZSTD compressed
            if (isZSTDCompressed)
            {
                ZstdNet.Compressor Cmp = new ZstdNet.Compressor();
                byte[] ret = Cmp.Wrap(acrFileBin);

                acrFileBin = ret;
            }

            // Write file normal
            using (var writer = new FileWriter(stream))
            {               
                writer.Write(acrFileBin);
            }
        }

        private void SaveBANCFileToArchive()
        {
            SaveMapObjList();

            BYML mSerealizedData = generateBANCFile();

            // Determine the file located in the banc folder
            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Banc")
                {
                    NitroFile SaveBYML = mSerealizedData.Save();
                    arc.files[i].SetData(SaveBYML.mFile.ToArray());
                    arc.SarcData.Files[arc.files[i].FileName] = SaveBYML.mFile.ToArray();
                    break;
                }
            }
        }

        private void SaveBANCFilesToArchive()
        {
            SaveMapObjList();

            Actors = new List<MuObj>();
            Actors.AddRange(ActorsFile0);
            Rails = new List<MuRail>();
            Rails.AddRange(RailsFile0);
            BYML mSerealizedData0 = generateBANCFile();

            Actors = new List<MuObj>();
            Actors.AddRange(ActorsFile1);
            Rails = new List<MuRail>();
            Rails.AddRange(RailsFile1);
            BYML mSerealizedData1 = generateBANCFile();

            Actors = new List<MuObj>();
            Rails = new List<MuRail>();

            // Determine the file located in the banc folder
            bool foundFile0 = false;
            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Banc")
                {
                    if (!foundFile0)
                    {
                        NitroFile SaveBYML = mSerealizedData0.Save();
                        arc.files[i].SetData(SaveBYML.mFile.ToArray());
                        arc.SarcData.Files[arc.files[i].FileName] = SaveBYML.mFile.ToArray();
                        foundFile0 = true;
                    }
                    else
                    {
                        NitroFile SaveBYML = mSerealizedData1.Save();
                        arc.files[i].SetData(SaveBYML.mFile.ToArray());
                        arc.SarcData.Files[arc.files[i].FileName] = SaveBYML.mFile.ToArray();
                        break;
                    }
                }
            }
        }

        private BYML generateBANCFile()
        {
            // Determine the file located in the banc folder
            ArchiveFileInfo BymlFile = null, LogicFile = null;

            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Banc")
                {
                    BymlFile = arc.files[i];
                    break;
                }
            }

            // Determine the file located in the Logic folder
            // If there are none, then the byml doesn't have a AiGroups array
            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Logic")
                {
                    LogicFile = arc.files[i];
                    break;
                }
            }

            BYML mSerealizedData = new BYML()
            {
                FileName = BymlFile.FileName,
                FileVersion = 7,
                IsBigEndian = false,
                Root = new BymlNode.DictionaryNode()
            };

            mSerealizedData.Root.Nodes.Add(generateActorsArray());

            if (LogicFile != null)
                mSerealizedData.Root.Nodes.Add(generateAiGroupsArray(LogicFile));

            // Trying to reconstruct FilePath
            string FilePath = "Work/Banc/Scene/" + Path.GetFileNameWithoutExtension(mSerealizedData.FileName) + ".json";
            mSerealizedData.Root.Nodes.Add(new BymlNode.String("FilePath", FilePath));

            mSerealizedData.Root.Nodes.Add(generateRailsArray());

            // Gets the new HashTable
            List<string> newHashTable = new List<string>();
            GetNewHashTable(mSerealizedData.Root, ref newHashTable);
            newHashTable.Sort(new HashTableOrder());

            // Gets the new StringTable
            List<string> newStringTable = new List<string>();
            GetNewStringTable(mSerealizedData.Root, ref newStringTable);
            newStringTable.Sort(new HashTableOrder());

            mSerealizedData.HashTable = newHashTable;
            mSerealizedData.StringTable = newStringTable;

            return mSerealizedData;
        }

        private BymlNode.ArrayNode generateActorsArray()
        {
            BymlNode.ArrayNode mActors = new BymlNode.ArrayNode()
            {
                Name = "Actors"
            };

            foreach (MuObj Actor in Actors)
            {
                List<MuObj.Link> originalLinks = new List<MuObj.Link>();
                List<int> RailNoDstIdx = new List<int>();

                foreach (MuObj.Link Link in Actor.Links)
                {
                    originalLinks.Add(Link.Clone());
                }

                Actor.Links.Clear();

                foreach (MuObj.Link Link in originalLinks)
                {
                    if (Link.Dst != 0)
                    {
                        Actor.Links.Add(Link.Clone());
                    }
                }

                BymlNode.DictionaryNode SerializedActor = new BymlNode.DictionaryNode(),
                    Phive = new BymlNode.DictionaryNode() { Name = "Phive" },
                    Placement = new BymlNode.DictionaryNode() { Name = "Placement" },
                    TeamCmp = new BymlNode.DictionaryNode() { Name = "TeamCmp" };

                BymlNode.ArrayNode Scale = new BymlNode.ArrayNode() { Name = "Scale" },
                    Rotate = new BymlNode.ArrayNode() { Name = "Rotate" },
                    Translate = new BymlNode.ArrayNode() { Name = "Translate" },
                    Links = new BymlNode.ArrayNode() { Name = "Links" };

                if (Actor.Bakeable) SerializedActor.AddNode("Bakeable", true);
                SerializedActor.AddNode("Gyaml", Actor.Gyaml);
                SerializedActor.AddNode("Hash", Actor.Hash);
                SerializedActor.AddNode("InstanceID", Actor.InstanceID);

                foreach (MuObj.Link Link in Actor.Links)
                {
                    BymlNode.DictionaryNode SerializedLink = new BymlNode.DictionaryNode();

                    SerializedLink.AddNode("Dst", Link.Dst);
                    SerializedLink.AddNode("Name", Link.Name);

                    Links.Nodes.Add(SerializedLink);
                }

                if (Links.Nodes.Count > 0)
                    SerializedActor.Nodes.Add(Links);

                if (Actor.Layer != "None")
                    SerializedActor.AddNode("Layer", Actor.Layer);

                if (Actor.Name != "")
                    SerializedActor.AddNode("Name", Actor.Name);

                Placement.AddNode("ID", Actor.Hash);
                Phive.Nodes.Add(Placement);
                SerializedActor.Nodes.Add(Phive);

                if (!Actor.Rotate.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)))
                {
                    Rotate.AddNode(Actor.Rotate.X);
                    Rotate.AddNode(Actor.Rotate.Y);
                    Rotate.AddNode(Actor.Rotate.Z);

                    SerializedActor.Nodes.Add(Rotate);
                }

                SerializedActor.AddNode("SRTHash", Actor.SRTHash);

                if (!Actor.Scale.Equals(new ByamlVector3F(1.0f, 1.0f, 1.0f)))
                {
                    Scale.AddNode(Actor.Scale.X);
                    Scale.AddNode(Actor.Scale.Y);
                    Scale.AddNode(Actor.Scale.Z);

                    SerializedActor.Nodes.Add(Scale);
                }

                TeamCmp.AddNode("Team", Actor.TeamCmp.Team);
                SerializedActor.Nodes.Add(TeamCmp);

                if (!Actor.Translate.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)))
                {
                    Translate.AddNode(Actor.Translate.X);
                    Translate.AddNode(Actor.Translate.Y);
                    Translate.AddNode(Actor.Translate.Z);

                    SerializedActor.Nodes.Add(Translate);
                }

                Actor.SaveLinks(SerializedActor);
                Actor.SaveAdditionalParameters(SerializedActor);

                mActors.Nodes.Add(SerializedActor);

                Actor.Links = originalLinks;
            }

            return mActors;
        }

        private BymlNode.ArrayNode generateAiGroupsArray(ArchiveFileInfo logicFile)
        {
            List<string> ActorsNeedAiRef = new List<string>()
            {
                "AerialRing",
                "AirBallParent",
                "GachihokoCheckPoint",
                "GlassCage",
                "InkRail",
                "ItemCardKey",
                "Lft_AbstractBlitzCompatibles",
                "Lft_AbstractDrawer",
                "Lft_AbstractRotateTogglePoint",
                "Lft_KeepOutPlayer",
                "LocatorAreaSwitch",
                "RivalAppearPoint",
                "SwitchShock"
            };

            List<string> ActorsNoNeedAiRef = new List<string>()
            {
                "EnemyCleaner",
            };

            BymlNode.ArrayNode mAiGroups = new BymlNode.ArrayNode()
            {
                Name = "AiGroups"
            },

            References = new BymlNode.ArrayNode()
            {
                Name = "References"
            };

            BymlNode.DictionaryNode LogicHashRef = new BymlNode.DictionaryNode();
            LogicHashRef.AddNode("Hash", (ulong)927818069396386034);

            string LogicPath = "Logic/Root/" + Path.GetFileNameWithoutExtension(logicFile.FileName).Split('_')[0] + "/" + Path.GetFileNameWithoutExtension(logicFile.FileName) + ".ain";
            LogicHashRef.AddNode("Logic", LogicPath);

            List<string> mAllSuffixes = new List<string>();
            foreach (MuObj Actor in Actors)
            {
                bool skip = true;

                if (ActorsNeedAiRef.Contains(Actor.Name))
                    skip = false;

                if (Actor.Name.Length >= 5 && Actor.Name.Substring(0, 5) == "Enemy")
                    skip = false;

                if (Actor.Name.Length >= 8 && Actor.Name.Substring(0, 8) == "SplEnemy")
                    skip = false;

                if (Actor.Name.Length >= 11 && Actor.Name.Substring(0, 11) == "ShootingBox")
                    skip = false;

                if (Actor.Name.Length >= 14 && Actor.Name.Substring(0, 14) == "ItemCanSpecial")
                    skip = false;

                if (ActorsNoNeedAiRef.Contains(Actor.Name))
                    skip = true;

                if (skip && Actor.AIGroupID != "") skip = false;

                if (skip) continue;

                string Suffix = Actor.AIGroupID;
                // Only generate a new suffix if the user didn't explicitly set one.
                // Multiple actors can share the same AIGroupID for multi-node AINB presets
                // (e.g., SwitchShock and Lft_AbstractRotateTogglePoint both use "_6bc5")
                if (Suffix == "")
                {
                    Suffix = GenAiGroupSuffix();
                    while (mAllSuffixes.Contains(Suffix))
                        Suffix = GenAiGroupSuffix();
                }

                string AiGroupRefName = Actor.Name + "_" + Suffix;

                BymlNode.DictionaryNode Reference = new BymlNode.DictionaryNode();
                Reference.AddNode("Id", AiGroupRefName);
                Reference.AddNode("Path", AiGroupRefName);
                Reference.AddNode("Reference", Actor.Hash);

                References.Nodes.Add(Reference);
                mAllSuffixes.Add(Suffix);
            }

            LogicHashRef.Nodes.Add(References);
            mAiGroups.Nodes.Add(LogicHashRef);

            return mAiGroups;
        }

        private BymlNode.ArrayNode generateRailsArray()
        {
            BymlNode.ArrayNode SerializedRails = new BymlNode.ArrayNode()
            {
                Name = "Rails"
            };

            // Let's try to write rails to the byaml
            foreach (MuRail Rail in Rails)
            {
                BymlNode.DictionaryNode SerializedRail = new BymlNode.DictionaryNode();
                BymlNode.ArrayNode SerializedPoints = new BymlNode.ArrayNode() { Name = "Points" },
                    Rotation = new BymlNode.ArrayNode() { Name = "Rotation" };

                SerializedRail.AddNode("Gyaml", Rail.Gyaml);
                SerializedRail.AddNode("Hash", Rail.Hash);
                SerializedRail.AddNode("InstanceID", Rail.InstanceID);
                SerializedRail.AddNode("IsClosed", Rail.IsClosed);

                if (Rail.Layer != "None")
                    SerializedRail.AddNode("Layer", Rail.Layer);

                SerializedRail.AddNode("Name", Rail.Name);

                // Run a first time to see if the rail is bezier
                /*
                bool IsBezier = false;
                foreach (MuRailPoint Point in Rail?.Points)
                {
                    if (!Point.Control0.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)) && !Point.Control1.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)))
                    {
                        IsBezier = true;
                        break;
                    }
                }
                */

                foreach (MuRailPoint Point in Rail?.Points)
                {
                    BymlNode.DictionaryNode game__LiftGraphRailNodeParam = new BymlNode.DictionaryNode("game__LiftGraphRailNodeParam"),
                        spl__GachiyaguraRailNodeParam = new BymlNode.DictionaryNode("spl__GachiyaguraRailNodeParam"),
                        PositionOffset = new BymlNode.DictionaryNode("PositionOffset"),
                        RotationDeg = new BymlNode.DictionaryNode("RotationDeg"),
                        RP_Rotation = new BymlNode.DictionaryNode("Rotation");

                    BymlNode.ArrayNode Translate = new BymlNode.ArrayNode() { Name = "Translate" },
                        Control0 = new BymlNode.ArrayNode() { Name = "Control0" },
                        Control1 = new BymlNode.ArrayNode() { Name = "Control1" };

                    BymlNode.DictionaryNode SerializedPoint = new BymlNode.DictionaryNode();

                    if (Point.hasControlPoints)
                    {
                        Control0.AddNode(Point.Control0.X);
                        Control0.AddNode(Point.Control0.Y);
                        Control0.AddNode(Point.Control0.Z);

                        SerializedPoint.Nodes.Add(Control0);

                        Control1.AddNode(Point.Control1.X);
                        Control1.AddNode(Point.Control1.Y);
                        Control1.AddNode(Point.Control1.Z);

                        SerializedPoint.Nodes.Add(Control1);
                    }

                    SerializedPoint.AddNode("Hash", Point.Hash);

                    Translate.AddNode(Point.Translate.X);
                    Translate.AddNode(Point.Translate.Y);
                    Translate.AddNode(Point.Translate.Z);

                    SerializedPoint.Nodes.Add(Translate);

                    if (Point.game__LiftGraphRailNodeParam.BreakTime != 0.0f)
                    {
                        game__LiftGraphRailNodeParam.AddNode("BreakTime", Point.game__LiftGraphRailNodeParam.BreakTime);
                    }

                    if (Point.game__LiftGraphRailNodeParam.Rotation.X != 0.0f || Point.game__LiftGraphRailNodeParam.Rotation.Y != 0.0f
                     || Point.game__LiftGraphRailNodeParam.Rotation.Z != 0.0f)
                    {
                        RP_Rotation.AddNode("X", Point.game__LiftGraphRailNodeParam.Rotation.X);
                        RP_Rotation.AddNode("Y", Point.game__LiftGraphRailNodeParam.Rotation.Y);
                        RP_Rotation.AddNode("Z", Point.game__LiftGraphRailNodeParam.Rotation.Z);

                        game__LiftGraphRailNodeParam.Nodes.Add(RP_Rotation);
                    }

                    if (game__LiftGraphRailNodeParam.Nodes.Count > 0)
                    {
                        SerializedPoint.Nodes.Add(game__LiftGraphRailNodeParam);
                    }

                    if (Rail.Name == "GachiyaguraRail")
                    {
                        if (Point.spl__GachiyaguraRailNodeParam.CheckPointHP > 0)
                            spl__GachiyaguraRailNodeParam.AddNode("CheckPointHP", Point.spl__GachiyaguraRailNodeParam.CheckPointHP);

                        if (Point.spl__GachiyaguraRailNodeParam.FillUpType != "")
                            spl__GachiyaguraRailNodeParam.AddNode("FillUpType", Point.spl__GachiyaguraRailNodeParam.FillUpType);

                        if (Point.spl__GachiyaguraRailNodeParam.PositionOffset.X != 0.0f || Point.spl__GachiyaguraRailNodeParam.PositionOffset.Y != 0.0f
                            || Point.spl__GachiyaguraRailNodeParam.PositionOffset.Z != 0.0f)
                        {
                            PositionOffset.AddNode("X", Point.spl__GachiyaguraRailNodeParam.PositionOffset.X);
                            PositionOffset.AddNode("Y", Point.spl__GachiyaguraRailNodeParam.PositionOffset.Y);
                            PositionOffset.AddNode("Z", Point.spl__GachiyaguraRailNodeParam.PositionOffset.Z);

                            spl__GachiyaguraRailNodeParam.Nodes.Add(PositionOffset);
                        }

                        if (Point.spl__GachiyaguraRailNodeParam.RotationDeg.X != 0.0f || Point.spl__GachiyaguraRailNodeParam.RotationDeg.Y != 0.0f
                            || Point.spl__GachiyaguraRailNodeParam.RotationDeg.Z != 0.0f)
                        {
                            RotationDeg.AddNode("X", Point.spl__GachiyaguraRailNodeParam.RotationDeg.X);
                            RotationDeg.AddNode("Y", Point.spl__GachiyaguraRailNodeParam.RotationDeg.Y);
                            RotationDeg.AddNode("Z", Point.spl__GachiyaguraRailNodeParam.RotationDeg.Z);

                            spl__GachiyaguraRailNodeParam.Nodes.Add(RotationDeg);
                        }

                        SerializedPoint.Nodes.Add(spl__GachiyaguraRailNodeParam);
                    }

                    SerializedPoints.Nodes.Add(SerializedPoint);
                }

                if (Rail.Rotation.X != 0.0f || Rail.Rotation.Y != 0.0f || Rail.Rotation.Z != 0.0f)
                {
                    Rotation.AddNode(Rail.Rotation.X);
                    Rotation.AddNode(Rail.Rotation.Y);
                    Rotation.AddNode(Rail.Rotation.Z);

                    SerializedRail.Nodes.Add(Rotation);
                }

                SerializedRail.Nodes.Add(SerializedPoints);

                SerializedRails.Nodes.Add(SerializedRail);
            }

            return SerializedRails;
        }

        private void SaveLogicFileToArchive()
        {
            // Skip if no AINB data loaded
            if (AINBFile == null)
            {
                Console.WriteLine("[AINB] No AINB data to save - skipping");
                return;
            }

            // Determine the file located in the Logic folder
            for (int i = 0; i < arc.files.Count; i++)
            {
                string[] words = arc.files[i].FileName.Split('/');

                if (words[0] == "Logic")
                {
                    Console.WriteLine($"[AINB] Saving AINB to {arc.files[i].FileName}...");
                    List<byte> ainbData = AINB.json2ainb(AINBFile.getJSONData());
                    arc.files[i].SetData(ainbData.ToArray());
                    arc.SarcData.Files[arc.files[i].FileName] = ainbData.ToArray();
                    Console.WriteLine($"[AINB] Saved {ainbData.Count} bytes, {AINBFile.Nodes?.Count ?? 0} nodes");
                    break;
                }
            }
        }

        private void SaveMapInfoFileToArchive()
        {
            if (!MapInfo.HasInfoMap) return;

            BYML mBymlFile = GenerateMapInfoFile();
            NitroFile SaveData = mBymlFile.Save();

            // Trying to delete the previous file to insert the new one
            Dictionary<string, byte[]> newSARCFiles = new Dictionary<string, byte[]>();
            List<string> FileNames = new List<string>();
            int InsertIndex = 0;

            int i = 0;
            foreach (KeyValuePair<string, byte[]> kvp in arc.SarcData.Files)
            {
                FileNames.Add(kvp.Key);

                string[] words = kvp.Key.Split('/');

                if (words[0] == "SceneComponent")
                {
                    switch (words[1])
                    {
                        case "VersusMapInfo":
                        case "CoopMapInfo":
                        case "MissionMapInfo":
                            InsertIndex = i;
                            break;
                    }
                }

                i++;
            }

            for (i = 0; i < FileNames.Count; i++)
            {
                if (InsertIndex == i)
                {
                    newSARCFiles.Add(mBymlFile.FileName, SaveData.mFile.ToArray());
                }
                else
                {
                    newSARCFiles.Add(FileNames[i], arc.SarcData.Files[FileNames[i]]);
                }
            }

            arc.SarcData.Files = newSARCFiles;
        }

        private BYML GenerateMapInfoFile()
        {
            string FileName = $"SceneComponent/";

            switch (MapInfo.mapInfoType)
            {
                case MapInfoType.Versus:
                    FileName += $"VersusMapInfo/{stageName}.spl__VersusMapInfo.bgyml";
                    break;

                case MapInfoType.SalmonRun:
                    FileName += $"CoopMapInfo/{stageName}.spl__CoopMapInfo.bgyml";
                    break;

                case MapInfoType.StoryMode:
                    FileName += $"MissionMapInfo/{stageName}.spl__MissionMapInfo.bgyml";
                    break;
            }

            BYML mSerealizedData = new BYML()
            {
                FileName = FileName,
                FileVersion = 7,
                IsBigEndian = false,
                Root = new BymlNode.DictionaryNode()
            };

            switch (MapInfo.mapInfoType)
            {
                case MapInfoType.Versus:
                    SaveVersusMapInfo(mSerealizedData);
                    break;

                case MapInfoType.SalmonRun:
                    SaveCoopMapInfo(mSerealizedData);
                    break;

                case MapInfoType.StoryMode:
                    SaveMissionMapinfo(mSerealizedData);
                    break;
            }

            // Gets the new HashTable
            List<string> newHashTable = new List<string>();
            GetNewHashTable(mSerealizedData.Root, ref newHashTable);
            newHashTable.Sort(new HashTableOrder());

            // Gets the new StringTable
            List<string> newStringTable = new List<string>();
            GetNewStringTable(mSerealizedData.Root, ref newStringTable);
            newStringTable.Sort(new HashTableOrder());

            mSerealizedData.HashTable = newHashTable;
            mSerealizedData.StringTable = newStringTable;

            return mSerealizedData;
        }

        private void SaveVersusMapInfo(BYML mSerealizedData)
        {
            BymlNode.DictionaryNode Root = (BymlNode.DictionaryNode)mSerealizedData.Root;

            Root.AddNode("DisplayOrder", MapInfo.DisplayOrder);
            Root.AddNode("Id", MapInfo.Id);
            Root.AddNode("Season", MapInfo.Season);
        }

        private void SaveCoopMapInfo(BYML mSerealizedData)
        {
            BymlNode.DictionaryNode Root = (BymlNode.DictionaryNode)mSerealizedData.Root;

            BymlNode.ArrayNode BadgeInfoAry = new BymlNode.ArrayNode() { Name = "BadgeInfoAry" };

            if (MapInfo.IsBadgeInfo)
            {
                string FileNameNoPrefix = stageName.Substring(4);

                for (int i = 0; i < 4; i++)
                {
                    BadgeInfoAry.AddNode($"Work/Gyml/BadgeInfo_CoopGrade_Normal_{FileNameNoPrefix}_Lv0{i}.spl__BadgeInfo.gyml");
                }

                Root.Nodes.Add(BadgeInfoAry);
            }

            Root.AddNode("DisplayOrder", MapInfo.DisplayOrder);
            Root.AddNode("Id", MapInfo.Id);

            if (MapInfo.IsBadgeInfo) Root.AddNode("IsBadgeInfo", MapInfo.IsBadgeInfo);
            if (MapInfo.IsBigRun) Root.AddNode("IsBigRun", MapInfo.IsBigRun);

            Root.AddNode("Season", MapInfo.Season);
        }

        private void SaveMissionMapinfo(BYML mSerealizedData)
        {
            BymlNode.DictionaryNode Root = (BymlNode.DictionaryNode)mSerealizedData.Root;

            BymlNode.ArrayNode ChallengeParamArray = new BymlNode.ArrayNode() { Name = "ChallengeParamArray" },
                OctaSupplyWeaponInfoArray = new BymlNode.ArrayNode() { Name = "OctaSupplyWeaponInfoArray" },
                StageDolphinMessage = new BymlNode.ArrayNode() { Name = "StageDolphinMessage" };

            if (MapInfo.isAdmissionWritten) Root.AddNode("Admission", MapInfo.Admission);

            if (MapInfo.isChallengeParamArrayWritten)
            {
                foreach (MuMapInfo.MuChallengeParam ChallengeParam in MapInfo.ChallengeParamArray)
                {
                    BymlNode.DictionaryNode SerializedChallengeParam = new BymlNode.DictionaryNode(),
                        BreakCounterParam = new BymlNode.DictionaryNode() { Name = "BreakCounterParam" },
                        InkLimitParam = new BymlNode.DictionaryNode() { Name = "InkLimitParam" },
                        OneShotMissDieParam = new BymlNode.DictionaryNode() { Name = "OneShotMissDieParam" },
                        TimeLimitParam = new BymlNode.DictionaryNode() { Name = "TimeLimitParam" };

                    if (ChallengeParam.isCountNumWritten || ChallengeParam.isIsOnlyPlayerBreakWritten
                        || ChallengeParam.isTargetActorNameListWritten || ChallengeParam.isViewUIRemainNumWritten)
                    {
                        if (ChallengeParam.isCountNumWritten) BreakCounterParam.AddNode("CountNum", ChallengeParam.CountNum);
                        if (ChallengeParam.isIsOnlyPlayerBreakWritten) BreakCounterParam.AddNode("IsOnlyPlayerBreak", ChallengeParam.IsOnlyPlayerBreak);

                        if (ChallengeParam.isTargetActorNameListWritten)
                        {
                            BymlNode.ArrayNode TargetActorNameList = new BymlNode.ArrayNode() { Name = "TargetActorNameList" };
                            foreach (string TargetActorName in ChallengeParam.TargetActorNameList)
                            {
                                TargetActorNameList.AddNode($"Work/Actor/{TargetActorName}.engine__actor__ActorParam.gyml");
                            }

                            BreakCounterParam.Nodes.Add(TargetActorNameList);
                        }

                        if (ChallengeParam.isViewUIRemainNumWritten) BreakCounterParam.AddNode("ViewUIRemainNum", ChallengeParam.ViewUIRemainNum);

                        SerializedChallengeParam.Nodes.Add(BreakCounterParam);
                    }

                    if (ChallengeParam.isInkLimitWritten)
                    {
                        InkLimitParam.AddNode("InkLimit", ChallengeParam.InkLimit);
                        SerializedChallengeParam.Nodes.Add(InkLimitParam);
                    }

                    if (ChallengeParam.isClearWaitTimeWritten)
                    {
                        OneShotMissDieParam.AddNode("ClearWaitTime", ChallengeParam.ClearWaitTime);
                        SerializedChallengeParam.Nodes.Add(OneShotMissDieParam);
                    }

                    if (ChallengeParam.isTimeLimitWritten)
                    {
                        TimeLimitParam.AddNode("TimeLimit", ChallengeParam.TimeLimit);
                        SerializedChallengeParam.Nodes.Add(TimeLimitParam);
                    }

                    if (ChallengeParam.Type != "") SerializedChallengeParam.AddNode("Type", ChallengeParam.Type);

                    ChallengeParamArray.Nodes.Add(SerializedChallengeParam);
                }

                Root.Nodes.Add(ChallengeParamArray);
            }

            if (MapInfo.isFirstRewardWritten) Root.AddNode("FirstReward", MapInfo.FirstReward);

            Root.AddNode("MapNameLabel", MapInfo.MapNameLabel);
            Root.AddNode("MapType", MapInfo.MapType);

            if (MapInfo.isOctaSupplyWeaponInfoArrayWritten)
            {
                foreach (MuMapInfo.MuOctaWeaponSupply OctaWeaponSupply in MapInfo.OctaSupplyWeaponInfoArray)
                {
                    BymlNode.DictionaryNode SerializedOctaWeaponSupply = new BymlNode.DictionaryNode(),
                        DolphinMessage = new BymlNode.DictionaryNode() { Name = "DolphinMessage" };

                    if (OctaWeaponSupply.isDolphinMessageWritten)
                    {
                        if (OctaWeaponSupply.DolphinMessage.Devtext != "") DolphinMessage.AddNode("DevText", OctaWeaponSupply.DolphinMessage.Devtext);
                        if (OctaWeaponSupply.DolphinMessage.Label != "") DolphinMessage.AddNode("Label", OctaWeaponSupply.DolphinMessage.Label);

                        SerializedOctaWeaponSupply.Nodes.Add(DolphinMessage);
                    }

                    if (OctaWeaponSupply.isFirstRewardWritten)
                        SerializedOctaWeaponSupply.AddNode("FirstReward", OctaWeaponSupply.FirstReward);

                    if (OctaWeaponSupply.isIsRecommendedWritten)
                        SerializedOctaWeaponSupply.AddNode("IsRecommended", OctaWeaponSupply.IsRecommended);

                    if (OctaWeaponSupply.isSecondRewardWritten)
                        SerializedOctaWeaponSupply.AddNode("SecondReward", OctaWeaponSupply.SecondReward);

                    if (OctaWeaponSupply.isSpecialWeaponWritten)
                    {
                        string Weapon = OctaWeaponSupply.SpecialWeapon == "" ? "" : $"Work/Gyml/{OctaWeaponSupply.SpecialWeapon}.spl__WeaponInfoSpecial.gyml";

                        SerializedOctaWeaponSupply.AddNode("SpecialWeapon", Weapon);
                    }

                    if (OctaWeaponSupply.isSubWeaponWritten)
                    {
                        string Weapon = OctaWeaponSupply.SubWeapon == "" ? "" : $"Work/Gyml/{OctaWeaponSupply.SubWeapon}.spl__WeaponInfoSub.gyml";

                        SerializedOctaWeaponSupply.AddNode("SubWeapon", Weapon);
                    }

                    if (OctaWeaponSupply.isSupplyWeaponTypeWritten)
                    {
                        SerializedOctaWeaponSupply.AddNode("SupplyWeaponType", OctaWeaponSupply.SupplyWeaponType);
                    }

                    if (OctaWeaponSupply.isWeaponMainWritten)
                    {
                        string Weapon = OctaWeaponSupply.WeaponMain == "" ? "" : $"Work/Gyml/{OctaWeaponSupply.WeaponMain}.spl__WeaponInfoMain.gyml";

                        SerializedOctaWeaponSupply.AddNode("WeaponMain", Weapon);
                    }

                    OctaSupplyWeaponInfoArray.Nodes.Add(SerializedOctaWeaponSupply);
                }

                Root.Nodes.Add(OctaSupplyWeaponInfoArray);
            }

            if (MapInfo.isRetryNumWritten) Root.AddNode("RetryNum", MapInfo.RetryNum);
            if (MapInfo.isSecondRewardWritten) Root.AddNode("SecondReward", MapInfo.SecondReward);

            if (MapInfo.isStageDolphinMessageWritten)
            {
                foreach (MuMapInfo.MuDolphinMessage Msg in MapInfo.StageDolphinMessage)
                {
                    BymlNode.DictionaryNode DolphinMessage = new BymlNode.DictionaryNode();

                    if (Msg.Devtext != "") DolphinMessage.AddNode("DevText", Msg.Devtext);
                    if (Msg.Label != "") DolphinMessage.AddNode("Label", Msg.Label);

                    StageDolphinMessage.Nodes.Add(DolphinMessage);
                }

                Root.Nodes.Add(StageDolphinMessage);
            }

            Root.AddNode("TeamColor", $"Work/Gyml/{MapInfo.TeamColorData.__RowId}.game__gfx__parameter__TeamColorDataSet.gyml");
        }

        public void SaveColorDataSetFile()
        {
            if (MapInfo.HasInfoMap)
            {
                bool isEntryExist = false;
                for (int i = 0; i < TeamColorDataSet.ColorDataArray.Count; i++)
                {
                    if (MapInfo.TeamColorData.__RowId == TeamColorDataSet.ColorDataArray[i].__RowId)
                    {
                        isEntryExist = true;
                        TeamColorDataSet.ColorDataArray[i] = MapInfo.TeamColorData.Clone();
                        break;
                    }
                }

                if (!isEntryExist)
                {
                    TeamColorDataSet.ColorDataArray.Add(MapInfo.TeamColorData.Clone());
                }

                TeamColorDataSet.ColorDataArray.OrderBy(x => x.__RowId);
            }

            BYML mSerealizedData = new BYML()
            {
                FileName = TeamColorDataSet.FileName,
                FileVersion = 7,
                IsBigEndian = false,
                Root = new BymlNode.ArrayNode()
            };

            foreach (MuTeamColorDataSet.MuTeamColorData ColorData in TeamColorDataSet.ColorDataArray)
            {
                mSerealizedData.Root.Nodes.Add(SerializeColorData(ColorData));
            }

            // Gets the new HashTable
            List<string> newHashTable = new List<string>();
            GetNewHashTable(mSerealizedData.Root, ref newHashTable);
            newHashTable.Sort(new HashTableOrder());

            // Gets the new StringTable
            List<string> newStringTable = new List<string>();
            GetNewStringTable(mSerealizedData.Root, ref newStringTable);
            newStringTable.Sort(new HashTableOrder());

            mSerealizedData.HashTable = newHashTable;
            mSerealizedData.StringTable = newStringTable;

            NitroFile SaveData = mSerealizedData.Save();

            if (TeamColorDataSet.IsZSTDCompressed)
            {
                ZstdNet.Compressor Cmp = new ZstdNet.Compressor();
                byte[] ret = Cmp.Wrap(SaveData.mFile.ToArray());

                SaveData.mFile = ret.ToList();
            }

            SaveData.Save();
        }

        private BymlNode.DictionaryNode SerializeColorData(MuTeamColorDataSet.MuTeamColorData ColorData)
        {
            BymlNode.DictionaryNode SerializedColorData = new BymlNode.DictionaryNode();

            SerializedColorData.AddNode("AlphaHueOffset", ColorData.AlphaHueOffset);
            SerializedColorData.AddNode("AlphaHueOffsetDetailBright", ColorData.AlphaHueOffsetDetailBright);
            SerializedColorData.AddNode("AlphaHueOffsetDetailDark", ColorData.AlphaHueOffsetDetailDark);
            SerializedColorData.AddNode("AlphaHueOffsetDetailEnable", ColorData.AlphaHueOffsetDetailEnable);
            SerializedColorData.AddNode("AlphaInkRimIntensity", ColorData.AlphaInkRimIntensity);
            SerializedColorData.AddNode("AlphaInkRimIntensityNight", ColorData.AlphaInkRimIntensityNight);
            SerializedColorData.AddNode("AlphaMapInkBrightnessOffset", ColorData.AlphaMapInkBrightnessOffset);
            SerializedColorData.Nodes.Add(SerializeColor("AlphaNightTeamColor", ColorData.AlphaNightTeamColor));
            SerializedColorData.Nodes.Add(SerializeColor("AlphaTeamColor", ColorData.AlphaTeamColor));
            SerializedColorData.Nodes.Add(SerializeColor("AlphaUIColor", ColorData.AlphaUIColor));
            SerializedColorData.Nodes.Add(SerializeColor("AlphaUIEffectColor", ColorData.AlphaUIEffectColor));

            SerializedColorData.AddNode("BravoHueOffset", ColorData.BravoHueOffset);
            SerializedColorData.AddNode("BravoHueOffsetDetailBright", ColorData.BravoHueOffsetDetailBright);
            SerializedColorData.AddNode("BravoHueOffsetDetailDark", ColorData.BravoHueOffsetDetailDark);
            SerializedColorData.AddNode("BravoHueOffsetDetailEnable", ColorData.BravoHueOffsetDetailEnable);
            SerializedColorData.AddNode("BravoInkRimIntensity", ColorData.BravoInkRimIntensity);
            SerializedColorData.AddNode("BravoInkRimIntensityNight", ColorData.BravoInkRimIntensityNight);
            SerializedColorData.AddNode("BravoMapInkBrightnessOffset", ColorData.BravoMapInkBrightnessOffset);
            SerializedColorData.Nodes.Add(SerializeColor("BravoNightTeamColor", ColorData.BravoNightTeamColor));
            SerializedColorData.Nodes.Add(SerializeColor("BravoTeamColor", ColorData.BravoTeamColor));
            SerializedColorData.Nodes.Add(SerializeColor("BravoUIColor", ColorData.BravoUIColor));
            SerializedColorData.Nodes.Add(SerializeColor("BravoUIEffectColor", ColorData.BravoUIEffectColor));

            SerializedColorData.AddNode("CharlieHueOffset", ColorData.CharlieHueOffset);
            SerializedColorData.AddNode("CharlieHueOffsetDetailBright", ColorData.CharlieHueOffsetDetailBright);
            SerializedColorData.AddNode("CharlieHueOffsetDetailDark", ColorData.CharlieHueOffsetDetailDark);
            SerializedColorData.AddNode("CharlieHueOffsetDetailEnable", ColorData.CharlieHueOffsetDetailEnable);
            SerializedColorData.AddNode("CharlieInkRimIntensity", ColorData.CharlieInkRimIntensity);
            SerializedColorData.AddNode("CharlieInkRimIntensityNight", ColorData.CharlieInkRimIntensityNight);
            SerializedColorData.AddNode("CharlieMapInkBrightnessOffset", ColorData.CharlieMapInkBrightnessOffset);
            SerializedColorData.Nodes.Add(SerializeColor("CharlieNightTeamColor", ColorData.CharlieNightTeamColor));
            SerializedColorData.Nodes.Add(SerializeColor("CharlieTeamColor", ColorData.CharlieTeamColor));
            SerializedColorData.Nodes.Add(SerializeColor("CharlieUIColor", ColorData.CharlieUIColor));
            SerializedColorData.Nodes.Add(SerializeColor("CharlieUIEffectColor", ColorData.CharlieUIEffectColor));

            SerializedColorData.AddNode("EnableInkRimIntensity", ColorData.EnableInkRimIntensity);
            SerializedColorData.AddNode("EnableMapInkBrightnessOffset", ColorData.EnableMapInkBrightnessOffset);
            SerializedColorData.AddNode("HueOffsetEnable", ColorData.HueOffsetEnable);
            SerializedColorData.AddNode("IsSetNightTeamColor", ColorData.IsSetNightTeamColor);
            SerializedColorData.AddNode("IsSetUIColor", ColorData.IsSetUIColor);
            SerializedColorData.AddNode("IsSetUIEffectColor", ColorData.IsSetUIEffectColor);

            SerializedColorData.Nodes.Add(SerializeColor("NeutralColor", ColorData.NeutralColor));
            SerializedColorData.AddNode("NeutralHueOffset", ColorData.NeutralHueOffset);
            SerializedColorData.AddNode("NeutralHueOffsetDetailBright", ColorData.NeutralHueOffsetDetailBright);
            SerializedColorData.AddNode("NeutralHueOffsetDetailDark", ColorData.NeutralHueOffsetDetailDark);
            SerializedColorData.AddNode("NeutralHueOffsetDetailEnable", ColorData.NeutralHueOffsetDetailEnable);

            SerializedColorData.AddNode("Tag", ColorData.Tag);
            SerializedColorData.AddNode("__RowId", $"Work/Gyml/{ColorData.__RowId}.game__gfx__parameter__TeamColorDataSet.gyml");

            return SerializedColorData;
        }

        private BymlNode.DictionaryNode SerializeColor(string Name, System.Numerics.Vector4 Color)
        {
            BymlNode.DictionaryNode SerializedColor = new BymlNode.DictionaryNode() { Name = Name };

            SerializedColor.AddNode("A", Color.W);
            SerializedColor.AddNode("B", Color.Z);
            SerializedColor.AddNode("G", Color.Y);
            SerializedColor.AddNode("R", Color.X);

            return SerializedColor;
        }

        private void GetNewHashTable(BymlNode mNode, ref List<string> newHashTable)
        {
            if (mNode.Type == BymlNodeType.DictionaryNode)
            {
                foreach (BymlNode node in ((BymlNode.DictionaryNode)mNode).Nodes)
                {
                    if (node.Type == BymlNodeType.ArrayNode || node.Type == BymlNodeType.DictionaryNode)
                    {
                        GetNewHashTable(node, ref newHashTable);

                        if (newHashTable.Contains(node.Name) == false)
                            newHashTable.Add(node.Name);
                    }
                    else
                    {
                        if (newHashTable.Contains(node.Name) == false)
                            newHashTable.Add(node.Name);
                    }
                }
            }
            else
            {
                foreach (BymlNode node in ((BymlNode.ArrayNode)mNode).Nodes)
                {
                    if (node.Type == BymlNodeType.ArrayNode || node.Type == BymlNodeType.DictionaryNode)
                        GetNewHashTable(node, ref newHashTable);
                }
            }
        }

        private void GetNewStringTable(BymlNode mNode, ref List<string> newStringTable)
        {
            if (mNode.Type == BymlNodeType.DictionaryNode)
            {
                foreach (BymlNode node in ((BymlNode.DictionaryNode)mNode).Nodes)
                {
                    if (node.Type == BymlNodeType.ArrayNode || node.Type == BymlNodeType.DictionaryNode)
                        GetNewStringTable(node, ref newStringTable);
                    else if (node.Type == BymlNodeType.String)
                    {
                        if (newStringTable.Contains(((BymlNode.String)node).Text) == false)
                            newStringTable.Add(((BymlNode.String)node).Text);
                    }
                }
            }
            else
            {
                foreach (BymlNode node in ((BymlNode.ArrayNode)mNode).Nodes)
                {
                    if (node.Type == BymlNodeType.ArrayNode || node.Type == BymlNodeType.DictionaryNode)
                        GetNewStringTable(node, ref newStringTable);
                    else if (node.Type == BymlNodeType.String)
                    {
                        if (newStringTable.Contains(((BymlNode.String)node).Text) == false)
                            newStringTable.Add(((BymlNode.String)node).Text);
                    }
                }
            }
        }





















        private void SaveMapObjList()
        {
            Console.WriteLine("~ Called SaveMapObjList(); ~");
        }


        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        //
        // NONE
        // 





        // ---- Objects ----

        /*/// <summary>
        /// Gets or sets the list of <see cref="Obj"/> instances.
        /// </summary>
        [ByamlMember("Objs")]
        public List<Obj> Objs { get; set; }*/

        /// <summary>
        /// Gets or sets the list of <see cref="MuElement"/> instances.
        /// </summary>
        [ByamlMember("Actors")]
        public List<MuObj> Actors { get; set; }

        public List<MuObj> ActorsFile0 { get; set; }
        public List<MuObj> ActorsFile1 { get; set; }

        //[ByamlMember("AiGroups")]
        //public List<MuElement> AiGroups { get; set; }


        // ---- Rails ----
        [ByamlMember("Rails", Optional = true)]
        public List<MuRail> Rails { get; set; }

        // SDOR File Handling
        public List<MuRail> RailsFile0 { get; set; }
        public List<MuRail> RailsFile1 { get; set; }

        public MuMapInfo MapInfo { get; set; } = new MuMapInfo();

        public MuTeamColorDataSet TeamColorDataSet { get; set; } = new MuTeamColorDataSet();

        public void DeserializeByaml(IDictionary<string, object> dictionary)
        {
            Console.WriteLine("~ Called StageDefinition.DeserializeByaml(); ~");
        }

        public void SerializeByaml(IDictionary<string, object> dictionary)
        {
            Console.WriteLine("~ Called StageDefinition.SerializeByaml(); ~");
        }

        private string UInt32ToHashString(uint Input)
        {
            string ret = "";

            if (Input == 0)
            {
                return "0";
            }

            while (Input > 0)
            {
                switch (Input % 16)
                {
                    case 0:
                        ret += "0";
                        break;

                    case 1:
                        ret += "1";
                        break;

                    case 2:
                        ret += "2";
                        break;

                    case 3:
                        ret += "3";
                        break;

                    case 4:
                        ret += "4";
                        break;

                    case 5:
                        ret += "5";
                        break;

                    case 6:
                        ret += "6";
                        break;

                    case 7:
                        ret += "7";
                        break;

                    case 8:
                        ret += "8";
                        break;

                    case 9:
                        ret += "9";
                        break;

                    case 10:
                        ret += "a";
                        break;

                    case 11:
                        ret += "b";
                        break;

                    case 12:
                        ret += "c";
                        break;

                    case 13:
                        ret += "d";
                        break;

                    case 14:
                        ret += "e";
                        break;

                    case 15:
                        ret += "f";
                        break;
                }

                Input /= 16;
            }

            return ret;
        }

        private string GenAiGroupSuffix()
        {
            Random random = new Random();
            string Suffix = "";

            for (int i = 0; i < 4; i++)
            {
                Suffix += UInt32ToHashString((uint)random.Next(0, 16));
            }

            return Suffix;
        }

        private MuObj GetActorByReference(ulong Ref)
        {
            foreach (MuObj Actor in Actors)
            {
                if (Actor.Hash == Ref) return Actor;
            }

            return null;
        }
    }
}