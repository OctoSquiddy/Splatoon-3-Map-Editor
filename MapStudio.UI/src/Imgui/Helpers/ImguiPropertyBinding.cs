using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ImGuiNET;
using System.Numerics;

namespace MapStudio.UI
{
    public partial class ImGuiHelper
    {
        public static bool InputFromInt(string label, object obj, string properyName, int step = 1, bool drag = true)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (int)input.GetValue(obj);

            bool edited = drag ? ImGui.DragInt(label, ref inputValue, step) :
                                 ImGui.InputInt(label, ref inputValue, step);
            if (edited)
            {
                input.SetValue(obj, (int)inputValue);
            }
            return edited;
        }

        public static bool InputFromUint(string label, object obj, string properyName, int step = 1, bool drag = true)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (int)((uint)input.GetValue(obj));

            bool edited = drag ? ImGui.DragInt(label, ref inputValue, step, 0) :
                                 ImGui.InputInt(label, ref inputValue, step, 0);
            if (edited)
            {
                input.SetValue(obj, (uint)inputValue);
            }
            return edited;
        }

        public static void InputFromShort(string label, object obj, string properyName, float step = 1)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (int)((short)input.GetValue(obj));

            bool edited = ImGui.DragInt(label, ref inputValue, 1, 0, 255);
            if (edited)
            {
                input.SetValue(obj, (short)inputValue);
            }
        }


        public static bool InputFromByte(string label, object obj, string properyName, float step = 1)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (int)((byte)input.GetValue(obj));

            bool edited = ImGui.DragInt(label, ref inputValue, 1, 0, 255);
            if (edited)
            {
                input.SetValue(obj, (byte)inputValue);
                return true;
            }
            return false;
        }


        public static bool InputFromFloat(string label, object obj, string properyName, bool drag = false, float step = 0.1f, float v_min = float.MinValue, float v_max = float.MaxValue)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float)input.GetValue(obj);

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat(label, ref inputValue, step, v_min, v_max);
            else
                edited = ImGui.InputFloat(label, ref inputValue, step);

            if (edited)
            {
                input.SetValue(obj, inputValue);
            }
            return edited;
        }

        public static bool InputFromVector4(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = input.GetValue(obj);

            Vector4 vec = (Vector4)inputValue;

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat4(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat4(label, ref vec);

            if (edited)
            {
                input.SetValue(obj, vec);
            }
            return edited;
        }

        public static bool InputFromVector3(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = input.GetValue(obj);

            Vector3 vec = (Vector3)inputValue;

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat3(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat3(label, ref vec);

            if (edited)
            {
                input.SetValue(obj, vec);
            }
            return edited;
        }

        public static bool InputTKVector2(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = input.GetValue(obj);

            var vec = new Vector2(
                    ((OpenTK.Vector2)inputValue).X,
                    ((OpenTK.Vector2)inputValue).Y);

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat2(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat2(label, ref vec);

            if (edited)
                input.SetValue(obj, new OpenTK.Vector2(vec.X, vec.Y));

            return edited;
        }

        public static bool InputFromVector2(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = input.GetValue(obj);

            var vec = new Vector2(0);
            if (inputValue is Syroot.Maths.Vector2F)
                vec = new Vector2(
                    ((Syroot.Maths.Vector2F)inputValue).X,
                    ((Syroot.Maths.Vector2F)inputValue).Y);
            if (inputValue is Vector2)
                vec = (Vector2)inputValue;

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat2(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat2(label, ref vec);

            if (edited)
            {
                if (inputValue is Vector2)
                    input.SetValue(obj, vec);
                else
                    input.SetValue(obj, new Syroot.Maths.Vector2F(vec.X, vec.Y));
            }
            return edited;
        }

        public static bool InputFloatsFromVector2(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector2(inputValue[0], inputValue[1]);

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat2(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat2(label, ref vec);

            if (edited)
            {
                input.SetValue(obj, new float[2] { vec.X, vec.Y });
                return true;
            }
            return false;
        }

        public static bool InputFloatsFromVector3(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector3(inputValue[0], inputValue[1], inputValue[2]);

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat3(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat3(label, ref vec);

            if (edited)
            {
                input.SetValue(obj, new float[3] { vec.X, vec.Y, vec.Z });
                return true;
            }
            return false;
        }

        public static bool InputTKVector3(string label, object obj, string properyName, float speed = 1)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (OpenTK.Vector3)input.GetValue(obj);
            var vec = new Vector3(inputValue[0], inputValue[1], inputValue[2]);

            float size = ImGui.GetFontSize();
            if (ImGui.DragFloat3(label, ref vec, speed))
            {
                input.SetValue(obj, new OpenTK.Vector3(vec.X, vec.Y, vec.Z));
                return true;
            }
            return false;
        }

        public static bool InputTKVector4(string label, object obj, string properyName)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (OpenTK.Vector4)input.GetValue(obj);
            var vec = new Vector4(inputValue[0], inputValue[1], inputValue[2], inputValue[3]);

            float size = ImGui.GetFontSize();
            if (ImGui.DragFloat4(label, ref vec))
            {
                input.SetValue(obj, new OpenTK.Vector4(vec.X, vec.Y, vec.Z, vec.W));
                return true;
            }
            return false;
        }


        public static bool InputTKVector3Color3(string label, object obj, string properyName, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (OpenTK.Vector3)input.GetValue(obj);
            var vec = new Vector3(inputValue[0], inputValue[1], inputValue[2]);

            float size = ImGui.GetFontSize();
            if (ImGui.ColorEdit3(label, ref vec, flags))
            {
                input.SetValue(obj, new OpenTK.Vector3(vec.X, vec.Y, vec.Z));
                return true;
            }
            return false;
        }

        public static void InputVector4Color4(string label, object obj, string properyName, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var vec = (Vector4)input.GetValue(obj);

            float size = ImGui.GetFontSize();
            if (ImGui.ColorEdit4(label, ref vec, flags))
            {
                input.SetValue(obj, vec);
            }
        }

        public static bool InputTKVector4Color4(string label, object obj, string properyName, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (OpenTK.Vector4)input.GetValue(obj);
            var vec = new Vector4(inputValue[0], inputValue[1], inputValue[2], inputValue[3]);

            float size = ImGui.GetFontSize();
            if (ImGui.ColorEdit4(label, ref vec, flags))
            {
                input.SetValue(obj, new OpenTK.Vector4(vec.X, vec.Y, vec.Z, vec.W));
                return true;
            }
            return false;
        }

        public static bool InputFloatsFromColor3Button(string label, object obj, string properyName, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector4(inputValue[0], inputValue[1], inputValue[2], 1.0f);

            float size = ImGui.GetFontSize();
            if (ImGui.ColorButton(label, vec, flags, new Vector2(size, size)))
            {
                input.SetValue(obj, new float[3] { vec.X, vec.Y, vec.Z });
                return true;
            }
            return false;
        }

        public static bool InputFloatsFromColor4Button(string label, object obj, string properyName, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector4(inputValue[0], inputValue[1], inputValue[2], inputValue[3]);

            float size = ImGui.GetFontSize();
            if (ImGui.ColorButton(label, vec, flags, new Vector2(size, size)))
            {
                input.SetValue(obj, new float[4] { vec.X, vec.Y, vec.Z, vec.W });
                return true;
            }
            return false;
        }

        public static bool InputFloatsFromVector4(string label, object obj, string properyName, bool drag = false)
        {
            var input = obj.GetType().GetProperty(properyName);

            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector4(inputValue[0], inputValue[1], inputValue[2], inputValue[3]);

            bool edited = false;
            if (drag)
                edited = ImGui.DragFloat4(label, ref vec, 0.1f);
            else
                edited = ImGui.InputFloat4(label, ref vec);

            if (edited)
            {
                input.SetValue(obj, new float[4] { vec.X, vec.Y, vec.Z, vec.W });
                return true;
            }
            return false;
        }


        public static bool InputFloatsFromColor3(string label, object obj, string properyName)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector3(inputValue[0], inputValue[1], inputValue[2]);
            if (ImGui.ColorEdit3(label, ref vec, ImGuiColorEditFlags.HDR | ImGuiColorEditFlags.Float))
            {
                input.SetValue(obj, new float[3] { vec.X, vec.Y, vec.Z });
                return true;
            }
            return false;
        }

        public static bool InputFloatsFromColor4(string label, object obj, string properyName, ImGuiColorEditFlags flags = ImGuiColorEditFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (float[])input.GetValue(obj);
            var vec = new Vector4(inputValue[0], inputValue[1], inputValue[2], inputValue[3]);
            if (ImGui.ColorEdit4(label, ref vec, flags | ImGuiColorEditFlags.HDR | ImGuiColorEditFlags.Float))
            {
                input.SetValue(obj, new float[4] { vec.X, vec.Y, vec.Z, vec.W });
                return true;
            }
            return false;
        }

        public static bool InputFromText(string label, object obj, string properyName, int bufferLength,
            ImGuiInputTextFlags exflags = ImGuiInputTextFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (string)input.GetValue(obj);

            var flags = exflags | ImGuiInputTextFlags.AutoSelectAll;
            if (ImGui.InputText(label, ref inputValue, (uint)bufferLength + 1, flags))
            {
                input.SetValue(obj, inputValue);
                return true;
            }

            return false;
        }

        public static bool InputFromBoolean(string label, object obj, string properyName)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = (bool)input.GetValue(obj);

            if (ImGui.Checkbox(label, ref inputValue))
            {
                input.SetValue(obj, inputValue);
                return true;
            }
            return false;
        }

        public static bool ComboFromEnum<T>(string label, object obj, string properyName, ImGuiComboFlags flags = ImGuiComboFlags.None)
        {
            var input = obj.GetType().GetProperty(properyName);
            var inputValue = input.GetValue(obj);

            if (TranslationSource.HasKey(label))
                label = TranslationSource.GetText(label);

            bool edited = false;
            if (ImGui.BeginCombo(label, inputValue.ToString(), flags))
            {
                var values = Enum.GetValues(typeof(T));
                foreach (var val in values)
                {
                    bool isSelected = inputValue == val;
                    string cblabel = val.ToString();
                    if (TranslationSource.HasKey(cblabel))
                        cblabel = TranslationSource.GetText(cblabel);

                    if (ImGui.Selectable(cblabel, isSelected))
                    {
                        input.SetValue(obj, val);
                        edited = true;
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            return edited;
        }

    }
}
