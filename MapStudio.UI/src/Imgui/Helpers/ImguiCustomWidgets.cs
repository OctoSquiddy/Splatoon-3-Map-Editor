using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Numerics;
using GLFrameworkEngine.UI;
using GLFrameworkEngine;
using Toolbox.Core.ViewModels;

namespace MapStudio.UI
{
    public class ImguiCustomWidgets
    {
        static Dictionary<string, string> selectedTabMenus = new Dictionary<string, string>();

        public static bool EyeToggle(string label, ref bool value, Vector2 size)
        {
            var texColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
            if (!value)
                 texColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];

            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0));
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0));
            ImGui.PushStyleColor(ImGuiCol.Text, texColor);

            ImGui.Button(value ? "   \uf06e   " : "   \uf070   " + $"##{label}", size);
            bool clicked = ImGui.IsItemClicked(0);
            if (clicked)
                value = !value;

            ImGui.PopStyleColor(4);

            return clicked;
        }

        public static bool ToggleBox(string label, ref bool value)
        {
            var color = value ? new Vector4(0, 0.5f, 0, 1) : new Vector4(0.5f, 0, 0, 1);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, color);
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, color);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, color);

            bool edited = ImGui.Checkbox(label, ref value);

            ImGui.PopStyleColor(3);
            return edited;
        }

        public static void ColorDialog(Vector4 color, Action<Vector4> colorChanged)
        {
            Vector4 defaultColor = new Vector4(color.X, color.Y, color.Z, color.W);

            DialogHandler.Show("Color Dialog", () =>
            {
                if (ImGui.ColorPicker4("colorPicker", ref color, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreviewHalf))
                    colorChanged?.Invoke(color);

                var size = new Vector2(ImGui.GetWindowWidth() / 2, 23);
                if (ImGui.Button(TranslationSource.GetText("CANCEL"), size))
                    DialogHandler.ClosePopup(false);

                ImGui.SameLine();
                if (ImGui.Button(TranslationSource.GetText("OK"), size))
                    DialogHandler.ClosePopup(true);

            }, (ok) =>
            {
                if (!ok)
                    colorChanged?.Invoke(defaultColor);
            });
        }

        public static bool MenuItemTooltip(string name, string tooltip, string shortcut = "")
        {
            bool item = ImGui.MenuItem(name, shortcut);

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text(TranslationSource.GetText(tooltip));
                if (!string.IsNullOrEmpty(shortcut)) {
                    ImGui.Text($"Shortcut: {shortcut}");
                }
                ImGui.EndTooltip();
            }
            return item;
        }

        public static bool ComboScrollable<T>(string key, string text, ref T selectedItem, Action propertyChanged = null, ImGuiComboFlags flags = ImGuiComboFlags.None) {
            return ComboScrollable(key, text, ref selectedItem, Enum.GetValues(typeof(T)).Cast<T>(), propertyChanged, flags);
        }

        public static bool ComboScrollable<T>(string key, string text, ref T selectedItem, IEnumerable<T> items, Action propertyChanged = null, ImGuiComboFlags flags = ImGuiComboFlags.None)
        {
            if (ImGui.BeginCombo(key, text, flags)) //Check for combo box popup and add items
            {
                foreach (T item in items)
                {
                    bool isSelected = item.Equals(selectedItem);
                    if (ImGui.Selectable(item.ToString(), isSelected)) {
                        selectedItem = item;
                        propertyChanged?.Invoke();
                    }
                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();

                return true;
            }
            if (ImGui.IsItemHovered()) //Check for combo box hover
            {
                var delta = ImGui.GetIO().MouseWheel;
                if (delta < 0) //Check for mouse scroll change going up
                {
                    var list = items.ToList();
                    int index = list.IndexOf(selectedItem);
                    if (index < list.Count - 1)
                    { //Shift upwards if possible
                        selectedItem = list[index + 1];
                        propertyChanged?.Invoke();
                    }
                }
                if (delta > 0) //Check for mouse scroll change going down
                {
                    var list = items.ToList();
                    int index = list.IndexOf(selectedItem);
                    if (index > 0)
                    { //Shift downwards if possible
                        selectedItem = list[index - 1];
                        propertyChanged?.Invoke();

                        return true;
                    }
                }
            }
            return false;
        }

        public static TransformOutput Transform(OpenTK.Vector3 position, OpenTK.Vector3 rotation, OpenTK.Vector3 scale)
        {
            //To system numerics to use in imgui
            return Transform(new Vector3(position.X, position.Y, position.Z),
                             new Vector3(rotation.X, rotation.Y, rotation.Z),
                             new Vector3(scale.X, scale.Y, scale.Z));
        }

        public static TransformOutput Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            bool edited0 = ImGui.DragFloat3("Translate", ref position);
            bool edited1 = ImGui.DragFloat3("Rotation", ref rotation);
            bool edited2 = ImGui.DragFloat3("Scale", ref scale);


            return new TransformOutput()
            {
                Position = position,
                Rotation = rotation,
                Scale = scale,
                Edited = edited0 | edited1 | edited2,
            };
        }

        public static void Spinner(float radius, float thickness, int num_segments, float speed, Vector4 color)
        {
            var window = ImGui.GetCurrentWindow();
            if (window.SkipItems)
                return;

            var pos = ImGui.GetCursorPos();
            Vector2 size = new Vector2(radius * 2, radius * 2);
            var bb = new ImRect() { Min = pos, Max = pos + size };
            ImGui.ItemSize(bb);
            if (!ImGui.ItemAdd(bb, 0))
                return;

            float time = (float)ImGui.GetTime() * speed;
            window.DrawList.PathClear();

            int start = (int)Math.Abs(MathF.Sin(time) * (num_segments - 5));
            float a_min = MathF.PI * 2.0f * ((float)start) / (float)num_segments;
            float a_max = MathF.PI * 2.0f * ((float)num_segments - 3) / (float)num_segments;
            Vector2 center = new Vector2(pos.X + radius, pos.Y + radius);
            for (int i = 0; i < num_segments; i++)
            {
                float a = a_min + ((float)i / (float)num_segments) * (a_max - a_min);
                window.DrawList.PathLineTo(new Vector2(
                    center.X + MathF.Cos(a + time * 8) * radius,
                    center.Y + MathF.Sin(a + time * 8) * radius));
            }
            window.DrawList.PathStroke(ImGui.ColorConvertFloat4ToU32(color), false, thickness);
        }

        public static bool ObjectLinkSelector(string label, object obj, string propertyName, EventHandler onObjectLink = null)
        {
            return ObjectLinkSelector(label, obj, propertyName, GLContext.ActiveContext.Scene.Objects, onObjectLink);
        }

        public static bool ObjectLinkSelector(string label, object obj, string propertyName, IEnumerable<object> drawables, EventHandler onObjectLink = null)
        {
            bool edited = false;

            var prop = obj.GetType().GetProperty(propertyName);
            var val = prop.GetValue(obj);
            string name = FindRenderName(val, drawables);

            if (ImGui.Button($"  {IconManager.EYE_DROPPER_ICON}  ##{label}"))
            {
                EventHandler handler = null;

                GLContext.ActiveContext.PickingTools.UseEyeDropper = true;
                GLContext.ActiveContext.PickingTools.OnObjectPicked += handler = (sender, e) =>
                {
                    //Only call the event once so remove it after execute
                    GLContext.ActiveContext.PickingTools.OnObjectPicked -= handler;
                    //Check if the object is a node type to find the same tag
                    //Objects match via the data that they are attached to.
                    var picked = sender as ITransformableObject;
                    if (picked is IRenderNode)
                    {
                        var tag = ((IRenderNode)picked).UINode.Tag;
                        if (tag == null || obj == tag)
                            return;
                        //Make sure the tag property matches with the target property needed as linked
                        if (tag.GetType() == prop.PropertyType)
                        {
                            prop.SetValue(obj, tag);
                            onObjectLink?.Invoke(obj, EventArgs.Empty);
                        }
                    }
                };
            }
            //Set the tooltip
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(TranslationSource.GetText("EYE_DROPPER_TOOL"));

            ImGui.SameLine();
            if (ImGui.BeginCombo(label, name))
            {
                if (ImGui.Selectable(TranslationSource.GetText("NONE"), val == null))
                {
                    prop.SetValue(obj, null);
                    edited = true;
                    onObjectLink?.Invoke(obj, EventArgs.Empty);
                }

                int index = 0;
                foreach (var render in drawables)
                {
                    NodeBase node = null;

                    if (render is IRenderNode)
                        node = ((IRenderNode)render).UINode;
                    if (render is NodeBase)
                        node = ((NodeBase)render);

                    if (node != null)
                    {
                        if (node.Tag == null || obj == node.Tag)
                            continue;

                        var cbName = node.Header;
                        if (node.Tag.GetType() == prop.PropertyType)
                        {
                            bool isSelected = node.Tag == val;
                            if (ImGui.Selectable($"{cbName} ({index++})", isSelected))
                            {
                                prop.SetValue(obj, node.Tag);
                                edited = true;
                                onObjectLink?.Invoke(obj, EventArgs.Empty);
                            }
                        }
                    }
                }

                ImGui.EndCombo();
            }
            return edited;
        }

        static string FindRenderName(object obj, IEnumerable<object> drawables)
        {
            if (obj == null)
                return "";

            foreach (var render in drawables)
            {
                NodeBase node = null;

                if (render is IRenderNode)
                    node = ((IRenderNode)render).UINode;
                if (render is NodeBase)
                    node = ((NodeBase)render);

                if (node != null && obj == node.Tag)
                    return node.Header;
            }
            return "";
        }

        public class TransformOutput
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;

            public bool Edited;
        }

        public static bool BeginTab(string menuKey, string text)
        {
            //Keep track of multiple loaded menus
            if (!selectedTabMenus.ContainsKey(menuKey))
                selectedTabMenus.Add(menuKey, "");

            var disabled = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];
            var normal = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];

            if (selectedTabMenus[menuKey] == text)
                ImGui.PushStyleColor(ImGuiCol.Text, normal);
            else
                ImGui.PushStyleColor(ImGuiCol.Text, disabled);

            bool active = ImGui.BeginTabItem(text);
            if (active) { selectedTabMenus[menuKey] = text; }

            ImGui.PopStyleColor();
            return active;
        }

        public static void DragHorizontalSeperator(string name, float height, float width, float delta, float padding)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
            bool done = false;

            ImGui.InvisibleButton(name + "...hseperator", new Vector2(-1, padding));
            if (ImGui.IsItemActive()) {
                delta = ImGui.GetMouseDragDelta().Y;
                done = true;
            }
            ImGui.Text(name);
            ImGui.SameLine();
            ImGui.InvisibleButton(name + "...hseperator2", new Vector2(1, 13));
            if (ImGui.IsItemActive()) {
                delta = ImGui.GetMouseDragDelta().Y;
                done = true;
            }
            if (!done) {
                height = height + delta;
                delta = 0;
            }
            ImGui.PopStyleVar();
        }

        public static bool ButtonToggle(string label, ref bool isValue, Vector2 size)
        {
            if (isValue)
            {
                var selectionColor = ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered];
                ImGui.PushStyleColor(ImGuiCol.Button, selectionColor);
            }
            else
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4());

          /*  ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);*/

            bool clicked = ImGui.Button(label, size);
            ImGui.PopStyleColor(1);

            if (clicked)
            {
                isValue = !isValue;
                return true;
            }
            return false;
        }

        public static bool ColorButtonToggle(string label, ref bool isValue, Vector4 colorDisabled, Vector4 colorEnabled, Vector2 size)
        {
            Vector4 color = (isValue ? colorEnabled : colorDisabled);

            ImGui.PushStyleColor(ImGuiCol.Button, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);

            bool clicked = ImGui.Button(label, size);
            ImGui.PopStyleColor(3);

            if (clicked)
            {
                if (isValue) 
                    isValue = false;
                else
                    isValue = true;

                return true;
            }
            return false;
        }

        public static bool ImageButtonToggle(int imageTrue, int imageFalse, ref bool isValue, Vector2 size)
        {
            var ptr = (IntPtr)(isValue ? imageTrue : imageFalse);
            if (ImGui.ImageButton(ptr, size)) {
                if (isValue)
                    isValue = false;
                else
                    isValue = true;

                return true;
            }
            return false;
        }

        public unsafe bool CustomTreeNode(string label)
        {
            var style = ImGui.GetStyle();
            var storage = ImGui.GetStateStorage();

            uint id = ImGui.GetID(label);
            int opened = storage.GetInt(id, 0);
            float x = ImGui.GetCursorPosX();
            ImGui.BeginGroup();
            if (ImGui.InvisibleButton(label, new Vector2(-1, ImGui.GetFontSize() + style.FramePadding.Y * 2)))
            {
                opened = storage.GetInt(id, 0);
              //  opened = p_opened == p_opened;
            }
            bool hovered = ImGui.IsItemHovered();
            bool active = ImGui.IsItemActive();
            if (hovered || active)
            {
                var col = ImGui.GetStyle().Colors[(int)(active ? ImGuiCol.HeaderActive : ImGuiCol.HeaderHovered)];
                ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), ImGui.ColorConvertFloat4ToU32(col));
            }
            ImGui.SameLine();
            ImGui.ColorButton("color_btn", opened == 1 ? new Vector4(1,1,1,1) : new Vector4(1, 0, 0, 1));
            ImGui.SameLine();
            ImGui.Text(label);
            ImGui.EndGroup();
            if (opened == 1)
                ImGui.TreePush(label);
            return opened != 0;
        }

        public static bool PathSelector(string label, ref string path, bool isValid = true)
        {
            if (!System.IO.Directory.Exists(path))
                isValid = false;

            bool clicked = ImGui.Button($"  -  ##{label}");
            bool edited = false;

            ImGui.SameLine();
            if (!isValid)
            {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.5f, 0, 0, 1));
                ImGui.InputText(label, ref path, 500, ImGuiInputTextFlags.ReadOnly);
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0.5f, 0, 1));
                ImGui.InputText(label, ref path, 500, ImGuiInputTextFlags.ReadOnly);
                ImGui.PopStyleColor();
            }

            if (ImGui.BeginPopupContextItem($"{label}_clear", ImGuiPopupFlags.MouseButtonRight))
            {
                if (ImGui.MenuItem(TranslationSource.GetText("CLEAR")))
                {
                    path = "";
                    edited = true;
                }
                ImGui.EndPopup();
            }

            if (clicked)
            {
                var dialog = new ImguiFolderDialog();
                if (dialog.ShowDialog())
                {
                    path = dialog.SelectedPath;
                    return true;
                }
            }
            return edited;
        }

        public static bool FileSelector(string label, ref string path, string[] extensions, bool isValid = true)
        {
            if (!System.IO.File.Exists(path))
                isValid = false;

            bool clicked = ImGui.Button($"  -  ##{label}");
            bool edited = false;

            ImGui.SameLine();
            if (!isValid)
            {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.5f, 0, 0, 1)); 
                ImGui.InputText(label, ref path, 500, ImGuiInputTextFlags.ReadOnly);
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0.5f, 0, 1));
                ImGui.InputText(label, ref path, 500, ImGuiInputTextFlags.ReadOnly);
                ImGui.PopStyleColor();
            }


            if (ImGui.BeginPopupContextItem($"{label}_clear", ImGuiPopupFlags.MouseButtonRight))
            {
                if (ImGui.MenuItem(TranslationSource.GetText("CLEAR")))
                {
                    path = "";
                    edited = true;
                }
                ImGui.EndPopup();
            }

            if (clicked)
            {
                var dialog = new ImguiFileDialog();
                foreach (var ext in extensions)
                    dialog.AddFilter(ext, ext);
                if (dialog.ShowDialog())
                {
                    path = dialog.FilePath;
                    edited = true;
                }
            }
            return edited;
        }
    }
}
