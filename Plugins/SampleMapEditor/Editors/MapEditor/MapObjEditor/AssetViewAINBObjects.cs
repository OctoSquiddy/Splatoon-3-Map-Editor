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
    /// Asset category for AINB-compatible objects (objects that work with AINB logic).
    /// </summary>
    public class AssetViewAINBObjects : IAssetLoader
    {
        public virtual string Name => "AINB Objects";

        public bool IsFilterMode => false;

        /// <summary>
        /// List of object names that are AINB-compatible.
        /// </summary>
        private static readonly HashSet<string> AINBObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SwitchShock",
            "SwitchPaint",
            "SwitchPaintSmall",
            "Periscope",
            "SwitchStep",
            "Lft_AbstractBlitzCompatibles",
            "Lft_AbstractDrawer",
            "LocatorAreaSwitch",
            "SplAutoWarpPoint",
            "GeneralLocator",
            "SnakeBlock"
        };

        /// <summary>
        /// Objects that spawn as presets with linked objects.
        /// </summary>
        private static readonly HashSet<string> PresetObjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SplAutoWarpPoint"
        };

        public virtual List<AssetItem> Reload()
        {
            List<AssetItem> assets = new List<AssetItem>();

            var actorList = GlobalSettings.ActorDatabase.Values.ToList();

            foreach (var actor in actorList)
            {
                if (AINBObjectNames.Contains(actor.Name))
                {
                    if (PresetObjects.Contains(actor.Name))
                        AddPresetAsset(assets, actor);
                    else
                        AddAsset(assets, actor);
                }
            }

            return assets;
        }

        public void AddAsset(List<AssetItem> assets, ActorDefinition actor)
        {
            string resName = actor.ResName;

            var icon = IconManager.GetTextureIcon("Node");
            if (IconManager.HasIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\{resName}.png"))
                icon = IconManager.GetTextureIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\{resName}.png");

            // Use MapObjectAsset so spawning works correctly
            assets.Add(new MapObjectAsset($"AINBObject_{actor.Name}")
            {
                Name = actor.Name,
                ActorDefinition = actor,
                Icon = icon,
            });
        }

        public void AddPresetAsset(List<AssetItem> assets, ActorDefinition actor)
        {
            string resName = actor.ResName;

            var icon = IconManager.GetTextureIcon("Node");
            if (IconManager.HasIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\{resName}.png"))
                icon = IconManager.GetTextureIcon($"{Runtime.ExecutableDir}\\Lib\\Images\\MapObjects\\{resName}.png");

            if (actor.Name == "SplAutoWarpPoint")
            {
                var preset = new PresetAssetItem($"AINBPreset_{actor.Name}")
                {
                    Name = actor.Name,
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
        }

        public bool UpdateFilterList()
        {
            return false;
        }
    }
}
