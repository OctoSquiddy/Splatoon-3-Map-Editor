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
    public class MapObjectUI
    {
        static bool DisplayUnusedParams = false;

        public void Render(MuElement mapObject, IEnumerable<object> selected)
        {
            MapStudio.UI.ImguiBinder.LoadProperties(mapObject, selected);
        }

        public void RenderRail(MuRail mapObject, IEnumerable<object> selected)
        {
            MapStudio.UI.ImguiBinder.LoadProperties(mapObject, selected);
        }

        public void RenderRailPoint(MuRailPoint mapObject, IEnumerable<object> selected)
        {
            MapStudio.UI.ImguiBinder.LoadProperties(mapObject, selected);
        }

        private void DisplayLinkUI(string text, MuElement mapObject, string properyName)
        {
            EventHandler onLink = (sender, e) => {
                mapObject.NotifyPropertyChanged(properyName);
            };

            ImguiCustomWidgets.ObjectLinkSelector(TranslationSource.GetText(text), mapObject, properyName, onLink);
        }

        private IDrawable GetDrawableLink(object obj)
        {
            foreach (var render in GLContext.ActiveContext.Scene.Objects)
            {
                if (render is IRenderNode)
                {
                    var tag = ((IRenderNode)render).UINode.Tag;
                    if (tag == null || obj != tag)
                        continue;

                    return render;
                }
            }
            return null;
        }

        private bool DisplayFloat(string id, string name, ref float value)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text(name);
            ImGui.NextColumn();
            bool input = ImGui.InputFloat($"###{id}", ref value);
            ImGui.NextColumn();
            return input;
        }
    }
}
