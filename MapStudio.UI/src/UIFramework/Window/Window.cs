using System;
using System.Numerics;
using System.Text;
using ImGuiNET;

namespace UIFramework
{
    /// <summary>
    /// Represents a window instance for rendering UI elements in.
    /// </summary>
    public class Window
    {
        public EventHandler WindowClosing;

        /// <summary>
        /// The name of the window.
        /// </summary>
        public virtual string Name { get; set; } = "Window";

        /// <summary>
        /// The flags of the window.
        /// </summary>
        public virtual ImGuiWindowFlags Flags { get; } = ImGuiWindowFlags.None;

        /// <summary>
        /// Determines if the window is opened or not.
        /// </summary>
        public bool Opened = true;

        public Vector2 Size { get; set; }

        public bool PlaceAtCenter = false;

        public bool IsFocused = false;

        public bool IsWindowHovered = false;

        private bool _windowClosing = false;
        protected bool loaded = false;

        public virtual string GetWindowID() => this.Name;
        public virtual string GetWindowName()
        {
            if (translatedName == "")
                translatedName = MapStudio.UI.TranslationSource.GetText(this.Name);

            return $"{translatedName}##{this.GetWindowID()}";
        }

        private string translatedName = "";

        public Window() { }

        public Window(string name) {
            Name = name;
        }

        public Window(string name, Vector2 size) {
            Name = name;
            Size = size;
        }

        /// <summary>
        /// Displays the window and renders it. This must be called during a render loop.
        /// </summary>
        public virtual bool Show()
        {
            if (!Opened)
            {
                IsWindowHovered = false;
                IsFocused = false;
                return false;
            }

            //Method for setting up stuff on load
            if (!loaded) {
                OnLoad();
                loaded = true;
            }

            if (Size.X != 0 && Size.Y != 0)
                ImGui.SetNextWindowSize(new Vector2(Size.X, Size.Y), ImGuiCond.Once);
            if (PlaceAtCenter)
            {
                var size = ImGui.GetMainViewport().Size;
                ImGui.SetNextWindowPos(new Vector2(size.X * 0.5f, size.Y * 0.5f), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            }

            string name = GetWindowName();
            bool visible = ImGui.Begin(name, ref Opened, Flags);

            IsWindowHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);
            IsFocused = ImGui.IsWindowFocused();
            Size = ImGui.GetWindowSize();

            //Window is no longer opened so call the closing method
            if (!Opened && !_windowClosing) {
                _windowClosing = true;
                WindowClosing?.Invoke(this, EventArgs.Empty);
                OnWindowClosing();
            }
            if (visible) {
                Render();
            }
            ImGui.End();
            return visible;
        }

        public virtual void OnLoad()
        {

        }

        public void Close() => this.Opened = false;

        /// <summary>
        /// Renders the UI elements in the window.
        /// </summary>
        public virtual void Render()
        {

        }

        /// <summary>
        /// Called when the window is about to close.
        /// </summary>
        public virtual void OnWindowClosing()
        {
        }
    }
}
