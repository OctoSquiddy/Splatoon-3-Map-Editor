using System;
using System.Collections.Generic;
using System.Text;
using GLFrameworkEngine;
using ImGuiNET;
using OpenTK.Input;

namespace MapStudio.UI
{
    /// <summary>
    /// The input state of the application to determine what the current key/mouse state is.
    /// </summary>
    public class InputState
    {
        public static KeyEventInfo CreateKeyState()
        {
            var keyInfo = KeyEventInfo.State;
            keyInfo.KeyChars.Clear();
            keyInfo.KeyShift = ImGui.GetIO().KeyShift;
            keyInfo.KeyCtrl = ImGui.GetIO().KeyCtrl;
            keyInfo.KeyAlt = ImGui.GetIO().KeyAlt;

            int index = 0;
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (Keyboard.GetState().IsKeyDown(Key.A + index)) keyInfo.KeyChars.Add(c.ToString());
                index++;
            }
            index = 0;
            for (char c = '0'; c <= '9'; c++)
            {
                if (Keyboard.GetState().IsKeyDown(Key.Number0 + index)) keyInfo.KeyChars.Add(c.ToString());
                index++;
            }

            if (Keyboard.GetState().IsKeyDown(Key.Period)) keyInfo.KeyChars.Add("period");
            if (Keyboard.GetState().IsKeyDown(Key.Space)) keyInfo.KeyChars.Add("Space");
            if (Keyboard.GetState().IsKeyDown(Key.BackSpace)) keyInfo.KeyChars.Add("backspace");
            if (Keyboard.GetState().IsKeyDown(Key.Delete)) keyInfo.KeyChars.Add("Delete");
            if (Keyboard.GetState().IsKeyDown(Key.Tab)) keyInfo.KeyChars.Add("Tab");
            if (Keyboard.GetState().IsKeyDown(Key.Minus)) keyInfo.KeyChars.Add("-");
            if (Keyboard.GetState().IsKeyDown(Key.Plus)) keyInfo.KeyChars.Add("+");
            if (Keyboard.GetState().IsKeyDown(Key.KeypadDecimal)) keyInfo.KeyChars.Add("KeypadDecimal");

            for (int i = 0; i < 10; i++)
                if (Keyboard.GetState().IsKeyDown(Key.Keypad0 + i)) keyInfo.KeyChars.Add($"keypad{i}");

            return keyInfo;
        }

        public static MouseEventInfo CreateMouseState()
        {
            var mouseInfo = new MouseEventInfo();

            //Prepare info
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
                mouseInfo.RightButton = ButtonState.Pressed;
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                mouseInfo.LeftButton = ButtonState.Pressed;

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                mouseInfo.RightButton = ButtonState.Released;
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                mouseInfo.LeftButton = ButtonState.Released;

            if (ImGui.IsMouseDown(ImGuiMouseButton.Middle))
                mouseInfo.MiddleButton = ButtonState.Pressed;

            MouseState mouseState = Mouse.GetState();
            mouseInfo.WheelPrecise = mouseState.WheelPrecise;

            //Construct relative position
            var windowPos = ImGui.GetWindowPos();

            var pos = ImGui.GetIO().MousePos;
            pos = new System.Numerics.Vector2(pos.X - windowPos.X, pos.Y - windowPos.Y);

            if (ImGui.IsMousePosValid())
                mouseInfo.Position = new System.Drawing.Point((int)pos.X, (int)pos.Y);
            else
                mouseInfo.HasValue = false;

            return mouseInfo;
        }
    }
}
