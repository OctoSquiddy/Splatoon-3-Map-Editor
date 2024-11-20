using System;
using System.Numerics;
using System.Linq;
using ImGuiNET;
using Toolbox.Core;
using UIFramework;

namespace MapStudio.UI
{
    public class UVWindow : DockWindow
    {
        public override string Name => "UV Window";

        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollbar;

        public UVViewport UVViewport = new UVViewport();

        private STGenericMaterial ActiveMaterial;

        public UVWindow(DockSpaceWindow parent) : base(parent)
        {
            Opened = false;
            UVViewport.Camera.Zoom = 30;
        }

        public void Load(STGenericMesh mesh) {
            Load(mesh.GetMaterials().FirstOrDefault());
        }

        public void Load(STGenericMaterial material)
        {
            ActiveMaterial = material;

            //Add meshes
            UVViewport.ActiveObjects.Clear();
            //Update UVs
            UVViewport.UpdateVertexBuffer = true;

            if (material != null)
            {
                var meshes = material.GetMappedMeshes();
                if (meshes != null)
                {
                    foreach (var mesh in meshes)
                        UVViewport.ActiveObjects.Add(mesh);
                }
                //Apply texture map
                if (material.TextureMaps.Count > 0)
                    UVViewport.ActiveTextureMap = material.TextureMaps[0];
            }
        }

        public override void Render()
        {
            var width = ImGui.GetWindowWidth();
            var height = ImGui.GetWindowHeight();

            ImGui.BeginMenuBar();
            DrawMenuBar();
            ImGui.EndMenuBar();

            if (ImGui.BeginChild("uvViewportChild", new Vector2(width, height - 50), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                UVViewport.Render((int)width, (int)height);
            }
            ImGui.EndChild();
        }

        private void DrawMenuBar()
        {
            int[] channels = new int[3] { 0, 1, 2 };

            ImGui.PushItemWidth(100);
            ImguiCustomWidgets.ComboScrollable("##UVLayer", $"   {IconManager.LAYER_GROUP_ICON}    Channel {UVViewport.UvChannelIndex}", ref UVViewport.UvChannelIndex, channels, () => {
                UVViewport.UpdateVertexBuffer = true;
            }, ImGuiComboFlags.NoArrowButton);
            ImGui.PopItemWidth();

            if(ActiveMaterial != null)
            {
                ImGui.PushItemWidth(120);

                string texture = UVViewport.ActiveTextureMap == null ? "None" : UVViewport.ActiveTextureMap.Name;
                if (ImGui.BeginCombo("##TextureMap", $"   {IconManager.IMAGE_ICON}     {texture}", ImGuiComboFlags.NoArrowButton))
                {
                    foreach (var tex in ActiveMaterial.TextureMaps)
                    {
                        bool select = tex == UVViewport.ActiveTextureMap;
                        if (IconManager.HasIcon(tex.Name))
                            IconManager.DrawIcon(tex.Name);
                        else
                            ImGui.Text($"   {IconManager.IMAGE_ICON}    ");

                        ImGui.SameLine();

                        if (ImGui.Selectable(tex.Name, select))
                            UVViewport.ActiveTextureMap = tex;

                        if (select)
                            ImGui.SetItemDefaultFocus();
                    }
                    ImGui.EndCombo();
                }
                ImGui.PopItemWidth();
            }

            ImGui.PushItemWidth(100);
            ImGui.Text($"  {IconManager.SUN_ICON}  ");
            ImGui.DragFloat("##Brightness", ref UVViewport.Brightness, 0.01f, 0, 1);
            ImGui.PopItemWidth();
        }
    }
}
