using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using ImGuiNET;
using MapStudio.UI;
using GLFrameworkEngine;
using Toolbox.Core;
using Toolbox.Core.Animations;
using Toolbox.Core.ViewModels;
using UIFramework;

namespace MapStudio.UI
{
    public class Viewport : DockWindow
    {
        public override string Name => "VIEWPORT";

        public ViewportRenderer Pipeline { get; set; }

        public List<ViewportScreen> Viewports = new List<ViewportScreen>();

        public List<MenuItemModel> ContextMenus = new List<MenuItemModel>();

        public bool IsFocused => Viewports.Any(x => x.IsFocused);

        GlobalSettings GlobalSettings { get; set; }

        private bool contextMenuOpen = false;
        private Workspace ParentWorkspace;

        public List<MenuItemModel> ToolSideMenuBarItems = new List<MenuItemModel>();

        public EventHandler DrawViewportMenuBar;
        public EventHandler DrawEditorDropdown;

        List<MenuItemModel> ToolMenuBarItems = new List<MenuItemModel>();

        public Viewport(Workspace workspace, GlobalSettings settings) : base(workspace)
        {
            GlobalSettings = settings;
            ParentWorkspace = workspace;

            Pipeline = new ViewportRenderer();
            Pipeline.InitBuffers();
            Pipeline._context.UseSRBFrameBuffer = true;

            GlobalSettings.ReloadContext(Pipeline._context, Pipeline._context.Camera);
            Pipeline._context.SetActive();
            Pipeline._context.TransformTools.TransformModeChanged += delegate
            {
                ReloadMenus();
            };
            SetScreenLayoutDefault();

            ReloadMenus();
        }

        public GLTexture2D SaveAsScreenshotGLTexture(int width, int height)
        {
            return Viewports[0].SaveAsScreenshotGLTexture(Pipeline, width, height, true);
        }

        public System.Drawing.Bitmap SaveAsScreenshot(int width, int height, bool alpha = false)
        {
            return Viewports[0].SaveAsScreenshot(Pipeline, width, height, alpha);
        }

        private void SetScreenLayoutDefault()
        {
            Viewports.Clear();
            Viewports.Add(new ViewportScreen("Main", Pipeline._context.Camera));
        }

        private void SetScreenLayout4x4()
        {
            var cam = Pipeline._context.Camera;

            Viewports.Clear();
            Viewports.Add(new ViewportScreen("Screen1", new Camera() { Direction = Camera.FaceDirection.Front }));
            Viewports.Add(new ViewportScreen("Screen2", new Camera() { Direction = Camera.FaceDirection.Top }));
            Viewports.Add(new ViewportScreen("Screen3", new Camera() { Direction = Camera.FaceDirection.Right }));
            Viewports.Add(new ViewportScreen("Screen4", new Camera() { Direction = Camera.FaceDirection.Left }));
           foreach (var viewport in this.Viewports) {
                viewport.Camera.Mode = cam.Mode;
                viewport.Camera.Fov = cam.Fov;
                viewport.Camera.ZNear = cam.ZNear;
                viewport.Camera.ZFar = cam.ZFar;
            }
        }

        public void LoadAddContextMenu()
        {
            if (ContextMenus.Count > 0)
                return;

            var scene = GLContext.ActiveContext.Scene;
            ContextMenus.AddRange(scene.MenuItemsAdd);
            contextMenuOpen = false;
        }

        public void SetActive() {
            Pipeline._context.SetActive();
        }

        public void Dispose()
        {
            foreach (var render in Pipeline._context.Scene.Objects)
                render.Dispose();

            Pipeline.Files.Clear();
            Pipeline._context.Scene.Objects.Clear();
        }

        public override void Render()
        {
            var width = ImGui.GetWindowWidth();
            var menubarHeight = ImGui.GetFrameHeight();
            var pos3 = ImGui.GetCursorScreenPos();

            if (ImGui.BeginChild("viewport_iconmenu2", new System.Numerics.Vector2(width, menubarHeight), false, ImGuiWindowFlags.MenuBar))
            {
                if (ImGui.BeginMenuBar())
                {
                    DrawViewportIconMenu(ToolMenuBarItems);
                    DrawViewportMenuBar?.Invoke(this, EventArgs.Empty);

                    DrawGizmoSettings(pos3);

                    ImGui.Checkbox(TranslationSource.GetText("DROP_TO_COLLISION"), ref Pipeline._context.EnableDropToCollision);

                    ImGui.EndMenuBar();
                }
            }
            ImGui.EndChild();

            var viewportWidth = ImGui.GetWindowWidth();
            var viewportHeight = ImGui.GetWindowHeight() - ImGui.GetCursorPosY() - 4;
            var pos = ImGui.GetCursorPos();

            if (ImGui.BeginChild("viewport_child1", new System.Numerics.Vector2(viewportWidth, viewportHeight)))
            {
                if (this.Viewports.Count == 4)
                {
                    ImGui.Columns(2);
                    for (int i = 0; i < Viewports.Count; i++)
                    {
                        if (ImGui.BeginChild($"viewport_segment{i}", new System.Numerics.Vector2(ImGui.GetColumnWidth(), (viewportHeight / 2) - 4)))
                        {
                            Viewports[i].RenderViewportDisplay(Pipeline);
                        }
                        ImGui.EndChild();
                        ImGui.NextColumn();
                    }
                    ImGui.Columns(1);
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(0, 0, 0, 1));
                    if (ImGui.BeginChild($"viewport_segment"))
                    {
                        foreach (var viewport in this.Viewports)
                            viewport.RenderViewportDisplay(Pipeline);
                    }
                    ImGui.EndChild();
                    ImGui.PopStyleColor();
                }

                float mheight = ImGui.CalcTextSize("A").Y;
                ImGui.SetCursorPos(new System.Numerics.Vector2(pos.X, pos.Y - 45));
                if (ImGui.BeginChild("viewport_child2", new System.Numerics.Vector2(viewportWidth, mheight + 7)))
                {
                    var color = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
                    var colorH = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBgHovered];

                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(color.X, color.Y, color.Z, 0.78f));
                    ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new System.Numerics.Vector4(colorH.X, colorH.Y, colorH.Z, 0.78f));

                    DrawViewMenu();
                    ImGui.SameLine();

                    DrawShadingMenu();

                    ImGui.SameLine();
                    DrawCameraMenu();

                    ImGui.SameLine();
                    DrawEditorMenu();

                    ImGui.SetCursorPos(new System.Numerics.Vector2(viewportWidth - 150, 0));
                    DrawGizmoMenu();

                    ImGui.PopStyleColor(2);
                }
                ImGui.EndChild();

                var fcolor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(fcolor.X, fcolor.Y, fcolor.Z, 0.58f));

                if (ToolSideMenuBarItems.Count > 0)
                {
                    ImGui.SetCursorPos(new System.Numerics.Vector2(pos.X, pos.Y));
                    if (ImGui.BeginChild("viewport_child3", new System.Numerics.Vector2(24, ToolSideMenuBarItems.Count * 28)))
                    {
                        DrawViewportIconMenu(ToolSideMenuBarItems, true);
                    }
                    ImGui.EndChild();
                }

                ImGui.PopStyleColor(1);
            }
            ImGui.EndChild();

            if (ContextMenus.Count > 0 && !contextMenuOpen) {
                ImGui.CloseCurrentPopup();

                ImGui.OpenPopup("contextMenuPopup");
                contextMenuOpen = true;
            }
            if (contextMenuOpen)
            {
                if (ImGui.BeginPopupContextItem("contextMenuPopup"))
                {
                    foreach (var item in ContextMenus)
                        ImGuiHelper.LoadMenuItem(item);
                    ImGui.EndPopup();
                }
                else
                {
                    GLContext.ActiveContext.Focused = true;
                    ContextMenus.Clear();
                    contextMenuOpen = false;
                }
            }
        }

        private void DrawGizmoSettings(System.Numerics.Vector2 pos)
        {
            var pos3 = ImGui.GetCursorPos();
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(pos.X, pos.Y + 23));
            if (ImGui.BeginPopup("gizmo_settings"))
            {
                var settings = Pipeline._context.TransformTools.TransformSettings;
                if (ImGui.Checkbox("Display Gizmo", ref settings.DisplayGizmo))
                    Pipeline._context.UpdateViewport = true;

                ImGui.DragFloat("Gizmo Size", ref settings.GizmoSize, 0.01f);

                ImGui.Checkbox("Scale By Middle Mouse", ref settings.MiddleMouseScale);
                ImGuiHelper.InputFromBoolean("Enable Snap", settings, "SnapTransform");

                if (settings.SnapTransform)
                {
                    ImGuiHelper.InputTKVector3("Translate Snap", settings, "TranslateSnapFactor");
                    ImGuiHelper.InputTKVector3("Rotate Snap", settings, "RotateSnapFactor");
                    ImGuiHelper.InputTKVector3("Scale Snap", settings, "ScaleSnapFactor");
                }
                ImguiCustomWidgets.ComboScrollable("Pivot", settings.PivotMode.ToString(), ref settings.PivotMode);

                ImGui.Checkbox(TranslationSource.GetText("DROP_TO_COLLISION"), ref Pipeline._context.EnableDropToCollision);
                ImGui.Checkbox("Rotate From Collision", ref settings.RotateFromNormal);

                ImGui.EndPopup();
            }
            ImGui.SetCursorPos(pos3);
        }

        private void DrawViewMenu()
        {
            var w = ImGui.GetCursorPosX();

            var size = new System.Numerics.Vector2(80, ImGui.GetWindowHeight() - 1);
            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]);
            if (ImGui.Button($"{TranslationSource.GetText("VIEW")}:", size))
            {
                ImGui.OpenPopup("viewMenu");
            }
            ImGui.PopStyleColor();

            var pos = ImGui.GetCursorScreenPos();

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(pos.X + w, pos.Y));
            if (ImGui.BeginPopup("viewMenu"))
            {
                foreach (var menu in this.GetViewMenuItems())
                    ImGuiHelper.LoadMenuItem(menu);
                foreach (var menu in Workspace.ActiveWorkspace.GetViewMenuItems())
                    ImGuiHelper.LoadMenuItem(menu);
                ImGui.EndPopup();
            }
        }

        private void DrawShadingMenu()
        {
            string text = $"{TranslationSource.GetText("SHADING")} : [{DebugShaderRender.DebugRendering}]";

            ImGui.PushItemWidth(150);
            ImguiCustomWidgets.ComboScrollable<DebugShaderRender.DebugRender>($"##debugShading", text, ref DebugShaderRender.DebugRendering, 
                Enum.GetValues(typeof(DebugShaderRender.DebugRender)).Cast<DebugShaderRender.DebugRender>(), () =>
                {
                    GLContext.ActiveContext.UpdateViewport = true;
                }, ImGuiComboFlags.NoArrowButton);

            ImGui.PopItemWidth();
        }

        private void DrawGizmoMenu()
        {
            var settings = Pipeline._context.TransformTools.TransformSettings;
            var mode = settings.TransformMode;

            ImGui.PushItemWidth(150);

            ImguiCustomWidgets.ComboScrollable($"##transformSpace",
                    $"{TranslationSource.GetText("MODE")} : [{settings.TransformMode}]", ref mode, () =>
                    {
                        settings.TransformMode = mode;
                        GLContext.ActiveContext.UpdateViewport = true;
                    }, ImGuiComboFlags.NoArrowButton);

            ImGui.PopItemWidth();
        }

        private void DrawEditorMenu()
        {
            if (DrawEditorDropdown != null)
            {
                DrawEditorDropdown?.Invoke(this, EventArgs.Empty);
                return;
            }

            List<string> editorList = ParentWorkspace.ActiveEditor.SubEditors;
            string activeEditor = ParentWorkspace.ActiveEditor.SubEditor;

            string text = $"{TranslationSource.GetText("EDITORS")} : [{TranslationSource.GetText(activeEditor)}]";

            ImGui.PushItemWidth(200);
            ImguiCustomWidgets.ComboScrollable<string>($"##editorMenu", text, ref activeEditor,
                editorList, () =>
                {
                    Workspace.ActiveWorkspace.ActiveEditor.SubEditor = activeEditor;
                    GLContext.ActiveContext.UpdateViewport = true;
                }, ImGuiComboFlags.NoArrowButton | ImGuiComboFlags.HeightLargest);
            ImGui.PopItemWidth();
        }

        private void DrawCameraMenu()
        {
            bool updateSettings = false;
            bool refreshScene = false;

            var w = ImGui.GetCursorPosX();

            string mode = Pipeline._context.Camera.IsOrthographic ? "Ortho" : "Persp";

            var size = new System.Numerics.Vector2(120, ImGui.GetWindowHeight() - 1);
            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]);
            if (ImGui.Button($"{TranslationSource.GetText("CAMERA")} : [{mode}]", size))
            {
                ImGui.OpenPopup("cameraMenu");
            }
            ImGui.PopStyleColor();

            var pos = ImGui.GetCursorScreenPos();

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(pos.X + w, pos.Y));
            if (ImGui.BeginPopup("cameraMenu"))
            {
                if (ImGui.Button(TranslationSource.GetText("RESET_TRANSFORM")))
                {
                    Pipeline._context.Camera.ResetViewportTransform();
                }

                if (ImGui.CollapsingHeader(TranslationSource.GetText("TRANSFORM")))
                {
                    updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("ROTATION_X"), Pipeline._context.Camera, "RotationDegreesX", true, 1f);
                    updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("ROTATION_Y"), Pipeline._context.Camera, "RotationDegreesY", true, 1f);
                    updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("ROTATION_Z"), Pipeline._context.Camera, "RotationDegreesZ", true, 1f);
                    updateSettings |= ImGuiHelper.InputTKVector3(TranslationSource.GetText("POSITION"), Pipeline._context.Camera, "TargetPosition", 1f);
                }

                ImGuiHelper.ComboFromEnum<Camera.FaceDirection>(TranslationSource.GetText("DIRECTION"), Pipeline._context.Camera, "Direction");
                if (ImGuiHelper.ComboFromEnum<Camera.CameraMode>(TranslationSource.GetText("MODE"), Pipeline._context.Camera, "Mode"))
                {
                    updateSettings = true;
                }

                updateSettings |= ImGuiHelper.InputFromBoolean(TranslationSource.GetText("ORTHOGRAPHIC"), Pipeline._context.Camera, "IsOrthographic");
                ImGuiHelper.InputFromBoolean(TranslationSource.GetText("LOCK_ROTATION"), Pipeline._context.Camera, "LockRotation");

                updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("FOV_(DEGREES)"), Pipeline._context.Camera, "FovDegrees", true, 1f);
                if (Pipeline._context.Camera.FovDegrees != 45)
                {
                    ImGui.SameLine(); if (ImGui.Button(TranslationSource.GetText("RESET"))) { Pipeline._context.Camera.FovDegrees = 45; }
                }

                updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("ZFAR"), Pipeline._context.Camera, "ZFar", true, 1f);
                if (Pipeline._context.Camera.ZFar != 100000.0f)
                {
                    ImGui.SameLine(); if (ImGui.Button(TranslationSource.GetText("RESET"))) { Pipeline._context.Camera.ZFar = 100000.0f; }
                }

                updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("ZNEAR"), Pipeline._context.Camera, "ZNear", true, 0.1f);
                if (Pipeline._context.Camera.ZNear != 1)
                {
                    ImGui.SameLine(); if (ImGui.Button(TranslationSource.GetText("RESET"))) { Pipeline._context.Camera.ZNear = 1; }
                }

                updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("ZOOM_SPEED"), Pipeline._context.Camera, "ZoomSpeed", true, 0.1f);
                if (Pipeline._context.Camera.ZoomSpeed != 1.0f)
                {
                    ImGui.SameLine(); if (ImGui.Button(TranslationSource.GetText("RESET"))) { Pipeline._context.Camera.ZoomSpeed = 1.0f; }
                }

                updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("PAN_SPEED"), Pipeline._context.Camera, "PanSpeed", true, 0.1f);
                if (Pipeline._context.Camera.PanSpeed != 1.0f)
                {
                    ImGui.SameLine(); if (ImGui.Button(TranslationSource.GetText("RESET"))) { Pipeline._context.Camera.PanSpeed = 1.0f; }
                }

                updateSettings |= ImGuiHelper.InputFromFloat(TranslationSource.GetText("KEY_MOVE_SPEED"), Pipeline._context.Camera, "KeyMoveSpeed", true, 0.1f);
                if (Pipeline._context.Camera.PanSpeed != 1.0f)
                {
                    ImGui.SameLine(); if (ImGui.Button(TranslationSource.GetText("RESET"))) { Pipeline._context.Camera.KeyMoveSpeed = 1.0f; }
                }

                if (updateSettings)
                    Pipeline._context.UpdateViewport = true;

                if (updateSettings)
                {
                    //Reload existing set values then save
                    GlobalSettings.LoadCurrentSettings();
                    GlobalSettings.Save();
                }

                ImGui.EndPopup();
            }

            if (refreshScene || updateSettings)
                Pipeline._context.UpdateViewport = true;

            if (updateSettings)
            {
                //Reload existing set values then save
                GlobalSettings.LoadCurrentSettings();
                GlobalSettings.Save();
            }
        }

        public void ReloadMenus()
        {
            ToolMenuBarItems = SetupIconMenu();
        }

        //For the edit menu in main toolbar
        public List<MenuItemModel> GetEditMenuItems()
        {
            List<MenuItemModel> menus = new List<MenuItemModel>();
            menus.Add(new MenuItemModel($"   {IconManager.SELECT_ICON}    {TranslationSource.GetText("SELECT_ALL")}", () => Pipeline._context.Scene.SelectAll(Pipeline._context)));
            menus.Add(new MenuItemModel($"   {IconManager.DESELECT_ICON}    {TranslationSource.GetText("DESELECT_ALL")}", () => Pipeline._context.Scene.DeselectAll(Pipeline._context)));

            menus.Add(new MenuItemModel($"   {IconManager.UNDO_ICON}    {TranslationSource.GetText("UNDO")}", () => Pipeline._context.Scene.Undo()));
            menus.Add(new MenuItemModel($"   {IconManager.REDO_ICON}    {TranslationSource.GetText("REDO")}", () => Pipeline._context.Scene.Redo()));
            menus.Add(new MenuItemModel(""));
            menus.Add(new MenuItemModel($"   {IconManager.COPY_ICON}    {TranslationSource.GetText("COPY")}", Pipeline._context.Scene.CopySelected));
            menus.Add(new MenuItemModel($"   {IconManager.PASTE_ICON}    {TranslationSource.GetText("PASTE")}", Pipeline._context.Scene.PasteSelected));
            menus.Add(new MenuItemModel($"   {IconManager.DELETE_ICON}    {TranslationSource.GetText("REMOVE")}", Pipeline._context.Scene.DeleteSelected));

            return menus;
        }

        public bool DrawPathDropdown()
        {
            bool changed = false;

            bool SelectTool(RenderablePath.ToolMode tool, bool auto = false)
            {
                string text = GetText(tool, auto);
                bool select = RenderablePath.EditToolMode == tool;
                if (ImGui.Selectable(text, select))
                {
                    RenderablePath.EditToolMode = tool;
                    RenderablePath.ConnectAuto = auto;
                    return true;
                }

                return false;
            };

            ImGui.PushItemWidth(150);
            string mode = GetText(RenderablePath.EditToolMode, RenderablePath.ConnectAuto);
            if (ImGui.BeginCombo("##Tools", mode))
            {
                bool select = false;
                select |= SelectTool(RenderablePath.ToolMode.Transform);
                select |= SelectTool(RenderablePath.ToolMode.Drawing);
                select |= SelectTool(RenderablePath.ToolMode.Connection);
                select |= SelectTool(RenderablePath.ToolMode.Connection, true);
                select |= SelectTool(RenderablePath.ToolMode.Erase);
                if (select)
                    changed = true;

                if (select)
                    ImGui.SetItemDefaultFocus();

                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();

            return changed;
        }

        private string GetText(RenderablePath.ToolMode mode, bool autoConnect = false)
        {
            if (mode == RenderablePath.ToolMode.Transform)
                return $"   {IconManager.PATH_MOVE}   Move";
            if (mode == RenderablePath.ToolMode.Drawing)
                return $"   {IconManager.PATH_DRAW}   Draw";
            if (autoConnect && mode == RenderablePath.ToolMode.Connection)
                return $"   {IconManager.PATH_CONNECT}   Connect (Auto)";
            if (mode == RenderablePath.ToolMode.Connection)
                return $"   {IconManager.PATH_CONNECT_AUTO}   Connect";
            if (mode == RenderablePath.ToolMode.Erase)
                return $"   {IconManager.ERASER}   Erase";

            return $"   {IconManager.PATH_MOVE}   Move";
        }

        public List<MenuItemModel> GetViewMenuItems()
        {
            List<MenuItemModel> menus = new List<MenuItemModel>();
            if (Pipeline.IsViewport2D)
                menus.Add(new MenuItemModel($"      {IconManager.ICON_3D}      View", () =>
                {
                    Pipeline.IsViewport2D = !Pipeline.IsViewport2D;
                    ReloadMenus();
                }, "VIEWPORT_2D"));
            else
                menus.Add(new MenuItemModel($"      {IconManager.ICON_2D}      View", () =>
                {
                    Pipeline.IsViewport2D = !Pipeline.IsViewport2D;
                    ReloadMenus();
                }, "VIEWPORT_3D"));

            menus.Add(new MenuItemModel($"      {IconManager.ICON_SHADOWS}      {TranslationSource.GetText("SHOW_SHADOWS")}", () =>
            {
                ShadowMainRenderer.Display = !ShadowMainRenderer.Display;
                ReloadMenus();
            }, "SHOW_SHADOWS", ShadowMainRenderer.Display));

            menus.Add(new MenuItemModel($"      {IconManager.ICON_BACKGROUND}      {TranslationSource.GetText("SHOW_BACKGROUND")}", () =>
            {
                DrawableBackground.Display = !DrawableBackground.Display;
                ReloadMenus();
            }, "SHOW_BACKGROUND", DrawableBackground.Display));

            menus.Add(new MenuItemModel($"      {IconManager.ICON_FLOOR}      {TranslationSource.GetText("SHOW_FLOOR")}", () =>
            {
                DrawableGridFloor.Display = !DrawableGridFloor.Display;
                ReloadMenus();
            }, "SHOW_FLOOR", DrawableGridFloor.Display));

            return menus;
        }

        private List<MenuItemModel> SetupIconMenu()
        {
            bool isSelectionMode = Pipeline._context.SelectionTools.IsSelectionMode;
            bool isTranslationActive = Pipeline._context.TransformTools.ActiveMode == TransformEngine.TransformActions.Translate;
            bool isRotationActive = Pipeline._context.TransformTools.ActiveMode == TransformEngine.TransformActions.Rotate;
            bool isScaleActive = Pipeline._context.TransformTools.ActiveMode == TransformEngine.TransformActions.Scale;
            bool isRectScaleActive = Pipeline._context.TransformTools.ActiveMode == TransformEngine.TransformActions.RectangleScale;
            bool isPlaying = StudioSystem.Instance != null && StudioSystem.Instance.IsPlaying;
            bool isMultiGizmo = Pipeline._context.TransformTools.ActiveMode == TransformEngine.TransformActions.MultiGizmo;

            List<MenuItemModel> menus = new List<MenuItemModel>();
            menus.Add(new MenuItemModel($"{IconManager.SETTINGS_ICON}", () =>
            {
                ImGui.OpenPopup("gizmo_settings");
            }, "SETTINGS"));

            if (!isPlaying)
                menus.Add(new MenuItemModel($"{IconManager.PLAY_ICON}", () => 
                {
                    StudioSystem.Instance.Run();
                    ReloadMenus();
                }, "PLAY"));
            else
                menus.Add(new MenuItemModel($"{IconManager.PAUSE_ICON}", () =>
                {
                    StudioSystem.Instance.Pause();
                    ReloadMenus();
                }, "STOP"));
            menus.Add(new MenuItemModel($"{IconManager.UNDO_ICON}", () => Pipeline._context.Scene.Undo(), "UNDO"));
            menus.Add(new MenuItemModel($"{IconManager.REDO_ICON}", () => Pipeline._context.Scene.Redo(), "REDO"));
          //  menus.Add(new MenuItemModel($"{IconManager.CAMERA_ICON}", TakeScreenshot, "SCREENSHOT"));

            menus.Add(new MenuItemModel(""));
            menus.Add(new MenuItemModel($"{IconManager.ARROW_ICON}", EnterSelectionMode, "SELECT", isSelectionMode));
            menus.Add(new MenuItemModel($"{IconManager.TRANSLATE_ICON}", () =>
            {
                EnterGizmoMode(TransformEngine.TransformActions.Translate);
            }, "TRANSLATE", isTranslationActive && !isSelectionMode));
            menus.Add(new MenuItemModel($"{IconManager.ROTATE_ICON}", () =>
            {
                EnterGizmoMode(TransformEngine.TransformActions.Rotate);
            }, "ROTATE", isRotationActive && !isSelectionMode));
            menus.Add(new MenuItemModel($"{IconManager.SCALE_ICON}", () =>
            {
                EnterGizmoMode(TransformEngine.TransformActions.Scale);
            }, "SCALE", isScaleActive && !isSelectionMode));
            menus.Add(new MenuItemModel($"{IconManager.MULTI_GIZMO_ICON}", () =>
            {
                EnterGizmoMode(TransformEngine.TransformActions.MultiGizmo);
            }, "MULTI_GIZMO", isMultiGizmo && !isSelectionMode));
            menus.Add(new MenuItemModel($"{IconManager.RECT_SCALE_ICON}", () =>
            {
                EnterGizmoMode(TransformEngine.TransformActions.RectangleScale);
            }, "RECTANGLE_SCALE", isRectScaleActive && !isSelectionMode));
            menus.Add(new MenuItemModel(""));
            menus.Add(new MenuItemModel(""));
            //A workspace can have its own menu icons per editor
            if (Workspace.ActiveWorkspace != null)
                menus.AddRange(Workspace.ActiveWorkspace.GetViewportMenuIcons());

            return menus;
        }

        private void DrawViewportIconMenu(List<MenuItemModel> items, bool vertical = false)
        {
            var h = ImGui.GetWindowHeight();
            if (vertical)
                h = 23;

            var menuSize = new System.Numerics.Vector2(h, h);

            //Make icon buttons invisible aside from the icon itself.
            ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4());
            {
                foreach (var item in items)
                {
                    if (item.Header == "")
                    {
                        ImGui.Separator();
                        continue;
                    }

                    if (item.IsChecked)
                    {
                        var selectionColor = ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered];
                        ImGui.PushStyleColor(ImGuiCol.Button, selectionColor);
                    }

                    if (ImGui.Button(item.Header, menuSize)) {
                        item.Command.Execute(item);
                    }
                    if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(item.ToolTip))
                        ImGui.SetTooltip(TranslationSource.GetText(item.ToolTip));

                    if (!vertical)
                        ImGui.SameLine();

                    if (item.IsChecked)
                        ImGui.PopStyleColor(1);
                }
            }
            ImGui.PopStyleColor();
        }

        private void EnterGizmoMode(TransformEngine.TransformActions action)
        {
            Pipeline._context.SelectionTools.IsSelectionMode = false;
            Pipeline._context.TransformTools.TransformSettings.DisplayGizmo = true;
            Pipeline._context.TransformTools.UpdateTransformMode(action);
            ReloadMenus();
        }

        private void EnterSelectionMode()
        {
            Pipeline._context.SelectionTools.IsSelectionMode = true;
            Pipeline._context.TransformTools.TransformSettings.DisplayGizmo = false;
            Pipeline._context.TransformTools.UpdateTransformMode(TransformEngine.TransformActions.Translate);

            ReloadMenus();
        }    }
}
