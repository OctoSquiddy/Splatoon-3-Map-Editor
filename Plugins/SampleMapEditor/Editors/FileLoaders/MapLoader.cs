using MapStudio.UI;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMapEditor.LayoutEditor
{
    public class MapLoader
    {
        //Debugging
        private bool DEBUG_PROBES = false;

        public StageLayoutPlugin Plugin;

        /// <summary>
        /// The stage layout information, such as spawn points and object placement.
        /// </summary>
        public StageDefinition stageDefinition = new StageDefinition();


        // Models

        // N/A


        // Effects

        public static MapLoader Instance = null;


        public MapLoader(StageLayoutPlugin plugin)
        {
            Instance = this;
            Plugin = plugin;
        }


        /// <summary>
        /// Initiates an empty stage with no objects placed (Except for 2 Respawn Points)
        /// </summary>
        public void Init(StageLayoutPlugin plugin) // "bool isSwitch" and "string model_path" are unnecessary here
        {
            // We don't need as much stuff as the MK8 tool because we are only dealing with the stage layout

            stageDefinition.Actors = new List<MuObj>();
            
            // Add a RespawnPos
            MuObj obj0 = new MuObj()
            {
                InstanceID = "25894b33-c05c-8c87-8238-6ff6d1aba0fd",
                //IsLinkDest = false,
                Layer = "Cmn",
                TeamCmp = new MuObj.TeamNode() { Team = "Alpha" },
                Name = "RespawnPos",
                Translate = new ByamlVector3F(-200f, 0f, -200f),
                Scale = new ByamlVector3F(1f, 1f, 1f),
                Rotate = new ByamlVector3F(0f, 0f, 0f),
                //Links = new List<LinkInfo> { },
            };
            stageDefinition.Actors.Add(obj0);

            // Add a second RespawnPos
            MuObj obj1 = new MuObj()
            {
                InstanceID = "25894b33-c05c-8c87-8238-6ff6d1aba0fe",
                //IsLinkDest = false,
                Layer = "Cmn",
                TeamCmp = new MuObj.TeamNode() { Team = "Bravo" },
                Name = "RespawnPos",
                Translate = new ByamlVector3F(200f, 0f, 200f),
                Scale = new ByamlVector3F(1f, 1f, 1f),
                Rotate = new ByamlVector3F(0f, 0f, 0f),
                //Links = new List<LinkInfo> { },
            };
            stageDefinition.Actors.Add(obj1);


        }

        public void Load(StageLayoutPlugin plugin, string folder, string byamlFile, string workingDir)
        {
            Instance = this;
            Plugin = plugin;

            Console.WriteLine("~ Called MapLoader.Load(....); ~");
            Console.WriteLine("Loading file!!!");

            // Load Stage Layout File
            //LoadStageLayoutFromBYAML(byaml) // ~Example
            LoadStageLayoutFile(byamlFile);
        }

        private void LoadStageLayoutFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            ProcessLoading.Instance.Update(15, 100, "Loading Stage Layout Byaml");

            stageDefinition = new StageDefinition(filePath);

            //
        }


    }
}
