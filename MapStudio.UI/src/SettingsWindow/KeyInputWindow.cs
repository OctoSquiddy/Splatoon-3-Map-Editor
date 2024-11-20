using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapStudio.UI;
using ImGuiNET;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class KeyInputWindow
    {
        static string active_edit = "";

        public static void Render()
        {
            if (ImGui.CollapsingHeader("Camera Movement", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Columns(2);
                DrawConfigurableInput("Front", InputSettings.INPUT.Camera, "MoveForward");
                DrawConfigurableInput("Back", InputSettings.INPUT.Camera, "MoveBack");
                DrawConfigurableInput("Left", InputSettings.INPUT.Camera, "MoveLeft");
                DrawConfigurableInput("Right", InputSettings.INPUT.Camera, "MoveRight");
                DrawConfigurableInput("Up", InputSettings.INPUT.Camera, "MoveUp"); 
                DrawConfigurableInput("Down", InputSettings.INPUT.Camera, "MoveDown");
                ImGui.Columns(1);
            }
            if (ImGui.CollapsingHeader("Viewport", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Columns(2);
                DrawConfigurableInput("Focus Selected", InputSettings.INPUT.Camera, "FocusOnSelectedObject");
                DrawConfigurableInput("Undo", InputSettings.INPUT.Scene, "Undo");
                DrawConfigurableInput("Redo", InputSettings.INPUT.Scene, "Redo");
                DrawConfigurableInput("Copy", InputSettings.INPUT.Scene, "Copy");
                DrawConfigurableInput("Paste", InputSettings.INPUT.Scene, "Paste");
                DrawConfigurableInput("Delete", InputSettings.INPUT.Scene, "Delete");
                DrawConfigurableInput("SelectionBox", InputSettings.INPUT.Scene, "SelectionBox");
                DrawConfigurableInput("SelectionCircle", InputSettings.INPUT.Scene, "SelectionCircle");
                DrawConfigurableInput("SelectAll", InputSettings.INPUT.Scene, "SelectAll");
                ImGui.Columns(1);
            }
            if (ImGui.CollapsingHeader("Tranform", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Columns(2);
                DrawConfigurableInput("AxisX", InputSettings.INPUT.Transform, "AxisX");
                DrawConfigurableInput("AxisY", InputSettings.INPUT.Transform, "AxisY");
                DrawConfigurableInput("AxisZ", InputSettings.INPUT.Transform, "AxisZ");
                DrawConfigurableInput("Rotate", InputSettings.INPUT.Transform, "Rotate");
                DrawConfigurableInput("Scale", InputSettings.INPUT.Transform, "Scale");
                DrawConfigurableInput("Translate", InputSettings.INPUT.Transform, "Translate");
                DrawConfigurableInput("Use Translate Gizmo", InputSettings.INPUT.Transform, "TranslateGizmo");
                DrawConfigurableInput("Use Rotate Gizmo", InputSettings.INPUT.Transform, "RotateGizmo");
                DrawConfigurableInput("Use Scale Gizmo", InputSettings.INPUT.Transform, "ScaleGizmo");
                DrawConfigurableInput("Use Rectangle Gizmo", InputSettings.INPUT.Transform, "RectangleGizmo");
                ImGui.Columns(1);
            }
            if (ImGui.CollapsingHeader("Path Edit", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Columns(2);
                DrawConfigurableInput("Extrude", InputSettings.INPUT.Scene, "Extrude");
                DrawConfigurableInput("Create", InputSettings.INPUT.Scene, "Create");
                DrawConfigurableInput("Fill", InputSettings.INPUT.Scene, "Fill");
                DrawConfigurableInput("Insert", InputSettings.INPUT.Scene, "Insert");
                DrawConfigurableInput("EditMode", InputSettings.INPUT.Scene, "EditMode");
                ImGui.Columns(1);
            }
        }

        static void DrawConfigurableInput(string label, object obj, string prop)
        {
            var field = obj.GetType().GetField(prop);
            var value = field.GetValue(obj) as string;

            string[] args = value.Split("+");
            string key = args.LastOrDefault();
            bool isControl = args.Contains("Ctrl");
            bool isShift = args.Contains("Shift");
            bool isAlt = args.Contains("Alt");

            bool isActive = label == active_edit;

            var width = ImGui.GetColumnWidth();
            ImGui.Text($"{label}"); ImGui.NextColumn();

            if (!isActive)
            {
                bool pressed = ImGui.Button($"{value}##INPUT_{label}", new System.Numerics.Vector2(width, 23)); ImGui.NextColumn();
                if (pressed)
                    active_edit = label;
            }
            else
            {
                bool pressed = ImGui.Button("", new System.Numerics.Vector2(width, 23)); ImGui.NextColumn();
                if (KeyEventInfo.State.HasKeyDown())
                {
                    string input = value;
                    //Check for the active key input
                    string newInput = KeyEventInfo.State.KeyChars.FirstOrDefault();
                    if (!string.IsNullOrEmpty(newInput)) {
                        input = newInput;
                        if (KeyEventInfo.State.KeyCtrl)
                            input = $"Ctrl+{input}";
                    }

                    field.SetValue(obj, input);
                    //Disable active input
                    active_edit = "";
                }
            }
        }
    }
}
