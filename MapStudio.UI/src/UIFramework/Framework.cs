using System;
using System.Reflection;
using System.ComponentModel;
using OpenTK;
using OpenTK.Input;
using MapStudio.UI;
using GLFrameworkEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace UIFramework
{
    /// <summary>
    /// Represents the framework backend to run the UI.
    /// </summary>
    public class Framework : GameWindow
    {
        MainWindow MainWindow;
        ImGuiController _controller;
        ProcessLoading ProcessLoading = null;

        public Framework(MainWindow window, GraphicsMode gMode, string asssemblyVersion,
            string name = "TRACK_STUDIO", int width = 1600, int height = 900) : base(width, height, gMode,
                             TranslationSource.GetText(name),
                             GameWindowFlags.Default,
                             DisplayDevice.Default,
                             3, 2, GraphicsContextFlags.Default)
        {
            MainWindow = window;
            window.Init(this);

            try
            {
                WindowsThemeUtil.Init(this.WindowInfo.Handle);
            }
            catch
            {

            }

            Title += $" Version: {asssemblyVersion}";
            Title += $": {TranslationSource.GetText("OPENGL_VERSION")}: " + GL.GetString(StringName.Version);

            Title = "Splatoon 3 Map Editor - Version 5.0";

            ProcessLoading = new ProcessLoading();
            ProcessLoading.OnUpdated += delegate
            {
                this.Update();
            };
        }

        private void Update()
        {
            //if (!ProcessLoading.IsLoading)
            //    return;
            return;
            var cont = OpenTK.Graphics.GraphicsContext.CurrentContext;
            cont.Update(this.WindowInfo);

            OnRenderFrame(new FrameEventArgs(0.0001f));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _controller = new ImGuiController(Width, Height);
            MainWindow.OnApplicationLoad();
        }

        bool renderingFrame = false;
        bool executingAction = false;
        bool drawnOnce = false;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (renderingFrame) return;

            //When a menu is clicked, it is executed in the render loop normally
            //Instead we may need to execute ouside the loop so we can update the rendering during the action
            //This is used for the progress bar to update properly.
            if (UIManager.ActionExecBeforeUIDraw != null && !executingAction)
            {
                //Redraw the UI atleast once so context menus can dissappear
                if (!drawnOnce)
                    drawnOnce = true;
                else
                {
                    executingAction = true;
                    UIManager.ActionExecBeforeUIDraw.Invoke();
                    UIManager.ActionExecBeforeUIDraw = null;
                    executingAction = false;
                    drawnOnce = false;
                }
            }

            if (!this.Focused && !MainWindow.ForceFocus && !ProcessLoading.IsLoading &&
                !(GLContext.ActiveContext != null && GLContext.ActiveContext.UpdateViewport))
            {
                System.Threading.Thread.Sleep(1);
                return;
            }

            base.OnRenderFrame(e);

            //Only force the focus once
            if (MainWindow.ForceFocus)
                MainWindow.ForceFocus = false;

            renderingFrame = true;

            _controller.Update(this, (float)e.Time);

            MainWindow.OnRenderFrame();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Width, Height);

            _controller.Render();

            SwapBuffers();

            renderingFrame = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Tell ImGui of the new size
            _controller.WindowResized(Width, Height);
            MainWindow.OnResize(Width, Height);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            base.OnFileDrop(e);
            MainWindow.OnFileDrop(e.FileName);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            _controller.PressChar(e.KeyChar);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            MainWindow.OnKeyDown(e);
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);
            MainWindow.OnFocusedChanged();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MainWindow.OnClosing(e);
            base.OnClosing(e);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
