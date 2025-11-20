using System;
using System.Numerics;
using UIFramework;
using ImGuiNET;
using MapStudio.UI;
using Toolbox.Core;

namespace MapStudio
{
    public class AboutWindow : Window
    {
        public override string Name => "Credits";

        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize;

        public AboutWindow()
        {
            Opened = false;
        }

        public override void Render()
        {
            Size = new Vector2(600, 400);

            ImGui.SetCursorPos(new Vector2(560, 10));
            if (ImGui.Button("Close"))
                Opened = false;

            ImGui.SetCursorPos(new Vector2(20, 20));

            ImGui.Text("Credits:");

            ImGui.BulletText("OctoSquiddy - Splatoon 3 file system understanding, coding, and more");

            ImGui.BulletText("Wheatley - Tool provision");

            ImGui.BulletText("heart2zara - UI themes, new features, bug fixes");

            ImGui.BulletText("graham cracker - Source collaboration and bug fixes");

            ImGui.NewLine();

            ImGui.Text("  Original Track Studio Credits:");

            ImGui.BulletText("Abood XD / MasterVermilli0n - Wii U/Switch texture swizzling and awesome effect decompile library");

            ImGui.BulletText("Syroot - Wii U Bfres library and binary I/O");

            ImGui.BulletText("Ryujinx - Shader libraries for decompiling and translating switch binaries to GLSL");

            ImGui.BulletText("JuPaHe64 - Timeline animation and gizmo tool fixes");

            ImGui.BulletText("OpenTK Team - C# OpenGL bindings");

            ImGui.BulletText("mellinoe and ImGUI Team - C# port of ImGUI library");

            ImGui.BulletText("Atlas & Wexos - Nintendo game research and help");
            ImGui.NewLine();
        }
    }
}
