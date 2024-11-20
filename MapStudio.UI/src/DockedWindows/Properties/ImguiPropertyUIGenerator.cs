using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using ImGuiNET;
using System.Numerics;
using System.Reflection;
using Toolbox.Core;
using MapStudio.UI;
using Toolbox.Core.Animations;
using OpenTK.Audio.OpenAL;

namespace MapStudio.UI
{
    public partial class ImguiBinder
    {
        static IEnumerable<object> SelectedObjects;

        public static void LoadProperties(object obj, IEnumerable<object> selected)
        {
            SelectedObjects = selected;
            LoadProperties(obj, OnPropertyChanged, false);
        }

        public static void LoadProperties(object obj, EventHandler propertyChanged = null, bool isSubClass = false)
        {
            if (obj == null)
                return;

            Dictionary<string, bool> categories = new Dictionary<string, bool>();

            var style = ImGui.GetStyle();
            var frameSize = style.FramePadding;
            var itemSpacing = style.ItemSpacing;

            style.ItemSpacing = (new Vector2(itemSpacing.X, 4));
            style.FramePadding = (new Vector2(frameSize.X, 4));

            LoadPropertieList(obj, categories, propertyChanged);

            style.FramePadding = frameSize;
            style.ItemSpacing = itemSpacing;

            ImGui.Columns(1);
        }

        public static void LoadPropertieList(object obj, Dictionary<string, bool> categories, EventHandler propertyChanged = null)
        {
            if (obj == null)
                return;

            var properties = obj.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var bindableAttribute = properties[i].GetCustomAttribute<BindGUI>();
                if (bindableAttribute == null)
                    continue;

                if (IsSubProperty(properties[i]))
                {
                    LoadPropertieList(properties[i].GetValue(obj), categories, propertyChanged);
                }
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var bindableAttribute = properties[i].GetCustomAttribute<BindGUI>();
                if (bindableAttribute == null || IsSubProperty(properties[i]))
                    continue;


                var columnProperties = GetRowProperties(properties, bindableAttribute, i);

                LoadProperties(obj, columnProperties, categories, propertyChanged);

                //Increase the amount of properties already used by this row
                i += (columnProperties.Length - 1);
            }
        }

        static void LoadProperties(object obj, PropertyInfo[] properties, Dictionary<string, bool> categories, EventHandler propertyChanged)
        {
            string category = "PROPERTIES";

            int numColumns = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                var bindGUI = properties[i].GetCustomAttribute<BindGUI>();
                if (properties[i].PropertyType == typeof(EventHandler))
                    numColumns += 1;
                else if (bindGUI.Control == BindControl.Default || bindGUI.Control == BindControl.ComboBox
                    || bindGUI.Control == BindControl.ColorBox)
                    numColumns += 2;
                else
                    numColumns += 1;

                if (bindGUI.Category != null)
                    category = bindGUI.Category;

                if (TranslationSource.HasKey(category))
                    category = TranslationSource.GetText(category);

                if (!categories.ContainsKey(category))
                {
                    bool open = ImGui.CollapsingHeader(category, ImGuiTreeNodeFlags.DefaultOpen);
                    categories.Add(category, open);
                }
            }

            float width = ImGui.GetWindowWidth();
            ImGui.BeginColumns("##" + category + numColumns.ToString(), numColumns);

            for (int i = 0; i < properties.Length; i++)
            {
                var bindGUI = properties[i].GetCustomAttribute<BindGUI>();
                if (bindGUI == null || IsSubProperty(properties[i]))
                    return;

                string label = !string.IsNullOrEmpty(bindGUI.Label) ? bindGUI.Label : properties[i].Name;
                string desc = !string.IsNullOrEmpty(bindGUI.Category) ? bindGUI.Category : properties[i].Name;
                bool readOnly = !properties[i].CanWrite;

                if (!categories[category])
                    continue;

                if (TranslationSource.HasKey(label))
                    label = TranslationSource.GetText(label);

                if (properties[i].PropertyType == typeof(EventHandler))
                {
                    var inputValue = (EventHandler)properties[i].GetValue(obj);
                    inputValue?.Invoke(null, EventArgs.Empty);
                }
                else
                {
                    if (bindGUI.Control == BindControl.Default || bindGUI.Control == BindControl.ComboBox
                        || bindGUI.Control == BindControl.ColorBox)
                    {
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text(label);
                        ImGui.NextColumn();
                    }

                    float colwidth = ImGui.GetColumnWidth();
                    if (properties[i].PropertyType == typeof(OpenTK.Vector3))
                    {
                        ImGui.SetColumnOffset(1, width * 0.25f);
                    }

                    ImGui.PushItemWidth(colwidth - 6);

                    var valueEdit = SetPropertyUI(properties[i], bindGUI, obj, bindGUI.Control, label, desc, readOnly);
                    if (valueEdit.Value != null)
                        propertyChanged?.Invoke(valueEdit.Value, new PropertyChangedCustomArgs(properties[i], properties[i].Name, obj));

                    ImGui.PopItemWidth();

                    ImGui.NextColumn();
                }
            }
            ImGui.EndColumns();
        }

        static bool IsSubProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == null || propertyInfo.PropertyType == typeof(string))
                return false;

            return (propertyInfo.PropertyType.IsClass);
        }

        public class PropertyChangedCustomArgs : EventArgs
        {
            public PropertyInfo PropertyInfo;
            public string Name;
            public object Object;

            public PropertyChangedCustomArgs(PropertyInfo property, string name, object obj)
            {
                PropertyInfo = property;
                Name = name;
                Object = obj;
            }
        }

        static PropertyInfo[] GetRowProperties(PropertyInfo[] properties, BindGUI att, int startIndex)
        {
            List<PropertyInfo> rowProperties = new List<PropertyInfo>();
            rowProperties.Add(properties[startIndex]);

            int currentIndex = 0;
            for (int i = startIndex + 1; i < properties.Length; i++)
            {
                var columnAttribute = properties[i].GetCustomAttribute<BindGUI>();
                if (columnAttribute == null)
                    continue;

                if (currentIndex >= columnAttribute.ColumnIndex)
                    break;

                currentIndex = columnAttribute.ColumnIndex;
                rowProperties.Add(properties[i]);
            }
            return rowProperties.ToArray();
        }

        unsafe static PropertyEdit SetPropertyUI(PropertyInfo property, BindGUI mBindGUI,
            object obj, BindControl control, string label, string desc, bool readOnly)
        {
            PropertyEdit propertyEdit = new PropertyEdit();
            propertyEdit.Value = null;

            var flags = ImGuiInputTextFlags.None;
            if (readOnly)
                flags |= ImGuiInputTextFlags.ReadOnly;

            string text = label;
            label = $"###{property.Name}";

            Type type = property.PropertyType;
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                type = nullableType;
            object value = property.GetValue(obj);

            if (readOnly)
            {
                var disabled = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];
                ImGui.PushStyleColor(ImGuiCol.Text, disabled);
            }

            if (control == BindControl.ComboBox)
            {
                IDictionary<object, string> ComboBoxData = mBindGUI.ComboBoxData;

                var inputValue = property.GetValue(obj);
                string cbCurrentlabel = inputValue != null ? ComboBoxData.ContainsKey(inputValue) ? ComboBoxData[inputValue] : "" : "";

                if (ImGui.BeginCombo(label, cbCurrentlabel, ImGuiComboFlags.HeightLarge))
                {
                    if (!readOnly)
                    {
                        foreach (KeyValuePair<object, string> kvp in ComboBoxData)
                        {
                            bool isSelected = inputValue == kvp.Key;

                            if (ImGui.Selectable(kvp.Value, isSelected))
                            {
                                propertyEdit.Value = kvp.Key;
                            }
                            if (isSelected)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }
                    }

                    ImGui.EndCombo();
                }

                if (readOnly)
                {
                    ImGui.PopStyleColor();
                }

                return propertyEdit;
            }

            if (control == BindControl.ColorBox)
            {
                Vector4 inputValue = value == null ? new Vector4() : (Vector4)value;

                if (ImGui.ColorEdit4(label, ref inputValue))
                {
                    propertyEdit.Value = inputValue;
                }

                if (readOnly)
                {
                    ImGui.PopStyleColor();
                }

                return propertyEdit;
            }

            if (type.IsEnum)
            {
                var inputValue = property.GetValue(obj);
                string cbCurrentlabel = inputValue != null ? inputValue.ToString() : "";
                if (TranslationSource.HasKey(cbCurrentlabel))
                    cbCurrentlabel = TranslationSource.GetText(cbCurrentlabel);

                if (ImGui.BeginCombo(label, cbCurrentlabel, ImGuiComboFlags.HeightLarge))
                {
                    if (!readOnly)
                    {
                        var values = Enum.GetValues(type);
                        foreach (var val in values)
                        {
                            bool isSelected = inputValue == val;
                            string cblabel = val.ToString();
                            if (IconManager.HasIcon(cblabel)) {
                                IconManager.DrawIcon(cblabel, 22); ImGui.SameLine();
                            }

                            if (TranslationSource.HasKey(cblabel))
                                cblabel = TranslationSource.GetText(cblabel);

                            if (ImGui.Selectable(cblabel, isSelected))
                            {
                                propertyEdit.Value = val;
                            }

                            if (isSelected)
                                ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }
            }
            if (type == typeof(string))
            {
                var inputValue = (string)property.GetValue(obj);
                if (string.IsNullOrEmpty(inputValue))
                    inputValue = " ";

                if (ImGui.InputText(label, ref inputValue, 0x1000, flags))
                {
                    propertyEdit.Value = inputValue;
                }
            }
            if (type == typeof(float))
            {
                float inputValue = value == null ? 0.0f : (float)value;
                if (ImGui.InputFloat(label, ref inputValue, 0, 0))
                {
                    propertyEdit.Value = (float)inputValue;
                }
            }
            if (type == typeof(uint))
            {
                uint inputValue = value == null ? 0 : (uint)value;
                uint* longptr = &inputValue;

                if (ImGui.InputScalar(label, ImGuiDataType.U32, (IntPtr)longptr))
                {
                    propertyEdit.Value = (uint)inputValue;
                }
            }
            if (type == typeof(long))
            {
                long inputValue = value == null ? 0 : (long)value;
                long* longptr = &inputValue;

                if (ImGui.InputScalar(label, ImGuiDataType.S64, (IntPtr)longptr))
                {
                    propertyEdit.Value = (long)inputValue;
                }
            }
            if (type == typeof(ulong))
            {
                ulong inputValue = value == null ? 0 : (ulong)value;
                ulong* ulongptr = &inputValue;

                if (ImGui.InputScalar(label, ImGuiDataType.U64, (IntPtr)ulongptr))
                {
                    propertyEdit.Value = (ulong)inputValue;
                }
            }
            if (type == typeof(int))
            {
                int inputValue = value == null ? 0 : (int)value;
                if (ImGui.InputInt(label, ref inputValue, 0, 0, flags))
                {
                    propertyEdit.Value = (int)inputValue;
                }
            }
            if (type == typeof(bool))
            {
                bool inputValue = value == null ? false : (bool)value;
                if (control == BindControl.ToggleButton)
                {
                    var enableColor = ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered];
                    var disableColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];

                    Vector2 size = new Vector2(ImGui.GetColumnWidth() - 8, 25);
                    if (ImguiCustomWidgets.ColorButtonToggle(text, ref inputValue,
                        disableColor,
                        enableColor,
                        size))
                    {
                        propertyEdit.Value = (bool)inputValue;
                    }
                }
                else
                {
                    if (ImGui.Checkbox(label, ref inputValue))
                    {
                        propertyEdit.Value = (bool)inputValue;
                    }
                }
            }
            if (type == typeof(OpenTK.Vector3))
            {
                var inputValue = (OpenTK.Vector3)property.GetValue(obj);
                var vec3 = new Vector3(inputValue.X, inputValue.Y, inputValue.Z);
                //Apply preview scale for transform types
            /*    if (obj is GLFrameworkEngine.GLTransform && property.Name == "Position")
                    vec3 /= GLFrameworkEngine.GLContext.PreviewScale;
                */
                if (ImGui.DragFloat3(label, ref vec3))
                {
                    var output = new OpenTK.Vector3(vec3.X, vec3.Y, vec3.Z);
                    /*if (obj is GLFrameworkEngine.GLTransform && property.Name == "Position")
                        output *= GLFrameworkEngine.GLContext.PreviewScale;
                    */
                    propertyEdit.Value = output;
                }
                if (ImGui.IsItemActivated())
                    ImGuiController.HideCursorInfiniteScroll();
                if (ImGui.IsItemDeactivated())
                    ImGuiController.SetNormalCursor();
            }
            if (type == typeof(Vector4))
            {
                Vector4 inputValue = value == null ? new Vector4() : (Vector4)value;

                if (ImGui.ColorEdit4(label, ref inputValue))
                {
                    propertyEdit.Value = inputValue;
                }

                if (readOnly)
                {
                    ImGui.PopStyleColor();
                }

                return propertyEdit;
            }
            if (readOnly)
            {
                ImGui.PopStyleColor();
            }

            return propertyEdit;
        }

        class PropertyEdit
        {
            public object Value = null;
        }

        static void OnPropertyChanged(object sender, EventArgs e)
        {
            var handler = (ImguiBinder.PropertyChangedCustomArgs)e;
            //Apply the property
            handler.PropertyInfo.SetValue(handler.Object, sender);

            //Batch editing for selected
            foreach (var node in SelectedObjects)
            {
                if (node.GetType() != handler.Object.GetType())
                    continue;

                var tag = node;
                var type = tag.GetType().GetProperty(handler.Name);
                type.SetValue(tag, sender);
            }
        }
    }
}
