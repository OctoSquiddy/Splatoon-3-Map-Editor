using System;
using System.Collections.Generic;
using System.Text;
using GLFrameworkEngine;
using Toolbox.Core.ViewModels;
using OpenTK;
using UIFramework;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents an editor for a single file instance. 
    /// Each file format has a file editor assigned to them.
    /// </summary>
    public class FileEditor
    {
        public virtual List<string> SubEditors { get; set; } = new List<string>();

        public virtual Action RenderNewFileDialog { get; }

        private string subEditor = "Default";

        /// <summary>
        /// The sub editor which represents what kind of things to edit and display.
        /// It is up to the developer on if they want to use this and how they want it used.
        /// Generally it is used to determine what children are loaded in the "Root" and objects visible in "Scene"
        /// </summary>
        public virtual string SubEditor
        {
            get { return subEditor; }
            set
            {
                if (subEditor != value) {
                    subEditor = value;
                    ReloadSubEditor();
                }
            }
        }

        /// <summary>
        /// The tree node list to add to the workspace outliner.
        /// </summary>
        public NodeBase Root { get; set; } = new NodeBase();

        /// <summary>
        /// The parent workspace of the file editor.
        /// </summary>
       public Workspace Workspace { get; set; }

        /// <summary>
        /// The optional tool window for drawing the tool UI.
        /// </summary>
        public IToolWindowDrawer ToolWindowDrawer { get; set; }

        /// <summary>
        /// The scene for loading rendered objects into the viewport
        /// </summary>
        public GLScene Scene { get; set; }

        public FileEditor() {
            Workspace = Workspace.ActiveWorkspace;
            Scene = new GLScene();
        }

        /// <summary>
        /// When the file is selected and editor has been changed.
        /// </summary>
        public virtual void SetActive()
        {
            Workspace.ViewportWindow.Pipeline._context.Scene = Scene;
            GLContext.ActiveContext.UpdateViewport = true;
        }

        public virtual void AfterLoaded()
        {

        }

        /// <summary>
        /// Creates a new file instance.
        /// </summary>
        public virtual bool CreateNew()
        {
            return false;
        }

        /// <summary>
        /// Reloads the UI based on the optionally used sub editor.
        /// </summary>
        public virtual void ReloadSubEditor()
        {

        }

        public virtual void RenderSaveFileSettings()
        {

        }

        /// <summary>
        /// Adds a renderer to the active workspace viewport.
        /// </summary>
        public void AddRender(IDrawable drawable, bool undo = false) {
            if (!Scene.Objects.Contains(drawable))
                Scene.AddRenderObject(drawable, undo);
        }

        /// <summary>
        /// Removes a renderer to the active workspace viewport.
        /// </summary>
        public void RemoveRender(IDrawable drawable, bool undo = false) {
            if (Scene.Objects.Contains(drawable))
                Scene.RemoveRenderObject(drawable, undo);
        }

        /// <summary>
        /// Prepares the dock layouts to be used for the file format.
        /// </summary>
        public virtual List<DockWindow> PrepareDocks()
        {
            List<DockWindow> windows = new List<DockWindow>();
            windows.Add(Workspace.Outliner);
            windows.Add(Workspace.PropertyWindow);
            windows.Add(Workspace.ConsoleWindow);
            windows.Add(Workspace.AssetViewWindow);
            windows.Add(Workspace.HelpWindow);
            windows.Add(Workspace.ToolWindow);
            windows.Add(Workspace.ViewportWindow);
            return windows;
        }

        /// <summary>
        /// Occurs when a file has been dropped into the current editor.
        /// Returns true if the file loaded. 
        /// </summary>
        public virtual bool OnFileDrop(string filePath)
        {
            return false;
        }

        /// <summary>
        /// When the main window gains focus after losing it.
        /// </summary>
        public virtual void OnEnter()
        {

        }

        /// <summary>
        /// Prints errors or warnings to Toolbox.Core.StudioLogger.
        /// </summary>
        public virtual void PrintErrors()
        {
        }

        public virtual void DrawViewportMenuBar()
        {

        }

        /// <summary>
        /// Determines the contents of the help window.
        /// </summary>
        public virtual void DrawHelpWindow()
        {
            if (ImGuiNET.ImGui.CollapsingHeader("Camera", ImGuiNET.ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiHelper.BoldTextLabel("WASD", "Move camera.");
                ImGuiHelper.BoldTextLabel("Spacebar", "Move up.");
                ImGuiHelper.BoldTextLabel("Spacebar + Ctrl", "Move down.");
                ImGuiHelper.BoldTextLabel("MouseWheel", "Zoom in/out.");
            }
        }

        /// <summary>
        /// Determines the contents of the tool window.
        /// </summary>
        public virtual void DrawToolWindow()
        {

        }

        /// <summary>
        /// Gets a list of menu icons for the viewport window.
        /// </summary>
        public virtual List<MenuItemModel> GetViewportMenuIcons() => new List<MenuItemModel>();

        /// <summary>
        /// Gets a list of menu icons for the outlier filter menu.
        /// </summary>
        public virtual List<MenuItemModel> GetFilterMenuItems() => new List<MenuItemModel>();

        /// <summary>
        /// Gets a list of menu icons for the edit menu in the main window.
        /// </summary>
        public virtual List<MenuItemModel> GetEditMenuItems() => new List<MenuItemModel>();

        /// <summary>
        /// Gets a list of menu icons for the view menu in the main window.
        /// </summary>
        public virtual List<MenuItemModel> GetViewMenuItems() => new List<MenuItemModel>();
        
        public virtual void AssetViewportDrop(AssetItem item, Vector2 screenCoords) { }

        public virtual void OnMouseMove(MouseEventInfo mouseInfo) { }
        public virtual void OnMouseDown(MouseEventInfo mouseInfo) { }
        public virtual void OnMouseUp(MouseEventInfo mouseInfo) { }
        public virtual void OnKeyDown(KeyEventInfo keyEventInfo) { }
    }
}
