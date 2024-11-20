using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using MapStudio.UI;
using GLFrameworkEngine;
using UIFramework;

namespace MapStudio.UI
{
   public class StatisticsWindow : Window
    {
        public static int SkeletalAnims = 0;
        public static int MaterialAnims = 0;

        public override string Name => TranslationSource.GetText("STATS");

        public StatisticsWindow()
        {
            Opened = false;
        }

        public override void Render()
        {
            ImGui.Text($"Num Draw Calls: {ResourceTracker.NumDrawCalls}");
            ImGui.Text($"Num Draw Triangles: {ResourceTracker.NumDrawTriangles}");

            ImGui.Text($"Num Effect DrawCalls: {ResourceTracker.NumEffectDrawCalls}");
            ImGui.Text($"Num Effect Instances: {ResourceTracker.NumEffectInstances}");
            ImGui.Text($"Num Effect Triangles: {ResourceTracker.NumEffectTriangles}");

            ImGui.Text($"Num Skeletal Anims: {SkeletalAnims}");
            ImGui.Text($"Num Material Anims: {MaterialAnims}");
        }
    }
}
