using ImGuiNET;
using MapStudio.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;

namespace SampleMapEditor.LayoutEditor
{
    /// <summary>
    /// Asset category for Presets that spawn multiple linked objects at once.
    /// </summary>
    public class AssetViewPresets : IAssetLoader
    {
        public string Name => "Presets";

        public bool IsFilterMode => false;

        public List<AssetItem> Reload()
        {
            List<AssetItem> assets = new List<AssetItem>();

            // Add all presets
            AddSpawnerPreset(assets);
            AddWarpPointPreset(assets);

            return assets;
        }

        private void AddSpawnerPreset(List<AssetItem> assets)
        {
            var icon = IconManager.GetTextureIcon("Node");
            if (IconManager.HasIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\LocatorSpawner.png"))
                icon = IconManager.GetTextureIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\LocatorSpawner.png");

            var preset = new PresetAssetItem("Preset_Spawner")
            {
                Name = "Spawner",
                Icon = icon,
                PresetType = PresetType.Spawner
            };

            // Define objects to spawn
            preset.ObjectsToSpawn = new List<PresetObjectDefinition>
            {
                new PresetObjectDefinition
                {
                    ActorName = "LocatorSpawner",
                    PositionOffset = new OpenTK.Vector3(0, 0, 0),
                    LinksToCreate = new List<PresetLinkDefinition>
                    {
                        new PresetLinkDefinition
                        {
                            LinkName = "ToTarget_Cube",
                            TargetObjectIndex = 1 // Links to LocatorSpawnerTargetCube
                        }
                    }
                },
                new PresetObjectDefinition
                {
                    ActorName = "LocatorSpawnerTargetCube",
                    PositionOffset = new OpenTK.Vector3(0, 50, 0) // Slightly above
                },
                new PresetObjectDefinition
                {
                    ActorName = "LocatorVersusStart",
                    PositionOffset = new OpenTK.Vector3(0, 0, 50) // Slightly in front
                }
            };

            assets.Add(preset);
        }

        private void AddWarpPointPreset(List<AssetItem> assets)
        {
            var icon = IconManager.GetTextureIcon("Node");
            if (IconManager.HasIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\SplAutoWarpPoint.png"))
                icon = IconManager.GetTextureIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\SplAutoWarpPoint.png");

            var preset = new PresetAssetItem("Preset_WarpPoint")
            {
                Name = "WarpPoint",
                Icon = icon,
                PresetType = PresetType.WarpPoint
            };

            preset.ObjectsToSpawn = new List<PresetObjectDefinition>
            {
                new PresetObjectDefinition
                {
                    ActorName = "SplAutoWarpPoint",
                    PositionOffset = new OpenTK.Vector3(0, 0, 0),
                    LinksToCreate = new List<PresetLinkDefinition>
                    {
                        new PresetLinkDefinition
                        {
                            LinkName = "ToGeneralLocator",
                            TargetObjectIndex = 1
                        }
                    }
                },
                new PresetObjectDefinition
                {
                    ActorName = "GeneralLocator",
                    PositionOffset = new OpenTK.Vector3(0, 0, 100) // Warp destination in front
                }
            };

            assets.Add(preset);
        }

        public bool UpdateFilterList()
        {
            return false;
        }
    }

    public enum PresetType
    {
        Spawner,
        WarpPoint,
        // Future preset types can be added here
    }

    /// <summary>
    /// Defines a single object within a preset
    /// </summary>
    public class PresetObjectDefinition
    {
        public string ActorName { get; set; }
        public OpenTK.Vector3 PositionOffset { get; set; } = OpenTK.Vector3.Zero;
        public List<PresetLinkDefinition> LinksToCreate { get; set; } = new List<PresetLinkDefinition>();
    }

    /// <summary>
    /// Defines a link to create between preset objects
    /// </summary>
    public class PresetLinkDefinition
    {
        public string LinkName { get; set; }
        public int TargetObjectIndex { get; set; } // Index in the ObjectsToSpawn list
    }

    /// <summary>
    /// Asset item for presets that spawn multiple objects
    /// </summary>
    public class PresetAssetItem : AssetItem
    {
        public PresetType PresetType { get; set; }
        public List<PresetObjectDefinition> ObjectsToSpawn { get; set; } = new List<PresetObjectDefinition>();

        public PresetAssetItem(string id) : base(id)
        {
        }
    }
}
