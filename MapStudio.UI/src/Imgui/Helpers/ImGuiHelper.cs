using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using ImGuiNET;
using System.Numerics;
using GLFrameworkEngine;
using System.Reflection;
using Toolbox.Core.ViewModels;

namespace MapStudio.UI
{
    public partial class ImGuiHelper
    {
        public class PropertyInfo
        {
            public bool CanDrag = false;

            public float Speed = 1.0f;
        }

        /// <summary>
        /// Draws a text label as bold.
        /// </summary>
        public static void BoldText(string text)
        {
            ImGuiHelper.BeginBoldText();
            ImGui.Text(text);
            ImGuiHelper.EndBoldText();
        }

        /// <summary>
        /// Draws a text label as bold with value text next to it.
        /// </summary>
        public static void BoldTextLabel(string key, string label)
        {
            ImGuiHelper.BeginBoldText();
            ImGui.Text($"{key}:");
            ImGuiHelper.EndBoldText();

            ImGui.SameLine();
            ImGui.TextColored(ImGui.GetStyle().Colors[(int)ImGuiCol.Text], label);
        }

        /// <summary>
        /// Makes any font used UI element as bold.
        /// </summary>
        public static void BeginBoldText() {
            ImGui.PushFont(ImGuiController.DefaultFontBold);
        }

        /// <summary>
        /// Closes the BeginBoldText()
        /// </summary>
        public static void EndBoldText() {
            ImGui.PopFont();
        }

        /// <summary>
        /// Creates a hyperlink visual with an underline.
        /// </summary>
        public static void HyperLinkText(string text)
        {
            var color = ThemeHandler.HyperLinkText;
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Text(text);

            var lineEnd = ImGui.GetItemRectMax();
            var lineStart = lineEnd;
            lineStart.X = ImGui.GetItemRectMin().X;
            ImGui.GetWindowDrawList().AddLine(lineStart, lineEnd, ImGui.ColorConvertFloat4ToU32(color));

            ImGui.PopStyleColor();
        }

        /// <summary>
        /// Creates a tooltip for the hovered item drawn before this is called.
        /// </summary>
        public static void Tooltip(string tooltip, string shortcut = "")
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text(TranslationSource.GetText(tooltip));
                ImGui.EndTooltip();
            }
        }

        /// <summary>
        /// Draws a menu item using a more portable menu item model from the Toolbox Core library.
        /// </summary>
        public static void LoadMenuItem(MenuItemModel item, bool alignFramePadding = true)
        {
            string header = item.Header;
            if (TranslationSource.HasKey(header))
                header = TranslationSource.GetText(header);

            if (item.Icon != null && IconManager.HasIcon(item.Icon)) {
                IconManager.DrawIcon(item.Icon);
            }

            if (string.IsNullOrEmpty(header)) {
                ImGui.Separator();
                return;
            }

            if (alignFramePadding)
                ImGui.AlignTextToFramePadding();

            bool opened = false;
            if (item.MenuItems.Count == 0)
            {
                if (ImGui.MenuItem(header, "", item.IsChecked, item.IsEnabled)) {
                    if (item.CanCheck)
                        item.IsChecked = !item.IsChecked;
                    UIManager.ActionExecBeforeUIDraw += delegate
                    {
                        item.Command.Execute(item);
                    };
                }
             
            }
            else {
                opened = ImGui.BeginMenu(header);
            }

            if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(item.ToolTip))
                ImGui.SetTooltip(TranslationSource.GetText(item.ToolTip));

            if (opened)
            {
                foreach (var child in item.MenuItems)
                    LoadMenuItem(child);

                ImGui.EndMenu();
            }
        }

        /// <summary>
        /// Increases the cursor position on the X direction.
        /// </summary>
        public static void IncrementCursorPosX(float amount) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + amount);
        }

        /// <summary>
        /// Increases the cursor position on the Y direction.
        /// </summary>
        public static void IncrementCursorPosY(float amount) {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + amount);
        }

        public static void DrawCenteredText(string text)
        {
            float windowWidth = ImGui.GetWindowSize().X;
            float textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        /// <summary>
        /// Draws a menu item. Must be inside a current popup or menubar.
        /// </summary>
        public static void DrawMenuItem(UIFramework.MenuItem item, bool alignFramePadding = true)
        {
            string header = item.Header;

            if (item.Icon?.Length == 1)
                header = $"    {item.Icon}    {header}";

            if (string.IsNullOrEmpty(header))
            {
                ImGui.Separator();
                return;
            }

            if (alignFramePadding)
                ImGui.AlignTextToFramePadding();

            bool opened = false;
            if (item.MenuItems.Count == 0 && item.RenderItems == null)
            {
                if (ImGui.MenuItem(header, item.Shortcut, item.IsChecked, item.Enabled))
                {
                    if (item.CanCheck)
                        item.IsChecked = !item.IsChecked;
                    MapStudio.UI.UIManager.ActionExecBeforeUIDraw += delegate
                    {
                        item.Execute();
                    };
                }
            }
            else
            {
                opened = ImGui.BeginMenu(header, item.Enabled);
            }

            if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(item.ToolTip))
                ImGui.SetTooltip(item.ToolTip);

            if (opened)
            {
                if (item.RenderItems != null)
                    item.RenderItems();

                foreach (var child in item.MenuItems)
                    DrawMenuItem(child, alignFramePadding);

                ImGui.EndMenu();
            }
        }
    }
}
