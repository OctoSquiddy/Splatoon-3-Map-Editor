using ByamlExt.Byaml;
using IONET.Collada.FX.Texturing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using static SampleMapEditor.MuObj;
using Wheatley.io;
using Wheatley.io.BYML;
using SampleMapEditor.Ainb;

namespace SampleMapEditor
{
    [ByamlObject]
    public class MuObj : MuElement, IByamlSerializable, IStageReferencable
    {
        [ByamlMember]
        [BindGUI("Team", new object[4] { "Neutral", "Alpha", "Bravo", "Charlie" }, 
                         new string[4] { "Neutral", "Alpha", "Bravo", "Charlie" }
            ,Category = "OBJECT", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string Team
        {
            get
            {
                return this.TeamCmp.Team;
            }
            set
            {
                this.TeamCmp.Team = value;
            }
        }

        [BindGUI("Ai Group ID", Category = "OBJECT", ColumnIndex = 0, Control = BindControl.Default)]
        public string AIGroupID { get; set; }

        [ByamlObject]
        public class Link
        {
            [ByamlMember]
            public ulong Dst { get; set; }

            [ByamlMember]
            public string Name { get; set; }

            public Link() 
            {
                Name = "";
                Dst = 0;
            }

            public Link(Link other)
            {
                Name = other.Name;
                Dst = other.Dst;
            }

            public Link Clone()
            {
                return new Link(this);
            }
        }

        [ByamlMember]
        public List<Link> Links { get; set; }

        [ByamlObject]
        public class TeamNode
        {
            [ByamlMember]
            [BindGUI("Team", Category = "OBJECT", ColumnIndex = 0, Control = BindControl.Default)]
            public string Team { get; set; }

            public TeamNode() { Team = "Neutral"; }

            public TeamNode(TeamNode other)
            {
                Team = other.Team;
            }

            public TeamNode Clone()
            {
                return new TeamNode(this);
            }
        }

        [ByamlMember("TeamCmp")]
        public TeamNode TeamCmp { get; set; }

        public MuObj() : base()
        {
            TeamCmp = new TeamNode();
            AIGroupID = "";

            if (Links == null) Links = new List<Link>();
        }

        // Copying Contstructor
        public MuObj(MuObj other) : base(other)
        {
            TeamCmp = other.TeamCmp.Clone();

            Links = new List<Link>();
            for (int i = 0; i < other.Links.Count; i++)
            {
                Links.Add(other.Links[i].Clone());
            }

            AIGroupID = other.AIGroupID;
        }

        public override MuObj Clone()
        {
            return new MuObj(this);
        }

        public void Set(MuObj other)
        {
            TeamCmp = other.TeamCmp.Clone();

            Links = new List<Link>();
            for (int i = 0; i < other.Links.Count; i++)
                Links.Add(other.Links[i].Clone());

            AIGroupID = other.AIGroupID;

            Layer = other.Layer;
            Name = other.Name;
            Gyaml = other.Gyaml;
            Bakeable = other.Bakeable;

            Scale = other.Scale;
            Translate = other.Translate;
            RotateDegrees = other.RotateDegrees;

            Hash = other.Hash;
            SRTHash = other.SRTHash;
            InstanceID = other.InstanceID;
        }

        public override void DeserializeByaml(IDictionary<string, object> dictionary)
        {
            base.DeserializeByaml(dictionary);
        }

        public override void SerializeByaml(IDictionary<string, object> dictionary)
        {
            base.SerializeByaml(dictionary);
        }

        public override void DeserializeReferences(StageDefinition stageDefinition)
        {
            base.DeserializeReferences(stageDefinition);
        }

        public override void SerializeReferences(StageDefinition stageDefinition)
        {
            base.SerializeReferences(stageDefinition);
        }

        public virtual void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
        {
            BymlNode.ArrayNode test = new BymlNode.ArrayNode();
            test.AddNode(1);
        }

        public void SaveLinks(BymlNode.DictionaryNode SerializedActor)
        {
            if (Links.Count == 0) return;

            foreach (MuObj.Link Link in this.Links)
            {
                if (Link.Name == "Accessories")
                {
                    BymlNode.DictionaryNode spl__EnemySharkKingBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySharkKingBancParam" };
                    if (SerializedActor["spl__EnemySharkKingBancParam"] != null)
                    {
                        spl__EnemySharkKingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySharkKingBancParam"];
                    }

                    BymlNode.DictionaryNode spl__EnemyUtsuboxBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyUtsuboxBancParam" };
                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] != null)
                    {
                        spl__EnemyUtsuboxBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyUtsuboxBancParam"];
                    }

                    BymlNode.ArrayNode Accessories = new BymlNode.ArrayNode() { Name = "Accessories" };
                    if (spl__EnemySharkKingBancParam["Accessories"] == null)
                    {
                        spl__EnemySharkKingBancParam.Nodes.Add(Accessories);
                    }
                    else
                    {
                        Accessories = (BymlNode.ArrayNode)spl__EnemySharkKingBancParam["Accessories"];
                    }
                    Accessories.AddNode(Link.Dst);

                    BymlNode.ArrayNode mAccessories = new BymlNode.ArrayNode() { Name = "Accessories" };
                    if (spl__EnemyUtsuboxBancParam["Accessories"] == null)
                    {
                        spl__EnemyUtsuboxBancParam.Nodes.Add(mAccessories);
                    }
                    else
                    {
                        mAccessories = (BymlNode.ArrayNode)spl__EnemyUtsuboxBancParam["Accessories"];
                    }
                    mAccessories.AddNode(Link.Dst);

                    if (SerializedActor["spl__EnemySharkKingBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemySharkKingBancParam);
                    }

                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyUtsuboxBancParam);
                    }

                }
                else if (Link.Name == "AreaLinks")
                {
                    BymlNode.DictionaryNode spl__KebaInkCoreBancParam = new BymlNode.DictionaryNode() { Name = "spl__KebaInkCoreBancParam" };
                    if (SerializedActor["spl__KebaInkCoreBancParam"] != null)
                    {
                        spl__KebaInkCoreBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__KebaInkCoreBancParam"];
                    }

                    BymlNode.ArrayNode AreaLinks = new BymlNode.ArrayNode() { Name = "AreaLinks" };
                    if (spl__KebaInkCoreBancParam["AreaLinks"] == null)
                    {
                        spl__KebaInkCoreBancParam.Nodes.Add(AreaLinks);
                    }
                    else
                    {
                        AreaLinks = (BymlNode.ArrayNode)spl__KebaInkCoreBancParam["AreaLinks"];
                    }
                    AreaLinks.AddNode(Link.Dst);

                    if (SerializedActor["spl__KebaInkCoreBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__KebaInkCoreBancParam);
                    }

                }
                else if (Link.Name == "BindLink")
                {
                    BymlNode.DictionaryNode spl__SpawnerForSprinklerGimmickParam = new BymlNode.DictionaryNode() { Name = "spl__SpawnerForSprinklerGimmickParam" };
                    if (SerializedActor["spl__SpawnerForSprinklerGimmickParam"] != null)
                    {
                        spl__SpawnerForSprinklerGimmickParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpawnerForSprinklerGimmickParam"];
                    }

                    if (spl__SpawnerForSprinklerGimmickParam["BindLink"] == null)
                    {
                        spl__SpawnerForSprinklerGimmickParam.AddNode("BindLink", Link.Dst);
                    }

                    if (SerializedActor["spl__SpawnerForSprinklerGimmickParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__SpawnerForSprinklerGimmickParam);
                    }

                }
                else if (Link.Name == "CoreBattleManagers")
                {
                    BymlNode.DictionaryNode spl__SpectacleManagerBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleManagerBancParam" };
                    if (SerializedActor["spl__SpectacleManagerBancParam"] != null)
                    {
                        spl__SpectacleManagerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleManagerBancParam"];
                    }

                    BymlNode.ArrayNode CoreBattleManagers = new BymlNode.ArrayNode() { Name = "CoreBattleManagers" };
                    if (spl__SpectacleManagerBancParam["CoreBattleManagers"] == null)
                    {
                        spl__SpectacleManagerBancParam.Nodes.Add(CoreBattleManagers);
                    }
                    else
                    {
                        CoreBattleManagers = (BymlNode.ArrayNode)spl__SpectacleManagerBancParam["CoreBattleManagers"];
                    }
                    CoreBattleManagers.AddNode(Link.Dst);

                    if (SerializedActor["spl__SpectacleManagerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__SpectacleManagerBancParam);
                    }

                }
                else if (Link.Name == "CorrectPoint")
                {
                    BymlNode.DictionaryNode spl__CoopGoldenIkuraDropCorrectAreaParam = new BymlNode.DictionaryNode() { Name = "spl__CoopGoldenIkuraDropCorrectAreaParam" };
                    if (SerializedActor["spl__CoopGoldenIkuraDropCorrectAreaParam"] != null)
                    {
                        spl__CoopGoldenIkuraDropCorrectAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopGoldenIkuraDropCorrectAreaParam"];
                    }

                    if (spl__CoopGoldenIkuraDropCorrectAreaParam["CorrectPoint"] == null)
                    {
                        spl__CoopGoldenIkuraDropCorrectAreaParam.AddNode("CorrectPoint", Link.Dst);
                    }

                    if (SerializedActor["spl__CoopGoldenIkuraDropCorrectAreaParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CoopGoldenIkuraDropCorrectAreaParam);
                    }

                }
                else if (Link.Name == "CorrectPointArray")
                {
                    BymlNode.DictionaryNode spl__CoopPathCorrectAreaParam = new BymlNode.DictionaryNode() { Name = "spl__CoopPathCorrectAreaParam" };
                    if (SerializedActor["spl__CoopPathCorrectAreaParam"] != null)
                    {
                        spl__CoopPathCorrectAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopPathCorrectAreaParam"];
                    }

                    BymlNode.ArrayNode CorrectPointArray = new BymlNode.ArrayNode() { Name = "CorrectPointArray" };
                    if (spl__CoopPathCorrectAreaParam["CorrectPointArray"] == null)
                    {
                        spl__CoopPathCorrectAreaParam.Nodes.Add(CorrectPointArray);
                    }
                    else
                    {
                        CorrectPointArray = (BymlNode.ArrayNode)spl__CoopPathCorrectAreaParam["CorrectPointArray"];
                    }
                    CorrectPointArray.AddNode(Link.Dst);

                    if (SerializedActor["spl__CoopPathCorrectAreaParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CoopPathCorrectAreaParam);
                    }

                }
                else if (Link.Name == "CrowdMorayHead")
                {
                    BymlNode.DictionaryNode spl__EnemyUtsuboxBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyUtsuboxBancParam" };
                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] != null)
                    {
                        spl__EnemyUtsuboxBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyUtsuboxBancParam"];
                    }

                    if (spl__EnemyUtsuboxBancParam["CrowdMorayHead"] == null)
                    {
                        spl__EnemyUtsuboxBancParam.AddNode("CrowdMorayHead", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyUtsuboxBancParam);
                    }

                }
                else if (Link.Name == "EnemyPaintAreaAirBall")
                {
                    BymlNode.DictionaryNode spl__TutorialDirectorBancParam = new BymlNode.DictionaryNode() { Name = "spl__TutorialDirectorBancParam" };
                    if (SerializedActor["spl__TutorialDirectorBancParam"] != null)
                    {
                        spl__TutorialDirectorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__TutorialDirectorBancParam"];
                    }

                    if (spl__TutorialDirectorBancParam["EnemyPaintAreaAirBall"] == null)
                    {
                        spl__TutorialDirectorBancParam.AddNode("EnemyPaintAreaAirBall", Link.Dst);
                    }

                    if (SerializedActor["spl__TutorialDirectorBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__TutorialDirectorBancParam);
                    }

                }
                else if (Link.Name == "FinalAirball")
                {
                    BymlNode.DictionaryNode spl__TutorialDirectorBancParam = new BymlNode.DictionaryNode() { Name = "spl__TutorialDirectorBancParam" };
                    if (SerializedActor["spl__TutorialDirectorBancParam"] != null)
                    {
                        spl__TutorialDirectorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__TutorialDirectorBancParam"];
                    }

                    if (spl__TutorialDirectorBancParam["FinalAirball"] == null)
                    {
                        spl__TutorialDirectorBancParam.AddNode("FinalAirball", Link.Dst);
                    }

                    if (SerializedActor["spl__TutorialDirectorBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__TutorialDirectorBancParam);
                    }

                }
                else if (Link.Name == "JumpPoints")
                {
                    BymlNode.DictionaryNode spl__CoopSakerocketJumpPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSakerocketJumpPointBancParam" };
                    if (SerializedActor["spl__CoopSakerocketJumpPointBancParam"] != null)
                    {
                        spl__CoopSakerocketJumpPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSakerocketJumpPointBancParam"];
                    }

                    BymlNode.ArrayNode JumpPoints = new BymlNode.ArrayNode() { Name = "JumpPoints" };
                    if (spl__CoopSakerocketJumpPointBancParam["JumpPoints"] == null)
                    {
                        spl__CoopSakerocketJumpPointBancParam.Nodes.Add(JumpPoints);
                    }
                    else
                    {
                        JumpPoints = (BymlNode.ArrayNode)spl__CoopSakerocketJumpPointBancParam["JumpPoints"];
                    }
                    JumpPoints.AddNode(Link.Dst);

                    if (SerializedActor["spl__CoopSakerocketJumpPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CoopSakerocketJumpPointBancParam);
                    }

                }
                else if (Link.Name == "JumpTarget")
                {
                    BymlNode.DictionaryNode spl__EnemySharkKingBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySharkKingBancParam" };
                    if (SerializedActor["spl__EnemySharkKingBancParam"] != null)
                    {
                        spl__EnemySharkKingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySharkKingBancParam"];
                    }

                    BymlNode.DictionaryNode spl__EnemyUtsuboxBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyUtsuboxBancParam" };
                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] != null)
                    {
                        spl__EnemyUtsuboxBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyUtsuboxBancParam"];
                    }

                    if (spl__EnemySharkKingBancParam["JumpTarget"] == null)
                    {
                        spl__EnemySharkKingBancParam.AddNode("JumpTarget", Link.Dst);
                    }

                    if (spl__EnemyUtsuboxBancParam["JumpTarget"] == null)
                    {
                        spl__EnemyUtsuboxBancParam.AddNode("JumpTarget", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemySharkKingBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemySharkKingBancParam);
                    }

                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyUtsuboxBancParam);
                    }

                }
                else if (Link.Name == "JumpTargetCandidates")
                {
                    BymlNode.DictionaryNode spl__RivalAppearPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__RivalAppearPointBancParam" };
                    if (SerializedActor["spl__RivalAppearPointBancParam"] != null)
                    {
                        spl__RivalAppearPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RivalAppearPointBancParam"];
                    }

                    BymlNode.ArrayNode JumpTargetCandidates = new BymlNode.ArrayNode() { Name = "JumpTargetCandidates" };
                    if (spl__RivalAppearPointBancParam["JumpTargetCandidates"] == null)
                    {
                        spl__RivalAppearPointBancParam.Nodes.Add(JumpTargetCandidates);
                    }
                    else
                    {
                        JumpTargetCandidates = (BymlNode.ArrayNode)spl__RivalAppearPointBancParam["JumpTargetCandidates"];
                    }
                    JumpTargetCandidates.AddNode(Link.Dst);

                    if (SerializedActor["spl__RivalAppearPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__RivalAppearPointBancParam);
                    }

                }
                else if (Link.Name == "LastHitPosArea")
                {
                    BymlNode.DictionaryNode spl__LocatorPlayerRejectAreaParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorPlayerRejectAreaParam" };
                    if (SerializedActor["spl__LocatorPlayerRejectAreaParam"] != null)
                    {
                        spl__LocatorPlayerRejectAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorPlayerRejectAreaParam"];
                    }

                    if (spl__LocatorPlayerRejectAreaParam["LastHitPosArea"] == null)
                    {
                        spl__LocatorPlayerRejectAreaParam.AddNode("LastHitPosArea", Link.Dst);
                    }

                    if (SerializedActor["spl__LocatorPlayerRejectAreaParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LocatorPlayerRejectAreaParam);
                    }

                }
                else if (Link.Name == "LinksToActor")
                {
                    BymlNode.DictionaryNode spl__IntervalSpawnerBancParam = new BymlNode.DictionaryNode() { Name = "spl__IntervalSpawnerBancParam" };
                    if (SerializedActor["spl__IntervalSpawnerBancParam"] != null)
                    {
                        spl__IntervalSpawnerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__IntervalSpawnerBancParam"];
                    }

                    BymlNode.ArrayNode LinksToActor = new BymlNode.ArrayNode() { Name = "LinksToActor" };
                    if (spl__IntervalSpawnerBancParam["LinksToActor"] == null)
                    {
                        spl__IntervalSpawnerBancParam.Nodes.Add(LinksToActor);
                    }
                    else
                    {
                        LinksToActor = (BymlNode.ArrayNode)spl__IntervalSpawnerBancParam["LinksToActor"];
                    }
                    LinksToActor.AddNode(Link.Dst);

                    if (SerializedActor["spl__IntervalSpawnerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__IntervalSpawnerBancParam);
                    }

                }
                else if (Link.Name == "LinksToMetaSpawnerSpecialTier1")
                {
                    BymlNode.DictionaryNode spl__RivalAppearSequencerBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__RivalAppearSequencerBancParamSdodr" };
                    if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] != null)
                    {
                        spl__RivalAppearSequencerBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__RivalAppearSequencerBancParamSdodr"];
                    }

                    BymlNode.ArrayNode LinksToMetaSpawnerSpecialTier1 = new BymlNode.ArrayNode() { Name = "LinksToMetaSpawnerSpecialTier1" };
                    if (spl__RivalAppearSequencerBancParamSdodr["LinksToMetaSpawnerSpecialTier1"] == null)
                    {
                        spl__RivalAppearSequencerBancParamSdodr.Nodes.Add(LinksToMetaSpawnerSpecialTier1);
                    }
                    else
                    {
                        LinksToMetaSpawnerSpecialTier1 = (BymlNode.ArrayNode)spl__RivalAppearSequencerBancParamSdodr["LinksToMetaSpawnerSpecialTier1"];
                    }
                    LinksToMetaSpawnerSpecialTier1.AddNode(Link.Dst);

                    if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__RivalAppearSequencerBancParamSdodr);
                    }

                }
                else if (Link.Name == "LinksToMetaSpawnerSpecialTier2")
                {
                    BymlNode.DictionaryNode spl__RivalAppearSequencerBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__RivalAppearSequencerBancParamSdodr" };
                    if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] != null)
                    {
                        spl__RivalAppearSequencerBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__RivalAppearSequencerBancParamSdodr"];
                    }

                    BymlNode.ArrayNode LinksToMetaSpawnerSpecialTier2 = new BymlNode.ArrayNode() { Name = "LinksToMetaSpawnerSpecialTier2" };
                    if (spl__RivalAppearSequencerBancParamSdodr["LinksToMetaSpawnerSpecialTier2"] == null)
                    {
                        spl__RivalAppearSequencerBancParamSdodr.Nodes.Add(LinksToMetaSpawnerSpecialTier2);
                    }
                    else
                    {
                        LinksToMetaSpawnerSpecialTier2 = (BymlNode.ArrayNode)spl__RivalAppearSequencerBancParamSdodr["LinksToMetaSpawnerSpecialTier2"];
                    }
                    LinksToMetaSpawnerSpecialTier2.AddNode(Link.Dst);

                    if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__RivalAppearSequencerBancParamSdodr);
                    }

                }
                else if (Link.Name == "LinksToMetaSpawnerZako")
                {
                    BymlNode.DictionaryNode spl__RivalAppearSequencerBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__RivalAppearSequencerBancParamSdodr" };
                    if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] != null)
                    {
                        spl__RivalAppearSequencerBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__RivalAppearSequencerBancParamSdodr"];
                    }

                    BymlNode.ArrayNode LinksToMetaSpawnerZako = new BymlNode.ArrayNode() { Name = "LinksToMetaSpawnerZako" };
                    if (spl__RivalAppearSequencerBancParamSdodr["LinksToMetaSpawnerZako"] == null)
                    {
                        spl__RivalAppearSequencerBancParamSdodr.Nodes.Add(LinksToMetaSpawnerZako);
                    }
                    else
                    {
                        LinksToMetaSpawnerZako = (BymlNode.ArrayNode)spl__RivalAppearSequencerBancParamSdodr["LinksToMetaSpawnerZako"];
                    }
                    LinksToMetaSpawnerZako.AddNode(Link.Dst);

                    if (SerializedActor["spl__RivalAppearSequencerBancParamSdodr"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__RivalAppearSequencerBancParamSdodr);
                    }

                }
                else if (Link.Name == "LinkToAnotherPipeline")
                {
                    BymlNode.DictionaryNode spl__PipelineBancParam = new BymlNode.DictionaryNode() { Name = "spl__PipelineBancParam" };
                    if (SerializedActor["spl__PipelineBancParam"] != null)
                    {
                        spl__PipelineBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PipelineBancParam"];
                    }

                    if (spl__PipelineBancParam["LinkToAnotherPipeline"] == null)
                    {
                        spl__PipelineBancParam.AddNode("LinkToAnotherPipeline", Link.Dst);
                    }

                    if (SerializedActor["spl__PipelineBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__PipelineBancParam);
                    }

                }
                else if (Link.Name == "LinkToCage")
                {
                    BymlNode.DictionaryNode spl__EnemySharkKingBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySharkKingBancParam" };
                    if (SerializedActor["spl__EnemySharkKingBancParam"] != null)
                    {
                        spl__EnemySharkKingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySharkKingBancParam"];
                    }

                    if (spl__EnemySharkKingBancParam["LinkToCage"] == null)
                    {
                        spl__EnemySharkKingBancParam.AddNode("LinkToCage", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemySharkKingBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemySharkKingBancParam);
                    }

                }
                else if (Link.Name == "LinkToCompass")
                {
                    BymlNode.DictionaryNode spl__SpectacleCoreBattleManagerBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleCoreBattleManagerBancParam" };
                    if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] != null)
                    {
                        spl__SpectacleCoreBattleManagerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleCoreBattleManagerBancParam"];
                    }

                    if (spl__SpectacleCoreBattleManagerBancParam["LinkToCompass"] == null)
                    {
                        spl__SpectacleCoreBattleManagerBancParam.AddNode("LinkToCompass", Link.Dst);
                    }

                    if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__SpectacleCoreBattleManagerBancParam);
                    }

                }
                else if (Link.Name == "LinkToEnemyGenerators")
                {
                    BymlNode.DictionaryNode spl__SpectacleCoreBattleManagerBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleCoreBattleManagerBancParam" };
                    if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] != null)
                    {
                        spl__SpectacleCoreBattleManagerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleCoreBattleManagerBancParam"];
                    }

                    BymlNode.ArrayNode LinkToEnemyGenerators = new BymlNode.ArrayNode() { Name = "LinkToEnemyGenerators" };
                    if (spl__SpectacleCoreBattleManagerBancParam["LinkToEnemyGenerators"] == null)
                    {
                        spl__SpectacleCoreBattleManagerBancParam.Nodes.Add(LinkToEnemyGenerators);
                    }
                    else
                    {
                        LinkToEnemyGenerators = (BymlNode.ArrayNode)spl__SpectacleCoreBattleManagerBancParam["LinkToEnemyGenerators"];
                    }
                    LinkToEnemyGenerators.AddNode(Link.Dst);

                    if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__SpectacleCoreBattleManagerBancParam);
                    }

                }
                else if (Link.Name == "LinkToField")
                {
                    BymlNode.DictionaryNode spl__EnemyTowerKingBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__EnemyTowerKingBancParamSdodr" };
                    if (SerializedActor["spl__EnemyTowerKingBancParamSdodr"] != null)
                    {
                        spl__EnemyTowerKingBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyTowerKingBancParamSdodr"];
                    }

                    if (spl__EnemyTowerKingBancParamSdodr["LinkToField"] == null)
                    {
                        spl__EnemyTowerKingBancParamSdodr.AddNode("LinkToField", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemyTowerKingBancParamSdodr"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyTowerKingBancParamSdodr);
                    }

                }
                else if (Link.Name == "LinkToLocator")
                {
                    BymlNode.DictionaryNode spl__EnemySharkKingBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySharkKingBancParam" };
                    if (SerializedActor["spl__EnemySharkKingBancParam"] != null)
                    {
                        spl__EnemySharkKingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySharkKingBancParam"];
                    }

                    BymlNode.DictionaryNode spl__PeriscopeBancParam = new BymlNode.DictionaryNode() { Name = "spl__PeriscopeBancParam" };
                    if (SerializedActor["spl__PeriscopeBancParam"] != null)
                    {
                        spl__PeriscopeBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PeriscopeBancParam"];
                    }

                    if (spl__EnemySharkKingBancParam["LinkToLocator"] == null)
                    {
                        spl__EnemySharkKingBancParam.AddNode("LinkToLocator", Link.Dst);
                    }

                    if (spl__PeriscopeBancParam["LinkToLocator"] == null)
                    {
                        spl__PeriscopeBancParam.AddNode("LinkToLocator", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemySharkKingBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemySharkKingBancParam);
                    }

                    if (SerializedActor["spl__PeriscopeBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__PeriscopeBancParam);
                    }

                }
                else if (Link.Name == "LinkToMoveArea")
                {
                    BymlNode.DictionaryNode spl__SpectacleCoreBattleManagerBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpectacleCoreBattleManagerBancParam" };
                    if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] != null)
                    {
                        spl__SpectacleCoreBattleManagerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpectacleCoreBattleManagerBancParam"];
                    }

                    if (spl__SpectacleCoreBattleManagerBancParam["LinkToMoveArea"] == null)
                    {
                        spl__SpectacleCoreBattleManagerBancParam.AddNode("LinkToMoveArea", Link.Dst);
                    }

                    if (SerializedActor["spl__SpectacleCoreBattleManagerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__SpectacleCoreBattleManagerBancParam);
                    }

                }
                else if (Link.Name == "LinkToTarget")
                {
                    BymlNode.DictionaryNode spl__ActorPaintCheckerBancParam = new BymlNode.DictionaryNode() { Name = "spl__ActorPaintCheckerBancParam" };
                    if (SerializedActor["spl__ActorPaintCheckerBancParam"] != null)
                    {
                        spl__ActorPaintCheckerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ActorPaintCheckerBancParam"];
                    }

                    if (spl__ActorPaintCheckerBancParam["LinkToTarget"] == null)
                    {
                        spl__ActorPaintCheckerBancParam.AddNode("LinkToTarget", Link.Dst);
                    }

                    if (SerializedActor["spl__ActorPaintCheckerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ActorPaintCheckerBancParam);
                    }

                }
                else if (Link.Name == "LinkToWater")
                {
                    BymlNode.DictionaryNode spl__EnemySharkKingBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySharkKingBancParam" };
                    if (SerializedActor["spl__EnemySharkKingBancParam"] != null)
                    {
                        spl__EnemySharkKingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySharkKingBancParam"];
                    }

                    if (spl__EnemySharkKingBancParam["LinkToWater"] == null)
                    {
                        spl__EnemySharkKingBancParam.AddNode("LinkToWater", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemySharkKingBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemySharkKingBancParam);
                    }

                }
                else if (Link.Name == "LocatorLink")
                {
                    BymlNode.DictionaryNode spl__CoopGoldenIkuraBoxDropPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopGoldenIkuraBoxDropPointBancParam" };
                    if (SerializedActor["spl__CoopGoldenIkuraBoxDropPointBancParam"] != null)
                    {
                        spl__CoopGoldenIkuraBoxDropPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopGoldenIkuraBoxDropPointBancParam"];
                    }

                    BymlNode.DictionaryNode spl__CoopSakePillarSpawnPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSakePillarSpawnPointBancParam" };
                    if (SerializedActor["spl__CoopSakePillarSpawnPointBancParam"] != null)
                    {
                        spl__CoopSakePillarSpawnPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSakePillarSpawnPointBancParam"];
                    }

                    BymlNode.ArrayNode LocatorLink = new BymlNode.ArrayNode() { Name = "LocatorLink" };
                    if (spl__CoopGoldenIkuraBoxDropPointBancParam["LocatorLink"] == null)
                    {
                        spl__CoopGoldenIkuraBoxDropPointBancParam.Nodes.Add(LocatorLink);
                    }
                    else
                    {
                        LocatorLink = (BymlNode.ArrayNode)spl__CoopGoldenIkuraBoxDropPointBancParam["LocatorLink"];
                    }
                    LocatorLink.AddNode(Link.Dst);

                    BymlNode.ArrayNode mLocatorLink = new BymlNode.ArrayNode() { Name = "LocatorLink" };
                    if (spl__CoopSakePillarSpawnPointBancParam["LocatorLink"] == null)
                    {
                        spl__CoopSakePillarSpawnPointBancParam.Nodes.Add(mLocatorLink);
                    }
                    else
                    {
                        mLocatorLink = (BymlNode.ArrayNode)spl__CoopSakePillarSpawnPointBancParam["LocatorLink"];
                    }
                    mLocatorLink.AddNode(Link.Dst);

                    if (SerializedActor["spl__CoopGoldenIkuraBoxDropPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CoopGoldenIkuraBoxDropPointBancParam);
                    }

                    if (SerializedActor["spl__CoopSakePillarSpawnPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CoopSakePillarSpawnPointBancParam);
                    }

                }
                else if (Link.Name == "NextAirBall")
                {
                    BymlNode.DictionaryNode spl__NavigateAirBallBancParam = new BymlNode.DictionaryNode() { Name = "spl__NavigateAirBallBancParam" };
                    if (SerializedActor["spl__NavigateAirBallBancParam"] != null)
                    {
                        spl__NavigateAirBallBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__NavigateAirBallBancParam"];
                    }

                    BymlNode.ArrayNode NextAirBall = new BymlNode.ArrayNode() { Name = "NextAirBall" };
                    if (spl__NavigateAirBallBancParam["NextAirBall"] == null)
                    {
                        spl__NavigateAirBallBancParam.Nodes.Add(NextAirBall);
                    }
                    else
                    {
                        NextAirBall = (BymlNode.ArrayNode)spl__NavigateAirBallBancParam["NextAirBall"];
                    }
                    NextAirBall.AddNode(Link.Dst);

                    if (SerializedActor["spl__NavigateAirBallBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__NavigateAirBallBancParam);
                    }

                }
                else if (Link.Name == "PropellerLink")
                {
                    BymlNode.DictionaryNode spl__GachihokoCheckPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__GachihokoCheckPointBancParam" };
                    if (SerializedActor["spl__GachihokoCheckPointBancParam"] != null)
                    {
                        spl__GachihokoCheckPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GachihokoCheckPointBancParam"];
                    }

                    BymlNode.ArrayNode PropellerLink = new BymlNode.ArrayNode() { Name = "PropellerLink" };
                    if (spl__GachihokoCheckPointBancParam["PropellerLink"] == null)
                    {
                        spl__GachihokoCheckPointBancParam.Nodes.Add(PropellerLink);
                    }
                    else
                    {
                        PropellerLink = (BymlNode.ArrayNode)spl__GachihokoCheckPointBancParam["PropellerLink"];
                    }
                    PropellerLink.AddNode(Link.Dst);

                    if (SerializedActor["spl__GachihokoCheckPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__GachihokoCheckPointBancParam);
                    }

                }
                else if (Link.Name == "Reference")
                {
                }
                else if (Link.Name == "RestartPos")
                {
                    BymlNode.DictionaryNode spl__RestartPosUpdateAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__RestartPosUpdateAreaBancParam" };
                    if (SerializedActor["spl__RestartPosUpdateAreaBancParam"] != null)
                    {
                        spl__RestartPosUpdateAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RestartPosUpdateAreaBancParam"];
                    }

                    if (spl__RestartPosUpdateAreaBancParam["RestartPos"] == null)
                    {
                        spl__RestartPosUpdateAreaBancParam.AddNode("RestartPos", Link.Dst);
                    }

                    if (SerializedActor["spl__RestartPosUpdateAreaBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__RestartPosUpdateAreaBancParam);
                    }

                }
                else if (Link.Name == "SafePosLinks")
                {
                    BymlNode.DictionaryNode spl__SpongeBancParam = new BymlNode.DictionaryNode() { Name = "spl__SpongeBancParam" };
                    if (SerializedActor["spl__SpongeBancParam"] != null)
                    {
                        spl__SpongeBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__SpongeBancParam"];
                    }

                    BymlNode.ArrayNode SafePosLinks = new BymlNode.ArrayNode() { Name = "SafePosLinks" };
                    if (spl__SpongeBancParam["SafePosLinks"] == null)
                    {
                        spl__SpongeBancParam.Nodes.Add(SafePosLinks);
                    }
                    else
                    {
                        SafePosLinks = (BymlNode.ArrayNode)spl__SpongeBancParam["SafePosLinks"];
                    }
                    SafePosLinks.AddNode(Link.Dst);

                    if (SerializedActor["spl__SpongeBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__SpongeBancParam);
                    }

                }
                else if (Link.Name == "ShortcutAirBall")
                {
                    BymlNode.DictionaryNode spl__TutorialDirectorBancParam = new BymlNode.DictionaryNode() { Name = "spl__TutorialDirectorBancParam" };
                    if (SerializedActor["spl__TutorialDirectorBancParam"] != null)
                    {
                        spl__TutorialDirectorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__TutorialDirectorBancParam"];
                    }

                    if (spl__TutorialDirectorBancParam["ShortcutAirBall"] == null)
                    {
                        spl__TutorialDirectorBancParam.AddNode("ShortcutAirBall", Link.Dst);
                    }

                    if (SerializedActor["spl__TutorialDirectorBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__TutorialDirectorBancParam);
                    }

                }
                else if (Link.Name == "SpawnObjLinks")
                {
                    BymlNode.DictionaryNode spl__KebaInkCoreBancParam = new BymlNode.DictionaryNode() { Name = "spl__KebaInkCoreBancParam" };
                    if (SerializedActor["spl__KebaInkCoreBancParam"] != null)
                    {
                        spl__KebaInkCoreBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__KebaInkCoreBancParam"];
                    }

                    BymlNode.ArrayNode SpawnObjLinks = new BymlNode.ArrayNode() { Name = "SpawnObjLinks" };
                    if (spl__KebaInkCoreBancParam["SpawnObjLinks"] == null)
                    {
                        spl__KebaInkCoreBancParam.Nodes.Add(SpawnObjLinks);
                    }
                    else
                    {
                        SpawnObjLinks = (BymlNode.ArrayNode)spl__KebaInkCoreBancParam["SpawnObjLinks"];
                    }
                    SpawnObjLinks.AddNode(Link.Dst);

                    if (SerializedActor["spl__KebaInkCoreBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__KebaInkCoreBancParam);
                    }

                }
                else if (Link.Name == "SubAreaInstanceIds")
                {
                    BymlNode.DictionaryNode spl__PaintTargetAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__PaintTargetAreaBancParam" };
                    if (SerializedActor["spl__PaintTargetAreaBancParam"] != null)
                    {
                        spl__PaintTargetAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PaintTargetAreaBancParam"];
                    }

                    BymlNode.ArrayNode SubAreaInstanceIds = new BymlNode.ArrayNode() { Name = "SubAreaInstanceIds" };
                    if (spl__PaintTargetAreaBancParam["SubAreaInstanceIds"] == null)
                    {
                        spl__PaintTargetAreaBancParam.Nodes.Add(SubAreaInstanceIds);
                    }
                    else
                    {
                        SubAreaInstanceIds = (BymlNode.ArrayNode)spl__PaintTargetAreaBancParam["SubAreaInstanceIds"];
                    }
                    SubAreaInstanceIds.AddNode(Link.Dst);

                    if (SerializedActor["spl__PaintTargetAreaBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__PaintTargetAreaBancParam);
                    }

                }
                else if (Link.Name == "Target")
                {
                    BymlNode.DictionaryNode spl__CompassBancParam = new BymlNode.DictionaryNode() { Name = "spl__CompassBancParam" };
                    if (SerializedActor["spl__CompassBancParam"] != null)
                    {
                        spl__CompassBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CompassBancParam"];
                    }

                    if (spl__CompassBancParam["Target"] == null)
                    {
                        spl__CompassBancParam.AddNode("Target", Link.Dst);
                    }

                    if (SerializedActor["spl__CompassBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CompassBancParam);
                    }

                }
                else if (Link.Name == "TargetArea")
                {
                    BymlNode.DictionaryNode spl__CompassBancParam = new BymlNode.DictionaryNode() { Name = "spl__CompassBancParam" };
                    if (SerializedActor["spl__CompassBancParam"] != null)
                    {
                        spl__CompassBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CompassBancParam"];
                    }

                    BymlNode.DictionaryNode spl__EnemyCleanerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyCleanerBancParam" };
                    if (SerializedActor["spl__EnemyCleanerBancParam"] != null)
                    {
                        spl__EnemyCleanerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyCleanerBancParam"];
                    }

                    BymlNode.ArrayNode TargetArea = new BymlNode.ArrayNode() { Name = "TargetArea" };
                    if (spl__CompassBancParam["TargetArea"] == null)
                    {
                        spl__CompassBancParam.Nodes.Add(TargetArea);
                    }
                    else
                    {
                        TargetArea = (BymlNode.ArrayNode)spl__CompassBancParam["TargetArea"];
                    }
                    TargetArea.AddNode(Link.Dst);

                    BymlNode.ArrayNode mTargetArea = new BymlNode.ArrayNode() { Name = "TargetArea" };
                    if (spl__EnemyCleanerBancParam["TargetArea"] == null)
                    {
                        spl__EnemyCleanerBancParam.Nodes.Add(mTargetArea);
                    }
                    else
                    {
                        mTargetArea = (BymlNode.ArrayNode)spl__EnemyCleanerBancParam["TargetArea"];
                    }
                    mTargetArea.AddNode(Link.Dst);

                    if (SerializedActor["spl__CompassBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CompassBancParam);
                    }

                    if (SerializedActor["spl__EnemyCleanerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyCleanerBancParam);
                    }

                }
                else if (Link.Name == "TargetLift")
                {
                    BymlNode.DictionaryNode spl__CoopSakeFlyBagManArrivalPointForLiftParam = new BymlNode.DictionaryNode() { Name = "spl__CoopSakeFlyBagManArrivalPointForLiftParam" };
                    if (SerializedActor["spl__CoopSakeFlyBagManArrivalPointForLiftParam"] != null)
                    {
                        spl__CoopSakeFlyBagManArrivalPointForLiftParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopSakeFlyBagManArrivalPointForLiftParam"];
                    }

                    BymlNode.DictionaryNode spl__LiftDecorationBancParam = new BymlNode.DictionaryNode() { Name = "spl__LiftDecorationBancParam" };
                    if (SerializedActor["spl__LiftDecorationBancParam"] != null)
                    {
                        spl__LiftDecorationBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LiftDecorationBancParam"];
                    }

                    if (spl__CoopSakeFlyBagManArrivalPointForLiftParam["TargetLift"] == null)
                    {
                        spl__CoopSakeFlyBagManArrivalPointForLiftParam.AddNode("TargetLift", Link.Dst);
                    }

                    if (spl__LiftDecorationBancParam["TargetLift"] == null)
                    {
                        spl__LiftDecorationBancParam.AddNode("TargetLift", Link.Dst);
                    }

                    if (SerializedActor["spl__CoopSakeFlyBagManArrivalPointForLiftParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CoopSakeFlyBagManArrivalPointForLiftParam);
                    }

                    if (SerializedActor["spl__LiftDecorationBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LiftDecorationBancParam);
                    }

                }
                else if (Link.Name == "TargetPoints")
                {
                    BymlNode.DictionaryNode spl__EnemyBombBlowSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBombBlowSdodrBancParam" };
                    if (SerializedActor["spl__EnemyBombBlowSdodrBancParam"] != null)
                    {
                        spl__EnemyBombBlowSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBombBlowSdodrBancParam"];
                    }

                    BymlNode.DictionaryNode spl__EnemyChargerTowerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyChargerTowerSdodrBancParam" };
                    if (SerializedActor["spl__EnemyChargerTowerSdodrBancParam"] != null)
                    {
                        spl__EnemyChargerTowerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyChargerTowerSdodrBancParam"];
                    }

                    BymlNode.DictionaryNode spl__EnemySprinklerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemySprinklerSdodrBancParam" };
                    if (SerializedActor["spl__EnemySprinklerSdodrBancParam"] != null)
                    {
                        spl__EnemySprinklerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemySprinklerSdodrBancParam"];
                    }

                    BymlNode.DictionaryNode spl__EnemyTreeSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyTreeSdodrBancParam" };
                    if (SerializedActor["spl__EnemyTreeSdodrBancParam"] != null)
                    {
                        spl__EnemyTreeSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyTreeSdodrBancParam"];
                    }

                    BymlNode.ArrayNode TargetPoints = new BymlNode.ArrayNode() { Name = "TargetPoints" };
                    if (spl__EnemyBombBlowSdodrBancParam["TargetPoints"] == null)
                    {
                        spl__EnemyBombBlowSdodrBancParam.Nodes.Add(TargetPoints);
                    }
                    else
                    {
                        TargetPoints = (BymlNode.ArrayNode)spl__EnemyBombBlowSdodrBancParam["TargetPoints"];
                    }
                    TargetPoints.AddNode(Link.Dst);

                    BymlNode.ArrayNode mTargetPoints = new BymlNode.ArrayNode() { Name = "TargetPoints" };
                    if (spl__EnemyChargerTowerSdodrBancParam["TargetPoints"] == null)
                    {
                        spl__EnemyChargerTowerSdodrBancParam.Nodes.Add(mTargetPoints);
                    }
                    else
                    {
                        mTargetPoints = (BymlNode.ArrayNode)spl__EnemyChargerTowerSdodrBancParam["TargetPoints"];
                    }
                    mTargetPoints.AddNode(Link.Dst);

                    BymlNode.ArrayNode nTargetPoints = new BymlNode.ArrayNode() { Name = "TargetPoints" };
                    if (spl__EnemySprinklerSdodrBancParam["TargetPoints"] == null)
                    {
                        spl__EnemySprinklerSdodrBancParam.Nodes.Add(nTargetPoints);
                    }
                    else
                    {
                        nTargetPoints = (BymlNode.ArrayNode)spl__EnemySprinklerSdodrBancParam["TargetPoints"];
                    }
                    nTargetPoints.AddNode(Link.Dst);

                    BymlNode.ArrayNode oTargetPoints = new BymlNode.ArrayNode() { Name = "TargetPoints" };
                    if (spl__EnemyTreeSdodrBancParam["TargetPoints"] == null)
                    {
                        spl__EnemyTreeSdodrBancParam.Nodes.Add(oTargetPoints);
                    }
                    else
                    {
                        oTargetPoints = (BymlNode.ArrayNode)spl__EnemyTreeSdodrBancParam["TargetPoints"];
                    }
                    oTargetPoints.AddNode(Link.Dst);

                    if (SerializedActor["spl__EnemyBombBlowSdodrBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyBombBlowSdodrBancParam);
                    }

                    if (SerializedActor["spl__EnemyChargerTowerSdodrBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyChargerTowerSdodrBancParam);
                    }

                    if (SerializedActor["spl__EnemySprinklerSdodrBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemySprinklerSdodrBancParam);
                    }

                    if (SerializedActor["spl__EnemyTreeSdodrBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyTreeSdodrBancParam);
                    }

                }
                else if (Link.Name == "TargetPropeller")
                {
                    BymlNode.DictionaryNode spl__PropellerOnlineDecorationBancParam = new BymlNode.DictionaryNode() { Name = "spl__PropellerOnlineDecorationBancParam" };
                    if (SerializedActor["spl__PropellerOnlineDecorationBancParam"] != null)
                    {
                        spl__PropellerOnlineDecorationBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PropellerOnlineDecorationBancParam"];
                    }

                    if (spl__PropellerOnlineDecorationBancParam["TargetPropeller"] == null)
                    {
                        spl__PropellerOnlineDecorationBancParam.AddNode("TargetPropeller", Link.Dst);
                    }

                    if (SerializedActor["spl__PropellerOnlineDecorationBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__PropellerOnlineDecorationBancParam);
                    }

                }
                else if (Link.Name == "ToActor")
                {
                    BymlNode.DictionaryNode spl__OneShotMissDieTagBancParam = new BymlNode.DictionaryNode() { Name = "spl__OneShotMissDieTagBancParam" };
                    if (SerializedActor["spl__OneShotMissDieTagBancParam"] != null)
                    {
                        spl__OneShotMissDieTagBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__OneShotMissDieTagBancParam"];
                    }

                    BymlNode.ArrayNode ToActor = new BymlNode.ArrayNode() { Name = "ToActor" };
                    if (spl__OneShotMissDieTagBancParam["ToActor"] == null)
                    {
                        spl__OneShotMissDieTagBancParam.Nodes.Add(ToActor);
                    }
                    else
                    {
                        ToActor = (BymlNode.ArrayNode)spl__OneShotMissDieTagBancParam["ToActor"];
                    }
                    ToActor.AddNode(Link.Dst);

                    if (SerializedActor["spl__OneShotMissDieTagBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__OneShotMissDieTagBancParam);
                    }

                }
                else if (Link.Name == "ToActors")
                {
                    BymlNode.DictionaryNode spl__LinkTargetReservationBancParam = new BymlNode.DictionaryNode() { Name = "spl__LinkTargetReservationBancParam" };
                    if (SerializedActor["spl__LinkTargetReservationBancParam"] != null)
                    {
                        spl__LinkTargetReservationBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LinkTargetReservationBancParam"];
                    }

                    BymlNode.ArrayNode ToActors = new BymlNode.ArrayNode() { Name = "ToActors" };
                    if (spl__LinkTargetReservationBancParam["ToActors"] == null)
                    {
                        spl__LinkTargetReservationBancParam.Nodes.Add(ToActors);
                    }
                    else
                    {
                        ToActors = (BymlNode.ArrayNode)spl__LinkTargetReservationBancParam["ToActors"];
                    }
                    ToActors.AddNode(Link.Dst);

                    if (SerializedActor["spl__LinkTargetReservationBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LinkTargetReservationBancParam);
                    }

                }
                else if (Link.Name == "ToArea")
                {
                    BymlNode.DictionaryNode spl__MissionCheckPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__MissionCheckPointBancParam" };
                    if (SerializedActor["spl__MissionCheckPointBancParam"] != null)
                    {
                        spl__MissionCheckPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MissionCheckPointBancParam"];
                    }

                    BymlNode.ArrayNode ToArea = new BymlNode.ArrayNode() { Name = "ToArea" };
                    if (spl__MissionCheckPointBancParam["ToArea"] == null)
                    {
                        spl__MissionCheckPointBancParam.Nodes.Add(ToArea);
                    }
                    else
                    {
                        ToArea = (BymlNode.ArrayNode)spl__MissionCheckPointBancParam["ToArea"];
                    }
                    ToArea.AddNode(Link.Dst);

                    if (SerializedActor["spl__MissionCheckPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__MissionCheckPointBancParam);
                    }

                }
                else if (Link.Name == "ToBindActor")
                {
                    BymlNode.DictionaryNode spl__NpcIdolDanceBancParam = new BymlNode.DictionaryNode() { Name = "spl__NpcIdolDanceBancParam" };
                    if (SerializedActor["spl__NpcIdolDanceBancParam"] != null)
                    {
                        spl__NpcIdolDanceBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__NpcIdolDanceBancParam"];
                    }

                    if (spl__NpcIdolDanceBancParam["ToBindActor"] == null)
                    {
                        spl__NpcIdolDanceBancParam.AddNode("ToBindActor", Link.Dst);
                    }

                    if (SerializedActor["spl__NpcIdolDanceBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__NpcIdolDanceBancParam);
                    }

                }
                else if (Link.Name == "ToBindObjLink")
                {
                    BymlNode.DictionaryNode spl__ActorMatrixBindableHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__ActorMatrixBindableHelperBancParam" };
                    if (SerializedActor["spl__ActorMatrixBindableHelperBancParam"] != null)
                    {
                        spl__ActorMatrixBindableHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ActorMatrixBindableHelperBancParam"];
                    }

                    BymlNode.DictionaryNode spl__ConstraintBindableHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__ConstraintBindableHelperBancParam" };
                    if (SerializedActor["spl__ConstraintBindableHelperBancParam"] != null)
                    {
                        spl__ConstraintBindableHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ConstraintBindableHelperBancParam"];
                    }

                    if (spl__ActorMatrixBindableHelperBancParam["ToBindObjLink"] == null)
                    {
                        spl__ActorMatrixBindableHelperBancParam.AddNode("ToBindObjLink", Link.Dst);
                    }

                    if (spl__ConstraintBindableHelperBancParam["ToBindObjLink"] == null)
                    {
                        spl__ConstraintBindableHelperBancParam.AddNode("ToBindObjLink", Link.Dst);
                    }

                    if (SerializedActor["spl__ActorMatrixBindableHelperBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ActorMatrixBindableHelperBancParam);
                    }

                    if (SerializedActor["spl__ConstraintBindableHelperBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ConstraintBindableHelperBancParam);
                    }

                }
                else if (Link.Name == "ToBuildItem")
                {
                    BymlNode.DictionaryNode spl__CanBuildMachineBancParam = new BymlNode.DictionaryNode() { Name = "spl__CanBuildMachineBancParam" };
                    if (SerializedActor["spl__CanBuildMachineBancParam"] != null)
                    {
                        spl__CanBuildMachineBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CanBuildMachineBancParam"];
                    }

                    if (spl__CanBuildMachineBancParam["ToBuildItem"] == null)
                    {
                        spl__CanBuildMachineBancParam.AddNode("ToBuildItem", Link.Dst);
                    }

                    if (SerializedActor["spl__CanBuildMachineBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__CanBuildMachineBancParam);
                    }

                }
                else if (Link.Name == "ToDropItem")
                {
                    BymlNode.DictionaryNode spl__ItemDropBancParam = new BymlNode.DictionaryNode() { Name = "spl__ItemDropBancParam" };
                    if (SerializedActor["spl__ItemDropBancParam"] != null)
                    {
                        spl__ItemDropBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ItemDropBancParam"];
                    }

                    if (spl__ItemDropBancParam["ToDropItem"] == null)
                    {
                        spl__ItemDropBancParam.AddNode("ToDropItem", Link.Dst);
                    }

                    if (SerializedActor["spl__ItemDropBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ItemDropBancParam);
                    }

                }
                else if (Link.Name == "ToFriendLink")
                {
                    BymlNode.DictionaryNode spl__EnemyTakopterBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyTakopterBancParam" };
                    if (SerializedActor["spl__EnemyTakopterBancParam"] != null)
                    {
                        spl__EnemyTakopterBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyTakopterBancParam"];
                    }

                    BymlNode.ArrayNode ToFriendLink = new BymlNode.ArrayNode() { Name = "ToFriendLink" };
                    if (spl__EnemyTakopterBancParam["ToFriendLink"] == null)
                    {
                        spl__EnemyTakopterBancParam.Nodes.Add(ToFriendLink);
                    }
                    else
                    {
                        ToFriendLink = (BymlNode.ArrayNode)spl__EnemyTakopterBancParam["ToFriendLink"];
                    }
                    ToFriendLink.AddNode(Link.Dst);

                    if (SerializedActor["spl__EnemyTakopterBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyTakopterBancParam);
                    }

                }
                else if (Link.Name == "ToGateway")
                {
                    BymlNode.DictionaryNode spl__LocatorNpcWorldBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorNpcWorldBancParam" };
                    if (SerializedActor["spl__LocatorNpcWorldBancParam"] != null)
                    {
                        spl__LocatorNpcWorldBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorNpcWorldBancParam"];
                    }

                    BymlNode.ArrayNode ToGateway = new BymlNode.ArrayNode() { Name = "ToGateway" };
                    if (spl__LocatorNpcWorldBancParam["ToGateway"] == null)
                    {
                        spl__LocatorNpcWorldBancParam.Nodes.Add(ToGateway);
                    }
                    else
                    {
                        ToGateway = (BymlNode.ArrayNode)spl__LocatorNpcWorldBancParam["ToGateway"];
                    }
                    ToGateway.AddNode(Link.Dst);

                    if (SerializedActor["spl__LocatorNpcWorldBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LocatorNpcWorldBancParam);
                    }

                }
                else if (Link.Name == "ToGeneralLocator")
                {
                    BymlNode.DictionaryNode spl__AutoWarpObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__AutoWarpObjBancParam" };
                    if (SerializedActor["spl__AutoWarpObjBancParam"] != null)
                    {
                        spl__AutoWarpObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AutoWarpObjBancParam"];
                    }

                    BymlNode.DictionaryNode spl__AutoWarpPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__AutoWarpPointBancParam" };
                    if (SerializedActor["spl__AutoWarpPointBancParam"] != null)
                    {
                        spl__AutoWarpPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AutoWarpPointBancParam"];
                    }

                    BymlNode.DictionaryNode spl__MissionSalmonBuddyLeadPlayerAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__MissionSalmonBuddyLeadPlayerAreaBancParam" };
                    if (SerializedActor["spl__MissionSalmonBuddyLeadPlayerAreaBancParam"] != null)
                    {
                        spl__MissionSalmonBuddyLeadPlayerAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MissionSalmonBuddyLeadPlayerAreaBancParam"];
                    }

                    if (spl__AutoWarpObjBancParam["ToGeneralLocator"] == null)
                    {
                        spl__AutoWarpObjBancParam.AddNode("ToGeneralLocator", Link.Dst);
                    }

                    if (spl__AutoWarpPointBancParam["ToGeneralLocator"] == null)
                    {
                        spl__AutoWarpPointBancParam.AddNode("ToGeneralLocator", Link.Dst);
                    }

                    if (spl__MissionSalmonBuddyLeadPlayerAreaBancParam["ToGeneralLocator"] == null)
                    {
                        spl__MissionSalmonBuddyLeadPlayerAreaBancParam.AddNode("ToGeneralLocator", Link.Dst);
                    }

                    if (SerializedActor["spl__AutoWarpObjBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__AutoWarpObjBancParam);
                    }

                    if (SerializedActor["spl__AutoWarpPointBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__AutoWarpPointBancParam);
                    }

                    if (SerializedActor["spl__MissionSalmonBuddyLeadPlayerAreaBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__MissionSalmonBuddyLeadPlayerAreaBancParam);
                    }

                }
                else if (Link.Name == "ToLevel1Devices")
                {
                    BymlNode.DictionaryNode spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam" };
                    if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] != null)
                    {
                        spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"];
                    }

                    BymlNode.ArrayNode ToLevel1Devices = new BymlNode.ArrayNode() { Name = "ToLevel1Devices" };
                    if (spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam["ToLevel1Devices"] == null)
                    {
                        spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam.Nodes.Add(ToLevel1Devices);
                    }
                    else
                    {
                        ToLevel1Devices = (BymlNode.ArrayNode)spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam["ToLevel1Devices"];
                    }
                    ToLevel1Devices.AddNode(Link.Dst);

                    if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam);
                    }

                }
                else if (Link.Name == "ToLevel2Devices")
                {
                    BymlNode.DictionaryNode spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam" };
                    if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] != null)
                    {
                        spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"];
                    }

                    BymlNode.ArrayNode ToLevel2Devices = new BymlNode.ArrayNode() { Name = "ToLevel2Devices" };
                    if (spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam["ToLevel2Devices"] == null)
                    {
                        spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam.Nodes.Add(ToLevel2Devices);
                    }
                    else
                    {
                        ToLevel2Devices = (BymlNode.ArrayNode)spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam["ToLevel2Devices"];
                    }
                    ToLevel2Devices.AddNode(Link.Dst);

                    if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam);
                    }

                }
                else if (Link.Name == "ToLevel3Devices")
                {
                    BymlNode.DictionaryNode spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam" };
                    if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] != null)
                    {
                        spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"];
                    }

                    BymlNode.ArrayNode ToLevel3Devices = new BymlNode.ArrayNode() { Name = "ToLevel3Devices" };
                    if (spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam["ToLevel3Devices"] == null)
                    {
                        spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam.Nodes.Add(ToLevel3Devices);
                    }
                    else
                    {
                        ToLevel3Devices = (BymlNode.ArrayNode)spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam["ToLevel3Devices"];
                    }
                    ToLevel3Devices.AddNode(Link.Dst);

                    if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam);
                    }

                }
                else if (Link.Name == "ToNotPaintableArea")
                {
                    BymlNode.DictionaryNode spl__LiftBancParam = new BymlNode.DictionaryNode() { Name = "spl__LiftBancParam" };
                    if (SerializedActor["spl__LiftBancParam"] != null)
                    {
                        spl__LiftBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LiftBancParam"];
                    }

                    BymlNode.ArrayNode ToNotPaintableArea = new BymlNode.ArrayNode() { Name = "ToNotPaintableArea" };
                    if (spl__LiftBancParam["ToNotPaintableArea"] == null)
                    {
                        spl__LiftBancParam.Nodes.Add(ToNotPaintableArea);
                    }
                    else
                    {
                        ToNotPaintableArea = (BymlNode.ArrayNode)spl__LiftBancParam["ToNotPaintableArea"];
                    }
                    ToNotPaintableArea.AddNode(Link.Dst);

                    if (SerializedActor["spl__LiftBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LiftBancParam);
                    }

                }
                else if (Link.Name == "ToParent")
                {
                    BymlNode.DictionaryNode spl__ailift__AILiftBancParam = new BymlNode.DictionaryNode() { Name = "spl__ailift__AILiftBancParam" };
                    if (SerializedActor["spl__ailift__AILiftBancParam"] != null)
                    {
                        spl__ailift__AILiftBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ailift__AILiftBancParam"];
                    }

                    if (spl__ailift__AILiftBancParam["ToParent"] == null)
                    {
                        spl__ailift__AILiftBancParam.AddNode("ToParent", Link.Dst);
                    }

                    if (SerializedActor["spl__ailift__AILiftBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ailift__AILiftBancParam);
                    }

                }
                else if (Link.Name == "ToPlayerFrontDirLocator")
                {
                    BymlNode.DictionaryNode spl__MissionGatewayBancParam = new BymlNode.DictionaryNode() { Name = "spl__MissionGatewayBancParam" };
                    if (SerializedActor["spl__MissionGatewayBancParam"] != null)
                    {
                        spl__MissionGatewayBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MissionGatewayBancParam"];
                    }

                    if (spl__MissionGatewayBancParam["ToPlayerFrontDirLocator"] == null)
                    {
                        spl__MissionGatewayBancParam.AddNode("ToPlayerFrontDirLocator", Link.Dst);
                    }

                    if (SerializedActor["spl__MissionGatewayBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__MissionGatewayBancParam);
                    }

                }
                else if (Link.Name == "ToProjectionAreas")
                {
                    BymlNode.DictionaryNode spl__HologramProjectorBancParam = new BymlNode.DictionaryNode() { Name = "spl__HologramProjectorBancParam" };
                    if (SerializedActor["spl__HologramProjectorBancParam"] != null)
                    {
                        spl__HologramProjectorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__HologramProjectorBancParam"];
                    }

                    BymlNode.ArrayNode ToProjectionAreas = new BymlNode.ArrayNode() { Name = "ToProjectionAreas" };
                    if (spl__HologramProjectorBancParam["ToProjectionAreas"] == null)
                    {
                        spl__HologramProjectorBancParam.Nodes.Add(ToProjectionAreas);
                    }
                    else
                    {
                        ToProjectionAreas = (BymlNode.ArrayNode)spl__HologramProjectorBancParam["ToProjectionAreas"];
                    }
                    ToProjectionAreas.AddNode(Link.Dst);

                    if (SerializedActor["spl__HologramProjectorBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__HologramProjectorBancParam);
                    }

                }
                else if (Link.Name == "ToRouteTargetPointArray")
                {
                    BymlNode.DictionaryNode spl__LocatorGachihokoRouteAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorGachihokoRouteAreaBancParam" };
                    if (SerializedActor["spl__LocatorGachihokoRouteAreaBancParam"] != null)
                    {
                        spl__LocatorGachihokoRouteAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorGachihokoRouteAreaBancParam"];
                    }

                    BymlNode.ArrayNode ToRouteTargetPointArray = new BymlNode.ArrayNode() { Name = "ToRouteTargetPointArray" };
                    if (spl__LocatorGachihokoRouteAreaBancParam["ToRouteTargetPointArray"] == null)
                    {
                        spl__LocatorGachihokoRouteAreaBancParam.Nodes.Add(ToRouteTargetPointArray);
                    }
                    else
                    {
                        ToRouteTargetPointArray = (BymlNode.ArrayNode)spl__LocatorGachihokoRouteAreaBancParam["ToRouteTargetPointArray"];
                    }
                    ToRouteTargetPointArray.AddNode(Link.Dst);

                    if (SerializedActor["spl__LocatorGachihokoRouteAreaBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LocatorGachihokoRouteAreaBancParam);
                    }

                }
                else if (Link.Name == "ToSearchLimitArea")
                {
                    BymlNode.DictionaryNode spl__AttentionTargetingBancParam = new BymlNode.DictionaryNode() { Name = "spl__AttentionTargetingBancParam" };
                    if (SerializedActor["spl__AttentionTargetingBancParam"] != null)
                    {
                        spl__AttentionTargetingBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AttentionTargetingBancParam"];
                    }

                    BymlNode.ArrayNode ToSearchLimitArea = new BymlNode.ArrayNode() { Name = "ToSearchLimitArea" };
                    if (spl__AttentionTargetingBancParam["ToSearchLimitArea"] == null)
                    {
                        spl__AttentionTargetingBancParam.Nodes.Add(ToSearchLimitArea);
                    }
                    else
                    {
                        ToSearchLimitArea = (BymlNode.ArrayNode)spl__AttentionTargetingBancParam["ToSearchLimitArea"];
                    }
                    ToSearchLimitArea.AddNode(Link.Dst);

                    if (SerializedActor["spl__AttentionTargetingBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__AttentionTargetingBancParam);
                    }

                }
                else if (Link.Name == "ToShopRoom")
                {
                    BymlNode.DictionaryNode spl__ShopBindableBancParam = new BymlNode.DictionaryNode() { Name = "spl__ShopBindableBancParam" };
                    if (SerializedActor["spl__ShopBindableBancParam"] != null)
                    {
                        spl__ShopBindableBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ShopBindableBancParam"];
                    }

                    if (spl__ShopBindableBancParam["ToShopRoom"] == null)
                    {
                        spl__ShopBindableBancParam.AddNode("ToShopRoom", Link.Dst);
                    }

                    if (SerializedActor["spl__ShopBindableBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ShopBindableBancParam);
                    }

                }
                else if (Link.Name == "ToSpawners")
                {
                    BymlNode.DictionaryNode spl__MetaSpawnerSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__MetaSpawnerSdodrBancParam" };
                    if (SerializedActor["spl__MetaSpawnerSdodrBancParam"] != null)
                    {
                        spl__MetaSpawnerSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MetaSpawnerSdodrBancParam"];
                    }

                    BymlNode.ArrayNode ToSpawners = new BymlNode.ArrayNode() { Name = "ToSpawners" };
                    if (spl__MetaSpawnerSdodrBancParam["ToSpawners"] == null)
                    {
                        spl__MetaSpawnerSdodrBancParam.Nodes.Add(ToSpawners);
                    }
                    else
                    {
                        ToSpawners = (BymlNode.ArrayNode)spl__MetaSpawnerSdodrBancParam["ToSpawners"];
                    }
                    ToSpawners.AddNode(Link.Dst);

                    if (SerializedActor["spl__MetaSpawnerSdodrBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__MetaSpawnerSdodrBancParam);
                    }

                }
                else if (Link.Name == "ToSyncActor")
                {
                    BymlNode.DictionaryNode spl__ObjIdolSyncerBancParam = new BymlNode.DictionaryNode() { Name = "spl__ObjIdolSyncerBancParam" };
                    if (SerializedActor["spl__ObjIdolSyncerBancParam"] != null)
                    {
                        spl__ObjIdolSyncerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ObjIdolSyncerBancParam"];
                    }

                    if (spl__ObjIdolSyncerBancParam["ToSyncActor"] == null)
                    {
                        spl__ObjIdolSyncerBancParam.AddNode("ToSyncActor", Link.Dst);
                    }

                    if (SerializedActor["spl__ObjIdolSyncerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__ObjIdolSyncerBancParam);
                    }

                }
                else if (Link.Name == "ToTable")
                {
                    BymlNode.DictionaryNode spl__LobbyMiniGameSeatBancParam = new BymlNode.DictionaryNode() { Name = "spl__LobbyMiniGameSeatBancParam" };
                    if (SerializedActor["spl__LobbyMiniGameSeatBancParam"] != null)
                    {
                        spl__LobbyMiniGameSeatBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LobbyMiniGameSeatBancParam"];
                    }

                    if (spl__LobbyMiniGameSeatBancParam["ToTable"] == null)
                    {
                        spl__LobbyMiniGameSeatBancParam.AddNode("ToTable", Link.Dst);
                    }

                    if (SerializedActor["spl__LobbyMiniGameSeatBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LobbyMiniGameSeatBancParam);
                    }

                }
                else if (Link.Name == "ToTarget_Cube")
                {
                    BymlNode.DictionaryNode spl__LocatorSpawnerBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorSpawnerBancParam" };
                    if (SerializedActor["spl__LocatorSpawnerBancParam"] != null)
                    {
                        spl__LocatorSpawnerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorSpawnerBancParam"];
                    }

                    if (spl__LocatorSpawnerBancParam["ToTarget_Cube"] == null)
                    {
                        spl__LocatorSpawnerBancParam.AddNode("ToTarget_Cube", Link.Dst);
                    }

                    if (SerializedActor["spl__LocatorSpawnerBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__LocatorSpawnerBancParam);
                    }

                }
                else if (Link.Name == "ToWallaObjGroupTag")
                {
                    BymlNode.DictionaryNode spl__WallaObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__WallaObjBancParam" };
                    if (SerializedActor["spl__WallaObjBancParam"] != null)
                    {
                        spl__WallaObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__WallaObjBancParam"];
                    }

                    if (spl__WallaObjBancParam["ToWallaObjGroupTag"] == null)
                    {
                        spl__WallaObjBancParam.AddNode("ToWallaObjGroupTag", Link.Dst);
                    }

                    if (SerializedActor["spl__WallaObjBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__WallaObjBancParam);
                    }

                }
                else if (Link.Name == "UtsuboxLocator")
                {
                    BymlNode.DictionaryNode spl__EnemyUtsuboxBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyUtsuboxBancParam" };
                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] != null)
                    {
                        spl__EnemyUtsuboxBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyUtsuboxBancParam"];
                    }

                    if (spl__EnemyUtsuboxBancParam["UtsuboxLocator"] == null)
                    {
                        spl__EnemyUtsuboxBancParam.AddNode("UtsuboxLocator", Link.Dst);
                    }

                    if (SerializedActor["spl__EnemyUtsuboxBancParam"] == null)
                    {
                        SerializedActor.Nodes.Add(spl__EnemyUtsuboxBancParam);
                    }

                }
            }
        }

    }
}
