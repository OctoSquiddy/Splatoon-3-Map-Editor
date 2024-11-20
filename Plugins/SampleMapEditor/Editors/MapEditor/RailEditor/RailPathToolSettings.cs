using GLFrameworkEngine;
using ImGuiNET;
using MapStudio.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMapEditor.LayoutEditor
{
    public class RailPathToolSettings : IToolWindowDrawer
    {
        RailEditor Editor;

        public RailPathToolSettings(RailEditor editor)
        {
            Editor = editor;
        }

        public void Render()
        {
            bool refreshScene = false;

            if (ImGui.CollapsingHeader("Action", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ImGui.Button($"                   {IconManager.ADD_ICON}                   "))
                {
                    Editor.CreateNewPathFromScratch();
                    refreshScene = true;
                }

                if (ImGui.Button($"                   {IconManager.COPY_ICON}                   "))
                {
                    Editor.CopySelected();
                }

                if (ImGui.Button($"                   {IconManager.PASTE_ICON}                   "))
                {
                    Editor.PasteSelected();
                }

                if (ImGui.Button($"                   {IconManager.DELETE_ICON}                   "))
                {
                    Editor.RemoveSelected();
                }
            }

            if (ImGui.CollapsingHeader(TranslationSource.GetText("LINEAR"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                refreshScene |= ImGui.InputFloat($"{TranslationSource.GetText("POINT_SIZE")}##vmenu11", ref RenderablePath.PointSize);
            }

            if (ImGui.CollapsingHeader(TranslationSource.GetText("BEZIER"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                refreshScene |= ImGui.InputFloat($"{TranslationSource.GetText("SIZE")}##vmenu12", ref RenderablePath.BezierPointScale);
                refreshScene |= ImGui.InputFloat($"{TranslationSource.GetText("LINE_WIDTH")}##vmenu13", ref RenderablePath.BezierLineWidth);
                refreshScene |= ImGui.InputFloat($"{TranslationSource.GetText("ARROW_LENGTH")}##vmenu14", ref RenderablePath.BezierArrowLength);
            }

            if (refreshScene)
                GLContext.ActiveContext.UpdateViewport = true;
        }
    }
}
