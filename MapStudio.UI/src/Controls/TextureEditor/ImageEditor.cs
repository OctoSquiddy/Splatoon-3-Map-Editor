using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Toolbox.Core;
using GLFrameworkEngine;
using System.Numerics;

namespace MapStudio.UI
{
    public class ImageEditor
    {
        ImageEditorViewport ImageCanvas;

        public static ImageEditor imageEditor = new ImageEditor();

        public int SelectedChannelIndex = -1;

        public STGenericTexture ActiveTexture { get; set; }

        public static bool UseChannelComponents = true;

        public bool DisplayAlpha = true;

        public BackgroundType SelectedBackground = BackgroundType.Checkerboard;

        public enum BackgroundType
        {
            Checkerboard,
            Black,
            White,
            Custom,
        }

        public int currentArrayLevel = 0;
        public int currentMipLevel = 0;

        static float h = 600;

        private void Init() {
            ImageCanvas = new ImageEditorViewport(this);
            ImageCanvas.OnLoad();
            ImageCanvas.Camera.Zoom = 150;
        }

        public static void LoadEditor(STGenericTexture texture) {
            imageEditor.Render(texture);
        }

        public void Render(STGenericTexture texture) {
            if (ImageCanvas == null)
                Init();

            var size = ImGui.GetWindowSize();

            if (ActiveTexture != texture) {
                ActiveTexture = texture;
                currentMipLevel = 0;
                currentArrayLevel = 0;
            }

            var menuSize = new Vector2(22, 22);
            var propertyWindowSize = new Vector2(size.X, size.Y - h - 20);
            var canvasWindowSize = new Vector2(size.X, h - 20);

            if (ImGui.BeginChild("##IMAGE_TABMENU", propertyWindowSize, true)) {

                ImGui.BeginTabBar("image_menu");
                if (ImguiCustomWidgets.BeginTab("image_menu", "Properties")) {
                    ImguiBinder.LoadPropertiesComponentModelBase(texture.DisplayProperties, (o, e) =>
                    {
                        texture.DisplayPropertiesChanged?.Invoke(o, e);
                    });
                    ImGui.EndTabItem();
                }
                if (ImguiCustomWidgets.BeginTab("image_menu", "User Data"))
                {
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
            ImGui.EndChild();

            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.Separator]);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetStyle().Colors[(int)ImGuiCol.SeparatorHovered]);
            ImGui.Button("##hsplitter", new Vector2(-1, 2));
            if (ImGui.IsItemActive())
            {
                var deltaY = ImGui.GetIO().MouseDelta.Y;
                if (h - deltaY < size.Y - 22 && h - deltaY > 22)
                    h -= deltaY;
            }

            ImGui.PopStyleColor(2);

            if (ImGui.BeginChild("CANVAS_WINDOW", canvasWindowSize, false,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
           /*     if (ImGui.BeginMenuBar())
                {
                          if (ImGui.BeginMenu("File"))
                          {
                              ImGui.EndMenu();
                          }
                          if (ImGui.BeginMenu("Edit"))
                          {
                              ImGui.EndMenu();
                          }
                          if (ImGui.BeginMenu("View"))
                          {
                              ImGui.EndMenu();
                          }
                          if (ImGui.BeginMenu("Image"))
                          {
                              ImGui.EndMenu();
                          }
                          if (ImGui.BeginMenu("Adjustments"))
                          {
                              ImGui.EndMenu();
                          }
                         
     
                    ImGui.EndMenuBar();
                }*/

                //Make icon buttons invisible aside from the icon itself.
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4());
                {
                    //Draw icon bar
                  //  ImGui.ImageButton((IntPtr)IconManager.GetTextureIcon("SAVE_BUTTON"), menuSize);
                //    ImGui.SameLine();
                  //  ImGui.ImageButton((IntPtr)IconManager.GetTextureIcon("IMG_EDIT_BUTTON"), menuSize);
                  //  ImGui.SameLine();
                    ImguiCustomWidgets.ImageButtonToggle(
                        IconManager.GetTextureIcon("IMG_ALPHA_BUTTON"),
                        IconManager.GetTextureIcon("IMG_NOALPHA_BUTTON"), ref DisplayAlpha, menuSize);
                    ImGui.SameLine();
                }

                ImGui.PushItemWidth(150);
                ImguiCustomWidgets.ComboScrollable("##imageCB", SelectedBackground.ToString(), ref SelectedBackground);
                ImGui.PopItemWidth();

                ImGui.PopStyleColor();

                //Draw the array and mip level counter buttons
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Array Level " + $"{currentArrayLevel} / {texture.ArrayCount - 1}");
                ImGui.SameLine();
                if (ImGui.Button("<##array", menuSize)) { AdjustArrayLevel(-1); }
                ImGui.SameLine();
                if (ImGui.Button(">##array", menuSize)) { AdjustArrayLevel(1); }
                ImGui.SameLine();

                ImGui.Text("Mip Level " + $"{currentMipLevel} / {texture.MipCount - 1}");
                ImGui.SameLine();
                if (ImGui.Button("<##mip", menuSize)) { AdjustMipLevel(-1); }
                ImGui.SameLine();
                if (ImGui.Button(">##mip", menuSize)) { AdjustMipLevel(1); }

                ImGui.SameLine();
                ImGui.PushItemWidth(150);

                string[] channelList = new string[] { "Red", "Green", "Blue", "Alpha" };
                string channel = SelectedChannelIndex == -1 ? "RGBA" : channelList[SelectedChannelIndex];
                if (ImGui.BeginCombo("##ChannelSel", $"Channel {channel}"))
                {
                    bool select = SelectedChannelIndex == -1;
                    if (ImGui.Selectable("RGBA", select))
                        SelectedChannelIndex = -1;

                    for (int i = 0; i < 4; i++)
                    {
                        select = SelectedChannelIndex == i;
                        if (ImGui.Selectable(channelList[i], select))
                            SelectedChannelIndex = i;
                        if (select)
                            ImGui.SetItemDefaultFocus();
                    }

                    if (select)
                        ImGui.SetItemDefaultFocus();

                    ImGui.EndCombo();
                }

                ImGui.PopItemWidth();

                //Draw the main image canvas
                DrawImageCanvas(canvasWindowSize);
            }
            ImGui.EndChild();
        }

        private void DrawImageCanvas(Vector2 size)
        {
            ImageCanvas.ActiveTexture = ActiveTexture;
            ImageCanvas.Render((int)size.X, (int)size.Y);
        }

        private void AdjustArrayLevel(int increment)
        {
            if (increment < 0 && currentArrayLevel > 0)
                currentArrayLevel--;
            if (increment > 0 && currentArrayLevel < ActiveTexture.ArrayCount - 1)
                currentArrayLevel++;
        }

        private void AdjustMipLevel(int increment)
        {
            if (increment < 0 && currentMipLevel > 0)
                currentMipLevel--;
            if (increment > 0 && currentMipLevel < ActiveTexture.MipCount - 1)
                currentMipLevel++;
        }
    }
}
