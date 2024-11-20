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

            foreach (EditableObject ob in copied)
            {
                var obj = ob.UINode.Tag as MuObj; //Obj;
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

                string InstanceID = GenInstanceID();
                while (mAllInstanceID.Contains(InstanceID))
                    InstanceID = GenInstanceID();

                ulong Hash = GenHash();
                while (mAllHash.Contains(Hash))
                    Hash = GenHash();

                uint SRTHash = GenSRTHash();
                while (mAllSRTHash.Contains(SRTHash))
                    SRTHash = GenSRTHash();

                var obj1 = duplicated.UINode.Tag as MuObj; //Obj;

                obj1.InstanceID = InstanceID;
                obj1.Hash = Hash;
                obj1.SRTHash = SRTHash;

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
            EditableObject render = new TransformableObject(Root);

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
                        obj.Links.Add(new MuObj.Link());
                    }

                    ImGui.SameLine();

                    bool ActiveCB = GlobalSettings.IsLinkingComboBoxActive;
                    if (ImGui.Checkbox("Activate Combo Box", ref ActiveCB))
                    {
                        GlobalSettings.IsLinkingComboBoxActive = ActiveCB;
                    }

                    ImGui.BeginGroup();
                    for (int i = 0; i < obj.Links.Count; i++)
                    {                       
                        if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###{category}.{i}"))
                        {
                            if (obj.Links.Count > 0)
                                obj.Links.RemoveAt(i);
                        }

                        ImGui.SameLine();

                        if (ImGui.Button($"...###{category}1.{i}"))
                        {
                            EditObjectLinkMenuAction(obj, i);
                        }

                        ImGui.SameLine();

                        if (ImGui.Button($"   {IconManager.COPY_ICON}   ###{category}2.{i}"))
                        {
                            MuObj.Link Duplicate = obj.Links[i].Clone();
                            obj.Links.Insert(i, Duplicate);
                        }
                    }
                    ImGui.EndGroup();

                    ImGui.SameLine();

                    ImGui.BeginGroup();
                    ImGui.BeginColumns("##" + category + numColumns.ToString(), numColumns);
                    for (int i = 0; i < obj.Links.Count; i++)
                    {
                        float colwidth = ImGui.GetColumnWidth();
                        ImGui.PushItemWidth(colwidth - 6);

                        string inputValueStr = obj.Links[i].Name;
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
                                obj.Links[i].Name = LinkNames[0];
                            }

                            if (ImGui.BeginCombo($"###LinkingCBox.Name{i}", inputValueStr, ImGuiComboFlags.HeightLarge))
                            {
                                foreach (string LinkName in LinkNames)
                                {
                                    bool isSelected = inputValueStr == LinkName;

                                    if (ImGui.Selectable(LinkName, isSelected))
                                    {
                                        obj.Links[i].Name = LinkName;
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
                                obj.Links[i].Name = inputValueStr;
                            }
                        }

                        ImGui.PopItemWidth();

                        ImGui.NextColumn();

                        colwidth = ImGui.GetColumnWidth();
                        ImGui.PushItemWidth(colwidth - 6);

                        ulong inputValue = obj.Links[i].Dst;
                        ulong* ulongptr = &inputValue;

                        if (ImGui.InputScalar($"###LinkingBox.Dst{i}", ImGuiDataType.U64, (IntPtr)ulongptr))
                        {
                            obj.Links[i].Dst = (ulong)inputValue;
                        }

                        ImGui.PopItemWidth();
                        ImGui.NextColumn();
                    }
                    ImGui.EndColumns();
                    ImGui.EndGroup();
                }

                var gui = new MapObjectUI();
                gui.Render(obj, Workspace.ActiveWorkspace.GetSelected().Select(x => x.Tag));
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

            /*var obj = render.UINode.Tag as Obj;
            foreach (var linkableObject in GLContext.ActiveContext.Scene.Objects)
            {
                if (linkableObject is RenderablePath)
                {
                    var path = linkableObject as RenderablePath;
                    TryFindPathLink(render, path, obj.Path, obj.PathPoint);
                    TryFindPathLink(render, path, obj.LapPath, obj.LapPathPoint);
                    TryFindPathLink(render, path, obj.ObjPath, obj.ObjPathPoint);
                    TryFindPathLink(render, path, obj.EnemyPath1, null);
                    TryFindPathLink(render, path, obj.EnemyPath2, null);
                    TryFindPathLink(render, path, obj.ItemPath1, null);
                    TryFindPathLink(render, path, obj.ItemPath2, null);

                }
                else if (linkableObject is EditableObject)
                {
                    var editObj = linkableObject as EditableObject;
                    TryFindObjectLink(render, editObj, obj.ParentArea);
                    TryFindObjectLink(render, editObj, obj.ParentObj);
                }
            }*/
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

            if (ObjectSubModelDisplay)
            {
                List<string> subModelNames = GlobalSettings.ActorDatabase[actorName].SubModels;

                foreach (var model in ((BfresRender)rend).Models)
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
