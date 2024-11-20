using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;

namespace SampleMapEditor
{
    [ByamlObject]
    public class MuElement : SpatialObject, IByamlSerializable, IStageReferencable
    {
        [BindGUI("Name", Category = "OBJECT", ColumnIndex = 0)]
        public string __Name
        {
            get
            {
                return this.Name;
            }
        }

        [ByamlMember]
        public string Name { get; set; }

        [BindGUI("Bakeable", Category = "OBJECT", ColumnIndex = 0)]
        public bool _Bakeable
        {
            get
            {
                return this.Bakeable;
            }
            set
            {
                this.Bakeable = value;

                if (value == true)
                {
                    if (this.Name.StartsWith("Lft_") || this.Name.StartsWith("Obj_"))
                        this.Gyaml = "Work/Actor/Mpt_" + this.Name.Substring(4) + ".engine__actor__ActorParam.gyml";
                    else
                        this.Gyaml = "Work/Actor/" + this.Name + ".engine__actor__ActorParam.gyml";
                }
                else
                {
                    this.Gyaml = this.Name;
                }
            }
        }

        [ByamlMember]
        public bool Bakeable = false;

        [ByamlMember]
        [BindGUI("Layer", 
            new object[28] { "None", "Cmn", "Pnt", "Var", "Vlf", "Vgl", "Vcl", "Tcl", "Low", "Mid", "High", "Day",
                "DayOrSuntet", "Sunset", "Night", "FestFirstHalf", "FestSecondHalf", "FestAnnounce", "FestWaitingForResult",
                "FestNone", "BigRun", "Sound", "Area01", "Area02", "Area03", "Area04", "Area05", "Area06" },

            new string[28] { "None", "Common", "Turf War", "Splat Zones", "Tower Control", "Rainmaker", "Clam Blitz", 
                "Tricolor Turf War", "Salmon Run Low Tide", "Salmon Run Mid Tide", "Salmon Run High Tide", "Day",
                "Either Day or Sunset", "Sunset", "Night", "First Half of the Splatfest", "Second Half of the Splatfest", 
                "Splatfest Announcement", "Splatfest Waiting for Results", "Outside of Splatfest", "Big Run", "Sound",
                "Future Utopia Island", "Cozy & Safe Factory", "Cryogenic Hopetown", "Landfill Dreamland", 
                "Eco-Forest Treehills", "Happiness Research Lab" },

            Category = "OBJECT", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string Layer { get; set; }

        [ByamlMember]
        [BindGUI("Gyaml", Category = "OBJECT", ColumnIndex = 0, Control = BindControl.Default)]
        public string Gyaml { get; set; }

        [ByamlMember]
        [BindGUI("Hash", Category = "OBJECT", ColumnIndex = 0, Control = BindControl.Default)]
        public ulong Hash { get; set; }

        [ByamlMember]
        [BindGUI("SRTHash", Category = "OBJECT", ColumnIndex = 0, Control = BindControl.Default)]
        public uint SRTHash { get; set; }

        public MuElement()
        {
            this.Layer = "None";
            this.Name = "Obj_GeneralBox_15x15x15_Blitz";

            this.Scale = new ByamlVector3F(1, 1, 1);
        }


        public MuElement(MuElement other)
        {
            Layer = other.Layer;
            Name = other.Name;
            Gyaml = other.Gyaml;
            Bakeable = other.Bakeable;

            Scale = other.Scale;
            Translate = other.Translate;
            RotateDegrees = other.RotateDegrees;
        }



        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Reads data from the given <paramref name="dictionary"/> to satisfy members.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{String, Object}"/> to read the data from.</param>
        public virtual void DeserializeByaml(IDictionary<string, object> dictionary)
        {
            Console.ResetColor();
            //ModeInclusion = ModeInclusion.FromDictionary(dictionary);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            try
            {
                /*
                var dictLinks = dictionary["Links"] as Dictionary<string, object>;
                Console.WriteLine($"Link Count: {dictLinks.Count}"); //Console.WriteLine($"Link Count: {((Dictionary<string, object>)dictionary["Links"]).Count}");
                Links = (Dictionary<string, List<LinkInfo>>)dictionary["Links"];
                for (int i = 0; i < dictLinks.Keys.Count; i++)
                {
                    string keyName = dictLinks.Keys.ElementAt(i);
                    for (int j = 0; j < ((List<dynamic>)dictLinks[keyName]).Count; j++)
                    {
                        var entry = ((List<dynamic>)dictLinks[keyName])[j];
                        Console.WriteLine(entry);
                        var li = new LinkInfo()
                        {
                            DefinitionName = entry["DefinitionName"],
                            DestUnitId = entry["DestUnitId"],
                        };
                        Console.WriteLine(li);
                        //Links[keyName].Add(li);
                        if (!Links.ContainsKey(keyName))
                            Links.Add(keyName, new List<LinkInfo>());

                        Links[keyName].Add(li);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.WriteLine(ex.Message);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Writes instance members into the given <paramref name="dictionary"/> to store them as BYAML data.
        /// </summary>
        /// <param name="dictionary">The <see cref="Dictionary{String, Object}"/> to store the data in.</param>
        public virtual void SerializeByaml(IDictionary<string, object> dictionary)
        {
            /*
            //ModeInclusion.ToDictionary(dictionary);
            dictionary.Add("Links", new Dictionary<string, object>());

            if (Links.Count == 0) return;

            foreach (var link in Links)
            {
                link.Value.ForEach(lnk => Console.WriteLine(lnk));
                if (!((Dictionary<string, object>)dictionary["Links"]).ContainsKey(link.Key))
                    ((Dictionary<string, object>)dictionary["Links"]).Add(link.Key, new List<object>()); //new Dictionary<string, object>());

                link.Value.ForEach(lnk =>
                {
                    var entry = new Dictionary<string, object>();
                    entry["DefinitionName"] = lnk.DefinitionName;
                    entry["DestUnitId"] = lnk.DestUnitId;
                    //entry["UnitFileName"] = lnk.UnitFileName;
                    ((dynamic)dictionary)["Links"][link.Key].Add(entry);
                });
            }
            */

            //((dynamic)dictionary["Links"]).Add("TEST_KEY");
            //((dynamic)dictionary["Links"]).Add(testDict);
            //((dynamic)dictionary["Links"])["TEST_KEY"].Add((Dictionary))
        }

        /// <summary>
        /// Allows references of course data objects to be resolved to provide real instances instead of the raw values
        /// in the BYAML.
        /// </summary>
        /// <param name="stageDefinition">The <see cref="StageDefinition"/> providing the objects.</param>
        public virtual void DeserializeReferences(StageDefinition stageDefinition)
        {
            /*// References to paths and their points.
            try
            {
                Path = _pathIndex == null ? null : courseDefinition.Paths[_pathIndex.Value];
                PathPoint = _pathPointIndex == null ? null : Path.Points[_pathPointIndex.Value];
                LapPath = _lapPathIndex == null ? null : courseDefinition.LapPaths[_lapPathIndex.Value];
                LapPathPoint = _lapPathPointIndex == null ? null : LapPath.Points[_lapPathPointIndex.Value];
                ObjPath = _objPathIndex == null ? null : courseDefinition.ObjPaths[_objPathIndex.Value];
                ObjPathPoint = _objPathPointIndex == null ? null : ObjPath.Points[_objPathPointIndex.Value];
                EnemyPath1 = _enemyPath1Index == null ? null : courseDefinition.EnemyPaths[_enemyPath1Index.Value];
                EnemyPath2 = _enemyPath2Index == null ? null : courseDefinition.EnemyPaths[_enemyPath2Index.Value];
                ItemPath1 = _itemPath1Index == null ? null : courseDefinition.ItemPaths[_itemPath1Index.Value];
                ItemPath2 = _itemPath2Index == null ? null : courseDefinition.ItemPaths[_itemPath2Index.Value];
            }
            catch
            {

            }

            // References to other unit objects.
            ParentArea = _areaIndex == null ? null : courseDefinition.Areas[_areaIndex.Value];
            ParentObj = _objIndex == null ? null : courseDefinition.Objs[_objIndex.Value];

            if (Path != null) Path.References.Add(this);*/
        }

        /// <summary>
        /// Allows references between course objects to be serialized into raw values stored in the BYAML.
        /// </summary>
        /// <param name="stageDefinition">The <see cref="StageDefinition"/> providing the objects.</param>
        public virtual void SerializeReferences(StageDefinition stageDefinition)
        {
            return;

            /*if (Single == false) Single = null;
            if (NoCol == false) NoCol = null;

            // References to paths and their points.
            _pathIndex = Path == null ? null : (int?)courseDefinition.Paths.IndexOf(Path);
            _pathPointIndex = PathPoint?.Index;
            _lapPathIndex = LapPath == null ? null : (int?)courseDefinition.LapPaths.IndexOf(LapPath);
            _lapPathPointIndex = LapPathPoint?.Index;
            _objPathIndex = ObjPath == null ? null : (int?)courseDefinition.ObjPaths.IndexOf(ObjPath);
            _objPathPointIndex = (ObjPath != null && ObjPathPoint != null) ? ObjPathPoint.Index : null;
            _enemyPath1Index = EnemyPath1 == null ? null : (int?)courseDefinition.EnemyPaths.IndexOf(EnemyPath1);
            _enemyPath2Index = EnemyPath2 == null ? null : (int?)courseDefinition.EnemyPaths.IndexOf(EnemyPath2);
            _itemPath1Index = ItemPath1 == null ? null : (int?)courseDefinition.ItemPaths.IndexOf(ItemPath1);
            _itemPath2Index = ItemPath2 == null ? null : (int?)courseDefinition.ItemPaths.IndexOf(ItemPath2);

            // References to other unit objects.
            _areaIndex = ParentArea == null ? null : (int?)courseDefinition.Areas.IndexOf(ParentArea);
            _objIndex = ParentObj == null ? null : (int?)courseDefinition.Objs.IndexOf(ParentObj);

            if (_objIndex == -1) _objIndex = null;
            if (_areaIndex == -1) _areaIndex = null;
            if (_pathIndex == -1) _pathIndex = null;
            if (_pathPointIndex == -1) _pathPointIndex = null;
            if (_objPathIndex == -1) _objPathIndex = null;
            if (_objPathPointIndex == -1) _objPathPointIndex = null;

            if (_pathIndex != null)
                System.Console.WriteLine($"Obj {ObjId} _pathIndex {_pathIndex}");
            if (_objPathIndex != null)
                System.Console.WriteLine($"Obj {ObjId} _objPathIndex {_pathIndex}");*/

            List<string> UsedObjIds = new List<string>();
            for (int i = 0; i < stageDefinition.Actors.Count; i++)
            {
                if (stageDefinition.Actors[i].InstanceID != null)
                {
                    UsedObjIds.Add(stageDefinition.Actors[i].InstanceID);
                }
            }

            List<int> UsedObjIdNums = new List<int>();
            UsedObjIds.ForEach(i => UsedObjIdNums.Add(int.Parse(i.Replace("obj", "")))); //Convert.ToInt32(i)));
#warning This may need to be changed later, because some objects are named abhorrently.

            int objIdxCounter = 0;
            for (int i = 0; i < stageDefinition.Actors.Count; i++)
            {
                if (stageDefinition.Actors[i].InstanceID != null) continue;
                while (UsedObjIdNums.Contains(objIdxCounter))
                    objIdxCounter++;
                stageDefinition.Actors[i].InstanceID = $"obj{objIdxCounter++}";
            }
        }


        public virtual MuElement Clone()
        {
            /*return new MuElement()
            {
                LayerConfigName = this.LayerConfigName,
                UnitConfigName = this.UnitConfigName,
                Links = this.Links,
                Scale = this.Scale,
                Translate = this.Translate,
                RotateDegrees = this.RotateDegrees,
            };*/
            return new MuElement(this);
        }

        public static string GetActorClassName(MuElement element)
        {
            return GlobalSettings.ActorDatabase[element.Name].ClassName;
        }
        public static string GetActorClassName(dynamic element)
        {
            string name = "";
            try
            {
                name = GlobalSettings.ActorDatabase[element["Name"]].ClassName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return name;
        }
    }
}
