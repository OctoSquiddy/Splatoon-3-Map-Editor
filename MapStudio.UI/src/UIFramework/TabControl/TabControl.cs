using System;
using System.Linq;
using System.Collections.Generic;
using ImGuiNET;

namespace MapStudio.UI
{
    public class TabControl
    {
        public List<TabPage> Pages = new List<TabPage>();

        public TabPage ActivePage = null;

        public uint VerticalTabWidth = 150;

        public Mode TabMode { get; set; } = Mode.Vertical_Tabs;

        private string ID;

        public TabControl(string id)
        {
            ID = id;
        }

        public virtual void Render()
        {
            //ImGuiHelper.ComboFromEnum< Mode>("Tab Mode", this, "TabMode");

            switch (TabMode)
            {
                case Mode.Horizontal_Tabs: DrawHorizontalTabs(); break;
                case Mode.Vertical_Tabs: DrawVerticalTabs(); break;
                case Mode.Header: DrawHeaderTabs(); break;
            }
        }

        private void DrawHorizontalTabs()
        {
            ImGui.BeginTabBar(ID);

            foreach (var page in this.Pages)
            {
                if (ImguiCustomWidgets.BeginTab(ID, page.Name))
                {
                    page.Render();
                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();
        }

        private void DrawVerticalTabs()
        {
            if (ActivePage == null)
                ActivePage = Pages.FirstOrDefault();


            ImGui.BeginColumns(ID + "columns", 2);

            ImGui.SetColumnWidth(0, VerticalTabWidth);

            ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]);
            if (ImGui.BeginChild(ID + "tabList"))
            {
                foreach (var page in this.Pages)
                {
                    if (ImGui.Selectable(page.Name, ActivePage == page))
                        ActivePage = page;
                    if (ImGui.IsItemFocused() && ActivePage != page)
                        ActivePage = page;
                }
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();

            ImGui.NextColumn();

            if (ImGui.BeginChild(ID + "properties"))
            {
                if (ActivePage != null)
                    ActivePage.Render();
            }
            ImGui.EndChild();

            ImGui.NextColumn();

            ImGui.EndColumns();
        }

        private void DrawHeaderTabs()
        {
            ImGui.BeginColumns(ID + "columns", 2);

            foreach (var page in this.Pages)
            {
                ImGui.Checkbox($"{page.Name}##{page.Name}chk", ref page.Visible);
                ImGui.NextColumn();
            }

            ImGui.EndColumns();

            foreach (var page in this.Pages)
            {
                if (page.Visible && ImGui.CollapsingHeader(page.Name, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (ImGui.BeginChild(ID + $"{page.Name}properties"))
                    {
                        page.Render();
                    }
                    ImGui.EndChild();
                }
            }
        }

        public enum Mode
        {
            Horizontal_Tabs,
            Vertical_Tabs,
            Header,
        }
    }

    public class TabPage
    {
        public string Name { get; set; }
        Action OnRender { get; set; }

        public bool Visible = true;

        public TabPage(string name, Action render) {
            Name = name;
            OnRender = render;
        }

        public virtual void Render()
        {
            OnRender?.Invoke();
        }
    }
}
