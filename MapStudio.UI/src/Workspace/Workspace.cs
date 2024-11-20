using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using ImGuiNET;
using MapStudio.UI;
using Toolbox.Core.ViewModels;
using GLFrameworkEngine;
using System.Reflection;
using OpenTK;
using Toolbox.Core;
using Toolbox.Core.Animations;
using UIFramework;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents a workspace instance of a workspace window.
    /// </summary>
    public class Workspace : DockSpaceWindow, IDisposable
    {
        public EventHandler OnProjectLoad;
        public EventHandler OnProjectSave;

        public static Workspace ActiveWorkspace { get; set; }

        /// <summary>
        /// System controller. Generally used for in tool animation playback and game calculations.
        /// </summary>
        public StudioSystem StudioSystem = new StudioSystem();

        /// <summary>
        /// The project resources to keep track of the project files.
        /// </summary>
        public ProjectResources Resources = new ProjectResources();

        /// <summary>
        /// Determines if the window can close or not during the dialog confirmation.
        /// </summary>
        public bool CanClose = false;

        private FileEditor _activeEditor;

        /// <summary>
        /// Represents the active file for editing.
        /// </summary>
        public FileEditor ActiveEditor
        {
            get { return _activeEditor; }
            set
            {
                if (_activeEditor != value) {
                    _activeEditor = value;
                    ReloadEditors();
                }
            }
        }

        /// <summary>
        /// The outliner tree for storing node data in a hiearchy.
        /// </summary>
        public Outliner Outliner { get; set; }

        /// <summary>
        /// The viewport for displaying 3D rendering.
        /// </summary>
        public Viewport ViewportWindow { get; set; }

        /// <summary>
        /// Property window for displaying current properties from a selected outliner node.
        /// </summary>
        public PropertyWindow PropertyWindow { get; set; }

        /// <summary>
        /// The console for printing out information, warnings and errors.
        /// </summary>
        public ConsoleWindow ConsoleWindow { get; set; }

        /// <summary>
        /// A window for storing assets such as models, textures, map objects, and materials.
        /// </summary>
        public AssetViewWindow AssetViewWindow { get; set; }

        /// <summary>
        /// A window for tool utilities.
        /// </summary>
        public ToolWindow ToolWindow { get; set; }

        /// <summary>
        /// A help window for explaining how to use the current editor.
        /// </summary>
        public HelpWindow HelpWindow { get; set; }

        public AnimationGraphWindow GraphWindow { get; set; }

        public TimelineWindow TimelineWindow { get; set; }

        public UVWindow UVWindow { get; set; }

        /// <summary>
        /// Tool menus for selecting different editor tools to use in the workspace.
        /// </summary>
        public List<MenuItemModel> WorkspaceTools = new List<MenuItemModel>();

        private MenuItemModel _activeWorkspaceTool;
        public MenuItemModel ActiveWorkspaceTool
        {
            get
            {
                if (_activeWorkspaceTool == null)
                    _activeWorkspaceTool = WorkspaceTools.FirstOrDefault();
                return _activeWorkspaceTool;
            }
            set {
                _activeWorkspaceTool = value;
            }
        }

        /// <summary>
        /// A window list of popup windows in the workspace.
        /// </summary>
        public List<Window> Windows = new List<Window>();

        public STAnimation GetActiveAnimation() => GraphWindow.ActiveAnimation;

        public string ID = "Space";

        public override string GetWindowID() => ID;

        public Workspace(GlobalSettings settings, string name) : base(name)
        {
            ID = name;

            //Window docks
            PropertyWindow = new PropertyWindow(this);
            Outliner = new Outliner(this);
            ToolWindow = new ToolWindow(this);
            ConsoleWindow = new ConsoleWindow(this);
            AssetViewWindow = new AssetViewWindow(this);
            ViewportWindow = new Viewport(this, settings);
            HelpWindow = new HelpWindow(this);
            GraphWindow = new AnimationGraphWindow(this);
            TimelineWindow = new TimelineWindow(this);
            UVWindow = new UVWindow(this);
            Windows.Add(GraphWindow.PropertyWindow);

            Outliner.SelectionChanged += delegate
            {
                //Assign the active file format if outliner has it selected
                if (Outliner.SelectedNode != null)
                {
                    //Select the active file to edit if one is selected
                    if (Outliner.SelectedNode.Tag is FileEditor && Outliner.SelectedNode.Tag != ActiveEditor)
                        ActiveEditor = (FileEditor)Outliner.SelectedNode.Tag;

                    //Select an animation for playback in the timeline window
                    if (Outliner.SelectedNode.Tag is STAnimation) {
                        TimelineWindow.AddAnimation((STAnimation)Outliner.SelectedNode.Tag);
                        GraphWindow.AddAnimation((STAnimation)Outliner.SelectedNode.Tag);

                    }
                    //Load a material to the UV window if one is selected
                    if (Outliner.SelectedNode.Tag is STGenericMaterial) {
                        UVWindow.Load((STGenericMaterial)Outliner.SelectedNode.Tag);
                    }
                    //Load a mesh to the UV window if one is selected
                    if (Outliner.SelectedNode.Tag is STGenericMesh) {
                        UVWindow.Load((STGenericMesh)Outliner.SelectedNode.Tag);
                    }
                }
                PropertyWindow.SelectedObject = Outliner.SelectedNode;
            };

            ToolWindow.UIDrawer += delegate {
                this.ActiveEditor?.DrawToolWindow();
            };

            ViewportWindow.DrawViewportMenuBar += delegate {
                this.ActiveEditor?.DrawViewportMenuBar();
            };

            HelpWindow.UIDrawer += delegate
            {
                this.ActiveEditor?.DrawHelpWindow();
            };

            this.DockedWindows.Add(Outliner);
            this.DockedWindows.Add(PropertyWindow);
            this.DockedWindows.Add(ConsoleWindow);
            this.DockedWindows.Add(AssetViewWindow);
            this.DockedWindows.Add(ToolWindow);
            this.DockedWindows.Add(ViewportWindow);
            this.DockedWindows.Add(HelpWindow);

            ReloadDefaultDockSettings();

            this.Windows.Add(new StatisticsWindow());

            //Ignore the workspace setting as the editor handles files differently
            Outliner.ShowWorkspaceFileSetting = false;
            Workspace.ActiveWorkspace = this;
            StudioSystem.Instance = this.StudioSystem;
        }

        public override void Render()
        {
            //Draw opened windows (non dockable)
            foreach (var window in Windows)
            {
                if (!window.Opened)
                    continue;

                if (ImGui.Begin(window.Name, ref window.Opened, window.Flags))
                {
                    window.Render();
                }
                ImGui.End();
            }

            if (ImGui.IsWindowFocused())
                Workspace.UpdateActive(this);

            if (ViewportWindow.IsFocused)
                PropertyWindow.SelectedObject = GetSelectedNode();

            base.Render();
        }

        public void ReloadDefaultDockSettings()
        {
            //Confiure the layout placements
            UVWindow.DockDirection = ImGuiDir.Down;
            UVWindow.ParentDock = Outliner;
            UVWindow.SplitRatio = 0.3f;

            Outliner.DockDirection = ImGuiDir.Left;
            Outliner.SplitRatio = 0.2f;

            ToolWindow.DockDirection = ImGuiDir.Up;
            ToolWindow.SplitRatio = 0.3f;
            ToolWindow.ParentDock = PropertyWindow;

            PropertyWindow.DockDirection = ImGuiDir.Right;
            PropertyWindow.SplitRatio = 0.3f;

            ConsoleWindow.DockDirection = ImGuiDir.Down;
            ConsoleWindow.SplitRatio = 0.3f;

            AssetViewWindow.DockDirection = ImGuiDir.Down;
            AssetViewWindow.SplitRatio = 0.3f;

            HelpWindow.DockDirection = ImGuiDir.Down;
            HelpWindow.ParentDock = Outliner;
            HelpWindow.SplitRatio = 0.3f;

            foreach (var window in DockedWindows)
                window.Opened = true;

            this.UpdateDockLayout = true;
        }

        public void ReloadViewportMenu() { ViewportWindow.ReloadMenus(); }

        /// <summary>
        /// Gets a list of menu items for toggling dock windows.
        /// </summary>
        public List<MenuItemModel> GetDockToggles()
        {
            List<MenuItemModel> menus = new List<MenuItemModel>();
            foreach (var dock in DockedWindows)
            {
                var item = new MenuItemModel(dock.Name, () => {
                    dock.Opened = !dock.Opened;
                });
                item.IsChecked = dock.Opened;
            }
            return menus;
        }

        /// <summary>
        /// Gets a list of menu items for viewport icons.
        /// </summary>
        public List<MenuItemModel> GetViewportMenuIcons()
        {
            List<MenuItemModel> menus = new List<MenuItemModel>();
            menus.AddRange(ActiveEditor.GetViewportMenuIcons());
            return menus;
        }

        /// <summary>
        /// Gets a list of menu items for general editing.
        /// </summary>
        public List<MenuItemModel> GetEditMenuItems()
        {
            List<MenuItemModel> menus = new List<MenuItemModel>();
            menus.AddRange(ViewportWindow.GetEditMenuItems());
            menus.AddRange(ActiveEditor.GetEditMenuItems());
            return menus;
        }

        /// <summary>
        /// Gets a list of menu items for general view adjusting.
        /// </summary>
        public List<MenuItemModel> GetViewMenuItems()
        {
            List<MenuItemModel> menus = new List<MenuItemModel>();
            menus.AddRange(ActiveEditor.GetViewMenuItems());
            return menus;
        }
        
        /// <summary>
        /// Adds an asset category to the asset window.
        /// </summary>
        public void AddAssetCategory(IAssetLoader category) {
            this.AssetViewWindow.AddCategory(category);
        }

        /// <summary>
        /// Loads a file format into the current workspace.
        /// </summary>
        public FileEditor LoadFileFormat(string filePath, bool isProject = false)
        {
            if (!File.Exists(filePath))
                return null;

            //working directory for linking original data for saving as project data
            if (string.IsNullOrEmpty(Resources.ProjectFile.WorkingDirectory))
                Resources.ProjectFile.WorkingDirectory = Path.GetDirectoryName(filePath);

            IFileFormat fileFormat = null;

            try
            {
                fileFormat = Toolbox.Core.IO.STFileLoader.OpenFileFormat(filePath);
            }
            catch (Exception ex)
            {
                DialogHandler.ShowException(ex);
            }

            var editor = fileFormat as FileEditor;
            if (fileFormat == null)
            {
                StudioLogger.WriteError(string.Format(TranslationSource.GetText("ERROR_FILE_UNSUPPORTED"), filePath));
                return null;
            }
            //File must be an editor. Todo I need to find a more intutive way for this to work.
            if (editor == null)
                return null;

            ActiveEditor = editor;

            //Node per file editor
            editor.Root.Header = fileFormat.FileInfo.FileName;
            editor.Root.Tag = fileFormat;

            //Add the file to the project resources
            Resources.AddFile(fileFormat);

            //Make sure the file format path is at the working directory instead of the project path
            //So when the user saves the files directly, they will save to the original place.
            fileFormat.FileInfo.FilePath = filePath;

            SetupActiveEditor(editor);

            StudioLogger.WriteLine(string.Format(TranslationSource.GetText("LOADING_FILE"), filePath));

            InitEditors(filePath);
            LoadProjectResources();

            return editor;
        }

        public void SetupActiveEditor(FileEditor editor)
        {
            //Add nodes to outliner
            Outliner.Nodes.Add(editor.Root);

            //Init the gl scene
            editor.Scene.Init();

            //Viewport on selection changed
            editor.Scene.SelectionUIChanged = null;
            editor.Scene.SelectionUIChanged += (o, e) =>
            {
                if (o == null) {
                    return;
                }

                if (ViewportWindow.IsFocused) {
                    if (!KeyEventInfo.State.KeyCtrl && !KeyEventInfo.State.KeyShift)
                        Outliner.DeselectAll();
                    ScrollToSelectedNode((NodeBase)o);
                }

                if (!Outliner.SelectedNodes.Contains((NodeBase)o)) {
                    Outliner.AddSelection((NodeBase)o);
                }

                //Update the property window.
                //This also updated in the outliner but the outliner doesn't have to be present this way
                if (o != null)
                    PropertyWindow.SelectedObject = (NodeBase)o;
            };
            GLContext.ActiveContext.Scene = editor.Scene;

            //Set active file format
            editor.SetActive();
            //Update editor viewport menus
            ReloadViewportMenu();
        }

        public void CreateNewProject(Type editor, Action<bool> onProjectCreated)
        {
            Name = "New Project";
            Resources = new ProjectResources();
            Resources.ProjectFile.WorkingDirectory = Runtime.ExecutableDir;
            LoadProjectResources();

            ActiveEditor = (FileEditor)Activator.CreateInstance(editor);

            //Editors must be IFileFormat types for loading/saving
            if (ActiveEditor is IFileFormat)
            {
                ((IFileFormat)ActiveEditor).FileInfo = new File_Info();
                //Add to project files for saving project data.
                Resources.AddFile((IFileFormat)ActiveEditor);
            }
            //Editors can have an optional dialog before creating new files.
            if (ActiveEditor.RenderNewFileDialog != null)
            {
                DialogHandler.Show(Name, 500, 105, () =>
                {
                    ActiveEditor.RenderNewFileDialog.Invoke();
                }, (ok) =>
                {
                    if (!ok)
                    {
                        onProjectCreated?.Invoke(false);
                        return;
                    }

                    ProcessLoading.Instance.IsLoading = true;

                    UIManager.ActionExecBeforeUIDraw += delegate
                    {
                        ActiveEditor.CreateNew();
                        SetupActiveEditor(ActiveEditor);
                        ActiveEditor.AfterLoaded();

                        onProjectCreated?.Invoke(true);
                    };
                });
            }
            else
            {
                ProcessLoading.Instance.IsLoading = true;

                ActiveEditor.CreateNew();
                SetupActiveEditor(ActiveEditor);
                onProjectCreated?.Invoke(true);
            }
        }

        /// <summary>
        /// Scrolls the outliner to the selected node.
        /// </summary>
        public void ScrollToSelectedNode(NodeBase node)
        {
            if (node.Parent == null)
                return;

            Outliner.ScrollToSelected(node);
        }

        /// <summary>
        /// Updates the active workspace instance.
        /// This should be applied when a workspace window is focused.
        /// </summary>
        public static void UpdateActive(Workspace workspace)
        {
            if (ActiveWorkspace == workspace)
                return;

            ActiveWorkspace = workspace;
            //Update error logger on switch
            workspace.PrintErrors();
            //Update the system instance.
            StudioSystem.Instance = workspace.StudioSystem;
            workspace.OnWindowLoaded();
        }

        public void OnWindowLoaded()
        {
            ViewportWindow.SetActive();
        }

        /// <summary>
        /// Loads the file data into the current workspace
        /// </summary>
        public bool LoadProjectFile(string filePath)
        {
            InitEditors(filePath);
            LoadProjectResources();

            return true;
        }

        private void LoadProjectResources()
        {
            ReloadEditors();
            LoadEditorNodes();
            OnProjectLoad?.Invoke(this, EventArgs.Empty);

            Outliner.UpdateScroll(0.0f, Resources.ProjectFile.OutlierScroll);
            OnWindowLoaded();

            ProcessLoading.Instance.Update(100, 100, "Finished loading!");
        }

        private void InitEditors(string filePath)
        {
            bool isProject = filePath.EndsWith(".json");

            ProcessLoading.Instance.Update(0, 100, "Loading Files");

            //Init the current file data
            string folder = System.IO.Path.GetDirectoryName(filePath);

            //Set current folder as project name
            if (isProject)
                Name = new DirectoryInfo(folder).Name;
            else
                Name = Path.GetFileName(filePath);

            //Load file resources
            if (isProject)
                Resources.LoadProject(filePath, ViewportWindow.Pipeline._context, this);

            ProcessLoading.Instance.Update(70, 100, "Loading Editors");

            ViewportWindow.ReloadMenus();
        }

        /// <summary>
        /// Checks and prints out any errors related to the current file data.
        /// </summary>
        public void PrintErrors()
        {
            StudioLogger.ResetErrors();
            ActiveEditor.PrintErrors();
        }

        /// <summary>
        /// Reloads the current editor data.
        /// </summary>
        public void ReloadEditors()
        {
            if (ActiveEditor == null)
                return;

            Outliner.FilterMenuItems = ActiveEditor.GetFilterMenuItems();

            ActiveEditor.SetActive();

            var docks = ActiveEditor.PrepareDocks();
            //A key to check if the existing layout changed
            string key = string.Join("", docks.Select(x => x.ToString()));
            string currentKey = string.Join("", DockedWindows.Select(x => x.ToString()));
            if (key != currentKey)
            {
                DockedWindows = docks;
                UpdateDockLayout = true;
            }
        }


        /// <summary>
        /// Saves the file data of the active editor.
        /// </summary>
        public void SaveFileData()
        {
            //Apply editor data
            SaveEditorData(false);

            var file = ActiveEditor as IFileFormat;
            string filePath = file.FileInfo.FilePath;

            //Save from project folder to working dir
            if (!string.IsNullOrEmpty(Resources.ProjectFolder))
            {
                string dir = Resources.ProjectFile.WorkingDirectory;
                filePath = Path.Combine(dir,Path.GetFileName(filePath));
                SaveFileData(file, filePath);
                return;
            }

            //Path doesn't exist so use a file dialog
            if (!File.Exists(filePath))
                SaveFileWithDialog(file);
            else
                SaveFileData(file, filePath);
        }

        public void SaveFileWithDialog()
        {
            //Save active file format 
            SaveFileWithDialog(ActiveEditor as IFileFormat);
        }

        public void SaveFileWithDialog(IFileFormat fileFormat)
        {
            ImguiFileDialog sfd = new ImguiFileDialog() { SaveDialog = true };
            sfd.FileName = fileFormat.FileInfo.FileName;

            for (int i = 0; i < fileFormat.Extension.Length; i++)
                sfd.AddFilter(fileFormat.Extension[i], fileFormat.Description?.Length > i ? fileFormat.Description[i] : "");
            if (sfd.ShowDialog("SAVE_FILE"))
                SaveFileData(fileFormat, sfd.FilePath);
        }

        public void SaveFileToModFolder(string ModPath)
        {
            SaveFileToModFolder(ModPath, ActiveEditor as IFileFormat);
        }

        public void SaveFileToModFolder(string ModPath, IFileFormat fileFormat)
        {
            //string ModFilePath = $"{ModPath}\\Pack\\Scene";
            string ModFilePath = $"{ModPath}";
            string FileName = Path.GetFileName(fileFormat.FileInfo.FilePath);

            if (!Directory.Exists(ModFilePath))
            {
                Directory.CreateDirectory(ModFilePath);
            }

            SaveFileData(fileFormat, $"{ModFilePath}\\{FileName}");
        }

        private void SaveFileData(IFileFormat fileFormat, string filePath)
        {
            //Apply editor data
            SaveEditorData(false);

            PrintErrors();

            string errorLog = StudioLogger.GetErrorLog();

            string name = Path.GetFileName(filePath);

            ProcessLoading.Instance.IsLoading = true;
            ProcessLoading.Instance.Update(0,100, $"Saving {name}", "Saving");

            try
            {
                //Save current file
                Toolbox.Core.IO.STFileSaver.SaveFileFormat(fileFormat, filePath, (o, s) => {
                    ProcessLoading.Instance.Update(70, 100, $"Compressing {fileFormat.FileInfo.Compression.ToString()}", "Saving");
                });
            }
            catch (Exception ex)
            {
                DialogHandler.ShowException(ex);
                ProcessLoading.Instance.IsLoading = false;
                return;
            }

            StudioLogger.WriteLine(string.Format(TranslationSource.GetText("SAVED_FILE"), filePath));

            if (!string.IsNullOrEmpty(errorLog))
                TinyFileDialog.MessageBoxErrorOk($"{errorLog}");

            ProcessLoading.Instance.Update(100, 100, $"Saving {name}", "Saving");
            ProcessLoading.Instance.IsLoading = false;

            TinyFileDialog.MessageBoxInfoOk($"File {filePath} has been saved!");
            PrintErrors();
        }

        public void SaveProject(string folderPath)
        {
            Resources.ProjectFolder = folderPath;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //Apply editor data
            SaveEditorData(true);
            //Save as project
            OnProjectSave?.Invoke(this, EventArgs.Empty);
            Resources.SaveProject(Path.Combine(folderPath,"Project.json"), ViewportWindow.Pipeline._context, this);
            //Save the thumbnail in the current view
            var thumb = ViewportWindow.SaveAsScreenshot(720, 512);
            thumb.Save(Path.Combine(folderPath,"Thumbnail.png"));

            //Update icon cache for thumbnails used
            IconManager.LoadTextureFile(Path.Combine(folderPath,"Thumbnail.png"), 64, 64, true);

            PrintErrors();
        }

        private void SaveEditorData(bool isProject)
        {
            //Apply editor data
            if (isProject)
            {
                Resources.ProjectFile.OutlierScroll = Outliner.ScrollY;

                //Reset node list
                Resources.ProjectFile.Nodes.Clear();
                //Save each node ID
                int nIndex = 0;
                foreach (var node in Outliner.Nodes)
                    SaveEditorNode(node, ref nIndex);
            }
        }

        //Load saved outliner node information like selection and expanded data.
        private void LoadEditorNodes()
        {
            //Each node in the hierachy will be checked via index
            int nIndex = 0;
            foreach (NodeBase node in Outliner.Nodes)
                LoadEditorNode(node, ref nIndex);
        }

        private void LoadEditorNode(NodeBase node, ref int nIndex)
        {
            nIndex++;

            if (Resources.ProjectFile.Nodes.ContainsKey(nIndex))
            {
                var n = Resources.ProjectFile.Nodes[nIndex];
                node.IsSelected = n.IsSelected;
                node.IsExpanded = n.IsExpaned;
            }

            foreach (NodeBase n in node.Children)
                LoadEditorNode(n, ref nIndex);
        }

        private void SaveEditorNode(NodeBase node, ref int nIndex)
        {
            nIndex++;

            Resources.ProjectFile.Nodes.Add(nIndex, new ProjectFile.NodeSettings()
            {
                IsExpaned = node.IsExpanded,
                IsSelected = node.IsSelected,
            });
            foreach (NodeBase n in node.Children)
                SaveEditorNode(n, ref nIndex);
        }

        public void RenderFileSaveSettings()
        {
            ActiveEditor?.RenderSaveFileSettings();
        }

        public NodeBase GetSelectedNode()
        {
            return GetSelected().FirstOrDefault();
        }

        public List<NodeBase> GetSelected()
        {
            List<NodeBase> selected = new List<NodeBase>();
            foreach (var node in Outliner.Nodes)
            {
                GetSelectedNode(node, ref selected);
            }
            return selected;
        }

        private void GetSelectedNode(NodeBase parent, ref List<NodeBase> selected)
        {
            if (parent.IsSelected && parent.Tag != null)
                selected.Add(parent);

            foreach (var child in parent.Children)
                GetSelectedNode(child, ref selected);

            return;
        }

        public void OnAssetViewportDrop()
        {
            var state = InputState.CreateMouseState();
            Vector2 screenPosition = new Vector2(state.Position.X, state.Position.Y);
            var asset = AssetViewWindow.DraggedAsset;

            //Handle the action before UI draw incase we want to update the UI (ie progress bar)
            UIManager.ActionExecBeforeUIDraw = () =>
            {
                ActiveEditor.AssetViewportDrop(asset, screenPosition);
            };
        }

        public void OnLanguageChanged()
        {
        }

        /// <summary>
        /// The key event for when a key has been pressed down.
        /// Used to perform editor shortcuts.
        /// </summary>
        public void OnKeyDown(KeyEventInfo keyInfo, bool isRepeat)
        {
            //Full screen docks
            if (KeyEventInfo.State.IsKeyDown(InputSettings.INPUT.Scene.FullScreen) && !isRepeat)
            {
                if (IsFullScreen)
                    DisableFullScreen();
                else
                {
                    //Focus on the currently focused dock and full screen it
                    var dock = this.DockedWindows.FirstOrDefault(x => x.IsWindowHovered);
                    if (dock != null)
                    {
                        foreach (var d in this.DockedWindows)
                            d.IsFullScreen = false;

                        dock.IsFullScreen = true;
                        IsFullScreen = true;
                    }
                }
            }

            if (!isRepeat)
                this.GraphWindow?.OnKeyDown(keyInfo);

            if (Outliner.IsFocused && !isRepeat)
                ViewportWindow.Pipeline._context.OnKeyDown(keyInfo, isRepeat, ViewportWindow.IsFocused);
            else if (ViewportWindow.IsFocused)
            {
                if (!isRepeat)
                    ActiveEditor.OnKeyDown(keyInfo);

                ViewportWindow.Pipeline._context.OnKeyDown(keyInfo, isRepeat, true);
                if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.ShowAddContextMenu))
                    ViewportWindow.LoadAddContextMenu();
            }
        }

        public void DisableFullScreen()
        {
            //Toggle off full screen
            foreach (var dock in this.DockedWindows.Where(x => x.IsFullScreen))
                dock.IsFullScreen = false;

            IsFullScreen = false;
        }

        /// <summary>
        /// The mouse down event for when the mouse has been pressed down.
        /// Used to perform editor shortcuts.
        /// </summary>
        public void OnMouseDown(MouseEventInfo mouseInfo)
        {
            ActiveEditor.OnMouseDown(mouseInfo);
        }

        /// <summary>
        /// The mouse move event for when the mouse has been moved around.
        /// </summary>
        public void OnMouseMove(MouseEventInfo mouseInfo)
        {
            ActiveEditor.OnMouseMove(mouseInfo);
        }

        public void OnApplicationEnter() {
            ActiveEditor?.OnEnter();
        }

        public void Dispose()
        {
            //Dispose files
            foreach (var file in Resources.Files)
                if (file is IDisposable)
                    ((IDisposable)file).Dispose();

            //Dispose renderables
            foreach (var render in DataCache.ModelCache.Values)
                render.Dispose();
            foreach (var tex in DataCache.TextureCache.Values)
                tex.Dispose();

            StudioSystem.Dispose();

            TimelineWindow.Dispose();
            GraphWindow.Dispose();
            ViewportWindow.Dispose();
            Outliner.ActiveFileFormat = null;
            Outliner.Nodes.Clear();
            Outliner.SelectedNodes.Clear();
            DataCache.ModelCache.Clear();
            DataCache.TextureCache.Clear();
        }
    }
}
