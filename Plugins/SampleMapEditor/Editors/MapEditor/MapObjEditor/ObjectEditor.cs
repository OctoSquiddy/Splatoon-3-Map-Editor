using CafeLibrary;
using CafeLibrary.Rendering;
using GLFrameworkEngine;
using GLFrameworkEngine.UI;
using ImGuiNET;
using IONET.Collada.FX.Rendering;
using MapStudio.UI;
using Newtonsoft.Json.Linq;
using OpenTK;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Toolbox.Core.IO;
using Toolbox.Core.ViewModels;
using static Toolbox.Core.Runtime;
using SampleMapEditor.Ainb;

namespace SampleMapEditor.LayoutEditor
{
    public class ObjectEditor : ILayoutEditor, UIEditToolMenu
    {
        public string Name => "Object Editor";

        public StageLayoutPlugin MapEditor { get; set; }

        public bool IsActive { get; set; } = false;

        public IToolWindowDrawer ToolWindowDrawer => new MapObjectToolMenu(this, MapEditor);

        public List<IDrawable> Renderers { get; set; } = new List<IDrawable>();

        //public NodeBase Root { get; set; } = new NodeBase(TranslationSource.GetText("MAP_OBJECTS")) { HasCheckBox = true };
        public NodeBase Root { get; set; } = new NodeBase("Map Objects") { HasCheckBox = true };

        public List<MenuItemModel> MenuItems { get; set; } = new List<MenuItemModel>();

        public int LayerSelectorIndex = 0;
        public bool ObjectNoModelHide = false;
        public bool ObjectSubModelDisplay = false;
        public bool ShowLinkedActors = false;

        //int SpawnObjectID = 1018;
        string SpawnObjectName = "RespawnPos";

        public List<NodeBase> GetSelected()
        {
            return Root.Children.Where(x => x.IsSelected).ToList();
        }

        static bool initIcons = false;
        //Loads the icons for map objects (once on init)
        static void InitIcons()
        {
            if (initIcons)
                return;

            initIcons = true;

            //Load icons for map objects
            string folder = System.IO.Path.Combine(Runtime.ExecutableDir, "Lib", "Images", "MapObjects");
            if (Directory.Exists(folder))
            {
                foreach (var imageFile in Directory.GetFiles(folder))
                {
                    IconManager.LoadTextureFile(imageFile, 32, 32);
                }
            }
        }


        //public ObjectEditor(StageLayoutPlugin editor, List<Obj> objs)
        public ObjectEditor(StageLayoutPlugin editor, List<MuObj> objs)
        {
            MapEditor = editor;
            InitIcons();

            Root.Icon = MapEditorIcons.OBJECT_ICON.ToString();

            Init(objs);

            GlobalSettings.LoadDataBase();

            var addMenu = new MenuItemModel("ADD_OBJECT", AddObjectMenuAction);
            var commonItemsMenu = new MenuItemModel("OBJECTS");
            commonItemsMenu.MenuItems.Add(new MenuItemModel("SPAWNPOINT", () => AddObject("RespawnPos", true)));

            GLContext.ActiveContext.Scene.MenuItemsAdd.Add(addMenu);
            GLContext.ActiveContext.Scene.MenuItemsAdd.Add(commonItemsMenu);

            MenuItems.AddRange(GetEditMenuItems());

            // Register the callback for colored link rendering
            ObjectLinkDrawer.GetLinkColorCallback = GetLinkColorForObjects;
        }

        /// <summary>
        /// Callback for ObjectLinkDrawer to get link-type-specific colors.
        /// </summary>
        private static (OpenTK.Vector4 srcColor, OpenTK.Vector4 destColor)? GetLinkColorForObjects(
            IObjectLink sourceObj, ITransformableObject destObj, int destIndex)
        {
            // Get the source MuObj to access its Links list
            if (sourceObj is EditableObject editSource)
            {
                var muObj = editSource.UINode.Tag as MuObj;
                if (muObj != null && muObj.Links != null && destIndex < muObj.Links.Count)
                {
                    var linkName = muObj.Links[destIndex].Name;
                    var color = ActorLinkRenderer.GetLinkColor(linkName);
                    // Use same color for both ends for solid colored line
                    return (color, color);
                }
            }
            return null;
        }

        public List<MenuItemModel> GetToolMenuItems()
        {
            List<MenuItemModel> items = new List<MenuItemModel>();
            return items;
        }

        MapObjectSelector ObjectSelector;

        public void DrawEditMenuBar()
        {
            DrawObjectSpawner();

            if (ImguiCustomWidgets.MenuItemTooltip($"   {IconManager.ADD_ICON}   ", "ADD", InputSettings.INPUT.Scene.Create))
            {
                AddObjectMenuAction();
            }
            if (ImguiCustomWidgets.MenuItemTooltip($"   {IconManager.DELETE_ICON}   ", "REMOVE", InputSettings.INPUT.Scene.Delete))
            {
                MapEditor.Scene.BeginUndoCollection();
                RemoveSelected();
                MapEditor.Scene.EndUndoCollection();
            }
            if (ImguiCustomWidgets.MenuItemTooltip($"   {IconManager.COPY_ICON}   ", "COPY", InputSettings.INPUT.Scene.Copy))
            {
                CopySelected();
            }
            if (ImguiCustomWidgets.MenuItemTooltip($"   {IconManager.PASTE_ICON}   ", "PASTE", InputSettings.INPUT.Scene.Paste))
            {
                PasteSelected();
            }
            if (ImguiCustomWidgets.MenuItemTooltip($"   {IconManager.PASTE_ICON}   ##Symetric", "Paste Symetrically", InputSettings.INPUT.Scene.PasteSymetric))
            {
                PasteSelected(true);
            }
        }

        public void DrawHelpWindow()
        {
            if (ImGuiNET.ImGui.CollapsingHeader("Objects", ImGuiNET.ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiHelper.BoldTextLabel(InputSettings.INPUT.Scene.Create, "Create Object.");
            }
        }



        private void DrawObjectSpawner()
        {
            //Selector popup window instance
            if (ObjectSelector == null)
            {
                var objects = GlobalSettings.ActorDatabase.Values.OrderBy(x => x.Name).ToList();
                ObjectSelector = new MapObjectSelector(objects);
                ObjectSelector.CloseOnSelect = true;
                //Update current spawn id when selection is changed // ??? ~~~ Remove comment ~~~
                // Update current spawn name when selection is changed
                ObjectSelector.SelectionChanged += delegate
                {
                    SpawnObjectName = ObjectSelector.GetSelectedID();
                };
            }
            // Current spawnable
            string resName = Obj.GetResourceName(SpawnObjectName);
            var pos = ImGui.GetCursorScreenPos();

            //Make the window cover part of the viewport
            var viewportHeight = GLContext.ActiveContext.Height;
            var spawnPopupHeight = viewportHeight;

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(pos.X, pos.Y + 27));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(300, spawnPopupHeight));

            //Render popup window when opened
            var flags = ImGuiWindowFlags.NoScrollbar;
            if (ImGui.BeginPopup("spawnPopup", ImGuiWindowFlags.Popup | flags))
            {

                if (ImGui.BeginChild("spawnableChild", new System.Numerics.Vector2(300, spawnPopupHeight), false, flags))
                {
                    ObjectSelector.Render(false);
                }
                ImGui.EndChild();
                ImGui.EndPopup();
            }

            //Menu to open popup
            ImGui.PushItemWidth(150);
            if (ImGui.BeginCombo("##spawnableCB", resName))
            {
                ImGui.EndCombo();
            }
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(0))
            {
                if (ImGui.IsPopupOpen("spawnPopup"))
                    ImGui.CloseCurrentPopup();
                else
                {
                    ImGui.OpenPopup("spawnPopup");
                    ObjectSelector.SetSelectedID(SpawnObjectName);
                }
            }
            ImGui.PopItemWidth();
        }


        public List<MenuItemModel> GetEditMenuItems()
        {
            List<MenuItemModel> items = new List<MenuItemModel>();
            items.Add(new MenuItemModel($"   {IconManager.ADD_ICON}   ", AddObjectMenuAction));

            bool hasSelection = GetSelected().Count > 0;

            items.Add(new MenuItemModel($"   {IconManager.COPY_ICON}   ", CopySelected) { IsEnabled = hasSelection, ToolTip = $"Copy ({InputSettings.INPUT.Scene.Copy})" });

            items.Add(new MenuItemModel($"   {IconManager.PASTE_ICON}   ", () => { PasteSelected(); }) { IsEnabled = hasSelection, ToolTip = $"Paste ({InputSettings.INPUT.Scene.Paste})" });

            items.Add(new MenuItemModel($"Paste Symtrically", () => { PasteSelected(true); }) { IsEnabled = hasSelection, ToolTip = $"Paste Symetrically ({InputSettings.INPUT.Scene.PasteSymetric})" });

            items.Add(new MenuItemModel($"   {IconManager.DELETE_ICON}   ", () =>
            {
                //GLContext.ActiveContext.Scene.DeleteSelected();
                MapEditor.Scene.BeginUndoCollection();
                RemoveSelected();
                MapEditor.Scene.EndUndoCollection();
            })
            { IsEnabled = hasSelection, ToolTip = $" Delete ({InputSettings.INPUT.Scene.Delete})" });

            return items;
        }

        public void ReloadEditor()
        {
            Root.Header = TranslationSource.GetText("MAP_OBJECTS");

            foreach (EditableObject render in Renderers)
            {
                UpdateObjectLinks(render);

                render.CanSelect = true;

                /*if (((Obj)render.UINode.Tag).IsSkybox)
                    render.CanSelect = false;*/
            }
        }

        void Init(List<MuObj> objs)
        {
            Root.Children.Clear();
            Renderers.Clear();

            //Load the current tree list
            for (int i = 0; i < objs?.Count; i++)
                Add(Create(objs[i]));

            if (Root.Children.Any(x => x.IsSelected))
                Root.IsExpanded = true;
        }

        // Function to save from renderable aboject to byml data
        public void OnSave(StageDefinition stage)
        {
            //stage.Objs = new List<Obj>();
            stage.Actors = new List<MuObj>();

            foreach (EditableObject render in Renderers)
            {
                var obj = (MuObj)render.UINode.Tag;
                obj.Translate = new ByamlVector3F(
                    render.Transform.Position.X / 10.0f,
                    render.Transform.Position.Y / 10.0f,
                    render.Transform.Position.Z / 10.0f);
                obj.Rotate = new ByamlVector3F(
                    render.Transform.RotationEuler.X,
                    render.Transform.RotationEuler.Y,
                    render.Transform.RotationEuler.Z);
                obj.Scale = new ByamlVector3F(
                    render is BfresRender ? render.Transform.Scale.X / 10.0f : render.Transform.Scale.X,
                    render is BfresRender ? render.Transform.Scale.Y / 10.0f : render.Transform.Scale.Y,
                    render is BfresRender ? render.Transform.Scale.Z / 10.0f : render.Transform.Scale.Z);
                stage.Actors.Add(obj);
            }
        }

        public void OnMouseDown(MouseEventInfo mouseInfo)
        {
            bool isActive = Workspace.ActiveWorkspace.ActiveEditor.SubEditor == this.Name;

            if (isActive && KeyEventInfo.State.KeyAlt && mouseInfo.LeftButton == OpenTK.Input.ButtonState.Pressed)
                AddObject(SpawnObjectName);
        }
        public void OnMouseUp(MouseEventInfo mouseInfo)
        {
        }
        public void OnMouseMove(MouseEventInfo mouseInfo)
        {
        }

        public void Add(EditableObject render, bool undo = false)
        {
            MapEditor.AddRender(render, undo);
        }

        public void Remove(EditableObject render, bool undo = false)
        {
            MapEditor.RemoveRender(render, undo);
        }



        /// <summary>
        /// When an object asset is drag and dropped into the viewport.
        /// </summary>
        //public void OnAssetViewportDrop(int id, Vector2 screenPosition)
        public void OnAssetViewportDrop(string actorName, Vector2 screenPosition)
        {
            var context = GLContext.ActiveContext;

            Quaternion rotation = Quaternion.Identity;
            //Spawn by drag/drop coordinates in 3d space.
            Vector3 position = context.ScreenToWorld(screenPosition.X, screenPosition.Y, 100);
            //Face the camera
            if (MapStudio.UI.GlobalSettings.Current.Asset.FaceCameraAtSpawn)
                rotation = Quaternion.FromEulerAngles(0, -context.Camera.RotationY, 0);
            //Drop to collision if used.
            if (context.EnableDropToCollision)
            {
                Quaternion rotateByDrop = Quaternion.Identity;
                CollisionDetection.SetObjectToCollision(context, context.CollisionCaster, screenPosition, ref position, ref rotateByDrop);
                if (context.TransformTools.TransformSettings.RotateFromNormal)
                    rotation *= rotateByDrop;
            }

            // Add the object with the dropped name and set the transform 
            var render = AddObject(actorName);
            var obj = render.UINode.Tag as Obj;

            //Set the dropped place based on where the current mouse is.
            render.Transform.Position = position;
            render.Transform.Rotation = rotation;
            render.Transform.UpdateMatrix(true);
            render.UINode.IsSelected = true;

            this.MapEditor.Scene.SelectionUIChanged?.Invoke(render.UINode, EventArgs.Empty);

            //Update the SRT tool if active
            GLContext.ActiveContext.TransformTools.UpdateOrigin();

            //Force the editor to display
            if (!IsActive)
            {
                IsActive = true;
                ((StageLayoutPlugin)Workspace.ActiveWorkspace.ActiveEditor).ReloadOutliner(true);
            }
        }

        /// <summary>
        /// When a preset asset is drag and dropped into the viewport.
        /// Spawns multiple objects at once with automatic linking.
        /// </summary>
        public void OnPresetViewportDrop(PresetAssetItem preset, Vector2 screenPosition)
        {
            var context = GLContext.ActiveContext;

            Quaternion rotation = Quaternion.Identity;
            // Spawn by drag/drop coordinates in 3D space
            Vector3 basePosition = context.ScreenToWorld(screenPosition.X, screenPosition.Y, 100);
            // Face the camera
            if (MapStudio.UI.GlobalSettings.Current.Asset.FaceCameraAtSpawn)
                rotation = Quaternion.FromEulerAngles(0, -context.Camera.RotationY, 0);
            // Drop to collision if used
            if (context.EnableDropToCollision)
            {
                Quaternion rotateByDrop = Quaternion.Identity;
                CollisionDetection.SetObjectToCollision(context, context.CollisionCaster, screenPosition, ref basePosition, ref rotateByDrop);
                if (context.TransformTools.TransformSettings.RotateFromNormal)
                    rotation *= rotateByDrop;
            }

            // Deselect all before spawning
            GLContext.ActiveContext.Scene.DeselectAll(GLContext.ActiveContext);

            // Spawn all objects from the preset
            List<EditableObject> spawnedObjects = new List<EditableObject>();
            foreach (var objDef in preset.ObjectsToSpawn)
            {
                var render = AddObjectForPreset(objDef.ActorName);
                if (render != null)
                {
                    // Set position with offset
                    render.Transform.Position = basePosition + objDef.PositionOffset;
                    render.Transform.Rotation = rotation;
                    render.Transform.UpdateMatrix(true);
                    spawnedObjects.Add(render);
                }
            }

            // Create links between objects
            for (int i = 0; i < preset.ObjectsToSpawn.Count; i++)
            {
                var objDef = preset.ObjectsToSpawn[i];
                if (objDef.LinksToCreate != null && objDef.LinksToCreate.Count > 0 && i < spawnedObjects.Count)
                {
                    var sourceObj = spawnedObjects[i].UINode.Tag as MuObj;
                    if (sourceObj != null)
                    {
                        foreach (var linkDef in objDef.LinksToCreate)
                        {
                            if (linkDef.TargetObjectIndex >= 0 && linkDef.TargetObjectIndex < spawnedObjects.Count)
                            {
                                var targetObj = spawnedObjects[linkDef.TargetObjectIndex].UINode.Tag as MuObj;
                                if (targetObj != null)
                                {
                                    var link = new MuObj.Link
                                    {
                                        Name = linkDef.LinkName,
                                        Dst = targetObj.Hash
                                    };
                                    sourceObj.Links.Add(link);
                                }
                            }
                        }
                    }
                }
            }

            // Select all spawned objects
            foreach (var render in spawnedObjects)
            {
                render.UINode.IsSelected = true;
            }

            // Update object links visualization
            foreach (var render in spawnedObjects)
            {
                UpdateObjectLinks(render);
            }

            if (spawnedObjects.Count > 0)
            {
                this.MapEditor.Scene.SelectionUIChanged?.Invoke(spawnedObjects[0].UINode, EventArgs.Empty);
            }

            // Update the SRT tool if active
            GLContext.ActiveContext.TransformTools.UpdateOrigin();

            // Force the editor to display
            if (!IsActive)
            {
                IsActive = true;
                ((StageLayoutPlugin)Workspace.ActiveWorkspace.ActiveEditor).ReloadOutliner(true);
            }

            Console.WriteLine($"Preset '{preset.Name}' spawned {spawnedObjects.Count} objects");
        }

        /// <summary>
        /// Adds an object without selecting it or changing cursor position (for preset spawning)
        /// </summary>
        private EditableObject AddObjectForPreset(string actorName)
        {
            Console.WriteLine($"~ Called ObjectEditor.AddObjectForPreset({actorName}) ~");

            if (!GlobalSettings.ActorDatabase.ContainsKey(actorName))
            {
                Console.WriteLine($"Warning: Actor '{actorName}' not found in database");
                return null;
            }

            // Get Actor Class Name
            string className = GlobalSettings.ActorDatabase[actorName].ClassName;
            Type elem = typeof(MuObj);
            ByamlSerialize.SetMapObjType(ref elem, actorName);
            var inst = (MuObj)Activator.CreateInstance(elem);

            inst.Name = actorName;
            inst.Gyaml = actorName;

            List<string> mAllInstanceID = new List<string>();
            List<ulong> mAllHash = new List<ulong>();
            List<uint> mAllSRTHash = new List<uint>();

            foreach (IDrawable obj in MapEditor.Scene.Objects)
            {
                if (obj is EditableObject && ((EditableObject)obj).UINode.Tag is MuObj)
                {
                    mAllInstanceID.Add(((MuObj)((EditableObject)obj).UINode.Tag).InstanceID);
                    mAllHash.Add(((MuObj)((EditableObject)obj).UINode.Tag).Hash);
                    mAllSRTHash.Add(((MuObj)((EditableObject)obj).UINode.Tag).SRTHash);
                }
            }

            string InstanceID = GenInstanceID();
            while (mAllInstanceID.Contains(InstanceID))
                InstanceID = GenInstanceID();

            ulong Hash = GenHash();
            while (mAllHash.Contains(Hash))
                Hash = GenHash();

            uint SRTHash = GenSRTHash();
            while (mAllSRTHash.Contains(SRTHash))
                SRTHash = GenSRTHash();

            inst.InstanceID = InstanceID;
            inst.Hash = Hash;
            inst.SRTHash = SRTHash;
            var rend = Create(inst);

            if (ObjectSubModelDisplay && rend is BfresRender bfresRender)
            {
                List<string> subModelNames = GlobalSettings.ActorDatabase[actorName].SubModels;

                foreach (var model in bfresRender.Models)
                {
                    if (subModelNames.Contains(model.Name))
                        model.IsVisible = true;
                }
            }

            Add(rend, true);

            return rend;
        }

        public void OnKeyDown(KeyEventInfo keyInfo)
        {
            bool isActive = Workspace.ActiveWorkspace.ActiveEditor.SubEditor == this.Name;

            if (isActive && !keyInfo.KeyCtrl && keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Create))
                AddObject(SpawnObjectName);
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Copy) && GetSelected().Count > 0)
                CopySelected();
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Paste))
                PasteSelected();
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.PasteSymetric))
                PasteSelected(true);
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Dupe))
            {
                CopySelected();
                PasteSelected();
                copied.Clear();
            }
        }

        public void ExportModel()
        {
            Console.WriteLine("~ Called ObjectEditor.ExportModel() ~ [Commented Out]");
            /*ImguiFileDialog dlg = new ImguiFileDialog();
            dlg.SaveDialog = true;
            dlg.AddFilter(".dae", ".dae");
            if (dlg.ShowDialog())
                ExportModel(dlg.FilePath);*/
        }

        List<IDrawable> copied = new List<IDrawable>();


        public void CopySelected()
        {
            var selected = Renderers.Where(x => ((EditableObject)x).IsSelected).ToList();

            copied.Clear();
            copied = selected;
        }

        public void PasteSelected(bool Symetrically = false)
        {
            GLContext.ActiveContext.Scene.DeselectAll(GLContext.ActiveContext);

            // Collect all existing IDs/Hashes first
            List<string> mAllInstanceID = new List<string>();
            List<ulong> mAllHash = new List<ulong>();
            List<uint> mAllSRTHash = new List<uint>();

            foreach (IDrawable objects in MapEditor.Scene.Objects)
            {
                if (objects is EditableObject && ((EditableObject)objects).UINode.Tag is MuObj)
                {
                    mAllInstanceID.Add(((MuObj)((EditableObject)objects).UINode.Tag).InstanceID);
                    mAllHash.Add(((MuObj)((EditableObject)objects).UINode.Tag).Hash);
                    mAllSRTHash.Add(((MuObj)((EditableObject)objects).UINode.Tag).SRTHash);
                }
            }

            // Build mapping of old hash -> new hash for link remapping
            Dictionary<ulong, ulong> hashMapping = new Dictionary<ulong, ulong>();
            List<(EditableObject duplicated, MuObj newObj, EditableObject original)> createdObjects = new List<(EditableObject, MuObj, EditableObject)>();

            // First pass: Create all objects and build hash mapping
            foreach (EditableObject ob in copied)
            {
                var obj = ob.UINode.Tag as MuObj;
                var duplicated = Create(obj.Clone());

                if (Symetrically)
                {
                    duplicated.Transform.Position = new OpenTK.Vector3(-ob.Transform.Position.X, ob.Transform.Position.Y, -ob.Transform.Position.Z);
                    duplicated.Transform.Scale = ob.Transform.Scale;
                    duplicated.Transform.RotationEulerDegrees = new OpenTK.Vector3(180.0f-ob.Transform.RotationEulerDegrees.X, -ob.Transform.RotationEulerDegrees.Y, 180.0f-ob.Transform.RotationEulerDegrees.Z);
                }
                else
                {
                    duplicated.Transform.Position = ob.Transform.Position;
                    duplicated.Transform.Scale = ob.Transform.Scale;
                    duplicated.Transform.Rotation = ob.Transform.Rotation;
                }

                duplicated.Transform.UpdateMatrix(true);
                duplicated.IsSelected = true;

                string InstanceID = GenInstanceID();
                while (mAllInstanceID.Contains(InstanceID))
                    InstanceID = GenInstanceID();
                mAllInstanceID.Add(InstanceID);

                ulong Hash = GenHash();
                while (mAllHash.Contains(Hash))
                    Hash = GenHash();
                mAllHash.Add(Hash);

                uint SRTHash = GenSRTHash();
                while (mAllSRTHash.Contains(SRTHash))
                    SRTHash = GenSRTHash();
                mAllSRTHash.Add(SRTHash);

                var obj1 = duplicated.UINode.Tag as MuObj;

                // Store mapping from old hash to new hash
                hashMapping[obj.Hash] = Hash;

                obj1.InstanceID = InstanceID;
                obj1.Hash = Hash;
                obj1.SRTHash = SRTHash;

                // Clear AINB link since it's a copy
                if (Symetrically)
                {
                    obj1.AIGroupID = "";
                }

                createdObjects.Add((duplicated, obj1, ob));
            }

            // Second pass: Remap links to point to new objects (if target was also copied)
            foreach (var (duplicated, newObj, original) in createdObjects)
            {
                if (Symetrically)
                {
                    // Remap links: if target was copied, update to new hash; otherwise remove link
                    var linksToKeep = new List<MuObj.Link>();
                    foreach (var link in newObj.Links)
                    {
                        if (hashMapping.TryGetValue(link.Dst, out ulong newDstHash))
                        {
                            // Target was also copied - update link to point to new object
                            var remappedLink = link.Clone();
                            remappedLink.Dst = newDstHash;
                            linksToKeep.Add(remappedLink);
                        }
                        // If target wasn't copied, don't keep the link
                    }
                    newObj.Links = linksToKeep;
                }

                Add(duplicated, true);
            }
        }

        public void RemoveSelected()
        {
            var selected = Renderers.Where(x => ((EditableObject)x).IsSelected).ToList();
            foreach (EditableObject ob in selected)
                Remove(ob, true);
        }

        public void RemoveByLayer(string layerName)
        {
            var selected = Renderers.Where(x => ((MuObj)((EditableObject)x).UINode.Tag).Layer == layerName).ToList();
            foreach (EditableObject ob in selected)
                Remove(ob, true);
        }

        //private EditableObject Create(Obj obj)
        unsafe private EditableObject Create(MuObj obj)
        {
            Console.WriteLine($"Creating object with name: {obj.Name}");
            string name = GetResourceName(obj);

            // Use AreaWireframeRender for Area/Locator/KeepOut objects (wireframe with correct scale)
            // Use TransformableObject (default cube) for other objects without models
            EditableObject render;
            if (obj.Name.Contains("Area") || obj.Name.Contains("Locator") || obj.Name.Contains("KeepOut"))
            {
                render = new AreaWireframeRender(Root);
            }
            else
            {
                render = new TransformableObject(Root);
            }

            var filePath = Obj.FindFilePath(Obj.GetResourceName(obj.Name));


            //Don't load it for now if the model is already cached. It should load up instantly
            //TODO should use a less intrusive progress bar (like top/bottom of the window)
            if (!DataCache.ModelCache.ContainsKey(filePath) && File.Exists(filePath))
            {
                ProcessLoading.Instance.IsLoading = true;
                ProcessLoading.Instance.Update(0, 100, $"Loading model {name}");
            }

            //Open a bfres resource if one exist.
            /*if (System.IO.File.Exists(filePath))  // for mk8. Splatoon 2 does it differently.
                render = new BfresRender(filePath, Root);*/

            // Open a bfres resource if one exists.
            if (File.Exists(filePath))
            {
                Console.WriteLine(filePath);

                MemoryStream stream1 = null;
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryDataReader br = new BinaryDataReader(stream, Encoding.UTF8, false);
                    uint ZSTDMagic = br.ReadUInt32();
                    br.Position = 0;

                    if (ZSTDMagic == 0xFD2FB528)
                    {
                        ZstdNet.Decompressor Dec = new ZstdNet.Decompressor();
                        byte[] res = Dec.Unwrap(br.ReadBytes((int)br.Length));

                        stream1 = new MemoryStream();
                        stream1.Write(res, 0, res.Length);
                        stream1.Position = 0;
                    }
                    else
                    {
                        stream.CopyTo(stream1);
                    }
                }

                //if (s.files.Find(x => x.FileName == "output.bfres") != null)
                if (stream1 != null)
                {
                    // Console.WriteLine($"File {stream1.} has a model");
                    render = new BfresRender(stream1, filePath, Root);
                }

                //render = new BfresRender(filePath, Root);
            }

            if (render is BfresRender)
            {
                if (GlobalSettings.ActorDatabase.ContainsKey(obj.Name))
                {
                    //Obj requires specific model to display
                    string modelName = GlobalSettings.ActorDatabase[obj.Name].FmdbName; // ??? -
                    List<string> subModelNames = GlobalSettings.ActorDatabase[obj.Name].SubModels;

                    if (!string.IsNullOrEmpty(modelName))
                    {
                        foreach (var model in ((BfresRender)render).Models)
                        {
                            bool loadMainModel = model.Name == modelName;
                            bool loadSubModel = ObjectSubModelDisplay && subModelNames.Contains(modelName);

                            if (!loadMainModel)
                                model.IsVisible = false;
                        }
                    }
                }
            }

            if (ProcessLoading.Instance.IsLoading)
            {
                ProcessLoading.Instance.Update(100, 100, $"Finished loading model {name}");
                ProcessLoading.Instance.IsLoading = false;
            }

            //Set the UI label and property tag
            render.UINode.Header = GetNodeHeader(obj);
            render.UINode.Tag = obj;
            render.UINode.ContextMenus.Add(new MenuItemModel("EXPORT", () => ExportModel()));
            //Set custom UI properties
            ((EditableObjectNode)render.UINode).UIProperyDrawer += delegate
            {
                // Get the currently selected object from the workspace
                // This ensures we always show the properties of the actually selected object
                var selectedNodes = Workspace.ActiveWorkspace.GetSelected().ToList();
                if (selectedNodes.Count == 0)
                    return;

                var currentObj = selectedNodes[0].Tag as MuObj;
                if (currentObj == null)
                    return;

                if (ImGui.CollapsingHeader(TranslationSource.GetText("EDIT"), ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (ImGui.Button(TranslationSource.GetText("CHANGE_OBJECT")))
                    {
                        EditObjectMenuAction();
                    }
                }

                string category = "Links";
                int numColumns = 2;

                if (ImGui.CollapsingHeader("Links", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (ImGui.Button($"   {IconManager.ADD_ICON}   ###{category}"))
                    {
                        currentObj.Links.Add(new MuObj.Link());
                    }

                    ImGui.SameLine();

                    bool ActiveCB = GlobalSettings.IsLinkingComboBoxActive;
                    if (ImGui.Checkbox("Activate Combo Box", ref ActiveCB))
                    {
                        GlobalSettings.IsLinkingComboBoxActive = ActiveCB;
                    }

                    ImGui.BeginGroup();
                    for (int i = 0; i < currentObj.Links.Count; i++)
                    {
                        if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###{category}.{i}"))
                        {
                            if (currentObj.Links.Count > 0)
                                currentObj.Links.RemoveAt(i);
                        }

                        ImGui.SameLine();

                        if (ImGui.Button($"...###{category}1.{i}"))
                        {
                            EditObjectLinkMenuAction(currentObj, i);
                        }

                        ImGui.SameLine();

                        if (ImGui.Button($"   {IconManager.COPY_ICON}   ###{category}2.{i}"))
                        {
                            MuObj.Link Duplicate = currentObj.Links[i].Clone();
                            currentObj.Links.Insert(i, Duplicate);
                        }
                    }
                    ImGui.EndGroup();

                    ImGui.SameLine();

                    ImGui.BeginGroup();
                    ImGui.BeginColumns("##" + category + numColumns.ToString(), numColumns);
                    for (int i = 0; i < currentObj.Links.Count; i++)
                    {
                        float colwidth = ImGui.GetColumnWidth();
                        ImGui.PushItemWidth(colwidth - 6);

                        string inputValueStr = currentObj.Links[i].Name;
                        if (string.IsNullOrEmpty(inputValueStr))
                            inputValueStr = " ";

                        if (GlobalSettings.IsLinkingComboBoxActive)
                        {
                            List<string> LinkNames = new List<string>()
                            {
                                "ToParent",
                                "ToGeneralLocator",
                                "ToDropItem",
                                "ToBindObjLink",
                                "NextAirBall",
                                "FinalAirball",
                                "LinkToLocator",
                                "ToBuildItem",
                                "Accessories",
                                "AreaLinks",
                                "BindLink",
                                "CoreBattleManagers",
                                "CorrectPoint",
                                "CorrectPointArray",
                                "CrowdMorayHead",
                                "EnemyPaintAreaAirBall",
                                "JumpPoints",
                                "JumpTarget",
                                "JumpTargetCandidates",
                                "LastHitPosArea",
                                "LinksToActor",
                                "LinkToAnotherPipeline",
                                "LinkToCage",
                                "LinkToCompass",
                                "LinkToEnemyGenerators",
                                "LinkToMoveArea",
                                "LinkToTarget",
                                "LinkToWater",
                                "LocatorLink",
                                "Reference",
                                "RestartPos",
                                "SafePosLinks",
                                "ShortcutAirBall",
                                "SpawnObjLinks",
                                "SubAreaInstanceIds",
                                "Target",
                                "TargetArea",
                                "TargetLift",
                                "TargetPropeller",
                                "ToActor",
                                "ToArea",
                                "ToBindActor",
                                "ToFriendLink",
                                "ToGateway",
                                "ToNotPaintableArea",
                                "ToPlayerFrontDirLocator",
                                "ToProjectionAreas",
                                "ToRouteTargetPointArray",
                                "ToSearchLimitArea",
                                "ToShopRoom",
                                "ToTable",
                                "ToTarget_Cube",
                                "ToWallaObjGroupTag",
                                "UtsuboxLocator",
                            };

                            if (!LinkNames.Contains(inputValueStr))
                            {
                                inputValueStr = LinkNames[0];
                                currentObj.Links[i].Name = LinkNames[0];
                            }

                            if (ImGui.BeginCombo($"###LinkingCBox.Name{i}", inputValueStr, ImGuiComboFlags.HeightLarge))
                            {
                                foreach (string LinkName in LinkNames)
                                {
                                    bool isSelected = inputValueStr == LinkName;

                                    if (ImGui.Selectable(LinkName, isSelected))
                                    {
                                        currentObj.Links[i].Name = LinkName;
                                    }

                                    if (isSelected)
                                    {
                                        ImGui.SetItemDefaultFocus();
                                    }
                                }

                                ImGui.EndCombo();
                            }
                        }
                        else
                        {
                            if (ImGui.InputText($"###LinkingBox.Name{i}", ref inputValueStr, 0x1000))
                            {
                                currentObj.Links[i].Name = inputValueStr;
                            }
                        }

                        ImGui.PopItemWidth();

                        ImGui.NextColumn();

                        colwidth = ImGui.GetColumnWidth();
                        ImGui.PushItemWidth(colwidth - 6);

                        ulong inputValue = currentObj.Links[i].Dst;
                        ulong* ulongptr = &inputValue;

                        if (ImGui.InputScalar($"###LinkingBox.Dst{i}", ImGuiDataType.U64, (IntPtr)ulongptr))
                        {
                            currentObj.Links[i].Dst = (ulong)inputValue;
                        }

                        ImGui.PopItemWidth();
                        ImGui.NextColumn();
                    }
                    ImGui.EndColumns();
                    ImGui.EndGroup();
                }

                // AINB Tab - Add/Remove AINB links for objects
                DrawAINBTab(currentObj);

                // AINB Category - Shows connected AINB nodes for objects with AIGroupID
                DrawAINBCategory(currentObj);

                var gui = new MapObjectUI();
                gui.Render(currentObj, Workspace.ActiveWorkspace.GetSelected().Select(x => x.Tag));
            };


            //Icons for map objects
            string folder = System.IO.Path.Combine(Runtime.ExecutableDir, "Lib", "Images", "MapObjects");
            if (IconManager.HasIcon(System.IO.Path.Combine(folder, $"{name}.png")))
            {
                render.UINode.Icon = System.IO.Path.Combine(folder, $"{name}.png");
                //A sprite drawer for displaying distant objects
                //Todo this is not used currently and may need improvements
                render.SpriteDrawer = new SpriteDrawer();
            }
            else
                render.UINode.Icon = "Node";

            //Disable selection on skyboxes
            render.CanSelect = true;    // We don't have any skyboxes to worry about here

            render.AddCallback += delegate
            {
                Console.WriteLine("~~ render.AddCallback called ~~");
                Renderers.Add(render);
                //StudioSystem.Instance.AddActor(ActorInfo);
            };
            render.RemoveCallback += delegate
            {
                Console.WriteLine("~~ render.RemoveCallback called ~~");
                //Remove actor data on disposing the object.
                Renderers.Remove(render);
                //StudioSystem.Instance.RemoveActor(ActorInfo);
                //ActorInfo?.Dispose();
            };


            //Custom frustum culling.
            //Map objects should just cull one big box rather than individual meshes.
            if (render is BfresRender)
                ((BfresRender)render).FrustumCullingCallback = () => {
                    /*if (!obj.IsSkybox)
                        ((BfresRender)render).UseDrawDistance = true;*/
                    ((BfresRender)render).UseDrawDistance = true;

                    return FrustumCullObject((BfresRender)render);
                };

            //Render links
            UpdateObjectLinks(render);

            //Update the render transform
            render.Transform.Position = new OpenTK.Vector3(
                obj.Translate.X * 10.0f,
                obj.Translate.Y * 10.0f,
                obj.Translate.Z * 10.0f);
            render.Transform.RotationEulerDegrees = new OpenTK.Vector3(
            /*
            obj.Rotate.X,
            obj.Rotate.Y,
            obj.Rotate.Z);*/
                obj.RotateDegrees.X,
                obj.RotateDegrees.Y,
                obj.RotateDegrees.Z);
            render.Transform.Scale = new OpenTK.Vector3(
                render is BfresRender ? obj.Scale.X * 10.0f : obj.Scale.X,
                render is BfresRender ? obj.Scale.Y * 10.0f : obj.Scale.Y,
                render is BfresRender ? obj.Scale.Z * 10.0f : obj.Scale.Z);
            render.Transform.UpdateMatrix(true);

            //Updates for property changes
            obj.PropertyChanged += delegate
            {
                render.UINode.Header = GetNodeHeader(obj);
                string objName = GetResourceName(obj);

                string folder = System.IO.Path.Combine(Runtime.ExecutableDir, "Lib", "Images", "MapObjects");
                string iconPath = System.IO.Path.Combine(folder, $"{objName}.png");

                if (IconManager.HasIcon(iconPath))
                    render.UINode.Icon = iconPath;
                else
                    render.UINode.Icon = "Node";

                UpdateObjectLinks(render);

                /*//Update actor parameters into the actor class
                ((ActorModelBase)ActorInfo).Parameters = obj.Params;*/ // ???

                //Update the view if properties are updated.
                GLContext.ActiveContext.UpdateViewport = true;
            };
            return render;
        }


        private void UpdateObjectLinks(EditableObject render)
        {
            render.DestObjectLinks.Clear();

            // Only show links if the setting is enabled
            if (!ShowLinkedActors)
                return;

            var obj = render.UINode.Tag as MuObj;
            if (obj == null || obj.Links == null || obj.Links.Count == 0)
                return;

            // Find target objects by their Hash
            foreach (var link in obj.Links)
            {
                if (link.Dst == 0)
                    continue;

                // Search for the linked object by its Hash
                foreach (var sceneObj in GLContext.ActiveContext.Scene.Objects)
                {
                    if (sceneObj is EditableObject editObj)
                    {
                        var targetMuObj = editObj.UINode.Tag as MuObj;
                        if (targetMuObj != null && targetMuObj.Hash == link.Dst)
                        {
                            render.DestObjectLinks.Add(editObj);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes all object links in the scene (called when ShowLinkedActors setting changes)
        /// </summary>
        public void RefreshAllObjectLinks()
        {
            foreach (EditableObject render in Renderers)
            {
                UpdateObjectLinks(render);
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }

        private void TryFindObjectLink(EditableObject render, EditableObject obj, object objInstance)
        {
            if (objInstance == null)
                return;

            if (obj.UINode.Tag == objInstance)
                render.DestObjectLinks.Add(obj);
        }

        private void TryFindPathLink(EditableObject render, RenderablePath path, object pathInstance, object pointInstance)
        {
            if (pathInstance == null)
                return;

            var properties = path.UINode.Tag;
            if (properties == pathInstance)
            {
                foreach (var point in path.PathPoints)
                {
                    if (point.UINode.Tag == pointInstance)
                    {
                        render.DestObjectLinks.Add(point);
                        return;
                    }
                }
                if (path.PathPoints.Count > 0)
                    render.DestObjectLinks.Add(path.PathPoints.FirstOrDefault());
            }
        }

        /// <summary>
        /// Draws the AINB category for objects with an AIGroupID.
        /// Shows all connected AINB nodes and their links.
        /// </summary>
        private void DrawAINBCategory(MuObj obj)
        {
            // Only show if object has an AIGroupID
            if (string.IsNullOrEmpty(obj.AIGroupID))
                return;

            // Get the AINB file from the stage definition
            AINB ainbData = MapEditor?.MapLoader?.stageDefinition?.AINBFile;
            if (ainbData?.Nodes == null)
            {
                // Draw header even if no AINB data to show debug info
                if (ImGui.CollapsingHeader($"AINB ({obj.AIGroupID})", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f),
                        "AINB data not loaded");
                }
                return;
            }

            // Build the AI Group reference name pattern: ActorName_AIGroupID
            string aiGroupPattern = $"{obj.Name}_{obj.AIGroupID}";

            // Find nodes by Internal Parameters -> string -> InstanceName ending with _AIGroupID
            // This is how AINB links to objects (e.g., "GrindRail_06cf" in InstanceName)
            var primaryNodes = ainbData.Nodes.Where(n =>
            {
                if (n.InternalParameters?.String == null)
                    return false;

                // Check for InstanceName parameter matching the pattern
                return n.InternalParameters.String.Any(s =>
                    s.Name == "InstanceName" &&
                    s.Value != null &&
                    (s.Value == aiGroupPattern || s.Value.EndsWith($"_{obj.AIGroupID}"))
                );
            }).ToList();

            // Collect all linked nodes from primary nodes
            var allConnectedNodes = new HashSet<int>();
            foreach (var node in primaryNodes)
            {
                CollectLinkedNodes(ainbData, node.NodeIndex, allConnectedNodes);
            }

            // Get all connected nodes
            var connectedNodes = ainbData.Nodes.Where(n => allConnectedNodes.Contains(n.NodeIndex)).ToList();

            // Draw the AINB category header
            if (ImGui.CollapsingHeader($"AINB ({obj.AIGroupID})", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (primaryNodes.Count == 0)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.7f, 0.7f, 0.7f, 1.0f),
                        $"No AINB nodes found for: {aiGroupPattern}");
                    return;
                }

                ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.8f, 1.0f, 1.0f),
                    $"AI Group: {aiGroupPattern}");
                ImGui.Separator();

                // Draw primary nodes (nodes with matching InstanceName)
                ImGui.TextColored(new System.Numerics.Vector4(0.2f, 1.0f, 0.4f, 1.0f), $"Entry Nodes ({primaryNodes.Count}):");
                foreach (var node in primaryNodes)
                {
                    DrawAINBNodeSummary(ainbData, node, true);
                }

                // Draw connected nodes (excluding primary nodes)
                var linkedOnlyNodes = connectedNodes.Where(n => !primaryNodes.Any(p => p.NodeIndex == n.NodeIndex)).ToList();
                if (linkedOnlyNodes.Count > 0)
                {
                    ImGui.Separator();
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.8f, 0.2f, 1.0f), $"Linked Nodes ({linkedOnlyNodes.Count}):");
                    foreach (var node in linkedOnlyNodes)
                    {
                        DrawAINBNodeSummary(ainbData, node, false);
                    }
                }
            }
        }

        /// <summary>
        /// Recursively collects all node indices linked from a starting node.
        /// </summary>
        private void CollectLinkedNodes(AINB ainbData, int nodeIndex, HashSet<int> collected)
        {
            if (collected.Contains(nodeIndex))
                return;

            collected.Add(nodeIndex);

            var node = ainbData.Nodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
            if (node?.LinkedNodes == null)
                return;

            // Collect from BoolFloatInputLinkAndOutputLink
            if (node.LinkedNodes.BoolFloatInputLinkAndOutputLink != null)
            {
                foreach (var link in node.LinkedNodes.BoolFloatInputLinkAndOutputLink)
                {
                    if (link.NodeIndex >= 0)
                        CollectLinkedNodes(ainbData, link.NodeIndex, collected);
                }
            }

            // Collect from IntInputLink
            if (node.LinkedNodes.IntInputLink != null)
            {
                foreach (var link in node.LinkedNodes.IntInputLink)
                {
                    if (link.NodeIndex >= 0)
                        CollectLinkedNodes(ainbData, link.NodeIndex, collected);
                }
            }
        }

        /// <summary>
        /// Draws a summary of an AINB node with right-click delete and DEL key support.
        /// </summary>
        private void DrawAINBNodeSummary(AINB ainbData, AINB.LogicNode node, bool isPrimary)
        {
            string nodeId = $"AINBNode{node.NodeIndex}";
            ImGui.PushID(nodeId);

            // Check if this node is selected
            bool isSelected = (_selectedAINBNodeIndex == node.NodeIndex && _selectedAINBData == ainbData);

            // Color based on selection and whether it's a primary or linked node
            var headerColor = isSelected
                ? new System.Numerics.Vector4(0.8f, 0.3f, 0.3f, 1.0f) // Red when selected
                : isPrimary
                    ? new System.Numerics.Vector4(0.2f, 0.6f, 0.3f, 1.0f)
                    : new System.Numerics.Vector4(0.4f, 0.4f, 0.5f, 1.0f);

            ImGui.PushStyleColor(ImGuiCol.Header, ImGui.GetColorU32(headerColor));

            // Get InstanceName if available to show which object this node references
            var instanceNameParam = node.InternalParameters?.String?.FirstOrDefault(s => s.Name == "InstanceName");
            string nodeHeader = instanceNameParam != null
                ? $"[{node.NodeIndex}] {node.Name ?? "Unnamed"} ({instanceNameParam.Value})"
                : $"[{node.NodeIndex}] {node.Name ?? "Unnamed"}";
            bool headerOpen = ImGui.CollapsingHeader(nodeHeader, ImGuiTreeNodeFlags.None);

            // Handle left click - select node
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                _selectedAINBNodeIndex = node.NodeIndex;
                _selectedAINBData = ainbData;
            }

            // Handle DEL key when this node is selected
            if (isSelected && ImGui.IsKeyPressed((int)ImGuiKey.Delete))
            {
                DeleteSingleAINBNode(ainbData, node.NodeIndex);
                _selectedAINBNodeIndex = -1;
                _selectedAINBData = null;
            }

            // Right-click context menu
            if (ImGui.BeginPopupContextItem($"AINBNodeContext{node.NodeIndex}"))
            {
                ImGui.TextColored(new System.Numerics.Vector4(0.7f, 0.7f, 0.7f, 1.0f), $"Node [{node.NodeIndex}] {node.Name}");
                ImGui.Separator();

                if (ImGui.MenuItem($"{IconManager.DELETE_ICON}  Remove Node", "Del"))
                {
                    DeleteSingleAINBNode(ainbData, node.NodeIndex);
                    _selectedAINBNodeIndex = -1;
                    _selectedAINBData = null;
                }

                ImGui.EndPopup();
            }

            if (headerOpen)
            {
                ImGui.Indent();

                // Node type
                if (!string.IsNullOrEmpty(node.NodeType))
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.7f, 0.7f, 0.7f, 1.0f), $"Type: {node.NodeType}");
                }

                // Node GUID
                if (!string.IsNullOrEmpty(node.GUID))
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.5f, 0.5f, 0.6f, 1.0f), $"GUID: {node.GUID}");
                }

                // Internal Parameters - show InstanceName (object link)
                if (node.InternalParameters?.String != null)
                {
                    var instanceName = node.InternalParameters.String.FirstOrDefault(s => s.Name == "InstanceName");
                    if (instanceName != null)
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(0.2f, 1.0f, 0.4f, 1.0f),
                            $"InstanceName: {instanceName.Value}");
                    }
                }

                // Internal Parameters summary
                if (node.InternalParameters != null)
                {
                    int internalCount = (node.InternalParameters.Int?.Count ?? 0) +
                                        (node.InternalParameters.Bool?.Count ?? 0) +
                                        (node.InternalParameters.Float?.Count ?? 0) +
                                        (node.InternalParameters.String?.Count ?? 0);

                    if (internalCount > 0 && ImGui.TreeNode("Internal Parameters"))
                    {
                        DrawInternalParametersSummary(node.InternalParameters);
                        ImGui.TreePop();
                    }
                }

                // Input Parameters summary
                if (node.InputParameters != null)
                {
                    int inputCount = (node.InputParameters.Int?.Count ?? 0) +
                                     (node.InputParameters.Bool?.Count ?? 0) +
                                     (node.InputParameters.Float?.Count ?? 0) +
                                     (node.InputParameters.String?.Count ?? 0) +
                                     (node.InputParameters.UserDefined?.Count ?? 0);

                    if (inputCount > 0 && ImGui.TreeNode("Input Parameters"))
                    {
                        DrawParametersSummary(node.InputParameters);
                        ImGui.TreePop();
                    }
                }

                // Output Parameters summary
                if (node.OutputParameters != null)
                {
                    int outputCount = (node.OutputParameters.Int?.Count ?? 0) +
                                      (node.OutputParameters.Bool?.Count ?? 0) +
                                      (node.OutputParameters.Float?.Count ?? 0) +
                                      (node.OutputParameters.String?.Count ?? 0) +
                                      (node.OutputParameters.UserDefined?.Count ?? 0);

                    if (outputCount > 0 && ImGui.TreeNode("Output Parameters"))
                    {
                        DrawOutputParametersSummary(node.OutputParameters);
                        ImGui.TreePop();
                    }
                }

                // Linked nodes
                if (node.LinkedNodes != null)
                {
                    int linkCount = (node.LinkedNodes.BoolFloatInputLinkAndOutputLink?.Count ?? 0) +
                                    (node.LinkedNodes.IntInputLink?.Count ?? 0);

                    if (linkCount > 0 && ImGui.TreeNode($"Links ({linkCount})"))
                    {
                        DrawLinksSummary(ainbData, node.LinkedNodes);
                        ImGui.TreePop();
                    }
                }

                ImGui.Unindent();
            }

            ImGui.PopStyleColor();
            ImGui.PopID();
        }

        /// <summary>
        /// Draws a summary of input parameters.
        /// </summary>
        private void DrawParametersSummary(AINB.InputParameters inputParams)
        {
            if (inputParams.Int != null)
            {
                foreach (var param in inputParams.Int)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.8f, 1.0f, 1.0f),
                        $"int {param.Name}: {param.Value}");
                }
            }
            if (inputParams.Bool != null)
            {
                foreach (var param in inputParams.Bool)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.8f, 0.4f, 1.0f),
                        $"bool {param.Name}: {param.Value}");
                }
            }
            if (inputParams.Float != null)
            {
                foreach (var param in inputParams.Float)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 1.0f, 0.4f, 1.0f),
                        $"float {param.Name}: {param.Value:F3}");
                }
            }
            if (inputParams.String != null)
            {
                foreach (var param in inputParams.String)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.6f, 0.8f, 1.0f),
                        $"string {param.Name}: \"{param.Value}\"");
                }
            }
            if (inputParams.UserDefined != null)
            {
                foreach (var param in inputParams.UserDefined)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f),
                        $"userdefined {param.Name} ({param.Class}): {param.Value}");
                }
            }
        }

        /// <summary>
        /// Draws a summary of output parameters.
        /// </summary>
        private void DrawOutputParametersSummary(AINB.OutputParameters outputParams)
        {
            if (outputParams.Int != null)
            {
                foreach (var param in outputParams.Int)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.8f, 1.0f, 1.0f),
                        $"int {param.Name}");
                }
            }
            if (outputParams.Bool != null)
            {
                foreach (var param in outputParams.Bool)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.8f, 0.4f, 1.0f),
                        $"bool {param.Name}");
                }
            }
            if (outputParams.Float != null)
            {
                foreach (var param in outputParams.Float)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 1.0f, 0.4f, 1.0f),
                        $"float {param.Name}");
                }
            }
            if (outputParams.String != null)
            {
                foreach (var param in outputParams.String)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.6f, 0.8f, 1.0f),
                        $"string {param.Name}");
                }
            }
            if (outputParams.UserDefined != null)
            {
                foreach (var param in outputParams.UserDefined)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f),
                        $"userdefined {param.Name} ({param.Class})");
                }
            }
        }

        /// <summary>
        /// Draws a summary of internal parameters (includes InstanceName for object linking).
        /// </summary>
        private void DrawInternalParametersSummary(AINB.InternalParameter internalParams)
        {
            if (internalParams.String != null)
            {
                foreach (var param in internalParams.String)
                {
                    bool isInstanceName = param.Name == "InstanceName";
                    var color = isInstanceName
                        ? new System.Numerics.Vector4(0.2f, 1.0f, 0.4f, 1.0f)
                        : new System.Numerics.Vector4(1.0f, 0.6f, 0.8f, 1.0f);
                    ImGui.TextColored(color, $"string {param.Name}: \"{param.Value}\"");
                }
            }
            if (internalParams.Int != null)
            {
                foreach (var param in internalParams.Int)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.8f, 1.0f, 1.0f),
                        $"int {param.Name}: {param.Value}");
                }
            }
            if (internalParams.Bool != null)
            {
                foreach (var param in internalParams.Bool)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.8f, 0.4f, 1.0f),
                        $"bool {param.Name}: {param.Value}");
                }
            }
            if (internalParams.Float != null)
            {
                foreach (var param in internalParams.Float)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 1.0f, 0.4f, 1.0f),
                        $"float {param.Name}: {param.Value:F3}");
                }
            }
        }

        /// <summary>
        /// Draws a summary of linked nodes.
        /// </summary>
        private void DrawLinksSummary(AINB ainbData, AINB.LinkedNodes linkedNodes)
        {
            if (linkedNodes.BoolFloatInputLinkAndOutputLink != null)
            {
                foreach (var link in linkedNodes.BoolFloatInputLinkAndOutputLink)
                {
                    string targetName = "Disconnected";
                    if (link.NodeIndex >= 0)
                    {
                        var targetNode = ainbData.Nodes.FirstOrDefault(n => n.NodeIndex == link.NodeIndex);
                        if (targetNode != null)
                            targetName = targetNode.Name ?? $"Node {link.NodeIndex}";
                    }
                    ImGui.TextColored(new System.Numerics.Vector4(0.6f, 0.8f, 0.6f, 1.0f),
                        $"-> [{link.NodeIndex}] {targetName} ({link.Parameter})");
                }
            }
            if (linkedNodes.IntInputLink != null)
            {
                foreach (var link in linkedNodes.IntInputLink)
                {
                    string targetName = "Disconnected";
                    if (link.NodeIndex >= 0)
                    {
                        var targetNode = ainbData.Nodes.FirstOrDefault(n => n.NodeIndex == link.NodeIndex);
                        if (targetNode != null)
                            targetName = targetNode.Name ?? $"Node {link.NodeIndex}";
                    }
                    ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.8f, 1.0f, 1.0f),
                        $"-> [{link.NodeIndex}] {targetName} ({link.Parameter})");
                }
            }
        }

        // AINB Preset selection index (per-object state stored statically for simplicity)
        private static int _selectedAINBPreset = 0;
        private static readonly string[] _ainbPresetNames = new string[]
        {
            // ─── SWITCH + LIFT (Combos) ───
            "[Switch+Lift] Switch + LftBlitzCompatibles",        // 0
            "[Switch+Lift] Switch + LftDrawer",                  // 1
            "[Switch+Lift] Switch + LftRotateTogglePoint",       // 2

            // ─── AREA + LIFT/ACTOR (Combos) ───
            "[Area+Lift] Area + LftBlitzCompatibles",            // 3
            "[Area+Lift] Area + LftDrawer",                      // 4
            "[Area+Actor] Area + Actor (Enemy Spawn)",           // 5

            // ─── TIMER + LIFT (Combos) ───
            "[Timer+Lift] Timer + LftBlitzCompatibles",          // 6

            // ─── TURF WAR + LIFT (Combos) ───
            "[TurfWar+Lift] GameTime + LftBlitz (Cmn)",          // 7  - GetFrame → ToSecond → Compare(90) → BoolToPulse → Delay → Lft
            "[TurfWar+Actor] GameTime + SplLogicActor (Cmn)",    // 8  - GetFrame → ToSecond → Compare(80) → BoolToPulse → Actor

            // ─── RANKED + LIFT (Combos) ───
            "[Ranked+Lift] GachiCount + LftBlitz (Hiagari)",     // 9  - Count(x2) → Min → Compare → BoolToPulse → Delay → Lft
            "[Ranked+Lift] Checkpoint + LftBlitz (Rainmaker)",   // 10 - Checkpoint(x2) → BoolToPulse(x2) → Join → Delay → Lft

            // ─── SPECIAL OBJECTS ───
            "[Special] KeyTreasureBox",                          // 11
            "[Special] Spawner (Sprinkler)",                     // 12
            "[Special] Periscope"                                // 13
        };

        // Descriptions for each AINB preset (shown on hover in preset picker popup)
        private static readonly string[] _ainbPresetDescriptions = new string[]
        {
            // ─── SWITCH + LIFT (Combos) ───
            "Moves Lft_AbstractBlitzCompatibles when switch is activated.\nUse with: SwitchShock, SwitchPaint, SwitchStep",
            "Opens/closes Lft_AbstractDrawer when switch is activated.\nUse with: SwitchShock, SwitchPaint, SwitchStep",
            "Rotates Lft_AbstractRotateTogglePoint between two positions.\nUse with: SwitchShock, SwitchPaint, SwitchStep",

            // ─── AREA + LIFT/ACTOR (Combos) ───
            "Moves Lft_AbstractBlitzCompatibles when player enters the area.\nUse with: LocatorAreaSwitch",
            "Opens Lft_AbstractDrawer when player enters the area.\nUse with: LocatorAreaSwitch",
            "Spawns/activates an actor (e.g. enemy) when player enters the area.\nUse with: LocatorAreaSwitch",

            // ─── TIMER + LIFT (Combos) ───
            "Moves Lft_AbstractBlitzCompatibles after a fixed time (default: 36 seconds).\nUse with: Any object",

            // ─── TURF WAR + LIFT (Combos) ───
            "Moves Lft_AbstractBlitzCompatibles after 90 seconds of game time.\n6 Nodes: GetGameFrame → FrameToSecond → CompareF32 → BoolToPulse → PulseDelay → Lft\nUse with: Any object (Turf War maps)",
            "Activates/shows actor after 80 seconds of game time.\n5 Nodes: GetGameFrame → FrameToSecond → CompareF32 → BoolToPulse → SplLogicActor\nUse with: Any object (Turf War maps)",

            // ─── RANKED + LIFT (Combos) ───
            "Moves Lft_AbstractBlitzCompatibles based on countdown.\n7 Nodes: GetGachiLeftCount(x2) → MinS32 → CompareS32 → BoolToPulse → PulseDelay → Lft\nUse with: Any object (Splat Zones maps)",
            "Moves Lft_AbstractBlitzCompatibles when either team passes a checkpoint.\n9 Nodes: IsPassedCheckpoint(x2) → BoolToPulse(x2) → JoinPulse → PulseDelay → Lft\nUse with: Any object (Rainmaker maps)",

            // ─── SPECIAL OBJECTS ───
            "Logic for key-treasure box system.\nUse with: ItemCardKey",
            "Spawns sprinkler gimmicks from a spawner.\nUse with: LocatorSpawner",
            "Logic for periscope objects.\nUse with: Periscope"
        };

        // State for preset picker popup
        private static bool _showPresetPicker = false;

        // Track selected AINB node for deletion
        private static int _selectedAINBNodeIndex = -1;
        private static AINB _selectedAINBData = null;

        /// <summary>
        /// Draws the AINB tab section for adding/managing AINB links.
        /// Similar to the Links tab but for AINB node connections.
        /// </summary>
        private void DrawAINBTab(MuObj obj)
        {
            if (ImGui.CollapsingHeader("AINB", ImGuiTreeNodeFlags.DefaultOpen))
            {
                // Get the AINB file from the stage definition
                AINB ainbData = MapEditor?.MapLoader?.stageDefinition?.AINBFile;

                // Check if AINB is available
                bool ainbAvailable = ainbData != null;

                if (!ainbAvailable)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f),
                        "AINB not loaded");
                }

                // Preset selector combo box
                ImGui.PushItemWidth(280);
                ImGui.Combo("###AINBPreset", ref _selectedAINBPreset, _ainbPresetNames, _ainbPresetNames.Length);
                ImGui.PopItemWidth();

                ImGui.SameLine();

                // Add button - creates nodes based on selected preset
                if (ImGui.Button($"   {IconManager.ADD_ICON}   ###AINBAdd"))
                {
                    if (ainbAvailable)
                    {
                        AddAINBNodeToObject(obj, ainbData, _selectedAINBPreset);
                    }
                    else
                    {
                        Console.WriteLine("Cannot add AINB node: AINB data not loaded");
                    }
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip($"Add {_ainbPresetNames[_selectedAINBPreset]}");
                }

                ImGui.SameLine();

                // Red "+" button - opens preset picker popup
                ImGui.PushStyleColor(ImGuiCol.Button, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.7f, 0.2f, 0.2f, 1.0f)));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.9f, 0.3f, 0.3f, 1.0f)));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(1.0f, 0.4f, 0.4f, 1.0f)));

                if (ImGui.Button($"   {IconManager.ADD_ICON}   ###AINBPresetPicker"))
                {
                    _showPresetPicker = true;
                    ImGui.OpenPopup("AINB Preset Picker");
                }

                ImGui.PopStyleColor(3);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Open preset picker");
                }

                // Preset picker popup window
                DrawAINBPresetPickerPopup(obj, ainbData, ainbAvailable);

                // Show current AIGroupID if set
                if (!string.IsNullOrEmpty(obj.AIGroupID))
                {
                    ImGui.SameLine();
                    ImGui.TextColored(new System.Numerics.Vector4(0.2f, 1.0f, 0.4f, 1.0f),
                        $"AIGroupID: {obj.AIGroupID}");

                    // Delete button - remove the AINB link
                    ImGui.SameLine();
                    if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###AINBDelete"))
                    {
                        if (ainbAvailable)
                        {
                            RemoveAINBNodeFromObject(obj, ainbData);
                        }
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Remove AINB node link");
                    }
                }
            }
        }

        /// <summary>
        /// Draws the AINB preset picker popup window.
        /// Shows all presets with descriptions on hover.
        /// </summary>
        private void DrawAINBPresetPickerPopup(MuObj obj, AINB ainbData, bool ainbAvailable)
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 400), ImGuiCond.FirstUseEver);

            if (ImGui.BeginPopup("AINB Preset Picker"))
            {
                ImGui.Text("Select AINB Preset:");
                ImGui.Separator();

                // Left side: Preset list
                ImGui.BeginChild("PresetList", new System.Numerics.Vector2(280, 320), true);

                string[] categories = new string[]
                {
                    "─── SWITCH + LIFT ───",
                    "─── AREA + LIFT/ACTOR ───",
                    "─── TIMER + LIFT ───",
                    "─── TURF WAR ───",
                    "─── RANKED ───",
                    "─── SPECIAL OBJECTS ───"
                };

                int[] categoryStarts = new int[] { 0, 3, 6, 7, 9, 11 };
                int[] categoryEnds = new int[] { 3, 6, 7, 9, 11, 14 };

                for (int cat = 0; cat < categories.Length; cat++)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.8f, 0.2f, 1.0f), categories[cat]);

                    for (int i = categoryStarts[cat]; i < categoryEnds[cat] && i < _ainbPresetNames.Length; i++)
                    {
                        bool isSelected = _selectedAINBPreset == i;

                        if (ImGui.Selectable($"  {_ainbPresetNames[i]}", isSelected))
                        {
                            _selectedAINBPreset = i;

                            // Add the preset immediately when clicked
                            if (ainbAvailable)
                            {
                                AddAINBNodeToObject(obj, ainbData, i);
                                ImGui.CloseCurrentPopup();
                            }
                        }

                        // Show description on hover (right side)
                        if (ImGui.IsItemHovered() && i < _ainbPresetDescriptions.Length)
                        {
                            ImGui.BeginTooltip();
                            ImGui.PushTextWrapPos(300);
                            ImGui.TextColored(new System.Numerics.Vector4(0.4f, 1.0f, 0.6f, 1.0f), "Description:");
                            ImGui.Text(_ainbPresetDescriptions[i]);
                            ImGui.PopTextWrapPos();
                            ImGui.EndTooltip();
                        }
                    }

                    ImGui.Spacing();
                }

                ImGui.EndChild();

                ImGui.SameLine();

                // Right side: Description panel
                ImGui.BeginChild("DescriptionPanel", new System.Numerics.Vector2(200, 320), true);
                ImGui.TextColored(new System.Numerics.Vector4(1.0f, 0.8f, 0.2f, 1.0f), "Selected:");
                ImGui.Separator();

                if (_selectedAINBPreset >= 0 && _selectedAINBPreset < _ainbPresetNames.Length)
                {
                    ImGui.TextWrapped(_ainbPresetNames[_selectedAINBPreset]);
                    ImGui.Spacing();

                    if (_selectedAINBPreset < _ainbPresetDescriptions.Length)
                    {
                        ImGui.TextColored(new System.Numerics.Vector4(0.4f, 1.0f, 0.6f, 1.0f), "Description:");
                        ImGui.TextWrapped(_ainbPresetDescriptions[_selectedAINBPreset]);
                    }
                }

                ImGui.EndChild();

                ImGui.Separator();

                // Close button
                if (ImGui.Button("Close", new System.Numerics.Vector2(120, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        /// <summary>
        /// Generates a unique 4-character hex AIGroupID.
        /// </summary>
        private string GenerateAIGroupID()
        {
            System.Random random = new System.Random();

            // Generate a 4-character hex string (e.g., "06cf", "b012")
            string aiGroupId = "";
            for (int i = 0; i < 4; i++)
            {
                int val = random.Next(0, 16);
                aiGroupId += val.ToString("x");
            }

            return aiGroupId;
        }

        /// <summary>
        /// Gets all existing AIGroupIDs from the current scene objects.
        /// </summary>
        private HashSet<string> GetExistingAIGroupIDs()
        {
            HashSet<string> existingIds = new HashSet<string>();

            foreach (IDrawable drawable in MapEditor.Scene.Objects)
            {
                if (drawable is EditableObject editObj && editObj.UINode.Tag is MuObj muObj)
                {
                    if (!string.IsNullOrEmpty(muObj.AIGroupID))
                    {
                        existingIds.Add(muObj.AIGroupID);
                    }
                }
            }

            return existingIds;
        }

        /// <summary>
        /// Generates a unique GUID string for AINB nodes.
        /// </summary>
        private string GenerateAINBGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates a SplLogicBhvSwitchOnOff node preset.
        /// Has Logic_Activate/Logic_Sleep/SwitchOff/SwitchOn inputs and IsOn/Logic_IsActive outputs.
        /// </summary>
        private AINB.LogicNode CreateSplLogicBhvSwitchOnOffNode(int nodeIndex, string instanceName, string guid)
        {
            var node = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicBhvSwitchOnOff",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter
                        {
                            Name = "InstanceName",
                            Value = instanceName
                        }
                    },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "SwitchOff",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "SwitchOn",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>
                    {
                        new AINB.OutputBoolParameter { Name = "IsOn" },
                        new AINB.OutputBoolParameter { Name = "Logic_IsActive" }
                    },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" },
                        new AINB.OutputUserDefinedParameter { Name = "SwitchOff", Class = "const game::ai::Pulse" },
                        new AINB.OutputUserDefinedParameter { Name = "SwitchOn", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };

            return node;
        }

        /// <summary>
        /// Adds a new AINB node to the object and links it via AIGroupID.
        /// </summary>
        private void AddAINBNodeToObject(MuObj obj, AINB ainbData, int presetIndex = 0)
        {
            // Generate unique AIGroupID if not already set
            if (string.IsNullOrEmpty(obj.AIGroupID))
            {
                var existingIds = GetExistingAIGroupIDs();
                string newId = GenerateAIGroupID();
                while (existingIds.Contains(newId)) newId = GenerateAIGroupID();
                obj.AIGroupID = newId;
            }

            string instanceName = $"{obj.Name}_{obj.AIGroupID}";

            if (ainbData.Nodes == null) ainbData.Nodes = new List<AINB.LogicNode>();
            if (ainbData.Commands == null) ainbData.Commands = new List<AINB.LogicCommand>();

            int nextNodeIndex = ainbData.Nodes.Count > 0 ? ainbData.Nodes.Max(n => n.NodeIndex) + 1 : 0;

            switch (presetIndex)
            {
                // ─── SWITCH + LIFT (Combos) ───
                case 0: // [Switch+Lift] Switch + LftBlitzCompatibles
                    AddPreset_SwitchOnOffOutputOnly_LftBlitzCompatibles(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                case 1: // [Switch+Lift] Switch + LftDrawer
                    AddPreset_SwitchOnOffOutputOnly_LftDrawer(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                case 2: // [Switch+Lift] Switch + LftRotateTogglePoint
                    AddPreset_SwitchOnOffOutputOnly_LftRotateTogglePoint(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                // ─── AREA + LIFT/ACTOR (Combos) ───
                case 3: // [Area+Lift] Area + LftBlitzCompatibles
                    AddPreset_AreaSwitch_LftBlitzCompatibles(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                case 4: // [Area+Lift] Area + LftDrawer
                    AddPreset_AreaSwitch_LftDrawer(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                case 5: // [Area+Actor] Area + Actor (Enemy Spawn)
                    AddPreset_AreaSwitch_Actor(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                // ─── TIMER + LIFT (Combos) ───
                case 6: // [Timer+Lift] Timer + LftBlitzCompatibles
                    AddPreset_Timer_LftBlitzCompatibles(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                // ─── TURF WAR + LIFT (Combos) ───
                case 7: // [TurfWar+Lift] GameTime + LftBlitz (Cmn)
                    AddPreset_TurfWar_LftBlitzCompatibles(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                case 8: // [TurfWar+Actor] GameTime + SplLogicActor (Cmn)
                    AddPreset_TurfWar_SplLogicActor(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                // ─── RANKED + LIFT (Combos) ───
                case 9: // [Ranked+Lift] GachiCount + LftBlitz (Hiagari)
                    AddPreset_GachiCount_LftBlitzCompatibles(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                case 10: // [Ranked+Lift] Checkpoint + LftBlitz (Rainmaker)
                    AddPreset_Checkpoint_LftBlitzCompatibles(ainbData, nextNodeIndex, instanceName, obj);
                    break;

                // ─── SPECIAL OBJECTS ───
                case 11: // [Special] KeyTreasureBox
                    AddPreset_SplLogicBhvKeyTreasureBox(ainbData, nextNodeIndex, instanceName);
                    break;

                case 12: // [Special] Spawner (Sprinkler)
                    AddPreset_SplLogicBhvSpawnerForSprinklerGimmick(ainbData, nextNodeIndex, instanceName);
                    break;

                case 13: // [Special] Periscope
                    AddPreset_SplLogicBhvPeriscope(ainbData, nextNodeIndex, instanceName);
                    break;
            }

            if (MapEditor?.MapLoader?.stageDefinition != null)
                MapEditor.MapLoader.stageDefinition.AINBModified = true;
        }

        #region AINB Preset Methods

        private void AddPreset_SwitchOnOffOutputOnly_LftBlitzCompatibles(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int switchIdx = nodeIndex, lftIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together (pattern: _AIGroupID)
            string lftInstanceName = $"Lft_AbstractBlitzCompatibles_{obj.AIGroupID}";

            // For Pulse connections (SwitchOn -> Start): LinkedNodes goes on SOURCE (switch)
            var switchNode = CreateSplLogicBhvSwitchOnOffOutputOnlyNode(switchIdx, instanceName, GenerateAINBGuid(), lftIdx, "SwitchOn");
            var lftNode = CreateSplLogicLftBlitzCompatiblesNode(lftIdx, lftInstanceName, GenerateAINBGuid(), switchIdx);
            ainbData.Nodes.Add(switchNode);
            ainbData.Nodes.Add(lftNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = switchIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {switchNode.Name} -> {instanceName}");
            Console.WriteLine($"[AINB] Added linked: {lftNode.Name} -> {lftInstanceName}");
        }

        private void AddPreset_AreaSwitch_Actor(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int areaIdx = nodeIndex, actorIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together (pattern: _AIGroupID)
            string actorInstanceName = $"{obj.Name}_{obj.AIGroupID}";

            var areaNode = CreateSplLogicBhvAreaSwitchNode(areaIdx, instanceName, GenerateAINBGuid());
            areaNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode { NodeIndex = actorIdx, Parameter = "OnEnter" });
            var actorNode = CreateSplLogicActorNode(actorIdx, actorInstanceName, GenerateAINBGuid(), areaIdx);
            ainbData.Nodes.Add(areaNode);
            ainbData.Nodes.Add(actorNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = areaIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {areaNode.Name} -> {instanceName}");
            Console.WriteLine($"[AINB] Added linked: {actorNode.Name} -> {actorInstanceName}");
        }

        private void AddPreset_SwitchOnOffOutputOnly_LftDrawer(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int switchIdx = nodeIndex, drawerIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together (pattern: _AIGroupID)
            string drawerInstanceName = $"Lft_AbstractDrawer_{obj.AIGroupID}";

            // For bool connections (IsOn -> IsPulling): LinkedNodes goes on DESTINATION (drawer), not source (switch)
            var switchNode = CreateSplLogicBhvSwitchOnOffOutputOnlyNode(switchIdx, instanceName, GenerateAINBGuid());
            var drawerNode = CreateSplLogicLftDrawerNode(drawerIdx, drawerInstanceName, GenerateAINBGuid(), switchIdx);
            ainbData.Nodes.Add(switchNode);
            ainbData.Nodes.Add(drawerNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = switchIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {switchNode.Name} -> {instanceName}");
            Console.WriteLine($"[AINB] Added linked: {drawerNode.Name} -> {drawerInstanceName}");
        }

        private void AddPreset_SplLogicBhvKeyTreasureBox(AINB ainbData, int nodeIndex, string instanceName)
        {
            var node = CreateSplLogicBhvKeyTreasureBoxNode(nodeIndex, instanceName, GenerateAINBGuid());
            ainbData.Nodes.Add(node);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = nodeIndex, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {node.Name} -> {instanceName}");
        }

        private void AddPreset_SplLogicBhvSpawnerForSprinklerGimmick(AINB ainbData, int nodeIndex, string instanceName)
        {
            var node = CreateSplLogicBhvSpawnerForSprinklerGimmickNode(nodeIndex, instanceName, GenerateAINBGuid());
            ainbData.Nodes.Add(node);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = nodeIndex, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {node.Name} -> {instanceName}");
        }

        private void AddPreset_SplLogicBhvPeriscope(AINB ainbData, int nodeIndex, string instanceName)
        {
            var node = CreateSplLogicBhvPeriscopeNode(nodeIndex, instanceName, GenerateAINBGuid());
            ainbData.Nodes.Add(node);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = nodeIndex, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {node.Name} -> {instanceName}");
        }

        /// <summary>
        /// Preset 3: AreaSwitch + LftBlitzCompatibles
        /// When player enters AreaSwitch trigger zone, the LftBlitzCompatibles moves/rotates once.
        /// Apply to: LocatorAreaSwitch object
        /// </summary>
        private void AddPreset_AreaSwitch_LftBlitzCompatibles(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int areaIdx = nodeIndex, lftIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together (pattern: _AIGroupID)
            string lftInstanceName = $"Lft_AbstractBlitzCompatibles_{obj.AIGroupID}";

            // AreaSwitch outputs OnEnter (Pulse) at userdefined index 0
            // LftBlitzCompatibles receives Start (Pulse) from OnEnter
            var areaNode = CreateSplLogicBhvAreaSwitchNode(areaIdx, instanceName, GenerateAINBGuid());
            // Add LinkedNodes to AreaSwitch pointing to LftBlitzCompatibles with "OnEnter"
            areaNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = lftIdx,
                Parameter = "OnEnter"
            });

            // LftBlitzCompatibles with parameterIndex = 0 (OnEnter is at index 0 in AreaSwitch userdefined outputs)
            var lftNode = CreateSplLogicLftBlitzCompatiblesNode(lftIdx, lftInstanceName, GenerateAINBGuid(), areaIdx, 0);

            ainbData.Nodes.Add(areaNode);
            ainbData.Nodes.Add(lftNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = areaIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {areaNode.Name} -> {instanceName}");
            Console.WriteLine($"[AINB] Added linked: {lftNode.Name} -> {lftInstanceName}");
        }

        /// <summary>
        /// Preset 8: SwitchOnOffOutputOnly + LftRotateTogglePoint
        /// When switch is triggered (e.g., SwitchShock), the LftRotateTogglePoint toggles between positions.
        /// Apply to: SwitchShock, SwitchPaint, or similar trigger objects
        /// </summary>
        private void AddPreset_SwitchOnOffOutputOnly_LftRotateTogglePoint(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int switchIdx = nodeIndex, lftIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together (pattern: _AIGroupID)
            string lftInstanceName = $"Lft_AbstractRotateTogglePoint_{obj.AIGroupID}";

            // For Bool connections (IsOn -> IsAccel): LinkedNodes goes on DESTINATION (lft), not source (switch)
            var switchNode = CreateSplLogicBhvSwitchOnOffOutputOnlyNode(switchIdx, instanceName, GenerateAINBGuid());
            var lftNode = CreateSplLogicLftRotateTogglePointNode(lftIdx, lftInstanceName, GenerateAINBGuid(), switchIdx);
            ainbData.Nodes.Add(switchNode);
            ainbData.Nodes.Add(lftNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = switchIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {switchNode.Name} -> {instanceName}");
            Console.WriteLine($"[AINB] Added linked: {lftNode.Name} -> {lftInstanceName}");
        }

        /// <summary>
        /// Preset 4: AreaSwitch + LftDrawer
        /// When player enters AreaSwitch trigger zone, the LftDrawer extends/retracts.
        /// The drawer stays extended while player is inside, retracts when player exits.
        /// Apply to: LocatorAreaSwitch object
        /// </summary>
        private void AddPreset_AreaSwitch_LftDrawer(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int areaIdx = nodeIndex, drawerIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together (pattern: _AIGroupID)
            string drawerInstanceName = $"Lft_AbstractDrawer_{obj.AIGroupID}";

            // AreaSwitch outputs IsInside (Bool)
            // LftDrawer receives IsPulling (Bool) - extends while true, retracts while false
            var areaNode = CreateSplLogicBhvAreaSwitchNode(areaIdx, instanceName, GenerateAINBGuid());
            // For Bool connections: LinkedNodes goes on DESTINATION (drawer), not source (area)
            var drawerNode = CreateSplLogicLftDrawerNode(drawerIdx, drawerInstanceName, GenerateAINBGuid(), areaIdx);

            ainbData.Nodes.Add(areaNode);
            ainbData.Nodes.Add(drawerNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = areaIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {areaNode.Name} -> {instanceName}");
            Console.WriteLine($"[AINB] Added linked: {drawerNode.Name} -> {drawerInstanceName}");
        }

        /// <summary>
        /// Preset 6: Timer + LftBlitzCompatibles
        /// Timer fires after delay, then LftBlitzCompatibles moves the object once.
        /// Used for: Timed water level changes, delayed platform movements
        /// Apply to: Lft_FldObj_HiagariWater or similar lift objects
        /// </summary>
        private void AddPreset_Timer_LftBlitzCompatibles(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int timerIdx = nodeIndex, lftIdx = nodeIndex + 1;
            // Use obj.AIGroupID so both nodes are found/removed together
            string lftInstanceName = $"Lft_AbstractBlitzCompatibles_{obj.AIGroupID}";

            // Timer outputs Pulse at userdefined "Output"
            // LftBlitzCompatibles receives "Start" pulse
            var timerNode = CreateGameFlowPulseDelayNode(timerIdx, instanceName, GenerateAINBGuid(), 36);
            // Add LinkedNodes to Timer pointing to LftBlitzCompatibles with "Output"
            timerNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = lftIdx,
                Parameter = "Output"
            });

            var lftNode = CreateSplLogicLftBlitzCompatiblesNode(lftIdx, lftInstanceName, GenerateAINBGuid(), timerIdx, 0);

            ainbData.Nodes.Add(timerNode);
            ainbData.Nodes.Add(lftNode);
            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = timerIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added: {timerNode.Name} -> {instanceName} (Delay: 36s)");
            Console.WriteLine($"[AINB] Added linked: {lftNode.Name} -> {lftInstanceName}");
        }

        // ─── TURF WAR + LIFT COMBO PRESETS ───

        /// <summary>
        /// Preset 7: TurfWar + LftBlitzCompatibles (Cmn layer)
        /// Full chain: GetGameFrame → FrameToSecond → CompareF32(90.0) → BoolToPulse → PulseDelay → LftBlitzCompatibles
        /// Water/platform moves after 90 seconds (1:30) in Turf War.
        /// 6 nodes total - exactly like Hiagari Cmn layer.
        /// </summary>
        private void AddPreset_TurfWar_LftBlitzCompatibles(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int getFrameIdx = nodeIndex;
            int frameToSecIdx = nodeIndex + 1;
            int compareIdx = nodeIndex + 2;
            int boolToPulseIdx = nodeIndex + 3;
            int delayIdx = nodeIndex + 4;
            int lftIdx = nodeIndex + 5;

            string lftInstanceName = $"Lft_AbstractBlitzCompatibles_{obj.AIGroupID}";

            // Create all nodes (6 total)
            var getFrameNode = CreateGetGameFrameNode(getFrameIdx, GenerateAINBGuid());
            var frameToSecNode = CreateFrameToSecondNode(frameToSecIdx, GenerateAINBGuid(), getFrameIdx);
            var compareNode = CreateCompareF32Node(compareIdx, GenerateAINBGuid(), 90.0f, 0, frameToSecIdx); // 90 seconds = 1:30
            var boolToPulseNode = CreateBoolToPulseNode(boolToPulseIdx, GenerateAINBGuid(), compareIdx);
            var delayNode = CreateGameFlowPulseDelayNode(delayIdx, $"Delay_{obj.AIGroupID}", GenerateAINBGuid(), 180); // 3 second smooth transition
            var lftNode = CreateSplLogicLftBlitzCompatiblesNode(lftIdx, lftInstanceName, GenerateAINBGuid(), delayIdx, 0);

            // Set up precondition for delay node
            delayNode.PreconditionNodes.Add(boolToPulseIdx);

            // Connect PulseDelay Input to BoolToPulse Output (Pulse)
            delayNode.InputParameters.UserDefined[0].NodeIndex = boolToPulseIdx;
            delayNode.InputParameters.UserDefined[0].ParameterIndex = 0; // Output "Pulse" is parameter 0

            // Set up LinkedNodes: BoolToPulse → PulseDelay
            boolToPulseNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = delayIdx,
                Parameter = "Pulse"
            });

            // Set up LinkedNodes: PulseDelay → LftBlitz
            delayNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = lftIdx,
                Parameter = "Output"
            });

            // Add all nodes
            ainbData.Nodes.Add(getFrameNode);
            ainbData.Nodes.Add(frameToSecNode);
            ainbData.Nodes.Add(compareNode);
            ainbData.Nodes.Add(boolToPulseNode);
            ainbData.Nodes.Add(delayNode);
            ainbData.Nodes.Add(lftNode);

            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = getFrameIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added TurfWar chain (6 nodes): GetGameFrame → FrameToSecond → CompareF32(90.0) → BoolToPulse → PulseDelay(180) → {lftInstanceName}");
        }

        /// <summary>
        /// Preset 8: TurfWar + SplLogicActor (Cmn layer)
        /// Full chain: GetGameFrame → FrameToSecond → CompareF32(80.0) → BoolToPulse → SplLogicActor
        /// Object appears/activates after 80 seconds in Turf War.
        /// 5 nodes total.
        /// </summary>
        private void AddPreset_TurfWar_SplLogicActor(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int getFrameIdx = nodeIndex;
            int frameToSecIdx = nodeIndex + 1;
            int compareIdx = nodeIndex + 2;
            int boolToPulseIdx = nodeIndex + 3;
            int actorIdx = nodeIndex + 4;

            string actorInstanceName = $"{obj.Name}_{obj.AIGroupID}";

            // Create all nodes (5 total)
            var getFrameNode = CreateGetGameFrameNode(getFrameIdx, GenerateAINBGuid());
            var frameToSecNode = CreateFrameToSecondNode(frameToSecIdx, GenerateAINBGuid(), getFrameIdx);
            var compareNode = CreateCompareF32Node(compareIdx, GenerateAINBGuid(), 80.0f, 0, frameToSecIdx); // 80 seconds
            var boolToPulseNode = CreateBoolToPulseNode(boolToPulseIdx, GenerateAINBGuid(), compareIdx);
            var actorNode = CreateSplLogicActorNode(actorIdx, actorInstanceName, GenerateAINBGuid(), boolToPulseIdx);

            // Set up precondition for actor node
            actorNode.PreconditionNodes.Add(boolToPulseIdx);

            // Set up LinkedNodes: BoolToPulse → SplLogicActor
            boolToPulseNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = actorIdx,
                Parameter = "Pulse"
            });

            // Add all nodes
            ainbData.Nodes.Add(getFrameNode);
            ainbData.Nodes.Add(frameToSecNode);
            ainbData.Nodes.Add(compareNode);
            ainbData.Nodes.Add(boolToPulseNode);
            ainbData.Nodes.Add(actorNode);

            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = getFrameIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added TurfWar+Actor chain (5 nodes): GetGameFrame → FrameToSecond → CompareF32(80.0) → BoolToPulse → {actorInstanceName}");
        }

        // ─── RANKED + LIFT COMBO PRESETS ───

        /// <summary>
        /// Preset 7: GachiCount + LftBlitzCompatibles (Hiagari style)
        /// Full chain: GetGachiLeftCount(x2) → MinS32 → CompareS32 → BoolToPulse → PulseDelay → LftBlitzCompatibles
        /// Water/platform moves when either team's count drops below threshold.
        /// 7 nodes total - exactly like Hiagari map.
        /// </summary>
        private void AddPreset_GachiCount_LftBlitzCompatibles(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int countAlphaIdx = nodeIndex;
            int countBravoIdx = nodeIndex + 1;
            int minIdx = nodeIndex + 2;
            int compareIdx = nodeIndex + 3;
            int boolToPulseIdx = nodeIndex + 4;
            int delayIdx = nodeIndex + 5;
            int lftIdx = nodeIndex + 6;

            string lftInstanceName = $"Lft_AbstractBlitzCompatibles_{obj.AIGroupID}";

            // Create all nodes (7 total)
            var countAlphaNode = CreateGetGachiLeftCountNode(countAlphaIdx, GenerateAINBGuid(), 0); // Team Alpha
            var countBravoNode = CreateGetGachiLeftCountNode(countBravoIdx, GenerateAINBGuid(), 1); // Team Bravo
            var minNode = CreateMinS32Node(minIdx, GenerateAINBGuid(), countAlphaIdx, countBravoIdx);
            var compareNode = CreateCompareS32Node(compareIdx, GenerateAINBGuid(), 61, 0, minIdx); // Threshold 61
            var boolToPulseNode = CreateBoolToPulseNode(boolToPulseIdx, GenerateAINBGuid(), compareIdx);
            var delayNode = CreateGameFlowPulseDelayNode(delayIdx, $"Delay_{obj.AIGroupID}", GenerateAINBGuid(), 36);
            var lftNode = CreateSplLogicLftBlitzCompatiblesNode(lftIdx, lftInstanceName, GenerateAINBGuid(), delayIdx, 0);

            // Set up precondition for delay node
            delayNode.PreconditionNodes.Add(boolToPulseIdx);

            // Connect PulseDelay Input to BoolToPulse Output (Pulse)
            delayNode.InputParameters.UserDefined[0].NodeIndex = boolToPulseIdx;
            delayNode.InputParameters.UserDefined[0].ParameterIndex = 0; // Output "Pulse" is parameter 0

            // Set up LinkedNodes: BoolToPulse → PulseDelay
            boolToPulseNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = delayIdx,
                Parameter = "Pulse"
            });

            // Set up LinkedNodes: PulseDelay → LftBlitz
            delayNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = lftIdx,
                Parameter = "Output"
            });

            // Add all nodes
            ainbData.Nodes.Add(countAlphaNode);
            ainbData.Nodes.Add(countBravoNode);
            ainbData.Nodes.Add(minNode);
            ainbData.Nodes.Add(compareNode);
            ainbData.Nodes.Add(boolToPulseNode);
            ainbData.Nodes.Add(delayNode);
            ainbData.Nodes.Add(lftNode);

            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = countAlphaIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added Hiagari chain (7 nodes): GetGachiLeftCount(x2) → MinS32 → CompareS32(61) → BoolToPulse → PulseDelay(36) → {lftInstanceName}");
        }

        /// <summary>
        /// Preset 8: Checkpoint + LftBlitzCompatibles (Rainmaker style)
        /// Full chain: IsPassedGachihokoCheckPoint(x2) → BoolToPulse(x2) → JoinPulse → PulseDelay → LftBlitzCompatibles
        /// Platform moves when EITHER team passes a checkpoint.
        /// 7 nodes total - exactly like Hiagari map.
        /// </summary>
        private void AddPreset_Checkpoint_LftBlitzCompatibles(AINB ainbData, int nodeIndex, string instanceName, MuObj obj)
        {
            int checkAlphaIdx = nodeIndex;
            int checkBravoIdx = nodeIndex + 1;
            int boolToPulseAlphaIdx = nodeIndex + 2;
            int boolToPulseBravoIdx = nodeIndex + 3;
            int joinPulseIdx = nodeIndex + 4;
            int delayIdx = nodeIndex + 5;
            int lftIdx = nodeIndex + 6;

            string lftInstanceName = $"Lft_AbstractBlitzCompatibles_{obj.AIGroupID}";

            // Create all nodes (7 total)
            var checkAlphaNode = CreateIsPassedGachihokoCheckPointNode(checkAlphaIdx, GenerateAINBGuid(), 0, 0); // Team Alpha
            var checkBravoNode = CreateIsPassedGachihokoCheckPointNode(checkBravoIdx, GenerateAINBGuid(), 1, 0); // Team Bravo
            var boolToPulseAlphaNode = CreateBoolToPulseNode(boolToPulseAlphaIdx, GenerateAINBGuid(), checkAlphaIdx);
            var boolToPulseBravoNode = CreateBoolToPulseNode(boolToPulseBravoIdx, GenerateAINBGuid(), checkBravoIdx);
            var joinPulseNode = CreateJoinPulseNode(joinPulseIdx, GenerateAINBGuid());
            var delayNode = CreateGameFlowPulseDelayNode(delayIdx, $"Delay_{obj.AIGroupID}", GenerateAINBGuid(), 36);
            var lftNode = CreateSplLogicLftBlitzCompatiblesNode(lftIdx, lftInstanceName, GenerateAINBGuid(), delayIdx, 0);

            // Set up preconditions for JoinPulse
            joinPulseNode.PreconditionNodes.Add(boolToPulseAlphaIdx);
            joinPulseNode.PreconditionNodes.Add(boolToPulseBravoIdx);

            // Set up JoinPulse input sources
            joinPulseNode.InputParameters.UserDefined[0].NodeIndex = -100; // Multi-source indicator
            joinPulseNode.InputParameters.UserDefined[0].ParameterIndex = 2;
            joinPulseNode.InputParameters.UserDefined[0].Sources = new List<AINB.UserDefinedParameterSource>
            {
                new AINB.UserDefinedParameterSource { NodeIndex = boolToPulseAlphaIdx, ParameterIndex = 0 },
                new AINB.UserDefinedParameterSource { NodeIndex = boolToPulseBravoIdx, ParameterIndex = 0 }
            };

            // Set up precondition for delay node
            delayNode.PreconditionNodes.Add(joinPulseIdx);

            // Connect PulseDelay Input to JoinPulse Output
            delayNode.InputParameters.UserDefined[0].NodeIndex = joinPulseIdx;
            delayNode.InputParameters.UserDefined[0].ParameterIndex = 0; // Output is parameter 0

            // Set up LinkedNodes: BoolToPulse Alpha → JoinPulse
            boolToPulseAlphaNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = joinPulseIdx,
                Parameter = "Pulse"
            });

            // Set up LinkedNodes: BoolToPulse Bravo → JoinPulse
            boolToPulseBravoNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = joinPulseIdx,
                Parameter = "Pulse"
            });

            // Set up LinkedNodes: JoinPulse → PulseDelay
            joinPulseNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = delayIdx,
                Parameter = "Output"
            });

            // Set up LinkedNodes: PulseDelay → LftBlitz
            delayNode.LinkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
            {
                NodeIndex = lftIdx,
                Parameter = "Output"
            });

            // Add all nodes
            ainbData.Nodes.Add(checkAlphaNode);
            ainbData.Nodes.Add(checkBravoNode);
            ainbData.Nodes.Add(boolToPulseAlphaNode);
            ainbData.Nodes.Add(boolToPulseBravoNode);
            ainbData.Nodes.Add(joinPulseNode);
            ainbData.Nodes.Add(delayNode);
            ainbData.Nodes.Add(lftNode);

            ainbData.Commands.Add(new AINB.LogicCommand { Name = instanceName, GUID = GenerateAINBGuid(), LeftNodeIndex = checkAlphaIdx, RightNodeIndex = -1 });
            Console.WriteLine($"[AINB] Added Rainmaker chain (7 nodes): IsPassedGachihokoCheckPoint(x2) → BoolToPulse(x2) → JoinPulse → PulseDelay(36) → {lftInstanceName}");
        }

        private string GenerateUniqueAIGroupID(string excludeId)
        {
            var existingIds = GetExistingAIGroupIDs();
            existingIds.Add(excludeId);
            string newId = GenerateAIGroupID();
            while (existingIds.Contains(newId)) newId = GenerateAIGroupID();
            return newId;
        }

        #endregion

        /// <summary>
        /// Creates a SplLogicBhvSwitchOnOffOutputOnly node preset (Node 0 type in linked preset).
        /// </summary>
        private AINB.LogicNode CreateSplLogicBhvSwitchOnOffOutputOnlyNode(int nodeIndex, string instanceName, string guid, int linkedNodeIndex = -1, string linkParameter = null)
        {
            // LinkedNodes only for Pulse/UserDefined outputs (like SwitchOn), not for bool outputs (like IsOn)
            var linkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                IntInputLink = new List<AINB.IntLinkedNode>()
            };

            // Only add LinkedNode for Pulse connections (e.g., SwitchOn -> Start)
            if (linkedNodeIndex >= 0 && !string.IsNullOrEmpty(linkParameter))
            {
                linkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
                {
                    NodeIndex = linkedNodeIndex,
                    Parameter = linkParameter
                });
            }

            var node = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicBhvSwitchOnOffOutputOnly",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter
                        {
                            Name = "InstanceName",
                            Value = instanceName
                        }
                    },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>
                    {
                        new AINB.OutputBoolParameter { Name = "IsOn" },
                        new AINB.OutputBoolParameter { Name = "Logic_IsActive" }
                    },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" },
                        new AINB.OutputUserDefinedParameter { Name = "SwitchOff", Class = "const game::ai::Pulse" },
                        new AINB.OutputUserDefinedParameter { Name = "SwitchOn", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = linkedNodes
            };

            return node;
        }

        /// <summary>
        /// Creates a SplLogicLftBlitzCompatibles node preset (Node 1 type in linked preset).
        /// parameterIndex: 2 for SwitchOn (from SwitchOnOffOutputOnly), 0 for OnEnter (from AreaSwitch)
        /// </summary>
        private AINB.LogicNode CreateSplLogicLftBlitzCompatiblesNode(int nodeIndex, string instanceName, string guid, int preconditionNodeIndex, int parameterIndex = 2)
        {
            var node = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicLftBlitzCompatibles",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int> { preconditionNodeIndex },
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>
                    {
                        new AINB.InternalStringParameter
                        {
                            Name = "InstanceName",
                            Value = instanceName
                        }
                    },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Start",
                            Class = "const game::ai::Pulse",
                            NodeIndex = preconditionNodeIndex,
                            ParameterIndex = parameterIndex,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Stop",
                            Class = "const game::ai::Pulse",
                            NodeIndex = -1,
                            ParameterIndex = -1,
                            Value = 0,
                            Sources = new List<AINB.UserDefinedParameterSource>()
                        }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };

            return node;
        }

        #region New Node Creation Methods

        /// <summary>
        /// Creates a SplLogicBhvAreaSwitch node - Trigger zone that fires when player enters/exits.
        /// Used for: LocatorAreaSwitch_*, LocatorAreaSwitchForLift_*
        /// </summary>
        private AINB.LogicNode CreateSplLogicBhvAreaSwitchNode(int nodeIndex, string instanceName, string guid)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicBhvAreaSwitch",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter> { new AINB.InternalBoolParameter { Name = "IsOneTime", Value = true } },
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "IsInside" } },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "OnEnter", Class = "const game::ai::Pulse" },
                        new AINB.OutputUserDefinedParameter { Name = "OnExit", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a SplLogicActor node - Activates/Deactivates actors (enemies, objects).
        /// Used for: EnemyHohei_*, EnemyTakopodDEV_*, AerialRing_*
        /// </summary>
        private AINB.LogicNode CreateSplLogicActorNode(int nodeIndex, string instanceName, string guid, int preconditionNodeIndex)
        {
            var node = new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicActor",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = preconditionNodeIndex >= 0 ? new List<int> { preconditionNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Activate", Class = "const game::ai::Pulse",
                            NodeIndex = preconditionNodeIndex, ParameterIndex = preconditionNodeIndex >= 0 ? 0 : -1,
                            Value = 0, Sources = new List<AINB.UserDefinedParameterSource>()
                        },
                        new AINB.UserDefinedParameter
                        {
                            Name = "Logic_Sleep", Class = "const game::ai::Pulse",
                            NodeIndex = -1, ParameterIndex = -1,
                            Value = 0, Sources = new List<AINB.UserDefinedParameterSource>()
                        }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "Logic_IsActive" } },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
            return node;
        }

        /// <summary>
        /// Creates a SplLogicLftDrawer node - Drawer/sliding platform controlled by switch.
        /// Used for: Lft_AbstractDrawer_*
        /// </summary>
        private AINB.LogicNode CreateSplLogicLftDrawerNode(int nodeIndex, string instanceName, string guid, int preconditionNodeIndex)
        {
            // For bool connections: LinkedNodes goes on the DESTINATION node (drawer) with the input parameter name
            var linkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                IntInputLink = new List<AINB.IntLinkedNode>()
            };

            // Add LinkedNode for bool input connection (IsPulling <- IsOn)
            if (preconditionNodeIndex >= 0)
            {
                linkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
                {
                    NodeIndex = preconditionNodeIndex,
                    Parameter = "IsPulling"
                });
            }

            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicLftDrawer",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = preconditionNodeIndex >= 0 ? new List<int> { preconditionNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>
                    {
                        new AINB.InputBoolParameter
                        {
                            Name = "IsPulling",
                            NodeIndex = preconditionNodeIndex,
                            ParameterIndex = preconditionNodeIndex >= 0 ? 0 : -1, // IsOn from switch (first bool output)
                            Value = false
                        }
                    },
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = linkedNodes
            };
        }

        /// <summary>
        /// Creates a SplLogicLftRotateTogglePoint node - Rotating platform that toggles between two positions.
        /// Used for: Lft_AbstractRotateTogglePoint_*
        /// </summary>
        private AINB.LogicNode CreateSplLogicLftRotateTogglePointNode(int nodeIndex, string instanceName, string guid, int preconditionNodeIndex)
        {
            // For bool connections: LinkedNodes goes on the DESTINATION node (lft) with the input parameter name
            var linkedNodes = new AINB.LinkedNodes
            {
                BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                IntInputLink = new List<AINB.IntLinkedNode>()
            };

            // Add LinkedNode for bool input connection (IsAccel <- IsOn)
            if (preconditionNodeIndex >= 0)
            {
                linkedNodes.BoolFloatInputLinkAndOutputLink.Add(new AINB.LinkedNode
                {
                    NodeIndex = preconditionNodeIndex,
                    Parameter = "IsAccel"
                });
            }

            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicLftRotateTogglePoint",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = preconditionNodeIndex >= 0 ? new List<int> { preconditionNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>
                    {
                        new AINB.InputBoolParameter
                        {
                            Name = "IsAccel",
                            NodeIndex = preconditionNodeIndex,
                            ParameterIndex = preconditionNodeIndex >= 0 ? 0 : -1, // IsOn from switch (first bool output)
                            Value = false
                        }
                    },
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = linkedNodes
            };
        }

        /// <summary>
        /// Creates a SplLogicBhvKeyTreasureBox node - Locked treasure chest.
        /// </summary>
        private AINB.LogicNode CreateSplLogicBhvKeyTreasureBoxNode(int nodeIndex, string instanceName, string guid)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicBhvKeyTreasureBox",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "CanOpen", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() },
                        new AINB.UserDefinedParameter { Name = "Open", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a SplLogicBhvSpawnerForSprinklerGimmick node - Spawns sprinkler objects.
        /// </summary>
        private AINB.LogicNode CreateSplLogicBhvSpawnerForSprinklerGimmickNode(int nodeIndex, string instanceName, string guid)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicBhvSpawnerForSprinklerGimmick",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Spawn", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() },
                        new AINB.UserDefinedParameter { Name = "Sprinkle", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a SplLogicBhvPeriscope node - Periscope/camera view switch.
        /// </summary>
        private AINB.LogicNode CreateSplLogicBhvPeriscopeNode(int nodeIndex, string instanceName, string guid)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicBhvPeriscope",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter> { new AINB.InternalStringParameter { Name = "InstanceName", Value = instanceName } },
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Logic_Activate", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() },
                        new AINB.UserDefinedParameter { Name = "Logic_Sleep", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "Logic_IsActive" } },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Logic_OnSleep", Class = "const game::ai::Pulse" },
                        new AINB.OutputUserDefinedParameter { Name = "OnLeaveView", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowPulseDelay node - Timer that fires a pulse after a delay.
        /// Used for: Timed water level changes (Hiagari), delayed platform movements
        /// Parameters:
        ///   - Delay (int): Time in seconds before firing the output pulse
        ///   - QueueSize (int): How many pulses can queue up (default 1)
        ///   - CanSave (bool): Whether to save state
        ///   - NeedToNetSync (bool): Network sync
        /// </summary>
        private AINB.LogicNode CreateGameFlowPulseDelayNode(int nodeIndex, string instanceName, string guid, int delaySeconds)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowPulseDelay",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>
                    {
                        new AINB.InternalBoolParameter { Name = "CanSave", Value = true },
                        new AINB.InternalBoolParameter { Name = "NeedToNetSync", Value = false }
                    },
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter { Name = "Delay", Value = delaySeconds },
                        new AINB.InternalIntParameter { Name = "QueueSize", Value = 1 }
                    },
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Input", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter> { new AINB.OutputIntParameter { Name = "StockNum" } },
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Output", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        // ─── GAMEFLOW NODE CREATORS ───

        /// <summary>
        /// Creates a SplLogicGetGachiLeftCount node - Gets remaining count in ranked.
        /// </summary>
        private AINB.LogicNode CreateGetGachiLeftCountNode(int nodeIndex, string guid, int team)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicGetGachiLeftCount",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>
                    {
                        new AINB.InputIntParameter { Name = "Team", NodeIndex = -1, ParameterIndex = -1, Value = team }
                    },
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter> { new AINB.OutputIntParameter { Name = "Output" } },
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowCompareS32 node - Compares int value against threshold.
        /// </summary>
        private AINB.LogicNode CreateCompareS32Node(int nodeIndex, string guid, int threshold, int compareOperator, int inputNodeIndex = -1)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowCompareS32",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = inputNodeIndex >= 0 ? new List<int> { inputNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter { Name = "Operator", Value = compareOperator }
                    },
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>
                    {
                        new AINB.InputIntParameter { Name = "S32", NodeIndex = inputNodeIndex, ParameterIndex = 0, Value = 0 },
                        new AINB.InputIntParameter { Name = "S32Base", NodeIndex = -1, ParameterIndex = -1, Value = threshold }
                    },
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "Bool" } },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = inputNodeIndex >= 0 ? new List<AINB.IntLinkedNode>
                    {
                        new AINB.IntLinkedNode { NodeIndex = inputNodeIndex, Parameter = "S32" }
                    } : new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowBoolToPulse node - Converts bool to pulse.
        /// </summary>
        private AINB.LogicNode CreateBoolToPulseNode(int nodeIndex, string guid, int inputNodeIndex = -1)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowBoolToPulse",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = inputNodeIndex >= 0 ? new List<int> { inputNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>
                    {
                        new AINB.InputBoolParameter { Name = "Bool", NodeIndex = inputNodeIndex, ParameterIndex = 0, Value = false }
                    },
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Pulse", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = inputNodeIndex >= 0 ? new List<AINB.LinkedNode>
                    {
                        new AINB.LinkedNode { NodeIndex = inputNodeIndex, Parameter = "Bool" }
                    } : new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowJoinPulse node - Combines multiple pulse inputs.
        /// </summary>
        private AINB.LogicNode CreateJoinPulseNode(int nodeIndex, string guid)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowJoinPulse",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>
                    {
                        new AINB.UserDefinedParameter { Name = "Input", Class = "const game::ai::Pulse", NodeIndex = -1, ParameterIndex = -1, Value = 0, Sources = new List<AINB.UserDefinedParameterSource>() }
                    },
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "Output", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowMinS32 node - Gets minimum of multiple int inputs.
        /// </summary>
        private AINB.LogicNode CreateMinS32Node(int nodeIndex, string guid, int inputNode1 = -1, int inputNode2 = -1)
        {
            var preconditions = new List<int>();
            var sources = new List<AINB.UserDefinedParameterSource>();

            if (inputNode1 >= 0)
            {
                preconditions.Add(inputNode1);
                sources.Add(new AINB.UserDefinedParameterSource { NodeIndex = inputNode1, ParameterIndex = 0 });
            }
            if (inputNode2 >= 0)
            {
                preconditions.Add(inputNode2);
                sources.Add(new AINB.UserDefinedParameterSource { NodeIndex = inputNode2, ParameterIndex = 0 });
            }

            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowMinS32",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = preconditions,
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>
                    {
                        new AINB.InputIntParameter { Name = "Inputs", NodeIndex = sources.Count > 0 ? -100 : -1, ParameterIndex = sources.Count, Value = 0 }
                    },
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter> { new AINB.OutputIntParameter { Name = "Min" } },
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = inputNode1 >= 0 ? new List<AINB.IntLinkedNode>
                    {
                        new AINB.IntLinkedNode { NodeIndex = inputNode1, Parameter = "Inputs" }
                    } : new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a SplLogicIsPassedGachihokoCheckPoint node - Rainmaker checkpoint trigger.
        /// </summary>
        private AINB.LogicNode CreateIsPassedGachihokoCheckPointNode(int nodeIndex, string guid, int team, int checkPointIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "SplLogicIsPassedGachihokoCheckPoint",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>
                    {
                        new AINB.InputIntParameter { Name = "Team", NodeIndex = -1, ParameterIndex = -1, Value = team },
                        new AINB.InputIntParameter { Name = "CheckPointIndex", NodeIndex = -1, ParameterIndex = -1, Value = checkPointIndex }
                    },
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "IsPassed" } },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>
                    {
                        new AINB.OutputUserDefinedParameter { Name = "OnPass", Class = "const game::ai::Pulse" }
                    }
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowGetGameFrame node - Gets current game frame counter.
        /// Used for Turf War time-based triggers.
        /// </summary>
        private AINB.LogicNode CreateGetGameFrameNode(int nodeIndex, string guid)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowGetGameFrame",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter> { new AINB.OutputIntParameter { Name = "GameFrame" } },
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowGameFrameToSecond node - Converts game frames to seconds.
        /// Used for Turf War time-based triggers.
        /// </summary>
        private AINB.LogicNode CreateFrameToSecondNode(int nodeIndex, string guid, int inputNodeIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowGameFrameToSecond",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = inputNodeIndex >= 0 ? new List<int> { inputNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>(),
                    Float = new List<AINB.InternalFloatParameter>()
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>
                    {
                        new AINB.InputIntParameter { Name = "GameFrame", NodeIndex = inputNodeIndex, ParameterIndex = 0, Value = 0 }
                    },
                    Float = new List<AINB.InputFloatParameter>(),
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter>(),
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter> { new AINB.OutputFloatParameter { Name = "Time" } },
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>(),
                    IntInputLink = new List<AINB.IntLinkedNode>
                    {
                        new AINB.IntLinkedNode { NodeIndex = inputNodeIndex, Parameter = "GameFrame" }
                    }
                }
            };
        }

        /// <summary>
        /// Creates a GameFlowCompareF32 node - Compares float value against threshold.
        /// Used for Turf War time-based triggers (e.g., 90.0 seconds = 1:30).
        /// Operator: 0 = <=, 1 = >=, etc.
        /// </summary>
        private AINB.LogicNode CreateCompareF32Node(int nodeIndex, string guid, float threshold, int operatorType, int inputNodeIndex)
        {
            return new AINB.LogicNode
            {
                NodeType = "UserDefined",
                NodeIndex = nodeIndex,
                Name = "GameFlowCompareF32",
                GUID = guid,
                Flags = new List<string> { "Is Precondition Node" },
                PreconditionNodes = inputNodeIndex >= 0 ? new List<int> { inputNodeIndex } : new List<int>(),
                InternalParameters = new AINB.InternalParameter
                {
                    String = new List<AINB.InternalStringParameter>(),
                    Bool = new List<AINB.InternalBoolParameter>(),
                    Int = new List<AINB.InternalIntParameter>
                    {
                        new AINB.InternalIntParameter { Name = "Operator", Value = operatorType }
                    },
                    Float = new List<AINB.InternalFloatParameter>
                    {
                        new AINB.InternalFloatParameter { Name = "Epsilon", Value = 0.001f }
                    }
                },
                InputParameters = new AINB.InputParameters
                {
                    Bool = new List<AINB.InputBoolParameter>(),
                    Int = new List<AINB.InputIntParameter>(),
                    Float = new List<AINB.InputFloatParameter>
                    {
                        new AINB.InputFloatParameter { Name = "A", NodeIndex = inputNodeIndex, ParameterIndex = 0, Value = 0.0f },
                        new AINB.InputFloatParameter { Name = "B", NodeIndex = -1, ParameterIndex = -1, Value = threshold }
                    },
                    String = new List<AINB.InputStringParameter>(),
                    UserDefined = new List<AINB.UserDefinedParameter>(),
                    Sources = new List<AINB.InputParameterSource>()
                },
                OutputParameters = new AINB.OutputParameters
                {
                    Bool = new List<AINB.OutputBoolParameter> { new AINB.OutputBoolParameter { Name = "Bool" } },
                    Int = new List<AINB.OutputIntParameter>(),
                    Float = new List<AINB.OutputFloatParameter>(),
                    String = new List<AINB.OutputStringParameter>(),
                    UserDefined = new List<AINB.OutputUserDefinedParameter>()
                },
                LinkedNodes = new AINB.LinkedNodes
                {
                    BoolFloatInputLinkAndOutputLink = new List<AINB.LinkedNode>
                    {
                        new AINB.LinkedNode { NodeIndex = inputNodeIndex, Parameter = "A" }
                    },
                    IntInputLink = new List<AINB.IntLinkedNode>()
                }
            };
        }

        #endregion

        /// <summary>
        /// Removes AINB nodes linked to this object via AIGroupID.
        /// This includes the entire node chain connected to the object.
        /// </summary>
        private void RemoveAINBNodeFromObject(MuObj obj, AINB ainbData)
        {
            if (string.IsNullOrEmpty(obj.AIGroupID))
                return;

            string instancePattern = $"_{obj.AIGroupID}";

            if (ainbData.Nodes == null)
                return;

            // Find primary nodes with matching InstanceName (e.g., SplLogicActor)
            var primaryNodes = ainbData.Nodes.Where(n =>
            {
                if (n.InternalParameters?.String == null)
                    return false;

                return n.InternalParameters.String.Any(s =>
                    s.Name == "InstanceName" &&
                    s.Value != null &&
                    s.Value.EndsWith(instancePattern)
                );
            }).ToList();

            // Collect all node indices to remove (including connected upstream nodes)
            HashSet<int> nodeIndicesToRemove = new HashSet<int>();

            foreach (var primaryNode in primaryNodes)
            {
                nodeIndicesToRemove.Add(primaryNode.NodeIndex);
                Console.WriteLine($"[AINB Delete] Primary node: [{primaryNode.NodeIndex}] {primaryNode.Name}");

                // Find all upstream nodes that connect TO this node
                CollectUpstreamNodes(ainbData, primaryNode.NodeIndex, nodeIndicesToRemove);
            }

            // Remove nodes in reverse order (highest index first to maintain indices during removal)
            var sortedIndicesToRemove = nodeIndicesToRemove.OrderByDescending(i => i).ToList();

            foreach (var nodeIndex in sortedIndicesToRemove)
            {
                var node = ainbData.Nodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
                if (node != null)
                {
                    Console.WriteLine($"[AINB Delete] Removing node: [{node.NodeIndex}] {node.Name}");
                    ainbData.Nodes.Remove(node);
                }
            }

            // Find and remove commands with matching names
            if (ainbData.Commands != null)
            {
                var commandsToRemove = ainbData.Commands.Where(c =>
                    c.Name != null && c.Name.EndsWith(instancePattern)
                ).ToList();

                foreach (var cmd in commandsToRemove)
                {
                    Console.WriteLine($"[AINB Delete] Removing command: {cmd.Name}");
                    ainbData.Commands.Remove(cmd);
                }
            }

            // Clean up LinkedNodes references in remaining nodes
            foreach (var node in ainbData.Nodes)
            {
                if (node.LinkedNodes?.BoolFloatInputLinkAndOutputLink != null)
                {
                    node.LinkedNodes.BoolFloatInputLinkAndOutputLink.RemoveAll(l => nodeIndicesToRemove.Contains(l.NodeIndex));
                }
                if (node.LinkedNodes?.IntInputLink != null)
                {
                    node.LinkedNodes.IntInputLink.RemoveAll(l => nodeIndicesToRemove.Contains(l.NodeIndex));
                }
                if (node.PreconditionNodes != null)
                {
                    node.PreconditionNodes.RemoveAll(n => nodeIndicesToRemove.Contains(n));
                }
            }

            // Clear the AIGroupID from the object
            obj.AIGroupID = "";

            // Reindex all nodes to ensure contiguous indices
            ReindexAINBNodes(ainbData);

            // Mark the AINB as modified
            if (MapEditor?.MapLoader?.stageDefinition != null)
            {
                MapEditor.MapLoader.stageDefinition.AINBModified = true;
            }
        }

        /// <summary>
        /// Recursively collects all upstream nodes that connect to the target node.
        /// Used for cascade deletion of entire AINB node chains.
        /// </summary>
        private void CollectUpstreamNodes(AINB ainbData, int targetNodeIndex, HashSet<int> collectedIndices)
        {
            foreach (var node in ainbData.Nodes)
            {
                if (collectedIndices.Contains(node.NodeIndex))
                    continue;

                bool connectsToTarget = false;

                // Check LinkedNodes
                if (node.LinkedNodes?.BoolFloatInputLinkAndOutputLink != null)
                {
                    if (node.LinkedNodes.BoolFloatInputLinkAndOutputLink.Any(l => l.NodeIndex == targetNodeIndex))
                        connectsToTarget = true;
                }
                if (!connectsToTarget && node.LinkedNodes?.IntInputLink != null)
                {
                    if (node.LinkedNodes.IntInputLink.Any(l => l.NodeIndex == targetNodeIndex))
                        connectsToTarget = true;
                }

                // Check PreconditionNodes
                if (!connectsToTarget && node.PreconditionNodes != null)
                {
                    if (node.PreconditionNodes.Contains(targetNodeIndex))
                        connectsToTarget = true;
                }

                if (connectsToTarget)
                {
                    Console.WriteLine($"[AINB Delete] Found upstream node: [{node.NodeIndex}] {node.Name} -> [{targetNodeIndex}]");
                    collectedIndices.Add(node.NodeIndex);
                    // Recursively find nodes that connect to this one
                    CollectUpstreamNodes(ainbData, node.NodeIndex, collectedIndices);
                }
            }
        }

        /// <summary>
        /// Deletes a single AINB node by its index.
        /// Also removes any commands referencing this node and cleans up linked references.
        /// </summary>
        private void DeleteSingleAINBNode(AINB ainbData, int nodeIndex)
        {
            if (ainbData?.Nodes == null)
                return;

            var nodeToRemove = ainbData.Nodes.FirstOrDefault(n => n.NodeIndex == nodeIndex);
            if (nodeToRemove == null)
                return;

            // Get the instance name for logging and command cleanup
            string instanceName = null;
            if (nodeToRemove.InternalParameters?.String != null)
            {
                var instanceParam = nodeToRemove.InternalParameters.String.FirstOrDefault(s => s.Name == "InstanceName");
                if (instanceParam != null)
                    instanceName = instanceParam.Value;
            }

            Console.WriteLine($"[AINB] Deleting node [{nodeIndex}] {nodeToRemove.Name} (InstanceName: {instanceName ?? "none"})");

            // Remove the node
            ainbData.Nodes.Remove(nodeToRemove);

            // Remove any commands that reference this node
            if (ainbData.Commands != null)
            {
                var commandsToRemove = ainbData.Commands.Where(c =>
                    c.LeftNodeIndex == nodeIndex || c.RightNodeIndex == nodeIndex ||
                    (instanceName != null && c.Name == instanceName)
                ).ToList();

                foreach (var cmd in commandsToRemove)
                {
                    Console.WriteLine($"[AINB] Removing command: {cmd.Name}");
                    ainbData.Commands.Remove(cmd);
                }
            }

            // Clean up LinkedNodes references in other nodes
            foreach (var node in ainbData.Nodes)
            {
                // Remove from BoolFloatInputLinkAndOutputLink
                if (node.LinkedNodes?.BoolFloatInputLinkAndOutputLink != null)
                {
                    node.LinkedNodes.BoolFloatInputLinkAndOutputLink.RemoveAll(l => l.NodeIndex == nodeIndex);
                }

                // Remove from IntInputLink
                if (node.LinkedNodes?.IntInputLink != null)
                {
                    node.LinkedNodes.IntInputLink.RemoveAll(l => l.NodeIndex == nodeIndex);
                }

                // Remove from PreconditionNodes
                if (node.PreconditionNodes != null)
                {
                    node.PreconditionNodes.RemoveAll(n => n == nodeIndex);
                }

                // Clean up Input Parameters referencing this node
                if (node.InputParameters?.UserDefined != null)
                {
                    foreach (var param in node.InputParameters.UserDefined)
                    {
                        if (param.NodeIndex == nodeIndex)
                        {
                            param.NodeIndex = -1;
                            param.ParameterIndex = -1;
                        }
                    }
                }

                if (node.InputParameters?.Bool != null)
                {
                    foreach (var param in node.InputParameters.Bool)
                    {
                        if (param.NodeIndex == nodeIndex)
                        {
                            param.NodeIndex = -1;
                            param.ParameterIndex = -1;
                        }
                    }
                }
            }

            // Mark as modified
            if (MapEditor?.MapLoader?.stageDefinition != null)
            {
                MapEditor.MapLoader.stageDefinition.AINBModified = true;
            }

            // Reindex all nodes to ensure contiguous indices
            ReindexAINBNodes(ainbData);

            Console.WriteLine($"[AINB] Node [{nodeIndex}] deleted successfully");
        }

        /// <summary>
        /// Reindexes all AINB nodes to ensure contiguous indices starting from 0.
        /// Updates all references in LinkedNodes, PreconditionNodes, Commands, and InputParameters.
        /// </summary>
        private void ReindexAINBNodes(AINB ainbData)
        {
            if (ainbData?.Nodes == null || ainbData.Nodes.Count == 0)
                return;

            // Create mapping from old index to new index
            var indexMap = new Dictionary<int, int>();
            for (int i = 0; i < ainbData.Nodes.Count; i++)
            {
                int oldIndex = ainbData.Nodes[i].NodeIndex;
                indexMap[oldIndex] = i;
                ainbData.Nodes[i].NodeIndex = i;
            }

            // Update all references in nodes
            foreach (var node in ainbData.Nodes)
            {
                // Update LinkedNodes references
                if (node.LinkedNodes?.BoolFloatInputLinkAndOutputLink != null)
                {
                    foreach (var link in node.LinkedNodes.BoolFloatInputLinkAndOutputLink)
                    {
                        if (link.NodeIndex >= 0 && indexMap.ContainsKey(link.NodeIndex))
                            link.NodeIndex = indexMap[link.NodeIndex];
                    }
                }

                if (node.LinkedNodes?.IntInputLink != null)
                {
                    foreach (var link in node.LinkedNodes.IntInputLink)
                    {
                        if (link.NodeIndex >= 0 && indexMap.ContainsKey(link.NodeIndex))
                            link.NodeIndex = indexMap[link.NodeIndex];
                    }
                }

                // Update PreconditionNodes
                if (node.PreconditionNodes != null)
                {
                    for (int i = 0; i < node.PreconditionNodes.Count; i++)
                    {
                        if (indexMap.ContainsKey(node.PreconditionNodes[i]))
                            node.PreconditionNodes[i] = indexMap[node.PreconditionNodes[i]];
                    }
                }

                // Update InputParameters references
                if (node.InputParameters?.UserDefined != null)
                {
                    foreach (var param in node.InputParameters.UserDefined)
                    {
                        if (param.NodeIndex >= 0 && indexMap.ContainsKey(param.NodeIndex))
                            param.NodeIndex = indexMap[param.NodeIndex];
                    }
                }

                if (node.InputParameters?.Bool != null)
                {
                    foreach (var param in node.InputParameters.Bool)
                    {
                        if (param.NodeIndex >= 0 && indexMap.ContainsKey(param.NodeIndex))
                            param.NodeIndex = indexMap[param.NodeIndex];
                    }
                }
            }

            // Update Commands references
            if (ainbData.Commands != null)
            {
                foreach (var cmd in ainbData.Commands)
                {
                    if (cmd.LeftNodeIndex >= 0 && indexMap.ContainsKey(cmd.LeftNodeIndex))
                        cmd.LeftNodeIndex = indexMap[cmd.LeftNodeIndex];
                    if (cmd.RightNodeIndex >= 0 && indexMap.ContainsKey(cmd.RightNodeIndex))
                        cmd.RightNodeIndex = indexMap[cmd.RightNodeIndex];
                }
            }

            Console.WriteLine($"[AINB] Reindexed {ainbData.Nodes.Count} nodes");
        }


        //Object specific frustum cull handling
        private bool FrustumCullObject(BfresRender render)
        {
            if (render.Models.Count == 0)
                return false;

            var transform = render.Transform;
            var context = GLContext.ActiveContext;

            var bounding = render.BoundingNode;
            bounding.UpdateTransform(transform.TransformMatrix);
            if (!context.Camera.InFustrum(bounding))
                return false;

            if (render.IsSelected)
                return true;

            //  if (render.UseDrawDistance)
            //    return context.Camera.InRange(transform.Position, 6000000);

            return true;
        }

        private ulong GenHash()
        {
            System.Random random = new System.Random();

            // Generate Hash
            uint thirtyBits = (uint)random.Next(1 << 30);
            uint twoBits = (uint)random.Next(1 << 2);
            ulong sixtyBits = (uint)random.Next(1 << 30);
            ulong sixtytwoBits = (uint)random.Next(1 << 2);

            ulong Hash = (ulong)(twoBits | (ulong)thirtyBits << 2 | sixtyBits << 32 | sixtytwoBits << 62);

            return Hash;
        }

        private uint GenSRTHash()
        {
            System.Random random = new System.Random();

            // Generate SRTHash
            uint thirtyBits = (uint)random.Next(1 << 30);
            uint twoBits = (uint)random.Next(1 << 2);

            uint SRTHash = (thirtyBits << 2) | twoBits;

            return SRTHash;
        }

        private string GenInstanceID()
        {
            System.Random random = new System.Random();
            string InstanceID = "";

            for (int i = 0; i < 8; i++)
            {
                InstanceID += UInt32ToHashString((uint)random.Next(0, 16));
            }
            InstanceID += "-";

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                    InstanceID += UInt32ToHashString((uint)random.Next(0, 16));

                InstanceID += "-";
            }

            for (int i = 0; i < 12; i++)
            {
                InstanceID += UInt32ToHashString((uint)random.Next(0, 16));
            }

            return InstanceID;
        }

        private string UInt32ToHashString(uint Input)
        {
            string ret = "";

            if (Input == 0)
            {
                return "0";
            }

            while (Input > 0)
            {
                switch (Input % 16)
                {
                    case 0:
                        ret += "0";
                        break;

                    case 1:
                        ret += "1";
                        break;

                    case 2:
                        ret += "2";
                        break;

                    case 3:
                        ret += "3";
                        break;

                    case 4:
                        ret += "4";
                        break;

                    case 5:
                        ret += "5";
                        break;

                    case 6:
                        ret += "6";
                        break;

                    case 7:
                        ret += "7";
                        break;

                    case 8:
                        ret += "8";
                        break;

                    case 9:
                        ret += "9";
                        break;

                    case 10:
                        ret += "a";
                        break;

                    case 11:
                        ret += "b";
                        break;

                    case 12:
                        ret += "c";
                        break;

                    case 13:
                        ret += "d";
                        break;

                    case 14:
                        ret += "e";
                        break;

                    case 15:
                        ret += "f";
                        break;
                }

                Input /= 16;
            }

            return ret;
        }

        //private EditableObject AddObject(int id, bool spawnAtCursor = false)
        private EditableObject AddObject(string actorName, bool spawnAtCursor = false)
        {
            Console.WriteLine($"~ Called ObjectEditor.AddObject() ~");
            //Force added sky boxes to edit existing if possible
            if (GlobalSettings.ActorDatabase.ContainsKey(actorName))
            {

            }


            //Get Actor Class Name
            string className = GlobalSettings.ActorDatabase[actorName].ClassName;
            Type elem = typeof(MuObj);
            ByamlSerialize.SetMapObjType(ref elem, actorName);
            var inst = (MuObj)Activator.CreateInstance(elem);

            inst.Name = actorName;
            inst.Gyaml = actorName;

            List<string> mAllInstanceID = new List<string>();
            List<ulong> mAllHash = new List<ulong>();
            List<uint> mAllSRTHash = new List<uint>();

            foreach (IDrawable obj in MapEditor.Scene.Objects)
            {
                if (obj is EditableObject && ((EditableObject)obj).UINode.Tag is MuObj)
                {
                    mAllInstanceID.Add(((MuObj)((EditableObject)obj).UINode.Tag).InstanceID);
                    mAllHash.Add(((MuObj)((EditableObject)obj).UINode.Tag).Hash);
                    mAllSRTHash.Add(((MuObj)((EditableObject)obj).UINode.Tag).SRTHash);
                }
            }

            string InstanceID = GenInstanceID();
            while (mAllInstanceID.Contains(InstanceID))
                InstanceID = GenInstanceID();

            ulong Hash = GenHash();
            while (mAllHash.Contains(Hash))
                Hash = GenHash();

            uint SRTHash = GenSRTHash();
            while (mAllSRTHash.Contains(SRTHash))
                SRTHash = GenSRTHash();

            inst.InstanceID = InstanceID;
            inst.Hash = Hash;
            inst.SRTHash = SRTHash;
            var rend = Create(inst);

            if (ObjectSubModelDisplay && rend is BfresRender bfresRender)
            {
                List<string> subModelNames = GlobalSettings.ActorDatabase[actorName].SubModels;

                foreach (var model in bfresRender.Models)
                {
                    if (subModelNames.Contains(model.Name))
                        model.IsVisible = true;
                }
            }

            Add(rend, true);

            var ob = rend.UINode.Tag as MuObj; //Obj;

            GLContext.ActiveContext.Scene.DeselectAll(GLContext.ActiveContext);

            //Get the default placements for our new object
            EditorUtility.SetObjectPlacementPosition(rend.Transform, spawnAtCursor);
            rend.UINode.IsSelected = true;
            return rend;
        }


        //private EditableObject EditObject(EditableObject render, int id)
        private EditableObject EditObject(EditableObject render, string actorName)
        {
            bool IsOriginalBfresModel = render is BfresRender;
            int index = render.UINode.Index;

            // Instead of just editing the name, let's make one from scratch
            Type elem = typeof(MuObj);
            ByamlSerialize.SetMapObjType(ref elem, actorName);
            var inst = (MuObj)Activator.CreateInstance(elem);

            var obj = render.UINode.Tag as MuObj; // Obj;
            inst.Set(obj);
            //obj.ObjId = id;

            if (inst.Bakeable == true)
            {
                if (actorName.StartsWith("Lft_") || actorName.StartsWith("Obj_"))
                    inst.Gyaml = "Work/Actor/Mpt_" + actorName.Substring(4) + ".engine__actor__ActorParam.gyml";
                else
                    inst.Gyaml = "Work/Actor/" + actorName + ".engine__actor__ActorParam.gyml";
            }
            else
            {
                inst.Gyaml = actorName;
            }

            inst.Name = actorName;

            //Remove the previous renderer
            GLContext.ActiveContext.Scene.RemoveRenderObject(render);

            //Create a new object with the current ID
            var editedRender = Create(inst);
            bool IsNewBfresModel = editedRender is BfresRender;

            Add(editedRender);

            Vector3 NewScale = new Vector3(render.Transform.Scale.X, render.Transform.Scale.Y, render.Transform.Scale.Z);

            if (IsOriginalBfresModel && !IsNewBfresModel)
            {
                NewScale /= 10.0f;
            }
            else if (!IsOriginalBfresModel && IsNewBfresModel)
            {
                NewScale *= 10.0f;
            }

            inst.Scale = new ByamlVector3F(NewScale.X, NewScale.Y, NewScale.Z);

            //Keep the same node order
            Root.Children.Remove(editedRender.UINode);
            Root.Children.Insert(index, editedRender.UINode);

            editedRender.Transform.Position = render.Transform.Position;
            editedRender.Transform.Scale = NewScale;
            editedRender.Transform.Rotation = render.Transform.Rotation;
            editedRender.Transform.UpdateMatrix(true);

            editedRender.UINode.IsSelected = true;

            /*//Skybox updated, change the cubemap
            if (obj.IsSkybox)
                Workspace.ActiveWorkspace.Resources.UpdateCubemaps = true;*/

            //Undo operation
            GLContext.ActiveContext.Scene.AddToUndo(new ObjectEditUndo(this, render, editedRender));

            return editedRender;
        }

        //private string GetResourceName(Obj obj)
        private string GetResourceName(MuElement obj)
        {
            Console.WriteLine("~ Called ObjectEditor.GetResourceName(Obj) ~");

            GlobalSettings.LoadDataBase();

            //Load through an in tool list if the database isn't loaded
            //string name = GlobalSettings.ObjectList.ContainsKey(obj.ObjId) ? $"{GlobalSettings.ObjectList[obj.ObjId]}" : obj.ObjId.ToString();
            string name = "";

            //Use object database instead if exists
            if (GlobalSettings.ActorDatabase.ContainsKey(obj.Name))
                name = GlobalSettings.ActorDatabase[obj.Name].ResName;

            return name;
        }

        //private string GetNodeHeader(Obj obj)
        private string GetNodeHeader(MuElement obj)
        {
            //string name = GlobalSettings.ObjectList.ContainsKey(obj.ObjId) ? $"{GlobalSettings.ObjectList[obj.ObjId]}" : obj.ObjId.ToString();
            string name = obj.Name;   //string name = "???";
            //Use object database instead if exists
            if (GlobalSettings.ActorDatabase.ContainsKey(obj.Name))
            {
                name = GlobalSettings.ActorDatabase[obj.Name].Name;
            }
#warning ^^ Not sure if FmdbName is correct here. Check again later. -- Update: it wasn't. Name is correct.

            /*//Start Ex parameter spawn index
            if (obj.ObjId == 8008)
                name += $" ({obj.Params[0]})";
            //Test start parameter spawn index
            if (obj.ObjId == 6002)
                name += $" ({obj.Params[7]})";

            if (obj.ParentObj != null)
                name += $"    {IconManager.MODEL_ICON}    ";
            if (obj.ParentArea != null)
                name += $"    {IconManager.RECT_SCALE_ICON}    ";
            if (obj.Path != null)
                name += $"    {IconManager.PATH_ICON}    ";
            if (obj.ObjPath != null)
                name += $"    {IconManager.ANIM_PATH_ICON}    ";*/

            return name;
        }



        private void AddObjectMenuAction()
        {
            var objects = GlobalSettings.ActorDatabase.Values.OrderBy(x => x.Name).ToList();

            MapObjectSelector selector = new MapObjectSelector(objects);
            MapStudio.UI.DialogHandler.Show(TranslationSource.GetText("SELECT_OBJECT"), 400, 800, () =>
            {
                selector.Render();
            }, (result) =>
            {
                var id = selector.GetSelectedID();
                //if (!result || id == 0)
                if (!result || string.IsNullOrEmpty(id))
                    return;

                AddObject(id, true);
            });
        }

        private void EditObjectMenuAction()
        {
            var selected = GetSelected().ToList();
            if (selected.Count == 0)
                return;

            var objects = GlobalSettings.ActorDatabase.Values.OrderBy(x => x.Name).ToList();

            MapObjectSelector selector = new MapObjectSelector(objects);
            MapStudio.UI.DialogHandler.Show(TranslationSource.GetText("SELECT_OBJECT"), 400, 800, () =>
            {
                selector.Render();
            }, (result) =>
            {
                var id = selector.GetSelectedID();
                //if (!result || id == 0)
                if (!result || string.IsNullOrEmpty(id))
                    return;

                var renders = selected.Select(x => ((EditableObjectNode)x).Object).ToList();

                GLContext.ActiveContext.Scene.BeginUndoCollection();
                foreach (EditableObjectNode ob in selected)
                {
                    //int previousID = ((Obj)ob.Tag).ObjId;
                    //var previousID = ((Obj)ob.Tag).UnitConfigName;

                    var render = EditObject(ob.Object, id);
                }
                GLContext.ActiveContext.Scene.EndUndoCollection();
            });
        }

        private void EditObjectLinkMenuAction(MuObj obj, int LinkIndex)
        {
            var selected = Root.Children;
            if (selected.Count == 0)
                return;

            MapObjectLinkerSelector selector = new MapObjectLinkerSelector(selected);
            MapStudio.UI.DialogHandler.Show(TranslationSource.GetText("SELECT_OBJECT"), 400, 800, () =>
            {
                selector.Render();
            }, (result) =>
            {
                ulong id = selector.GetSelectedID();
                //if (!result || id == 0)
                if (!result || id == 0)
                    return;

                obj.Links[LinkIndex].Dst = id;
            });
        }

        class ObjectEditUndo : IRevertable
        {
            private List<ObjectInfo> Objects = new List<ObjectInfo>();

            public ObjectEditUndo(List<ObjectInfo> objects)
            {
                this.Objects = objects;
            }

            public ObjectEditUndo(ObjectEditor editor, EditableObject previousObj, EditableObject newObj)
            {
                Objects.Add(new ObjectInfo(editor, previousObj, newObj));
            }

            public IRevertable Revert()
            {
                var redoList = new List<ObjectInfo>();
                foreach (var info in Objects)
                {
                    redoList.Add(new ObjectInfo(info.Editor, info.NewRender, info.PreviousRender));

                    info.Editor.Remove(info.NewRender);
                    info.Editor.Add(info.PreviousRender);

                    /*//Skybox updated, change the cubemap
                    if (((Obj)info.NewRender.UINode.Tag).IsSkybox)
                        Workspace.ActiveWorkspace.Resources.UpdateCubemaps = true;*/
                }
                return new ObjectEditUndo(redoList);
            }

            public class ObjectInfo
            {
                public EditableObject PreviousRender;
                public EditableObject NewRender;

                public ObjectEditor Editor;

                public ObjectInfo(ObjectEditor editor, EditableObject previousObj, EditableObject newObj)
                {
                    Editor = editor;
                    PreviousRender = previousObj;
                    NewRender = newObj;
                }
            }
        }
    }
}
