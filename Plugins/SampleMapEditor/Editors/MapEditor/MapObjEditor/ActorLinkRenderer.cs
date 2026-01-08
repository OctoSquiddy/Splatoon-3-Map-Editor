using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GLFrameworkEngine;

namespace SampleMapEditor.LayoutEditor
{
    /// <summary>
    /// Renders colored lines between linked actors in the 3D view.
    /// Each link type has a specific color (no green to avoid confusion with areas).
    /// </summary>
    public static class ActorLinkRenderer
    {
        // Link type colors (no green - reserved for areas)
        private static readonly Dictionary<string, Vector4> LinkTypeColors = new Dictionary<string, Vector4>
        {
            // Yellow tones
            { "ToParent", new Vector4(1.0f, 0.9f, 0.0f, 1.0f) },           // Bright Yellow
            { "ToGeneralLocator", new Vector4(1.0f, 0.8f, 0.2f, 1.0f) },   // Golden Yellow

            // Light Blue tones only (no dark blue)
            { "ToTarget_Cube", new Vector4(0.5f, 0.8f, 1.0f, 1.0f) },      // Light Sky Blue
            { "LinkToTarget", new Vector4(0.6f, 0.85f, 1.0f, 1.0f) },      // Pale Blue
            { "LinkToLocator", new Vector4(0.55f, 0.8f, 1.0f, 1.0f) },     // Light Blue

            // Purple/Magenta tones
            { "ToArea", new Vector4(0.8f, 0.3f, 0.9f, 1.0f) },             // Magenta
            { "AreaLinks", new Vector4(0.7f, 0.2f, 0.8f, 1.0f) },          // Purple
            { "ToProjectionAreas", new Vector4(0.6f, 0.3f, 0.7f, 1.0f) },  // Violet

            // Orange/Red tones
            { "ToActor", new Vector4(1.0f, 0.5f, 0.0f, 1.0f) },            // Orange
            { "LinksToActor", new Vector4(1.0f, 0.4f, 0.1f, 1.0f) },       // Dark Orange
            { "ToBindActor", new Vector4(1.0f, 0.3f, 0.2f, 1.0f) },        // Red-Orange
            { "ToBindObjLink", new Vector4(0.9f, 0.2f, 0.2f, 1.0f) },      // Red

            // Cyan tones
            { "LinkToWater", new Vector4(0.0f, 0.8f, 0.9f, 1.0f) },        // Cyan
            { "ToGateway", new Vector4(0.1f, 0.7f, 0.8f, 1.0f) },          // Teal

            // Pink tones
            { "ToFriendLink", new Vector4(1.0f, 0.4f, 0.7f, 1.0f) },       // Pink
            { "SpawnObjLinks", new Vector4(1.0f, 0.5f, 0.6f, 1.0f) },      // Light Pink

            // White/Gray tones
            { "Reference", new Vector4(0.9f, 0.9f, 0.9f, 1.0f) },          // White
            { "BindLink", new Vector4(0.7f, 0.7f, 0.7f, 1.0f) },           // Gray

            // Brown/Tan tones
            { "JumpTarget", new Vector4(0.8f, 0.6f, 0.3f, 1.0f) },         // Tan
            { "JumpPoints", new Vector4(0.7f, 0.5f, 0.2f, 1.0f) },         // Brown
            { "JumpTargetCandidates", new Vector4(0.9f, 0.7f, 0.4f, 1.0f) }, // Light Brown

            // More specific link types
            { "Target", new Vector4(1.0f, 0.6f, 0.2f, 1.0f) },             // Amber
            { "TargetArea", new Vector4(0.9f, 0.5f, 0.3f, 1.0f) },         // Coral
            { "TargetLift", new Vector4(0.8f, 0.4f, 0.4f, 1.0f) },         // Salmon
            { "TargetPropeller", new Vector4(0.7f, 0.3f, 0.5f, 1.0f) },    // Plum

            { "ToDropItem", new Vector4(0.6f, 0.8f, 0.4f, 1.0f) },         // Light Lime (slight green ok for items)
            { "ToBuildItem", new Vector4(0.5f, 0.7f, 0.3f, 1.0f) },        // Olive

            { "NextAirBall", new Vector4(0.5f, 0.9f, 1.0f, 1.0f) },        // Light Cyan
            { "FinalAirball", new Vector4(0.3f, 0.8f, 0.9f, 1.0f) },       // Aqua
            { "ShortcutAirBall", new Vector4(0.6f, 0.85f, 0.95f, 1.0f) },   // Light Steel Blue
            { "EnemyPaintAreaAirBall", new Vector4(0.6f, 0.5f, 0.9f, 1.0f) }, // Lavender

            { "RestartPos", new Vector4(1.0f, 1.0f, 0.5f, 1.0f) },         // Light Yellow
            { "SafePosLinks", new Vector4(0.9f, 0.9f, 0.4f, 1.0f) },       // Pale Yellow

            { "CorrectPoint", new Vector4(0.8f, 0.5f, 0.8f, 1.0f) },       // Orchid
            { "CorrectPointArray", new Vector4(0.7f, 0.4f, 0.7f, 1.0f) },  // Medium Orchid

            { "LinkToAnotherPipeline", new Vector4(0.9f, 0.6f, 0.9f, 1.0f) }, // Thistle
            { "LinkToCage", new Vector4(0.7f, 0.8f, 1.0f, 1.0f) },         // Pale Slate Blue
            { "LinkToCompass", new Vector4(0.65f, 0.75f, 0.95f, 1.0f) },   // Light Periwinkle
            { "LinkToEnemyGenerators", new Vector4(0.9f, 0.3f, 0.3f, 1.0f) }, // Indian Red
            { "LinkToMoveArea", new Vector4(0.6f, 0.8f, 1.0f, 1.0f) },     // Light Cornflower Blue

            { "ToSearchLimitArea", new Vector4(0.7f, 0.6f, 0.5f, 1.0f) },  // Rosy Brown
            { "ToNotPaintableArea", new Vector4(0.6f, 0.5f, 0.4f, 1.0f) }, // Dark Tan
            { "ToPlayerFrontDirLocator", new Vector4(0.8f, 0.7f, 0.6f, 1.0f) }, // Wheat
            { "ToRouteTargetPointArray", new Vector4(0.9f, 0.8f, 0.5f, 1.0f) }, // Khaki
            { "ToShopRoom", new Vector4(0.7f, 0.6f, 0.85f, 1.0f) },        // Light Slate Blue
            { "ToTable", new Vector4(0.75f, 0.65f, 0.9f, 1.0f) },          // Light Purple
            { "ToWallaObjGroupTag", new Vector4(0.6f, 0.7f, 0.5f, 1.0f) }, // Dark Sea Green (slight green ok)

            { "UtsuboxLocator", new Vector4(0.8f, 0.6f, 0.4f, 1.0f) },     // Peru
            { "CrowdMorayHead", new Vector4(0.7f, 0.5f, 0.3f, 1.0f) },     // Sienna
            { "LastHitPosArea", new Vector4(0.9f, 0.4f, 0.4f, 1.0f) },     // Light Coral
            { "Accessories", new Vector4(0.8f, 0.8f, 0.6f, 1.0f) },        // Pale Goldenrod
            { "CoreBattleManagers", new Vector4(0.6f, 0.4f, 0.6f, 1.0f) }, // Medium Purple
            { "SubAreaInstanceIds", new Vector4(0.7f, 0.8f, 0.9f, 1.0f) }, // Pale Steel Blue
        };

        // Default color for unknown link types (bright magenta to make them visible)
        private static readonly Vector4 DefaultLinkColor = new Vector4(1.0f, 0.0f, 0.8f, 1.0f);

        /// <summary>
        /// Gets the color for a specific link type name.
        /// </summary>
        public static Vector4 GetLinkColor(string linkName)
        {
            if (string.IsNullOrEmpty(linkName))
                return DefaultLinkColor;

            if (LinkTypeColors.TryGetValue(linkName, out Vector4 color))
                return color;

            return DefaultLinkColor;
        }

        /// <summary>
        /// Gets all registered link type colors for display purposes.
        /// </summary>
        public static IReadOnlyDictionary<string, Vector4> GetAllLinkColors()
        {
            return LinkTypeColors;
        }
    }
}
